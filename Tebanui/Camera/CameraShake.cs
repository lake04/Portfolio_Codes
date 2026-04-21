using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [SerializeField] private float globalShakeForce = 1f;
    public CinemachineImpulseListener impulseListener;

    private CinemachineImpulseDefinition impulseDefinition;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
     
    }

    private void Update()
    {
     
    }

    public void Shake(CinemachineImpulseSource _impulseSource)
    {
        _impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

    public void AttackShake(float intensity, float time)
    {

    }

    public void ScreenShakeFromProfile(ScreenShakeProfile _profile, CinemachineImpulseSource _impulseSource)
    {
        //세팅
        SetUpScreenShakeSettings(_profile, _impulseSource);

        //shake
        _impulseSource.GenerateImpulseWithForce(_profile.impactForce);
    }

    private void SetUpScreenShakeSettings(ScreenShakeProfile _profile, CinemachineImpulseSource _impulseSource)
    {
        impulseDefinition = _impulseSource.m_ImpulseDefinition;

        //impulse 세팅 수정
        impulseDefinition.m_ImpulseDuration = _profile.impactTime;

        _impulseSource.m_DefaultVelocity = _profile.defaulteVelocity;
        impulseDefinition.m_CustomImpulseShape = _profile.impulaseCurve;

        //Listener 세팅 수정
        impulseListener.m_ReactionSettings.m_AmplitudeGain = _profile.listenerAmplitude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = _profile.listenerFrequency;
        impulseListener.m_ReactionSettings.m_Duration = _profile.listenerDuration;
    }


}