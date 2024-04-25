namespace CongEspVilaGuilhermeApi.AppCore.Exceptions
{
    public class Error
    {
        public int Status { get; internal set; }
        public string Title { get; internal set; } = String.Empty;
    }
}
