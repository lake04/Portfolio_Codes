using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneCameraSwich : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Transform spawnPos;


    [SerializeField] private float inFadeTime;
    [SerializeField] private float outFadeTime;
    [SerializeField] private float waitScene = 1.3f;

    public IEnumerator InvokeImgCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        Player.instance.isMove = false;
        // 화면 페이드 인
       
        image.gameObject.SetActive(true);
        image.color = new Color(0, 0, 0, 0);
        yield return image.DOFade(1.0f, inFadeTime).WaitForCompletion();

        yield return new WaitForSeconds(waitScene);
        Player.instance.isMove = true;
        // 화면 페이드 아웃
        image.DOFade(0f, outFadeTime).OnComplete(() => {
            image.gameObject.SetActive(false);
        });

        
        GameManager.Instance.player.transform.position = spawnPos.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(InvokeImgCoroutine());
        }
    }
}
