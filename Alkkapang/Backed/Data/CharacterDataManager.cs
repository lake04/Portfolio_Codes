using BackEnd;
using BACKND.Database;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance { get; private set; }

    public static Client DBClient;
    private bool _initialized = false;
    private Task _initializeTask;

    private Dictionary<int, CharacterMasterData> masterById = new();
    private Dictionary<string, CharacterMasterData> masterByKey = new();
    private UserData userData = new();

    private const string USER_DATA_TABLE = "USER_CHARACTER";

    public List<CharacterSpineAsset> spineAssets = new List<CharacterSpineAsset>();
    public List<CharacterSpineAsset> ingmaeSpineAssets = new List<CharacterSpineAsset>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsOwned(int characterId)
    {
        return userData != null && userData.ownedCharacterIds.Contains(characterId);
    }

    public bool IsOwned(string characterKey)
    {
        if (masterByKey.TryGetValue(characterKey, out var data) == false)
            return false;

        return IsOwned(data.characterId);
    }

    public bool IsEquipped(int characterId)
    {
        return userData != null && userData.equippedCharacterId == characterId;
    }

    public bool IsEquipped(string characterKey)
    {
        if (masterByKey.TryGetValue(characterKey, out var data) == false)
            return false;

        return IsEquipped(data.characterId);
    }

    public CharacterMasterData GetMaster(int characterId)
    {
        masterById.TryGetValue(characterId, out var data);
        return data;
    }

    public CharacterMasterData GetMasterByKey(string characterKey)
    {
        masterByKey.TryGetValue(characterKey, out var data);
        return data;
    }

    public bool IsDataReady => _initialized;

    public async void InitializeDatabase()
    {
        await InitializeDatabaseAsync();
    }

    public Task InitializeDatabaseAsync()
    {
        if (_initializeTask == null)
            _initializeTask = InitializeDatabaseInternal();

        return _initializeTask;
    }

    private async Task InitializeDatabaseInternal()
    {
        if (_initialized) return;

        DBClient = new Client("019c2c62-be25-730e-9ff9-ae397976a561");
        await DBClient.Initialize();

        Debug.Log("데이터베이스 초기화 완료");

        await LoadMasterCharacter();
        await LoadOrCreateUserData();
        _initialized = true;
    }

    #region Load / Create

    public async Task LoadMasterCharacter()
    {
        var characters = await DBClient.From<CharacterMaster>().ToList();

        Debug.Log($"캐릭터 수: {characters.Count}");
        masterById.Clear();
        masterByKey.Clear();

        foreach (var c in characters)
        {
            CharacterMasterData data = new CharacterMasterData(c);
            if (data.characterId < 0)
            {
                Debug.LogWarning($"Invalid character_id from backend. Raw:{c.CharacterId} / name:{data.name}");
                continue;
            }

            masterById[data.characterId] = data;
            masterByKey[data.characterKey] = data;

            Debug.Log($"ID:{data.characterId} / KEY:{data.characterKey} / name:{data.name} / stats W:{data.weight} S:{data.speed} D:{data.defense} P:{data.power} H:{data.handling}");
        }

        Debug.Log($"마스터 캐릭터 로드 완료: {masterById.Count}");
    }

    public async Task LoadOrCreateUserData()
    {
        var ucs = new UniTaskCompletionSource<BackendReturnObject>();

        SendQueue.Enqueue(Backend.GameData.GetMyData, USER_DATA_TABLE, new BackEnd.Where(), (callback) =>
        {
            ucs.TrySetResult(callback);
        });

        BackendReturnObject bro = await ucs.Task;

        if (!bro.IsSuccess())
        {
            Debug.LogError($"유저 데이터 조회 실패 : {bro}");
            return;
        }

        LitJson.JsonData rows = bro.FlattenRows();

        if (rows.Count <= 0)
        {
            Debug.Log("유저 데이터가 없어서 기본값으로 생성합니다.");
            await CreateDefaultUserData();
            return;
        }

        LitJson.JsonData row = rows[0];
        userData = ParseUserData(row);
        Debug.Log($"유저 데이터 로드 완료 / 보유:{userData.ownedCharacterIds.Count} / 장착:{userData.equippedCharacterId}");
    }

    private async Task CreateDefaultUserData()
    {
        userData = new UserData();
        int defaultCharacterId = 10;
        userData.ownedCharacterIds.Add(defaultCharacterId);
        userData.equippedCharacterId = defaultCharacterId;

        Param param = new Param();
        param.Add("ownedCharacterIds", userData.ownedCharacterIds);
        param.Add("equippedCharacterId", userData.equippedCharacterId);

        var ucs = new UniTaskCompletionSource<BackendReturnObject>();

        SendQueue.Enqueue(Backend.GameData.Insert, USER_DATA_TABLE, param, (callback) =>
        {
            ucs.TrySetResult(callback);
        });

        BackendReturnObject bro = await ucs.Task;

        if (bro.IsSuccess())
        {
            userData.inDate = bro.GetInDate();
            Debug.Log("기본 유저 데이터 생성 완료");
        }
        else
        {
            Debug.LogError($"기본 유저 데이터 생성 실패 : {bro}");
        }
    }

    

    private UserData ParseUserData(LitJson.JsonData row)
    {
        UserData data = new UserData();

        if (row.ContainsKey("inDate"))
            data.inDate = row["inDate"].ToString();

        if (row.ContainsKey("equippedCharacterId"))
            data.equippedCharacterId = int.Parse(row["equippedCharacterId"].ToString());

        if (row.ContainsKey("ownedCharacterIds"))
        {
            LitJson.JsonData ownedList = row["ownedCharacterIds"];

            for (int i = 0; i < ownedList.Count; i++)
                data.ownedCharacterIds.Add(int.Parse(ownedList[i].ToString()));
        }

        return data;
    }

    #endregion

    #region Save
    public async Task SaveUserDataAsync()
    {
        if (userData == null || string.IsNullOrEmpty(userData.inDate)) return;

        Param param = new Param();
        param.Add("ownedCharacterIds", userData.ownedCharacterIds);
        param.Add("equippedCharacterId", userData.equippedCharacterId);

        var ucs = new UniTaskCompletionSource<BackendReturnObject>();

        SendQueue.Enqueue(Backend.GameData.UpdateV2, USER_DATA_TABLE, userData.inDate, Backend.UserInDate, param, (callback) =>
        {
            ucs.TrySetResult(callback);
        });

        BackendReturnObject bro = await ucs.Task;

        if (bro.IsSuccess())
            Debug.Log("유저 데이터 저장 완료");
        else
            Debug.LogError($"유저 데이터 저장 실패 : {bro}");
    }

    #endregion

    #region Character Logic

    public async Task<bool> AddCharacterByIdAsync(int characterId)
    {
        if (masterById.ContainsKey(characterId) == false)
        {
            Debug.LogWarning($"존재하지 않는 캐릭터 ID : {characterId}");
            return false;
        }

        if (IsOwned(characterId))
        {
            Debug.Log($"이미 보유중인 캐릭터 : {characterId}");
            return false;
        }

        userData.ownedCharacterIds.Add(characterId);
        await SaveUserDataAsync();

        Debug.Log($"캐릭터 획득 완료 : {characterId}");
        return true;
    }

    public async Task<bool> AddCharacterByKeyAsync(string characterKey)
    {
        if (masterByKey.TryGetValue(characterKey, out var data) == false)
        {
            Debug.LogWarning($"존재하지 않는 캐릭터 Key : {characterKey}");
            return false;
        }

        return await AddCharacterByIdAsync(data.characterId);
    }

    public async Task<bool> EquipCharacterByIdAsync(int characterId)
    {
        if (masterById.ContainsKey(characterId) == false)
        {
            Debug.LogWarning($"존재하지 않는 캐릭터 ID : {characterId}");
            return false;
        }

        if (IsOwned(characterId) == false)
        {
            Debug.LogWarning($"보유하지 않은 캐릭터는 장착할 수 없음 : {characterId}");
            return false;
        }

        if (userData.equippedCharacterId == characterId)
        {
            Debug.Log($"이미 장착 중인 캐릭터 : {characterId}");
            return true;
        }

        userData.equippedCharacterId = characterId;
        await SaveUserDataAsync();

        Debug.Log($"캐릭터 장착 완료 : {characterId}");
        return true;
    }

    public async Task<bool> EquipCharacterByKeyAsync(string characterKey)
    {
        if (masterByKey.TryGetValue(characterKey, out var data) == false)
        {
            Debug.LogWarning($"존재하지 않는 캐릭터 Key : {characterKey}");
            return false;
        }

        return await EquipCharacterByIdAsync(data.characterId);
    }

    #endregion

    public IEnumerable<CharacterMasterData> GetAllCharacters()
    {
        return masterById.Values;
    }

    public int GetEquippedCharacterId()
    {
        return userData != null ? userData.equippedCharacterId : -1;
    }

    public string GetEquippedCharacterKey()
    {
        var master = GetMaster(GetEquippedCharacterId());
        return master != null ? master.characterKey : string.Empty;
    }

    #region Spine
    public void ChangeCharacterHomeModel(SkeletonGraphic targetGraphic, int characterId)
    {
        var targetAsset = spineAssets.Find(a => a.characterId == characterId);
        if (targetAsset != null)
        {
            targetGraphic.Clear();

            targetGraphic.skeletonDataAsset = targetAsset.dataAsset;
            targetGraphic.initialSkinName = "";
            targetGraphic.startingAnimation = "";

            targetGraphic.Initialize(true);

            targetGraphic.material = targetAsset.dataAsset.atlasAssets[0].PrimaryMaterial;
            targetGraphic.OverrideTexture = null;

            string targetSkinName = targetAsset.defaultSkinName;
            var foundSkin = targetGraphic.Skeleton.Data.FindSkin(targetSkinName);

            if (foundSkin != null)
            {
                targetGraphic.Skeleton.SetSkin(foundSkin);
            }
            else
            {
                Debug.LogWarning($"[스킨 없음] '{targetSkinName}' 스킨이 없어 강제로 default 스킨을 적용합니다.");
                targetGraphic.Skeleton.SetSkin("default");
            }

            targetGraphic.Skeleton.SetSlotsToSetupPose();
            string targetAnimName = targetAsset.defaultAnimationName;
            var foundAnim = targetGraphic.Skeleton.Data.FindAnimation(targetAnimName);

            if (foundAnim != null)
            {
                targetGraphic.AnimationState.SetAnimation(0, foundAnim, true);
            }
            else
            {
                if (targetGraphic.Skeleton.Data.Animations.Count > 0)
                {
                    var fallbackAnim = targetGraphic.Skeleton.Data.Animations.Items[0];
                    Debug.LogWarning($"[애니메이션 없음] '{targetAnimName}'을 찾을 수 없어 강제로 '{fallbackAnim.Name}'(을)를 재생합니다!");
                    targetGraphic.AnimationState.SetAnimation(0, fallbackAnim, true);
                }
                else
                {
                    Debug.LogError("이 스파인 에셋에는 애니메이션이 아예 없습니다!");
                }
            }

            Debug.Log($"캐릭터 모델 변경 완료 : {characterId}");
        }
        else
        {
            Debug.LogWarning($"스파인 에셋이 없는 캐릭터 ID : {characterId}");
        }
    }

    public CharacterSpineAsset ChangeCharacterIngameModel(int characterId)
    {
        var targetAsset = ingmaeSpineAssets.Find(a => a.characterId == characterId);
        return targetAsset;
    }

    public CharacterSpineAsset GetCharacterSpineAsset(int characterId)
    {
        var targetAsset = spineAssets.Find(a => a.characterId == characterId);
        return targetAsset;
    }
    #endregion
}