using DG.Tweening;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    public static FadeInOut instance;

    [SerializeField] private Image fade_Img;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string nextScene;
    [SerializeField] private int     nextSceneInt;

    private RectTransform rect;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //transform.SetParent(null);
            //DontDestroyOnLoad(gameObject);

            if (fade_Img != null) fade_Img.color = new Color(0, 0, 0, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScrene(string _screne)
    {
        nextScene = _screne;
        Fade();
    }

    public void Fade()
    {
        fade_Img.DOFade(1, fadeDuration)
        .OnComplete(() => {
            var runner = NetworkManager.runnerInstance;

            if (runner != null && runner.IsRunning)
            {
                Debug.Log($"멀티: {nextScene}");
                runner.LoadScene(nextScene);
            }
            else
            {
                Debug.Log($"일반 {nextScene}");
                SceneManager.LoadScene(nextScene);
            }

            //fade_Img.DOFade(0, fadeDuration).SetDelay(0.5f);
        });
    }

}
