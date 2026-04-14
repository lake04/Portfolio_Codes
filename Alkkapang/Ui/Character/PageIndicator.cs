using UnityEngine;
using UnityEngine.UI;

public class PageIndicator : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] public RectTransform indicator;
    [SerializeField] private float minX = -30f;
    [SerializeField] private float maxX = 30f;

    private void Update()
    {
        float t = scrollRect.horizontalNormalizedPosition;
        float x = Mathf.Lerp(minX, maxX, t);

        Vector2 pos = indicator.anchoredPosition;
        pos.x = x;
        indicator.anchoredPosition = pos;
    }
}
