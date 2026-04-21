using BackEnd;
using CustomBackEnd.BackendLogin;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToucnText : MonoBehaviour
{
    [SerializeField]
    private GameObject login;

    [SerializeField]
    private bool isStart = false;

    private CanvasGroup LoginCanvasGroup;

    private void Awake()
    {
        var bro = Backend.Initialize();

        if (bro.IsSuccess())
        {
            Debug.Log("뒤끝 초기화 성공!");
            SendQueue.StartSendQueue(true);

        }
        else
        {
            Debug.LogError("뒤끝 초기화 실패 : " + bro);
        }
    }

    void Start()
    {
        GetComponent<RectTransform>().DOAnchorPosY(20f, 1f)
            .SetRelative()
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        Touch();

    }

    void Update()
    {
        if (Input.anyKeyDown && isStart)
        {
            gameObject.SetActive(false);
            ShowButton();
            //BackendLogin.Instance.BackendTokenLogin((bool result, string error) =>
            //{ 
            //    if (result)
            //    {
            //        Debug.Log("토큰 로그인 성공 씬 이둥");
            //        SceneManager.LoadScene("MainScene");
            //    }
            //    else
            //    {
            //        Debug.LogWarning($"토큰 로그인 실패 : {error}");
            //        ShowButton();
            //    }
            //});
        }
    }

    private async void Touch()
    {
        await UniTask.WaitForSeconds(0.5f);
        isStart = true;

    }

    private void ShowButton()
    {
        TMP_Text myText = GetComponent<TMP_Text>();
        myText.DOFade(0f, 0.7f).OnComplete(() => gameObject.SetActive(false));

        login.SetActive(true);
        login.transform.localScale = Vector3.zero;
        login.transform.DOScale(1.2f, 0.8f).SetEase(Ease.OutBack);
    }
}
