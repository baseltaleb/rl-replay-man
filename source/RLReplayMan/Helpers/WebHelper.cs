using HtmlAgilityPack;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using WpfTreeView;

namespace RLReplayMan
{
    public static class WebHelper
    {
        public static async Task<ObservableCollection<FileItemViewModel>> GetReplaysFromUrl(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            ObservableCollection<FileItemViewModel> replayList = new ObservableCollection<FileItemViewModel>();

            var replaysHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("row1")).ToList();

            foreach (var row in replaysHtml)
            {
                var _name = row.SelectSingleNode("h2/a").InnerText.Trim();

                var _url = row.SelectSingleNode("div/div/a").Attributes["data-post-url"].Value.Trim();

                FileItemViewModel replay = new FileItemViewModel(_url, 0);
                replay.IsRemote = true;
                replayList.Add(replay);
            }

            return replayList;

        }

        public static async Task<string> DownloadFile(string url, string fileName)
        {
            try
            {
                WebClient wClient = new WebClient();
                await Task.Run(() => wClient.DownloadFile(url, fileName));
                return url + fileName;
            }
            catch (System.Exception e)
            {
                MessageBoxResult result = MessageBox.Show(
                    "File download faild: " + e.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }

        }

        public static string ExtractDomainNameFromURL(string Url)
        {
            Url = System.Text.RegularExpressions.Regex.Replace(
                Url,
                @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$",
                "$0"
            );
            return Url;
        }

        public static string ExtractFileNameFromURL(string Url)
        {
            var name = new Uri(Url).Segments.Last();
            return name;
        }

    }
}
