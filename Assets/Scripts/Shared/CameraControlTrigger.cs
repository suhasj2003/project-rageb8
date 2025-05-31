using UnityEngine;
using Unity.Cinemachine;
using UnityEditor;

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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CustomInspectorObjects.PanCameraOnContact)
            {
                CameraManager.Instance.PanCameraOnContact(
                   CustomInspectorObjects.PanDistance,
                   CustomInspectorObjects.PanTime,
                   CustomInspectorObjects.PanDirection,
                   true);
            }
        }
    }

}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool SwapCameras = false;
    public bool PanCameraOnContact = false;

    [HideInInspector] public CinemachineCamera CameraOnLeft;
    [HideInInspector] public CinemachineCamera CameraOnRight;

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
            _cameraControlTrigger.CustomInspectorObjects.CameraOnLeft 
                = EditorGUILayout.ObjectField(
                    "Camera On Left", 
                    _cameraControlTrigger.CustomInspectorObjects.CameraOnLeft, 
                    typeof(CinemachineCamera), 
                    true) as CinemachineCamera;

            _cameraControlTrigger.CustomInspectorObjects.CameraOnRight 
                = EditorGUILayout.ObjectField(
                    "Camera On Right", 
                    _cameraControlTrigger.CustomInspectorObjects.CameraOnRight, 
                    typeof(CinemachineCamera), 
                    true) as CinemachineCamera;
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
