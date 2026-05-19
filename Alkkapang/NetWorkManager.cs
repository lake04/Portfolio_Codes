using BackEnd;
using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BackEndMatchManager;

public class NetWorkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetWorkManager Instance;
    public static NetworkRunner runnerInstance;

    [Header("References")]
    [SerializeField] private NetworkObject stonePrefab;
    [SerializeField] private GameRuleManager ruleManager;
    [SerializeField] private TeamManager teamManager;

    private StoneLauncher _myLocalLauncher;
    private NetworkSceneManagerDefault _sceneManager;
    private int _localCharacterId;
    private bool _hasLocalCharacterId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

     
    }

    private void Start()
    {
        if (runnerInstance == null)
            runnerInstance = GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();

        runnerInstance.ProvideInput = true;
        runnerInstance.AddCallbacks(this);

        var physicsSimulation = runnerInstance.GetComponent<RunnerSimulatePhysics2D>() ?? runnerInstance.gameObject.AddComponent<RunnerSimulatePhysics2D>();
        physicsSimulation.ClientPhysicsSimulation = ClientPhysicsSimulation.Disabled;

        Application.runInBackground = true;
    }

    /// <summary>
    /// 뒤끝 매치메이킹 완료 후 포톤 게임 세션에 접속
    /// </summary>
    public async void StartMatchGame(string sessionName, MatchInfo matchInfo, int requestedCharacterId = -1)
    {
        if (_sceneManager == null)
        {
            _sceneManager = GetComponent<NetworkSceneManagerDefault>() ?? gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        string safeSessionName = sessionName.Trim().Replace(" ", "");
        Debug.Log($"[NetworkManager] 포톤 진입 시도 - 세션명: {safeSessionName}");

        BackEndMatchManager.Instance.isConnectMatchServer = false;

        await System.Threading.Tasks.Task.Delay(500);

        CharacterDataManager characterManager = await EnsureCharacterDataManagerReady();

        int myCharacterId = requestedCharacterId;
        if (myCharacterId < 0 && characterManager != null)
            myCharacterId = characterManager.GetEquippedCharacterId();

        if (myCharacterId < 0)
        {
            Debug.LogWarning("[NetworkManager] Equipped character id missing. Falling back to CharacterID 0.");
            myCharacterId = 0;
        }

        if (characterManager != null && characterManager.GetMaster(myCharacterId) == null)
        {
            Debug.LogWarning($"[NetworkManager] CharacterID {myCharacterId} is not loaded from backend master data.");
        }

        byte[] connectionToken = BitConverter.GetBytes(myCharacterId);
        _localCharacterId = myCharacterId;
        _hasLocalCharacterId = true;
        Debug.Log($"[NetworkManager] 연결 토큰 생성 - CharacterID: {myCharacterId} / Token: {BitConverter.ToString(connectionToken)}");
        var result = await runnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = safeSessionName,
            PlayerCount = int.Parse(matchInfo.headCount),
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = _sceneManager,
            ConnectionToken = connectionToken
        });

        if (result.Ok)
        {
            Debug.Log("[NetworkManager] 포톤 세션 진입 성공");
            BackEndMatchManager.Instance.LeaveMatchServer();
        }
        else
        {
            Debug.LogError($"[NetworkManager] 포톤 진입 실패: {result.ShutdownReason}");
            BackEndMatchManager.Instance.isConnectMatchServer = true;
        }
    }

    public void RegisterLocalStone(StoneLauncher launcher)
    {
        _myLocalLauncher = launcher;
        Debug.Log("[NetworkManager] 로컬 스톤(Launcher) 등록 완료");
    }

    #region INetworkRunnerCallbacks

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 서버(호스트)에서만 스폰 처리를 수행
        if (runner.IsServer)
        {
            if (runner.TryGetPlayerObject(player, out NetworkObject existingObject) && existingObject != null)
                return;

            if (stonePrefab == null || ruleManager == null || teamManager == null)
            {
                Debug.LogError("[NetworkManager] Spawn dependency missing. Check stonePrefab/ruleManager/teamManager references.");
                return;
            }

            int teamID = teamManager.AssignTeam(player);
            if (teamID < 0)
                return;

            int targetCharacterIndex = ResolveCharacterIdForPlayer(runner, player);

            Vector3 spawnPos = ruleManager.GetRespawnPosition(teamID);

            NetworkObject playerObj = runner.Spawn(stonePrefab, spawnPos, Quaternion.identity, player);

            if (playerObj != null && playerObj.TryGetComponent(out Stone stone))
            {
                runner.SetPlayerObject(player, playerObj);

                stone.SetCharacterIndex(targetCharacterIndex);
                stone.SetTeam(teamID);
                stone.SetReady(true);

                ruleManager.RegisterParticipant(stone);
                

                Debug.Log($"[NetworkManager] 플레이어 스폰 완료: {player} / 팀: {teamID}");
            }
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_myLocalLauncher != null)
        {
            input.Set(_myLocalLauncher.GetLocalInput());
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer && teamManager != null)
        {
            teamManager.RemovePlayer(player);
            Debug.Log($"[NetworkManager] 플레이어 퇴장: {player}");
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    // 에러 및 상태 로그
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) => Debug.LogWarning($"Shutdown: {shutdownReason}");
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) => Debug.LogWarning($"Disconnected: {reason}");
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) => Debug.LogError($"Connect Failed: {reason}");

    #region Unused Callbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
    #endregion

    private async System.Threading.Tasks.Task<CharacterDataManager> EnsureCharacterDataManagerReady()
    {
        CharacterDataManager characterManager = CharacterDataManager.Instance;
        if (characterManager == null)
        {
            GameObject managerObject = new GameObject("CharacterDataManager_Runtime");
            characterManager = managerObject.AddComponent<CharacterDataManager>();
            Debug.LogWarning("[NetworkManager] CharacterDataManager missing. Runtime instance created for backend character data.");
        }

        await characterManager.InitializeDatabaseAsync();
        return characterManager;
    }

    private int ResolveCharacterIdForPlayer(NetworkRunner runner, PlayerRef player)
    {
        byte[] token = runner.GetPlayerConnectionToken(player);
        if (token != null && token.Length == 4)
        {
            int characterId = BitConverter.ToInt32(token, 0);
            Debug.Log($"[NetworkManager] Character token resolved / player:{player} / characterId:{characterId}");
            return characterId;
        }

        if (_hasLocalCharacterId && player == runner.LocalPlayer)
        {
            Debug.LogWarning($"[NetworkManager] Missing character token for local player {player}. Using local CharacterID {_localCharacterId}.");
            return _localCharacterId;
        }

        Debug.LogWarning($"[NetworkManager] Missing character token for player {player}. Falling back to CharacterID 0.");
        return 0;
    }
}

public static class NetworkManager
{
    public static NetWorkManager Instance => NetWorkManager.Instance;
    public static NetworkRunner runnerInstance => NetWorkManager.runnerInstance;
}
