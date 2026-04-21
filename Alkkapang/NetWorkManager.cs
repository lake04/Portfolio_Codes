using BackEnd;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BackEndMatchManager;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runnerInstance;
    public static NetworkManager Instance;

    public string roomNamePrefix = "Room_";

    public SessionListUiHandler sessionListHandler;

    private StoneLauncher _myLocalLauncher;

    [SerializeField] private NetworkObject stonePrefab;
    [SerializeField] private GameRuleManager ruleManager;
    [SerializeField] private TeamManager teamManager;

    private NetworkSceneManagerDefault sceneManager;
 

    private void Awake()
    {
        if (Instance == null)
        {
            transform.SetParent(null);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        runnerInstance = GetComponent<NetworkRunner>();
        runnerInstance.AddCallbacks(this);

        Application.runInBackground = true;
    }

    public async void StartMatchGame(string sessionName, int index, MatchInfo matchInfo)
    {
        if (sceneManager == null)
        {
            sceneManager = GetComponent<NetworkSceneManagerDefault>();
            if (sceneManager == null) sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        if (runnerInstance == null)
        {
            runnerInstance = GetComponent<NetworkRunner>();
            runnerInstance.AddCallbacks(this);
            Debug.Log("Runner Instance 생성됨");
        }

        string safeSessionName = sessionName.Trim().Replace(" ", "");
        Debug.Log($"포톤 진입 시도 - 고유 세션명: {safeSessionName}, 설정 인원: {matchInfo.headCount}");

        BackEndMatchManager.Instance.isConnectMatchServer = false;

        await System.Threading.Tasks.Task.Delay(500);
        var result = await runnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = safeSessionName,
            PlayerCount = int.Parse(matchInfo.headCount),
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });

        if (result.Ok)
        {
            Debug.Log("포톤 세션 진입 성공");
            BackEndMatchManager.Instance.LeaveMatchServer();
        }
        else
        {
            Debug.LogError($"포톤 진입 실패: {result.ShutdownReason}");
            BackEndMatchManager.Instance.isConnectMatchServer = true;
        }
    }

    public void JoinSelectedSession(SessionInfo info)
    {
        //sessionListHandler.OnLookingSessionFound();
        //StartGame(GameMode.Client, info.Name);
    }

    public async void RefreshSessionList()
    {
        if (runnerInstance.IsRunning)
        {
            await runnerInstance.Shutdown();
        }

        if (runnerInstance == null)
        {
            runnerInstance = gameObject.GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();
        }

        //sessionListHandler.OnLookingSessionFound();

    }

    #region INetworkRunnerCallbacks 기본 함수들
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer && player == runner.LocalPlayer)
        {
            Debug.Log("Host 들어옴");
        }
        else
        {
            Debug.Log("Client 들어옴");
        }
        if (runner.IsServer)
        {
            int teamID = teamManager.AssignTeam(player);
            Vector3 spawnPos = ruleManager.GetRespawnPosition(teamID);

            NetworkObject playerObj = runner.Spawn(stonePrefab, spawnPos, Quaternion.identity, player);
            GameManager.instance.stone = playerObj.gameObject;
            DontDestroyOnLoad(playerObj);

            if (playerObj != null && playerObj.TryGetComponent(out Stone stone))
            {
                runner.SetPlayerObject(player, playerObj);
                stone.SetTeam(teamID);
                stone.SetReady(true);

                Debug.Log($"[AutoConnect] SetReady 완료 / player:{player} / ready:{stone.IsReady}");

                ruleManager.RegisterParticipant(stone);

                Debug.Log($"[AutoConnect] 스폰 완료 / player:{player} / team:{teamID}");
            }
        }
    }

    public void RegisterLocalStone(StoneLauncher launcher)
    {
        _myLocalLauncher = launcher;
        Debug.Log("[AutoConnect] 로컬 스톤 등록 완료");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
            return;

        if (teamManager != null)
            teamManager.RemovePlayer(player);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_myLocalLauncher != null)
            input.Set(_myLocalLauncher.GetLocalInput());
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if(sessionListHandler !=null)
        {
            sessionListHandler.ClearList();

            if (sessionList.Count == 0)
            {
                sessionListHandler.OnNoSessionFound();
            }
            else
            {
                foreach (SessionInfo session in sessionList)
                {
                    sessionListHandler.AddToList(session);
                }
            }
        }
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }


    #endregion
}

