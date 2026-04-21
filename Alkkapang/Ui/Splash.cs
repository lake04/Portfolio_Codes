using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;

    [Space(10)]
    [SerializeField]
    private GameObject logoBackgroundPanel;
    [SerializeField]
    private Image logoImage;

    private void Start()
    {
        StartCoroutine(InvokeLogoCoroutine());
    }

    private IEnumerator InvokeLogoCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        Sequence sequence = DOTween.Sequence();

        logoImage.color = new Color(1, 1, 1, 0);

        logoBackgroundPanel.gameObject.SetActive(true);
        logoImage.gameObject.SetActive(true);
        sequence.Append(logoImage.DOFade(1.0f, 1.1f))
                .Join(logoImage.transform.DOScale(new Vector3(1, 1, 1), 1.5f))
                .Append(logoImage.DOFade(0.0f, 1f))
                .OnComplete(() =>
                {
                    SceneManager.LoadScene(nextSceneName);
                });
    }
}
