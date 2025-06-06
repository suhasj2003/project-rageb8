using UnityEngine;
using Unity.Cinemachine;
using UnityEditor;
using System;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects CustomInspectorObjects;

    private Collider2D _coll;

    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CustomInspectorObjects.PanCameraOnContact)
            {
                CameraManager.Instance.PanCameraOnContact(
                    CustomInspectorObjects.PanDistance, 
                    CustomInspectorObjects.PanTime, 
                    CustomInspectorObjects.PanDirection,
                    false);
            }
        }

        Debug.Log("CameraControlTrigger: Player entered trigger area.");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;
            if (CustomInspectorObjects.SwapCameras && 
                CustomInspectorObjects.CameraSource != null && 
                CustomInspectorObjects.CameraDestination != null)
            {
                CameraManager.Instance.SwapCamera(
                    CustomInspectorObjects.CameraSource,
                    CustomInspectorObjects.CameraDestination,
                    exitDirection,
                    CustomInspectorObjects.HorizontalSwap);
            }

            if (CustomInspectorObjects.PanCameraOnContact)
            {
                CameraManager.Instance.PanCameraOnContact(
                   CustomInspectorObjects.PanDistance,
                   CustomInspectorObjects.PanTime,
                   CustomInspectorObjects.PanDirection,
                   true);
            }
        }

        Debug.Log("CameraControlTrigger: Player exited trigger area.");
    }
}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool SwapCameras = false;
    public bool PanCameraOnContact = false;

    [HideInInspector] public CinemachineCamera CameraSource;
    [HideInInspector] public CinemachineCamera CameraDestination;
    [HideInInspector] public bool HorizontalSwap = true;

    [HideInInspector] public PanDirection PanDirection;
    [HideInInspector] public float PanDistance = 3f;
    [HideInInspector] public float PanTime = 0.35f;
}

public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}

[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    CameraControlTrigger _cameraControlTrigger;

    public void OnEnable()
    {
        _cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (_cameraControlTrigger.CustomInspectorObjects.SwapCameras)
        {
            _cameraControlTrigger.CustomInspectorObjects.CameraSource
                = EditorGUILayout.ObjectField(
                    "Source Camera", 
                    _cameraControlTrigger.CustomInspectorObjects.CameraSource, 
                    typeof(CinemachineCamera), 
                    true) as CinemachineCamera;

            _cameraControlTrigger.CustomInspectorObjects.CameraDestination
                = EditorGUILayout.ObjectField(
                    "Destination Camera", 
                    _cameraControlTrigger.CustomInspectorObjects.CameraDestination, 
                    typeof(CinemachineCamera), 
                    true) as CinemachineCamera;

            _cameraControlTrigger.CustomInspectorObjects.HorizontalSwap
               = (Boolean)EditorGUILayout.Toggle(
                   "Horizontal Swap",
                   _cameraControlTrigger.CustomInspectorObjects.HorizontalSwap);
        }

        if (_cameraControlTrigger.CustomInspectorObjects.PanCameraOnContact)
        {
            _cameraControlTrigger.CustomInspectorObjects.PanDirection 
                = (PanDirection)EditorGUILayout.EnumPopup("Pan Direction", 
                _cameraControlTrigger.CustomInspectorObjects.PanDirection);

            _cameraControlTrigger.CustomInspectorObjects.PanDistance 
                = EditorGUILayout.FloatField("Pan Distance", 
                _cameraControlTrigger.CustomInspectorObjects.PanDistance);

            _cameraControlTrigger.CustomInspectorObjects.PanTime 
                = EditorGUILayout.FloatField("Pan Time", 
                _cameraControlTrigger.CustomInspectorObjects.PanTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_cameraControlTrigger);
        }
    }
}
