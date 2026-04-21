using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUi : NetworkBehaviour
{
    public Image[] characterSelects;

    public GameObject[] characters;
    public StoneData[]  stoneDatas;
    [SerializeField] private int selectCharacter = 0;

    public Image teammateSelectImg;
    public Text statusText;
    [SerializeField] private GameObject meanualOffButton;
    [SerializeField] private GameObject meanualOnButton;


    Stone stone;

    void Start()
    {
        for (int i = 1; i < characters.Length; i++)
        {
                characters[i].SetActive(false);
        }

        for (int i = 1; i < characterSelects.Length; i++)
        {
            characterSelects[i].enabled = false;
        }

        if(GameManager.instance.stone != null)
        {
            stone = GameManager.instance.stone.GetComponent<Stone>();
        }

        meanualOffButton.GetComponent<Button>().onClick.AddListener(() => MeanualSetAtive());
        meanualOnButton.GetComponent<Button>().onClick.AddListener(() => MeanualSetAtive());

    }

    void Update()
    {
        statusText.text = $"방 이름: {NetworkManager.runnerInstance.SessionInfo.Name}\n" +
                          $"인원: {NetworkManager.runnerInstance.SessionInfo.PlayerCount}/{NetworkManager.runnerInstance.SessionInfo.MaxPlayers}" +
                          $"플레이어: {NetworkManager.runnerInstance.LocalPlayer}";
    }

    public void NextCharacter()
    {
        UpdateCharacterSelection(1);
    }

    public void PreviousCharacter()
    {
        UpdateCharacterSelection(-1);
    }

    private void UpdateCharacterSelection(int direction)
    {
        characters[selectCharacter].SetActive(false);
        characterSelects[selectCharacter].enabled = false;

        selectCharacter = (selectCharacter + direction + characters.Length) % characters.Length;
        GameManager.instance.selectCharacter = selectCharacter;

        characters[selectCharacter].SetActive(true);
        characterSelects[selectCharacter].enabled = true;

        stone.data = stoneDatas[selectCharacter];

        RPC_UpdateSelection(NetworkManager.runnerInstance.LocalPlayer, selectCharacter);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateSelection(PlayerRef sender, int _charIndex)
    {
        if (sender == NetworkManager.runnerInstance.LocalPlayer)
        {
            Debug.Log("snder랑 같음");
            return;
        }

        var myObject = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);
        var senderObj = NetworkManager.runnerInstance.GetPlayerObject(sender);
       
        if (myObject != null && senderObj !=null)
        {

            //int myTeam = myObject.GetComponent<testPlayer>().teamID;
            //int senderTeam = senderObj.GetComponent<testPlayer>().teamID;
            
            SpriteRenderer sr = characters[_charIndex].GetComponent<SpriteRenderer>();
            //if (myTeam == senderTeam)
            //{
            //    if (sr != null && teammateSelectImg != null)
            //    {
            //        teammateSelectImg.sprite = sr.sprite;
            //        Color color = sr.color;
            //        teammateSelectImg.color = new Color(color.r, color.g, color.b, 0.5f);
            //    }
            //}
        }
    }

    public void Confirmed()
    {
        OnReadyButtonClick();

        RPC_ConfirmedSelectPlayer(NetworkManager.runnerInstance.LocalPlayer);
    }

    private void MeanualSetAtive()
    {
        if (meanualOffButton.activeSelf)
        {
            meanualOffButton.SetActive(false);
            meanualOnButton.SetActive(true);
        }
        else
        {
            meanualOffButton.SetActive(true);
            meanualOnButton.SetActive(false);
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ConfirmedSelectPlayer(PlayerRef sender)
    {
        if (sender == NetworkManager.runnerInstance.LocalPlayer) return;

        Color color = teammateSelectImg.color;
        teammateSelectImg.color = new Color(color.r, color.g, color.b, 1f);

    }

    public void OnReadyButtonClick()
    {
        var myPlayer = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);

        if (myPlayer != null)
        {
            var stone = myPlayer.GetComponent<Stone>();
            //stone.RPC_SetReady(!stone.IsReady);
            //stone.CheckAllPlayersReady();
        }
    }

    
}

