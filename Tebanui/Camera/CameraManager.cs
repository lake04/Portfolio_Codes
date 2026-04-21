using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
   public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;

    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool isLerpingYDamping {  get; private set; }
    public bool lerpedFromPlayerFalling { get; set; }

    private Coroutine lerpYPanCoroutine;
    private Coroutine panCameraCoroutine;

    public CinemachineVirtualCamera curCamera;
    private CinemachineFramingTransposer framingTransposer;

    private float normYPanAmount;

    private Vector2 startingTrackedObjectoffset;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        for(int i=0;i<allVirtualCameras.Length;i++)
        {
            if (allVirtualCameras[i].enabled)
            {
                curCamera = allVirtualCameras[i];

                framingTransposer = curCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }

            normYPanAmount = framingTransposer.m_YDamping;

            startingTrackedObjectoffset = framingTransposer.m_TrackedObjectOffset;
        }
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        isLerpingYDamping = true;

        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        if(isPlayerFalling)
        {
            endDampAmount = fallPanAmount;
            lerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = normYPanAmount;
        }

        float elapsedTime = 0f;
        while(elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / fallYPanTime));
            framingTransposer.m_YDamping = lerpedPanAmount;
            yield return null;
        }

        isLerpingYDamping = false;
    }
    #endregion

    #region Pan Camera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistace, float panTime,  PanDirection panDirection,bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if(!panToStartingPos)
        {
            switch(panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;

                 default:
                    break;
            }

            endPos *= panDistace;
            startingPos = startingTrackedObjectoffset;

            endPos += startingPos;
        }
        else
        {
            startingPos = framingTransposer.m_TrackedObjectOffset;
            endPos = startingTrackedObjectoffset;
        }

        float elapsedTime = 0f;
        while(elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region Swap Cameras

    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if(curCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true;

            cameraFromLeft.enabled = false;

            curCamera = cameraFromRight;

            framingTransposer = curCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        else if(curCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            cameraFromLeft.enabled = true;

            cameraFromRight.enabled = false;

            curCamera = cameraFromLeft;

            framingTransposer = curCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        Player.instance.OnCameraSwitched(curCamera);
        CameraShake.Instance.impulseListener = curCamera.GetComponent<CinemachineImpulseListener>();

    }

    #endregion
}
