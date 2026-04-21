using UnityEngine;
using DG.Tweening;

public class TitieLogo : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBack);
    }
}
