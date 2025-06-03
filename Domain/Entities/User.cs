using Amazon.DynamoDBv2.DocumentModel;

namespace CongEspVilaGuilhermeApi.Domain.Entities
{
    public class User
    {
        internal static readonly string DefaultRole = "Reader";
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }
        public string? ResetPasswordId { get; internal set; }
        public DateTime? ResetPasswordRequestedAt { get; internal set; }

        public void RequestResetPassord()
        {
           ResetPasswordId =  Guid.NewGuid().ToString();
           ResetPasswordRequestedAt = DateTime.UtcNow;
        }

        public bool CanResetPassword(string resetPasswordId)
        {
            if (ResetPasswordRequestedAt == null || this.ResetPasswordId != resetPasswordId)
                return false;
            var timeSinceRequest = DateTime.UtcNow - ResetPasswordRequestedAt.Value;
            return timeSinceRequest.Minutes < 5;
        }
    }
}
