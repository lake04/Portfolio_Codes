using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public Animator fadeAnim;
    [SerializeField] private GameObject setting;


    private void Start()
    {
        fadeAnim.SetBool("On", true);
    }
    
    public void Play()
    {
        //SoundManager.instance.ButtonSound();
        Invoke("PlaySceneChange", 1);

        fadeAnim.SetBool("On", false);
    }

    private void PlaySceneChange()
    {
        SceneManager.LoadScene("Monster");
    }

    public void Opition()
    {
        //SoundManager.instance.ButtonSound();
        //설정 유아이 팝업
        setting.SetActive(!setting.activeSelf);
    }

    public void Credit()
    {
        //SoundManager.instance.ButtonSound();
        //크레딧
    }

    public void Leave()
    {
        //SoundManager.instance.ButtonSound();
        //나가기
        LoadingManager.instance.Loading("Title");
    }
}
