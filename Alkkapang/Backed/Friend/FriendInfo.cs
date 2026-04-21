using BackEnd;
using System;
using UnityEngine;
using UnityEngine.UI;
using BackEnd.BackndLitJson;

public class FriendInfo : MonoBehaviour
{
    [SerializeField] private Image _userProfile;
    [SerializeField] private Text _userName;
    [SerializeField] private Text _userContent;
    [SerializeField] private Text _userLevel;
    [SerializeField] private Button _requestFriendButton;


    string _userInDate = String.Empty;

    public bool Initialize(LitJson.JsonData userDataJson)
    {
        try
        {
            //_userLevel.text = userDataJson["level"].ToString();
            //_userContent.text = userDataJson["content"].ToString();

            _userName.text = userDataJson["nickName"].ToString();

            _requestFriendButton.onClick.AddListener(() => {
                var bro = Backend.Friend.RequestFriend(_userInDate);
                if (bro.IsSuccess())
                {
                    Debug.Log("친구 요청을 보냈습니다");
                }
                else
                {
                    Debug.LogError("친구 요청을 보내지 못했습니다.");
                }

                _requestFriendButton.enabled = true;
                _requestFriendButton.name = "요청됨";
            });

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("데이터 파싱중 에러가 발생하였습니다. : " + e);
            return false;
        }
    }

}
