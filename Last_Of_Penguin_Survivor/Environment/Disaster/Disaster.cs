using System.Collections;
using UnityEngine;
using System;

public class Disaster : MonoBehaviour
{
    public DisasterData disasterData;
    public Action<Disaster> OnDisasterEnded;


    public void StartDisaster()
    {
        StartCoroutine(IE_InternalRoutine());
    }

    private IEnumerator IE_InternalRoutine()
    {
        yield return StartCoroutine(IE_StartDisaster());

        OnDisasterEnded?.Invoke(this);

        if (LOPNetworkManager.Instance.isConnected)
        {
            LOPNetworkManager.Instance.NetworkDestroy(gameObject);
        }
        else if (LOPNetworkManager.Instance.isConnected == false)
        {
            Destroy(gameObject);
        }
    }

    public  virtual IEnumerator IE_StartDisaster()
    {

        yield return new WaitForSeconds(disasterData.Duration);

        OnDisasterEnded?.Invoke(this);
        if (LOPNetworkManager.Instance.isConnected)
        {
            LOPNetworkManager.Instance.NetworkDestroy(gameObject);
        }
        else if (LOPNetworkManager.Instance.isConnected == false)
        {
            Destroy(gameObject);
        }
    }

  
}
