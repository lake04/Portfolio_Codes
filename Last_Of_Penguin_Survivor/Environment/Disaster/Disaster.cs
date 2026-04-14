using System.Collections;
using UnityEngine;

public class Disaster : MonoBehaviour
{
    public DisasterData disasterData;

    public void StartDisaster()
    {
        StartCoroutine(IE_StartDisaster());
    }

    public  virtual IEnumerator IE_StartDisaster()
    {

        yield return new WaitForSeconds(disasterData.Duration);
       
    }
}
