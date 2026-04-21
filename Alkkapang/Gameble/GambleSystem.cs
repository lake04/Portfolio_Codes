using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GambleSystem : MonoBehaviour
{
    [SerializeField] private GameObject gamblePopup;
    [SerializeField] private Image curCharacterImgae;
    [SerializeField] private TMP_Text curCharacterText;

    [SerializeField] private int curGamebleCount = 0;
    private bool isTanGameble = false;
    [SerializeField] private Sprite[] characterSprites;
    List<ProbabilityCharacter> characterList = new List<ProbabilityCharacter>(10);

    [Header("Skip Settings")]
    [SerializeField] private GameObject tanGamble;
    [SerializeField] private Image[] characterimages = new Image[10];
    [SerializeField] private bool isSkilpPopup = false;

    private void Update()
    {
        if(isTanGameble)
        {
            gamblePopup.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                if(curGamebleCount >9)
                {
                    isTanGameble = false;
                    characterList.Clear();
                    gamblePopup.SetActive(false);

                    curGamebleCount = 0;
                    return;
                }
                ProbabilityCharacter character = characterList[curGamebleCount];                                                    
                curCharacterImgae.sprite = characterSprites[character.num - 1];
                curCharacterText.text = $"{character.itemName}";
                curGamebleCount++;

            }
        }
        if(isSkilpPopup)
        {
            if(Input.anyKeyDown)
            {
                isSkilpPopup = false;
                tanGamble.SetActive(false);
            }
        }
    }

    public void Gameble()
    {
        var bro = Backend.Probability.GetProbability("18796");

        if (bro.IsSuccess())
        {
            var selectedItem = bro.GetReturnValue();
            Debug.Log($"»ÌÈù Ä³¸¯ÅÍ: {selectedItem}");

            LitJson.JsonData json = bro.GetFlattenJSON();

            ProbabilityCharacter item = new ProbabilityCharacter();

            item.itemID = json["elements"]["character_ID"].ToString();
            item.characterKey = json["elements"]["character_Key"].ToString();
            item.itemName = json["elements"]["character_Name"].ToString();
            item.rating = json["elements"]["rating"].ToString();
            item.num = int.Parse(json["elements"]["num"].ToString());

            CharacterDataManager.Instance.AddCharacterByKey(item.characterKey);
            curCharacterImgae.sprite = characterSprites[item.num - 1];
        }
        else
        {
            Debug.Log("»Ì±â ½ÇÆÐ");
        }
    }

    public void TanGameble()
    {
        isTanGameble = true;
        isSkilpPopup = false;
        curGamebleCount = 0;
        characterList.Clear();

        string selectedProbabilityFileId = "18796";

        var bro = Backend.Probability.GetProbabilitys(selectedProbabilityFileId, 10);

        if (bro.IsSuccess())
        {
            LitJson.JsonData json = bro.GetFlattenJSON()["elements"];

            for (int i = 0; i < json.Count; i++)
            {
                ProbabilityCharacter character = new ProbabilityCharacter();

                character.itemID = json[i]["character_ID"].ToString();
                character.characterKey = json[i]["character_Key"].ToString();
                character.itemName = json[i]["character_Name"].ToString();
                character.rating = json[i]["rating"].ToString();
                character.num = int.Parse(json[i]["num"].ToString());

                CharacterDataManager.Instance.AddCharacterByKey(character.characterKey);
                characterList.Add(character);
            }
        }
        else
        {
            Debug.LogError(bro.ToString());
        }


        foreach (var character in characterList)
        {
            Debug.Log(character.ToString());
        }

    }

    public void Skip()
    {
        StartCoroutine(SkipDelay());
        gamblePopup.SetActive(false);
        tanGamble.SetActive(true);
        isTanGameble = false;

        foreach (var character in characterList)
        {
            if(curGamebleCount > 9)
            {
                break;
            }
            characterimages[curGamebleCount].sprite = characterSprites[character.num - 1];
            curGamebleCount++;
        }
    }

    private IEnumerator SkipDelay()
    {
        yield return new WaitForSeconds(0.3f);
        isSkilpPopup = true;

    }

    public void SkipPopup()
    {
        if(tanGamble.activeSelf)
        {
            StopAllCoroutines();
            tanGamble.SetActive(false);
            isSkilpPopup = false;
        }
        else
        {
            tanGamble.SetActive(true);
        }
    }

}
