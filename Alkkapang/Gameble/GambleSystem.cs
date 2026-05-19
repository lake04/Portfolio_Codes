    using BackEnd;
    using Cysharp.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class GambleSystem : MonoBehaviour
    {
        [SerializeField] private GameObject gamblePopup;
        [SerializeField] private Image curCharacterImage;
        [SerializeField] private TMP_Text curCharacterText;

        [SerializeField] private int curGambleCount = 0;
        private bool isTenGamble = false;
        private bool isSingleGamble = false;
        [SerializeField] private Sprite[] characterSprites;
        List<ProbabilityCharacter> characterList = new List<ProbabilityCharacter>(10);

        [Header("Skip Settings")]
        [SerializeField] private GameObject tenGamblePopup;
        [SerializeField] private Image[] characterImages = new Image[10];
        [SerializeField] private bool isSkipPopup = false;
        [SerializeField] private GameObject skipButton;

        private bool isGambling = false;

        private void Update()
        {
            if (isTenGamble && Input.GetMouseButtonDown(0))
            {
                ShowNextCharacter();
            }
            else if (isSingleGamble && Input.GetMouseButtonDown(0))
            {
                isSingleGamble = false;
                gamblePopup.SetActive(false);
                StartCoroutine(GamebleDelay());
            }

            if (isSkipPopup && Input.anyKeyDown)
            {
                isSkipPopup = false;
                tenGamblePopup.SetActive(false);
                isGambling = false;
                StartCoroutine(GamebleDelay());
            }
        }

        /// <summary>
        /// 1»∏ ªÃ±‚
        /// </summary>
        public async void Gamble()
        {
            if (isGambling) return;
            isGambling = true;

            var ucs = new UniTaskCompletionSource<BackendReturnObject>();

            SendQueue.Enqueue(Backend.Probability.GetProbability, "18796", (callback) =>
            {
                ucs.TrySetResult(callback);
            });

            BackendReturnObject bro = await ucs.Task;

            if (bro.IsSuccess())
            {
                StartCoroutine(SingleGambleDelqy());
                isTenGamble = false;
                gamblePopup.SetActive(true);
                if (skipButton != null) skipButton.SetActive(false);

                LitJson.JsonData json = bro.GetFlattenJSON();
                ProbabilityCharacter item = new ProbabilityCharacter();

                item.itemID = json["elements"]["character_ID"].ToString();
                item.characterKey = json["elements"]["character_Key"].ToString();
                item.itemName = json["elements"]["character_Name"].ToString();
                item.rating = json["elements"]["rating"].ToString();
                item.num = int.Parse(json["elements"]["num"].ToString());

                await CharacterDataManager.Instance.AddCharacterByKeyAsync(item.characterKey);
                curCharacterImage.preserveAspect = true;
                curCharacterImage.sprite = characterSprites[item.num - 1];
                curCharacterText.text = $"{item.itemName}";
                CharacterUiInstaller.Instance._presenter.RefreshCharacterList();
            }
            else
            {
                Debug.LogError("¥‹¿œ ªÃ±‚ Ω«∆–: " + bro.ToString());
                isGambling = false;
            }
        }

        /// <summary>
        /// 10ø¨º” ªÃ±‚
        /// </summary>
        public async void TenGamble()
        {
            if (isGambling) return;
            isGambling = true;
            var bro = Backend.Probability.GetProbabilitys("18796", 10);

            if (bro.IsSuccess())
            {
                curGambleCount = 0;
                characterList.Clear();
                StartCoroutine(TanGambleDelqy());
                isSingleGamble = false;
                isSkipPopup = false;

                gamblePopup.SetActive(true);

                if (skipButton != null) skipButton.SetActive(true);

                LitJson.JsonData json = bro.GetFlattenJSON()["elements"];

                for (int i = 0; i < json.Count; i++)
                {
                    ProbabilityCharacter character = new ProbabilityCharacter();

                    character.itemID = json[i]["character_ID"].ToString();
                    character.characterKey = json[i]["character_Key"].ToString();
                    character.itemName = json[i]["character_Name"].ToString();
                    character.rating = json[i]["rating"].ToString();
                    character.num = int.Parse(json[i]["num"].ToString());

                    await CharacterDataManager.Instance.AddCharacterByKeyAsync(character.characterKey);
                    characterList.Add(character);
                }

                CharacterUiInstaller.Instance._presenter.RefreshCharacterList();

                ShowNextCharacter();
            }
            else
            {
                Debug.LogError("10ø¨º” ªÃ±‚ Ω«∆–: " + bro.ToString());
                isGambling =false;
            }
        }

        /// <summary>
        /// 10ø¨ªÃ ≈¨∏Ø ø¨√‚ ∑Œ¡˜ 
        /// </summary>
        private void ShowNextCharacter()
        {
            if (curGambleCount >= characterList.Count)
            {
                isTenGamble = false;
                gamblePopup.SetActive(false);
                curGambleCount = 0;
                isGambling = false;
                return;
            }
            curCharacterImage.preserveAspect = true;
            ProbabilityCharacter character = characterList[curGambleCount];
            curCharacterImage.sprite = characterSprites[character.num - 1];
            curCharacterText.text = $"{character.itemName}";

            curGambleCount++;
        }

        /// <summary>
        /// 10ø¨ªÃ Ω∫≈µ πˆ∆∞
        /// </summary>
        public void Skip()
        {
            isTenGamble = false;
            gamblePopup.SetActive(false);
            tenGamblePopup.SetActive(true);
            isGambling = false;
            StartCoroutine(SkipDelay());

            for (int i = 0; i < characterList.Count; i++)
            {
                if (i >= characterImages.Length)
                {
                    isGambling = false;
                    break;
                }
                 characterImages[i].preserveAspect = true;
                 characterImages[i].sprite = characterSprites[characterList[i].num - 1];
            }
        }

        private IEnumerator SkipDelay()
        {
            yield return new WaitForSeconds(0.3f);
            isSkipPopup = true;
        }

        public void SkipPopup()
        {
            if (tenGamblePopup.activeSelf)
            {
                StopAllCoroutines();
                tenGamblePopup.SetActive(false);
                isSkipPopup = false;
                isGambling = false;
            }
            else
            {
                tenGamblePopup.SetActive(true);
            }
        }

        private IEnumerator GamebleDelay()
        {
            yield return new WaitForSeconds(0.2f);
            isGambling = false;
        }

        private IEnumerator SingleGambleDelqy()
        {
            yield return new WaitForSeconds(0.2f);
            isSingleGamble = true;
        }
        private IEnumerator TanGambleDelqy()
        {
            yield return new WaitForSeconds(0.2f);
            isTenGamble = true;
        }

}
