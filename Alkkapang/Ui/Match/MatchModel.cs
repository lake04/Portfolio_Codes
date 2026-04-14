using UnityEngine;

public class MatchModel : ModelBase
{
    public void StartMatching()
    {
        BackEndMatchManager.Instance.JoinMatchServer();
    }

    public void CancelMatching()
    {
        BackEndMatchManager.Instance.CancelRegistMatchMaking();
    }
}