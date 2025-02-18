
namespace CongEspVilaGuilhermeApi
{
    public class Settings
    {
        public static string TsvUrl { get; private set; } = string.Empty;
        public static string PasswordHashSecret { get; private set; } = string.Empty;
        public static string EmailAddress { get; private set; } = string.Empty;
        public static string EmailPassword { get; private set; } = string.Empty;
        public static string EmailServerHost { get; private set; } = string.Empty;
        public static string TokenSecret { get; private set; } = string.Empty;
        public static string DynamoDBAccessKey { get; private set; } = string.Empty;
        public static string DynamoDBSecretKey { get; private set; } = string.Empty;
        public static string FrontAppHost { get; private set; } = string.Empty;

        public static void LoadFromConfigFiles(IConfiguration configuration)
        {
            PasswordHashSecret = configuration["Auth:PasswordHashSecret"]!;
            EmailAddress = configuration["Email:Address"]!;
            EmailPassword = configuration["Email:Password"]!;
            EmailServerHost = configuration["Email:Host"]!;
            TokenSecret = configuration["Auth:TokenSecret"]!;
            DynamoDBAccessKey = configuration["DynamoDB:accessKey"]!;
            DynamoDBSecretKey = configuration["DynamoDB:secretKey"]!;
            FrontAppHost = configuration[nameof(FrontAppHost)]!;
            TsvUrl = configuration[nameof(TsvUrl)]!;
        }
    }
}