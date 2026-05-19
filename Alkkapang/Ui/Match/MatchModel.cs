using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchModel : ModelBase
{
    public void StartMatching()
    {
        BackEndMatchManager.Instance.JoinMatchServer();
        //SceneManager.LoadScene("Ingame");
    }

    public void CancelMatching()
    {
        BackEndMatchManager.Instance.CancelRegistMatchMaking();
    }
}