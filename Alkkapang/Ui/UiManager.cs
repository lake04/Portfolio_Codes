using BackEnd;
using BackEnd.Tcp;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CustomBackEnd.BackendLogin;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [Header("Buttons")]
    [SerializeField] private Button settingButtom;

    [SerializeField] private GameObject popup;
    [SerializeField] private RectTransform chaerterSelect;

    private Stack<GameObject> uiStack = new Stack<GameObject>();

    [SerializeField] private GameObject matchingStatusPanel; 
    [SerializeField] private Text statusText;

    private GameObject prePopup;
    [SerializeField] private GameObject detailUi;

    [Header("View")]
    [SerializeField] private CharacterDetailView characterDetailView;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            BackendLogin.Instance.Logout();
            SceneManager.LoadScene("Title");
        }
    }


    public void PushUi(GameObject uiPrefab)
    {
        if (uiPrefab.activeSelf) return;

        uiPrefab.SetActive(true);
        uiStack.Push(uiPrefab);
    }

    public void CloseTopUi()
    {
        if (uiStack.Count > 0)
        {
            GameObject top = uiStack.Pop();
            top.SetActive(false);
        }
    }

    public void Popup(GameObject popup)
    {
        if (prePopup == popup && popup.activeSelf)
        {
            CloseTopUi();
            prePopup = null;
            return;
        }

        if (uiStack.Count > 0)
        {
            CloseTopUi();
        }

        PushUi(popup);
        prePopup = popup;
    }

    public void FriendPopup(GameObject friendPopup)
    {
        if (uiStack.Count > 0)
        {
            CloseTopUi();
        }
        else
        {
            PushUi(friendPopup);
            BackendFriend.Instance.UpdateRecommendFirend();
        }
    }

    public void DetailPopup(int characterId, CharacterUiData currentUiData)
    { 
        Popup(detailUi);

        CharacterMasterData masterData = CharacterDataManager.Instance.GetMaster(characterId);

        characterDetailView.SetData(masterData, currentUiData);
    }

   

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
