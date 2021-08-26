using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using Dropdown = TMPro.TMP_Dropdown;

public class Win : MonoBehaviour
{

    public static bool IsWin = false;

    public GameObject overlay;
    public GameObject overlaySurvey;
    public Button continueButton;
    public Dropdown _confidenceSelection;
    public bool showSurvey = true;
	public string levelCompletedSoundName = "LevelCompleted";
    private Action winCallback = () => {};

    private ToggleGroup _levelImageSelection;
    private string selectedLevelImage = null;
    private int selectedConfidence = 0;

    private void Start() {
        overlay.SetActive(false);
        _levelImageSelection = overlaySurvey.GetComponent<ToggleGroup>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EventLogging.logEvent(new LevelCompletedEvent());
        IsWin = true;
        AudioOut.PlayOneShotAttached(levelCompletedSoundName, gameObject);
        overlay.SetActive(true);
        overlaySurvey.SetActive(showSurvey);

        if (showSurvey) {
            EventLogging.logEvent(new SurveyStartedEvent());

            // don't preselect choices
            _levelImageSelection.allowSwitchOff = true;
            _levelImageSelection.SetAllTogglesOff();
            selectedLevelImage = null;

            // value 0 is "please select..."
            selectedConfidence = 0;

            // disable continue button until choices have been made
            continueButton.interactable = false;
        }
    }

    public void OnLevelImageSelect() {
        if(_levelImageSelection && _levelImageSelection.AnyTogglesOn()) {
            // disallow empty choice
            _levelImageSelection.allowSwitchOff = false;

            var selectedToggle = _levelImageSelection.ActiveToggles().FirstOrDefault();
            selectedLevelImage = selectedToggle.name;
            UpdateContinueButton();
        }
    }

    public void OnConfidenceSelect() {
        selectedConfidence = _confidenceSelection.value;
        UpdateContinueButton();
    }

    private void UpdateContinueButton() {
        // enable continue button only when valid values have been selected
        continueButton.interactable = (selectedLevelImage != null && selectedConfidence > 0);
    }

    // (can only happen if button is interactable)
    public void OnContinueButtonClick() {
        if (showSurvey) {
            // submit survey
            EventLogging.logEvent(new SurveySubmittedEvent(selectedLevelImage, selectedConfidence));
        }
        winCallback();
    }

    public void RegisterWinCallback(Action winCallback)
    {
        this.winCallback = winCallback;
    }
}

