using Fusion;
using System;
using UnityEngine;
using UnityEngine.UI;


public class SessionInfoLisUiItem : MonoBehaviour
{
    public Text sessionNameText;
    public Text playerCountText;
    public Button joinButton;

    SessionInfo sessionInfo;

    public event Action<SessionInfo> OnJoinSession;

    public void SetInformoation(SessionInfo _sessionInfo)
    {
        this.sessionInfo = _sessionInfo;

        sessionNameText.text = _sessionInfo.Name;
        playerCountText.text = $"{_sessionInfo.PlayerCount.ToString()}/{_sessionInfo.MaxPlayers.ToString()}" ;

        bool isJoinActive = true;
        if(_sessionInfo.PlayerCount >= _sessionInfo.MaxPlayers)
        {
            isJoinActive = false;
        }

        joinButton.gameObject.SetActive(isJoinActive);
    }

    public void OnClick()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }
}
