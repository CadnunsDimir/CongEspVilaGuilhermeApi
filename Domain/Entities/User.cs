namespace CongEspVilaGuilhermeApi.Domain.Entities
{
    public class User
    {
        internal static readonly string DefaultRole = "Reader";
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; internal set; }
    }
}
