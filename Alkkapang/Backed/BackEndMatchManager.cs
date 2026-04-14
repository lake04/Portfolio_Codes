using BackEnd;
using BackEnd.Tcp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class BackEndMatchManager : MonoBehaviour {
    public class MatchInfo
    {
        public string title;                // 매칭 명
        public string inDate;               // 매칭 inDate (UUID)
        public MatchType matchType;         // 매치 타입
        public MatchModeType matchModeType; // 매치 모드 타입
        public string headCount;            // 매칭 인원
        public bool isSandBoxEnable;        // 샌드박스 모드 (AI매칭)
    }

    public static BackEndMatchManager Instance;
    public MatchType nowMatchType { get; private set; } = MatchType.None;     // 현재 선택된 매치 타입
    public MatchModeType nowModeType { get; private set; } = MatchModeType.None; // 현재 선택된 매치 모드 타입
    public List<MatchInfo> matchInfos { get; private set; } = new List<MatchInfo>();
    // 디버그 로그
    private string NOTCONNECT_MATCHSERVER = "매치 서버에 연결되어 있지 않습니다.";
    private string RECONNECT_MATCHSERVER = "매치 서버에 접속을 시도합니다.";
    private string FAIL_CONNECT_MATCHSERVER = "매치 서버 접속 실패 : {0}";
    private string SUCCESS_CONNECT_MATCHSERVER = "매치 서버 접속 성공";
    private string SUCCESS_MATCHMAKE = "매칭 성공 : {0}";
    private string SUCCESS_REGIST_MATCHMAKE = "매칭 대기열에 등록되었습니다.";
    private string FAIL_REGIST_MATCHMAKE = "매칭 실패 : {0}";
    private string CANCEL_MATCHMAKE = "매칭 신청 취소 : {0}";
    private string INVAILD_MATCHTYPE = "잘못된 매치 타입입니다.";
    private string INVALID_MODETYPE = "잘못된 모드 타입입니다.";
    private string INVALID_OPERATION = "잘못된 요청입니다\n{0}";
    private string EXCEPTION_OCCUR = "예외 발생 : {0}\n다시 매칭을 시도합니다.";

    public bool isConnectMatchServer = false;
    private bool isConnectInGameServer = false;
    private int curMatchIndex = 0;

    public event Action<bool> OnMatchingStateChanged;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
        {
            Backend.Match.OnJoinMatchMakingServer += (args) => {
                ProcessAccessMatchMakingServer(args.ErrInfo);
            };

            Backend.Match.OnMatchMakingResponse += (args) => {
                ProcessMatchMakingResponse(args);
            };

            Backend.Match.OnMatchMakingRoomCreate += (args) => {
                if (args.ErrInfo == ErrorCode.Success)
                {
                    Debug.Log("방 생성 성공 -> 자동으로 매칭 신청을 시작합니다.");
                    OnMatchingStateChanged?.Invoke(true);
                    RequestMatchMaking(curMatchIndex);
                }
            };
    }

    void Update()
    {
            if (isConnectMatchServer)
            {
                Backend.Match.Poll();
            }
    }

    // 매칭 카드 리스트를 서버에서 불러오는 함수
    public void GetMatchListFromServer(System.Action<bool> onComplete = null)
    {
        matchInfos.Clear();

        Backend.Match.GetMatchList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError("매칭 카드 리스트 불러오기 실패: " + callback);
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log("매칭 카드 리스트 불러오기 성공");

            foreach (LitJson.JsonData row in callback.Rows())
            {
                MatchInfo info = new MatchInfo();
                info.title = row["matchTitle"]["S"].ToString();
                info.inDate = row["inDate"]["S"].ToString();
                info.headCount = row["matchHeadCount"]["N"].ToString();
                info.isSandBoxEnable = row["enable_sandbox"]["BOOL"].ToString().ToLower() == "true";

                // matchType (MMR, Point, Random 등) 파싱
                string typeStr = row["matchType"]["S"].ToString().ToLower();
                foreach (MatchType type in System.Enum.GetValues(typeof(MatchType)))
                {
                    if (type.ToString().ToLower().Equals(typeStr))
                    {
                        info.matchType = type;
                        break;
                    }
                }

                // matchModeType (OneOnOne, TeamOnTeam 등) 파싱
                string modeStr = row["matchModeType"]["S"].ToString().ToLower();
                foreach (MatchModeType mode in System.Enum.GetValues(typeof(MatchModeType)))
                {
                    if (mode.ToString().ToLower().Equals(modeStr))
                    {
                        info.matchModeType = mode;
                        break;
                    }
                }

                matchInfos.Add(info);
                Debug.Log($"불러온 매치: {info.title} ({info.matchType}/{info.matchModeType})");
            }

            onComplete?.Invoke(true);
        });
    }

    public void SetNowMatchInfo(MatchType matchType, MatchModeType matchModeType)
        {
            this.nowMatchType = matchType;
            this.nowModeType = matchModeType;

            Debug.Log(string.Format("매칭 타입/모드 : {0}/{1}", nowMatchType, nowModeType));
        }

        // 매칭 서버 접속
    public void JoinMatchServer()
        {
            if (isConnectMatchServer)
            {
                CreateMatchRoom();  
                return;
            }
            ErrorInfo errorInfo;
            isConnectMatchServer = true;
            if (!Backend.Match.JoinMatchMakingServer(out errorInfo))
            {
                var errorLog = string.Format(FAIL_CONNECT_MATCHSERVER, errorInfo.ToString());
                Debug.Log(errorLog);
            }
        }

    // 매칭 서버 접속종료
    public void LeaveMatchServer()
        {
            isConnectMatchServer = false;
            Backend.Match.LeaveMatchMakingServer();
        }

    // 매칭 대기 방 만들기
    // 혼자 매칭을 하더라도 무조건 방을 생성한 뒤 매칭을 신청해야 함
    public bool CreateMatchRoom()
        {
            // 매청 서버에 연결되어 있지 않으면 매칭 서버 접속
            if (!isConnectMatchServer)
            {
                Debug.Log(NOTCONNECT_MATCHSERVER);
                Debug.Log(RECONNECT_MATCHSERVER);
                JoinMatchServer();
                return false;
            }
            Debug.Log("방 생성 요청을 서버로 보냄");
            Backend.Match.CreateMatchRoom();
            return true;
        }

    // 매칭 대기 방 나가기
    public void LeaveMatchLoom()
        {
            Backend.Match.LeaveMatchRoom();
        }

    // 매칭 신청하기
    // MatchType (1:1 / 개인전 / 팀전)
    // MatchModeType (랜덤 / 포인트 / MMR)
    public void RequestMatchMaking(int index)
        {
            // 매청 서버에 연결되어 있지 않으면 매칭 서버 접속
            if (!isConnectMatchServer)
            {
                Debug.Log(NOTCONNECT_MATCHSERVER);
                Debug.Log(RECONNECT_MATCHSERVER);
                JoinMatchServer();
                return;
            }
            // 변수 초기화
            isConnectInGameServer = false;
            curMatchIndex = index;
            Backend.Match.RequestMatchMaking(matchInfos[index].matchType, matchInfos[index].matchModeType, matchInfos[index].inDate);
            if (isConnectInGameServer)
            {
                Backend.Match.LeaveGameServer(); //인게임 서버 접속되어 있을 경우를 대비해 인게임 서버 리브 호출
            }
            isConnectInGameServer = false;
            Backend.Match.RequestMatchMaking(matchInfos[index].matchType, matchInfos[index].matchModeType, matchInfos[index].inDate);
        }

    // 매칭 신청 취소하기
    public void CancelRegistMatchMaking()
        {
            Backend.Match.CancelMatchMaking();
        }

    // 매칭 서버 접속에 대한 리턴값
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
        {
            if (errInfo != ErrorInfo.Success)
            {
                // 접속 실패
                isConnectMatchServer = false;
            }

            if (!isConnectMatchServer)
            {
                var errorLog = string.Format(FAIL_CONNECT_MATCHSERVER, errInfo.ToString());
                // 접속 실패
                Debug.Log(errorLog);
            }
            else
            {
                //접속 성공
                Debug.Log(SUCCESS_CONNECT_MATCHSERVER);
                CreateMatchRoom();
            }
        }

    /*
	     * 매칭 신청에 대한 리턴값 (호출되는 종류)
	     * 매칭 신청 성공했을 때
	     * 매칭 성공했을 때
	     * 매칭 신청 실패했을 때
	    */
    private void ProcessMatchMakingResponse(MatchMakingResponseEventArgs args)
        {
            string debugLog = string.Empty;
            bool isError = false;
            switch (args.ErrInfo)
            {
                case ErrorCode.Success:
                    // 매칭 성공했을 때
                    debugLog = string.Format(SUCCESS_MATCHMAKE, args.Reason);
                    OnMatchingStateChanged?.Invoke(false);
                    ProcessMatchSuccess(args);
                    break;
                case ErrorCode.Match_InProgress:
                    // 매칭 신청 성공했을 때 or 매칭 중일 때 매칭 신청을 시도했을 때

                    // 매칭 신청 성공했을 때
                    if (args.Reason == string.Empty)
                    {
                        debugLog = SUCCESS_REGIST_MATCHMAKE;
                    }
                    break;
                case ErrorCode.Match_MatchMakingCanceled:
                    // 매칭 신청이 취소되었을 때
                    debugLog = string.Format(CANCEL_MATCHMAKE, args.Reason);
                    OnMatchingStateChanged?.Invoke(false);
                break;
                case ErrorCode.Match_InvalidMatchType:
                    isError = true;
                    // 매치 타입을 잘못 전송했을 때
                    debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVAILD_MATCHTYPE);

                    break;
                case ErrorCode.Match_InvalidModeType:
                    isError = true;
                    // 매치 모드를 잘못 전송했을 때
                    debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVALID_MODETYPE);

                    break;
                case ErrorCode.InvalidOperation:
                    isError = true;
                    // 잘못된 요청을 전송했을 때
                    debugLog = string.Format(INVALID_OPERATION, args.Reason);
                    break;
                case ErrorCode.Match_Making_InvalidRoom:
                    isError = true;
                    // 잘못된 요청을 전송했을 때
                    debugLog = string.Format(INVALID_OPERATION, args.Reason);
                    break;
                case ErrorCode.Exception:
                    isError = true;
                    // 매칭 되고, 서버에서 방 생성할 때 에러 발생 시 exception이 리턴됨
                    // 이 경우 다시 매칭 신청해야 됨
                    debugLog = string.Format(EXCEPTION_OCCUR, args.Reason);

                    break;
            }

            if (!debugLog.Equals(string.Empty))
            {
                if (isError == true)
                {
                
                }
            }
        }

    // 매칭 성공했을 때
    // 인게임 서버로 접속해야 한다.
    private void ProcessMatchSuccess(MatchMakingResponseEventArgs args)
    {
        string fusionSessionName = args.RoomInfo.m_inGameRoomToken;
        MatchInfo matchInfo = matchInfos[curMatchIndex];

        StartCoroutine(JoinPhotonDelayed(fusionSessionName, matchInfo));
    }

    private IEnumerator JoinPhotonDelayed(string sessionName, MatchInfo info)
        {
            Debug.Log("뒤끝 통신 일시 정지 및 포톤 진입 준비...");

            isConnectMatchServer = false;

            yield return new WaitForSeconds(1.5f);

            NetworkManager.Instance.StartMatchGame(sessionName, curMatchIndex, info);
        }
}