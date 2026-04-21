using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // ScrollRect 참조를 위해 추가

public class CharacterDetailScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private const int SIZE = 2;
    private float[] pos = new float[SIZE];
    private float distance;

    public int TargetIndex { get; private set; }
    public float TargetPos { get; private set; }

    [SerializeField] private CharacterDetailView characterDetailView;
    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++)
        {
            pos[i] = distance * i;
        }
    }

    private void Update()
    {
        scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, TargetPos, 0.1f);
    }

    public void UpdateTargetByScrollValue(float scrollValue, float deltaX)
    {
        int newIndex = TargetIndex;

        // 1. 현재 위치에서 가장 가까운 인덱스 찾기
        for (int i = 0; i < SIZE; i++)
        {
            if (scrollValue < pos[i] + distance * 0.5f && scrollValue > pos[i] - distance * 0.5f)
            {
                newIndex = i;
                break;
            }
        }

        if (Mathf.Abs(deltaX) > 5f)
        {
            if (deltaX > 0 && newIndex > 0) newIndex--;
            else if (deltaX < 0 && newIndex < SIZE - 1) newIndex++;
        }

        TargetIndex = newIndex;
        TargetPos = pos[TargetIndex];

        if (characterDetailView != null)
        {
            characterDetailView.RenderTabState(TargetIndex, TargetPos);
        }
    }

    public void OnDrag(PointerEventData eventData) { }
    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UpdateTargetByScrollValue(scrollRect.horizontalNormalizedPosition, eventData.delta.x);
    }

  
}