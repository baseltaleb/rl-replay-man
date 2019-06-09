using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RLReplayMan.Helpers
{
    public static class WebHelper
    {
        public static async Task<List<HtmlNode>> GetNodesFromUrl(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            //var replaysHtml = htmlDocument.DocumentNode.Descendants("div")
            //    .Where(node => node.GetAttributeValue("class", "")
            //    .Equals("actions")).ToList();

            var replaysHtml = htmlDocument.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("actions")).ToList();

            return replaysHtml;
        }
    }
}
