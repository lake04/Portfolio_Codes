public class SearchUserItem
{
    public string nickname;
    public string inDate;
    public string lastLogin;
    public string guildName;
    public string countryCode;
    public string propertyGroup;

    public override string ToString()
    {
        return $"nickname: {nickname}\n" +
        $"inDate: {inDate}\n" +
        $"lastLogin: {lastLogin}\n" +
        $"guildName: {guildName}\n" +
        $"countryCode: {countryCode}\n" +
        $"propertyGroup: {propertyGroup}\n";
    }
};