using System;
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
    private int _maskUi;
    private int _maskPlayer;

    private void Start()
    {
        _maskUi = 1 << LayerMask.NameToLayer("UI");
        _maskPlayer = 1 << LayerMask.NameToLayer("Player");
        _camera = GetComponent<Camera>();
        _cullingMaskEverything = _camera.cullingMask;
        _defaultSize = _camera.orthographicSize;
    }

    private void Update()
    {
        UseVisualization(visualization);
        if (Input.GetKeyDown("v"))
        {
            visualization = NextVisualization(visualization);
        }
    }

    private VisualizationMode NextVisualization(VisualizationMode v)
    {
        switch (v)
        {
            case VisualizationMode.Everything: return VisualizationMode.OnlyPlayer;
            case VisualizationMode.OnlyPlayer: return VisualizationMode.Nothing;
            case VisualizationMode.Nothing: return VisualizationMode.Everything;
            default: throw new Exception();
        }
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
                _camera.cullingMask = _maskUi;
                _camera.orthographicSize = _defaultSize;
                break;
            case VisualizationMode.OnlyPlayer:
                _camera.cullingMask = _maskPlayer | _maskUi;
                _camera.orthographicSize = sizeOnOnlyPlayer;
                break;
        }
    }
}
