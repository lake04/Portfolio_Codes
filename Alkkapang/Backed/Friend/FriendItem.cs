using UnityEngine;

public class FriendItem
{
    public string nickname;
    public string inDate;
    public string lastLogin;
    public string createdAt;
    public override string ToString()
    {
        return $"nickname : {nickname}\ninDate : {inDate}\nlastLogin : {lastLogin}\ncreatedAt : {createdAt}\n";
    }
};
