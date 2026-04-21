using BackEnd;
using BackEnd.BackndLitJson;
using System.Collections.Generic;
using UnityEngine;

public class BackendFriend : Singleton<BackendFriend>
{
    [SerializeField] private GameObject friendItemObject;
    [SerializeField] private Transform userListGameObject;

    private int level = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            RegistrationFriendInfo();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            UpdateRecommendFirend();
        }
    }

    public void GetUserInfoByNickNameTest()
    {
        string userNickname = "lake";

        var bro = Backend.Social.GetUserInfoByNickName(userNickname);

        if (!bro.IsSuccess())
        {
            Debug.LogError(bro.ToString());
            return;
        }

        LitJson.JsonData json = bro.GetReturnValuetoJSON();

        var row = json["row"];

        SearchUserItem userInfo = new SearchUserItem();

        userInfo.nickname = row["nickname"].ToString();
        userInfo.inDate = row["inDate"].ToString();
        userInfo.lastLogin = row["lastLogin"].ToString();

        userInfo.guildName = row.ContainsKey("guildName") && row["guildName"] != null
                             ? row["guildName"].ToString() : null;

        userInfo.countryCode = row.ContainsKey("countryCode") && row["countryCode"] != null
                               ? row["countryCode"].ToString() : null;

        userInfo.propertyGroup = row.ContainsKey("propertyGroup") && row["propertyGroup"] != null
                                 ? row["propertyGroup"].ToString() : null;

        Debug.Log(userInfo.ToString());
    }

    public void GetFriendListTest()
    {
        var bro = Backend.Friend.GetFriendList();

        if (!bro.IsSuccess())
            return;

        LitJson.JsonData json = bro.FlattenRows();
        List<FriendItem> freindList = new List<FriendItem>();

        for (int i = 0; i < json.Count; i++)
        {
            FriendItem friendItem = new FriendItem();

            if (json[i].ContainsKey("nickname"))
            {
                friendItem.nickname = json[i]["nickname"].ToString();
            }
            friendItem.inDate = json[i]["inDate"].ToString();
            friendItem.lastLogin = json[i]["lastLogin"].ToString();
            friendItem.createdAt = json[i]["createdAt"].ToString();

            freindList.Add(friendItem);
            Debug.Log(friendItem.ToString());
        }
    }

    public void RegistrationFriendInfo()
    {
        int profile_id = Random.Range(0, 3);

        Param param = new Param();
        param.Add("profile_id", profile_id);
        param.Add("nickName", Backend.UserNickName);

        var bro = Backend.GameData.Insert("PROFILE", param);

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("데이터 등록 중 에러가 발생했습니다. " + bro);
            return;
        }

         bro = Backend.RandomInfo.SetRandomData(RandomType.User, "a24f6130-024b-11f1-9314-6b53a238f74b", level);

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("랜덤 데이터 등록 중 에러가 발생했습니다. : " + bro);
        }
        else
        {
            Debug.Log("랜덤 데이터 등록");
        }
    }

    public void UpdateRecommendFirend()
    {
        var bro = Backend.RandomInfo.GetRandomData(RandomType.User, "a24f6130-024b-11f1-9314-6b53a238f74b", level, 5, 10);

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("랜덤 조회중 에러가 발생했습니다. : " + bro);
        }

        List<TransactionValue> transactionValues = new List<TransactionValue>();

        for (int i = 0; i < bro.Rows().Count; i++)
        {
            Where where = new Where();

            //2. 트랜잭션 리스트에 where.Equal(”owner_inDate”, 유저 inDate)를 가진 Get 트랜잭션 추가
            where.Equal("owner_inDate", bro.Rows()[i]["gamerInDate"].ToString());

            transactionValues.Add(TransactionValue.SetGet("PROFILE", where));

            if (transactionValues.Count > 10)
            {
                break;
            }
        }


        // 3. 트랜잭션 읽기 실행
        bro = Backend.GameData.TransactionReadV2(transactionValues);

        if (bro.IsSuccess())
        {
            foreach (LitJson.JsonData gameDataJson in bro.GetFlattenJSON()["Responses"])
            {
                Debug.Log("랜덤 유저 불러오기가 완료되었습니다.");

                //JsonData finalData = gameDataJson;
                //if (gameDataJson.ContainsKey("row"))
                //{
                //    finalData = gameDataJson["row"];
                //}

                var friendObject = Instantiate(friendItemObject, userListGameObject.transform);

                if (friendObject.GetComponent<FriendInfo>().Initialize(gameDataJson) == false)
                {
                    Debug.LogError("친구 아이템 초기화에 실패했습니다.");
                    Destroy(friendObject);
                }
            }

        }
        else
        {
            Debug.LogError("트랜잭션 읽어오기 도중 에러가 발생했습니다." + bro);
        }
    }


}
