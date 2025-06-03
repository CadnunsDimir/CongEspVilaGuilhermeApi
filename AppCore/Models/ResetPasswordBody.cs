namespace CongEspVilaGuilhermeApi.AppCore.Models
{
    public class ResetPasswordBody
    {
        public required string NewPassword { get; set; }
        public required string ResetPasswordId { get; set; }
    }
}
