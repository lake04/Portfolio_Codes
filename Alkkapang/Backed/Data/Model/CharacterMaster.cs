using BACKND.Database;

[Table("character_master", TableType.FlexibleTable, ClientAccess = true, ReadPermissions = new[] { TablePermission.SELF, TablePermission.OTHERS }, WritePermissions = new[] {TablePermission.SELF })]
public class CharacterMaster : BaseModel
{
    [PrimaryKey]
    [Column("character_id", DatabaseType.Int32, NotNull = true, DefaultValue = "none")]
    public int CharacterId { get; set; } = 0;

    [Column("name", DatabaseType.String, NotNull = true, DefaultValue = "none")]
    public string Name { get; set; } = "none";

    [Column("weight", DatabaseType.Int32, NotNull = true, DefaultValue = "0")]
    public int Weight { get; set; } = 0;

    [Column("speed", DatabaseType.Int32, NotNull = true, DefaultValue = "0")]
    public int Speed { get; set; } = 0;

    [Column("defense", DatabaseType.Int32, NotNull = true, DefaultValue = "0")]
    public int Defense { get; set; } = 0;

    [Column("power", DatabaseType.Int32, NotNull = true, DefaultValue = "0")]
    public int Power { get; set; } = 0;

    [Column("handling", DatabaseType.Int32, NotNull = true, DefaultValue = "0")]
    public int Handling { get; set; } = 0;

    [Column("role", DatabaseType.String, NotNull = true, DefaultValue = "none")]
    public string Role { get; set; } = "none";

    [Column("ability_id", DatabaseType.String, NotNull = true, DefaultValue = "none")]
    public string AbilityId { get; set; } = "none";

    [Column("ability_desc", DatabaseType.String, NotNull = true, DefaultValue = "none")]
    public string AbilityDesc { get; set; } = "none";

    [Column("icon", DatabaseType.String, NotNull = true, DefaultValue = "none")]
    public string Icon { get; set; } = "none";

    [Column("prefab_name", DatabaseType.String, NotNull = true, DefaultValue = "none")]
    public string PrefabName { get; set; } = "none";

    [Column("rarity", DatabaseType.Int32, NotNull = true, DefaultValue = "1")]
    public int Rarity { get; set; } = 1;

    [Column("is_active", DatabaseType.Bool, NotNull = true, DefaultValue = "true")]
    public bool IsActive { get; set; } = true;

    [Column("sort_order", DatabaseType.Int32, NotNull = true, DefaultValue = "0")]
    public int SortOrder { get; set; } = 0;

    [Column("character_key", DatabaseType.String, NotNull = true, DefaultValue = "none")]
    public string CharacterKey { get; set; } = "none";
}