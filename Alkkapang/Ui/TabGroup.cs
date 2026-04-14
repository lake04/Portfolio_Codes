using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class TabGroup : MonoBehaviour
{
    public List<RectTransform> buttons;
    public List<Image> buttonImages;
    public List<RectTransform> panels;

    [Header("Button Click Setting")]
    [SerializeField] private Color activeColor = Color.cyan;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Vector2 activeScale;
    [SerializeField] private Vector2 originScale;
    [SerializeField] private float duration = 0.2f;

    private int curIndex = 0;

    private void Start()
    {
        foreach(RectTransform b in buttons)
        {
            originScale = b.localScale;
        }
    }

    public void OnTabClick(int _index)
    {
        for(int i=0;i< buttons.Count;i++)
        {
            if(i == _index)
            {
                if(_index == curIndex)
                {
                    break;
                }
                buttons[i].DOScale(activeScale, duration).SetEase(Ease.OutBack);
                buttonImages[i].DOColor(activeColor, duration);
                panels[i].gameObject.SetActive(true);
                if(i <  buttons.Count/2)
                {
                    CharterSelect(i,-1);
                }
                else
                {
                    CharterSelect(i, 1);
                }
                curIndex = i;
            }
            else 
            {
                buttons[i].DOScale(originScale, duration).SetEase(Ease.OutQuad);
                buttonImages[i].DOColor(idleColor, duration);
                panels[i].gameObject.SetActive(false);
            }
        }
    }

    public void CharterSelect(int _index,int _dir)
    {
        panels[_index].anchoredPosition = new Vector2(1080 * _dir, 0);

        Sequence seq = DOTween.Sequence();

        seq.Append(panels[_index].DOAnchorPos(Vector2.zero, 0.4f));
    }
}
