using BackEnd;
using BackEnd.Tcp;
using CustomBackEnd.BackendLogin;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private GameObject loadingPanel;
    public SkeletonGraphic homeSkeletonhGraphic;


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



    private void Start()
    {
        loadingPanel.SetActive(true);
        LoadDataAndStartGame().Forget();
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


    private async UniTaskVoid LoadDataAndStartGame()
    {
        await CharacterDataManager.Instance.InitializeDatabaseAsync();

        await CharacterDataManager.Instance.LoadOrCreateUserData();

        int equippedId = CharacterDataManager.Instance.GetEquippedCharacterId();

        CharacterDataManager.Instance.ChangeCharacterHomeModel(homeSkeletonhGraphic, equippedId);
        CharacterUiInstaller.Instance._presenter.RefreshCharacterList();

        Debug.Log("모든 데이터 및 캐릭터 로딩 완료!");

        loadingPanel.SetActive(false);
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
