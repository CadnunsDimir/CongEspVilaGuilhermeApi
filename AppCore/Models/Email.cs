namespace CongEspVilaGuilhermeApi.Core.Models
{
    public class Email
    {
        public string? MultiLineMessage { get; init; }
        public string? Subject { get; init; }
        public required string EmailAddress { get; init; }
        public string? HtmlMessage { get; init; }
        public string? Body
        {
            get
            {
                return HtmlMessage ?? MultiLineMessage;
            }
        }

        public bool IsBodyHtml
        {
            get
            {
                return HtmlMessage != null;
            }
        }
    }
}
