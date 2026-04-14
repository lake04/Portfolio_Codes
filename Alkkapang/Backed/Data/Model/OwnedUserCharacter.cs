using BACKND.Database;

[Table("owned_user_character", TableType.UserTable, ClientAccess = true, ReadPermissions = new[] { TablePermission.SELF }, WritePermissions = new[] { TablePermission.SELF })]
public class OwnedUserCharacter : BaseModel
{
    [PrimaryKey]
    [Column("id", DatabaseType.Int32, NotNull = true, DefaultValue = "none")]
    public int Id { get; set; } = 0;

    [Column("user_character", DatabaseType.String, NotNull = true, DefaultValue = "none")]
    public string UserCharacter { get; set; } = "none";

    [Column("is_equipped", DatabaseType.Bool, NotNull = true, DefaultValue = "false")]
    public bool IsEquipped { get; set; } = false;
}