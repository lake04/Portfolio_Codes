using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RegionTrigger : MonoBehaviour
{
    [SerializeField] private RegionText text;
    [SerializeField] private Image image;
    [SerializeField] private Transform spawnPos;

    [Header("Background Settings")]
    [SerializeField] private GameObject activeBackground;  

    [SerializeField] private GameObject deactiveBackground;

    public static GameObject currentActiveMap;

    [SerializeField] private float inFadeTime;
    [SerializeField] private float outFadeTime;
    [SerializeField] private float waitScene = 1.3f;

    public IEnumerator InvokeImgCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        // 1. 화면 암전
        image.gameObject.SetActive(true);
        image.color = new Color(0, 0, 0, 0);
        yield return image.DOFade(1.0f, inFadeTime).WaitForCompletion();

        yield return new WaitForSeconds(waitScene);

        if (Camera.main != null)
            Camera.main.transform.position = new Vector3(spawnPos.position.x, spawnPos.position.y, -10f);

        if (GameManager.Instance != null && GameManager.Instance.player != null)
            GameManager.Instance.player.transform.position = spawnPos.position;

        // 3. 배경 교체 (자동화 로직)

        if (currentActiveMap != null)
        {
            currentActiveMap.SetActive(false);
        }
        else if (deactiveBackground != null)
        {
            deactiveBackground.SetActive(false);
        }

        if (activeBackground != null)
        {
            activeBackground.SetActive(true);
            currentActiveMap = activeBackground;
        }

        // 4. 지역 이름 출력
        if (text != null) StartCoroutine(text.InvokeTextCoroutine());

        // 5. 화면 밝아짐
        image.DOFade(0f, outFadeTime).OnComplete(() => {
            image.gameObject.SetActive(false);
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(InvokeImgCoroutine());
        }
    }
}