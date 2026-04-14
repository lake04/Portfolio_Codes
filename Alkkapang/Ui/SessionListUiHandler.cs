using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class SessionListUiHandler : MonoBehaviour
{
    public Text statueText;
    public GameObject sessionItemListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    public void ClearList()
    {
        foreach(Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        statueText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo _sessionInfo)
    {
        Debug.Log("dd");
        SessionInfoLisUiItem sessionInfoItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoLisUiItem>();
        sessionInfoItem.SetInformoation(_sessionInfo);

        sessionInfoItem.OnJoinSession += NetworkManager.Instance.JoinSelectedSession;
    }

    private void AddedSessionInfoListUiItem_OnJoinSession(SessionInfo _obj)
    {

    }

    public void OnNoSessionFound()
    {
        statueText.text = "No gane session found";
        statueText.gameObject.SetActive(true);
    }

    public void OnLookingSessionFound()
    {
        statueText.text = "Looking for gmae session";
        statueText.gameObject.SetActive(true);
    }

}
