using Amazon.DynamoDBv2.Model;
using HtmlAgilityPack;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    public class WebContentService
    {
        public async Task<HtmlNode?> GetAsync(string url)
        {
            using HttpClient client = new();
            var html = await client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc.DocumentNode;
        }
    }
}
