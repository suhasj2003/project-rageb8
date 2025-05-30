using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineCamera[] _virtualCameras;

    [Header("Y Damping player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float FallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpredFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionComposer;

    private float _normYPanAmount;

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

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / _fallYPanTime);
            _positionComposer.Damping.y = lerpedPanAmount;
            
            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion
}
