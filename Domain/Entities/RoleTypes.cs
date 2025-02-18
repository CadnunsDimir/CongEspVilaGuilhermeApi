


public class RoleTypes
{
    public const string LifeAndMinistryAdmins = "LifeAndMinistryAdmins";
    public const string TerritoryServant = "TerritoryServant";
    public const string Admin = "Admin";

    public static readonly string[] ValidRoles = new []
    {
        nameof(TerritoryServant), nameof(Admin)
    };    

    public static bool IsValid(string role) => ValidRoles.Contains(role);

    public static string ValidRolesSeparedByColma() => string.Join(", ", ValidRoles);
}