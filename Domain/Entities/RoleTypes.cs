


public class RoleTypes
{
    public static string TerritoryServant = "TerritoryServant";
    public static string Admin = "Admin";

    public static readonly string[] ValidRoles = new []
    {
        nameof(TerritoryServant), nameof(Admin)
    };    

    public static bool IsValid(string role) => ValidRoles.Contains(role);

    public static string ValidRolesSeparedByColma() => string.Join(", ", ValidRoles);
}