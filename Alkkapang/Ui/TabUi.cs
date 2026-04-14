using UnityEngine;
using UnityEngine.UI;

public class TabUi : MonoBehaviour
{
    public GameObject[] tab;
    public Image[] tapBtnImage;
    public Sprite[] idleSprite, selecetSprite;



    void Start() => TabClick(0);
   

   public void TabClick(int _n)
    {
        for(int i=0;i<tab.Length;i++)
        {
            tab[i].SetActive(i ==_n);
            tapBtnImage[i].sprite = i == _n ? selecetSprite[i] : idleSprite[i];
        }
    }
}
