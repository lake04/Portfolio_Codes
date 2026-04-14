using Lop.Survivor;
using System.Collections;
using UnityEngine;

public class SnowWeatherDisaster : Disaster
{
    public ParticleSystem snowEffect;
    [SerializeField] private Vector3 spawnPos;
    private ParticleSystem snowParticle;

    public override IEnumerator IE_StartDisaster()
    {
        Debug.Log("눈 재해 시작");
        if(snowParticle == null)
        {
            var snowInstance = Instantiate(snowEffect, spawnPos, Quaternion.identity);
            snowParticle = snowInstance;
            snowInstance.Play();
        }
       
        EnableFog();
        TemperatureManager.Instance.SetAmbientTemperature(disasterData.TemperatureChange);
        yield return new WaitForSeconds(disasterData.Duration);

        TemperatureManager.Instance.ResetToDayCycle();
        DisableFog();
    }

    private void EnableFog()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.8f, 0.8f, 0.8f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.05f;
        Debug.Log("안개 효과 활성화.");
    }

    private void DisableFog()
    {
        RenderSettings.fog = false;
        snowParticle.Stop();
        Destroy(snowParticle);
        Debug.Log("안개 효과 비활성화.");
    }
}
