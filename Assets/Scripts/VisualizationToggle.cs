using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationToggle : MonoBehaviour
{
    public enum  VisualizationMode
    {
        Nothing, Everything, OnlyPlayer
    }

    public VisualizationMode visualization = VisualizationMode.Everything;
    public float sizeOnOnlyPlayer;

    private Camera _camera;
    private int _cullingMaskEverything;
    private VisualizationMode _visualization;
    private float _defaultSize;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _cullingMaskEverything = _camera.cullingMask;
        _defaultSize = _camera.orthographicSize;
    }

    private void Update()
    {
        UseVisualization(visualization);
    }

    private void UseVisualization(VisualizationMode v)
    {
        switch (v)
        {
            case VisualizationMode.Everything:
                _camera.cullingMask = _cullingMaskEverything;
                _camera.fieldOfView = _defaultSize;
                break;
            case VisualizationMode.Nothing:
                _camera.cullingMask = 0;
                _camera.orthographicSize = _defaultSize;
                break;
            case VisualizationMode.OnlyPlayer:
                _camera.cullingMask = 1 << LayerMask.NameToLayer("Player");
                _camera.orthographicSize = sizeOnOnlyPlayer;
                break;
        }
    }
}
