using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineCamera[] _virtualCameras;

    [Header("Y Damping player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.17f;
    [SerializeField] private float _fallYPanTime = 0.28f;
    public float FallSpeedYDampingChangeThreshold = -10f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpredFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionComposer;

    private float _normYPanAmount;

    private Vector2 _startingTrackedObjectOffset;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }

        for (int i = 0; i < _virtualCameras.Length; ++i)
        {
            if (_virtualCameras[i].enabled)
            {
                _currentCamera = _virtualCameras[i];
                _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();

            }
        }

        _normYPanAmount = _positionComposer.Damping.y;

        _startingTrackedObjectOffset = _positionComposer.TargetOffset;
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _positionComposer.Damping.y;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpredFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
            _positionComposer.Damping.y = lerpedPanAmount;
            
            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion

    #region Pan Camera

    public void PanCameraOnContact(
        float panDistance, 
        float panTime, 
        PanDirection panDirection,
        bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(
            PanCamera(
                panDistance, 
                panTime, 
                panDirection, 
                panToStartingPos));
    }

    public IEnumerator PanCamera(
        float panDistance, 
        float panTime, 
        PanDirection panDirection, 
        bool panToStartinPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStartinPos)
        { 
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
            }

            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }
        else
        {
            startingPos = _positionComposer.TargetOffset;
            endPos = _startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 panLerp = Vector3.Lerp(
                startingPos,
                endPos,
                (elapsedTime / panTime));

            _positionComposer.TargetOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region Swap Camera

    public void SwapCamera(
        CinemachineCamera cameraSource, 
        CinemachineCamera cameraDestination, 
        Vector2 triggerExitDirection,
        bool horizontalSwap)
    {
        float exitDirection = horizontalSwap ? triggerExitDirection.x : triggerExitDirection.y;
    
        if (_currentCamera == cameraSource && exitDirection > 0f)
        {
            cameraDestination.enabled = true;
            cameraSource.enabled = false;

            _currentCamera = cameraDestination;

            _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
        }
        else if (_currentCamera == cameraDestination && exitDirection < 0f)
        {
            cameraDestination.enabled = false;
            cameraSource.enabled = true;

            _currentCamera = cameraSource;

            _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
        }
    }

    #endregion
}
