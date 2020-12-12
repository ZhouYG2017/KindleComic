using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KindleComic
{
    class ComicSource
    {
        private readonly HttpClient _client = new HttpClient();
        public string ComicSourceName { get; set; }
        public string ComicSourceUrl { get; set; }
        public string SearchListXPath { get; set; }
        public string SearchListComicNameXPath { get; set; }
        public string SearchListComicUrlXPath { get; set; }
        public string SearchListComicCoverXPath { get; set; }
        public string SearchPageUrlFormat { get; set; }
        public string ComicContentsXPath { get; set; }
        public string ComicChapterUrlXPath { get; set; }
        public string ComicChapterXPath { get; set; }
        public string ImgBaseUrl { get; set; }
        public ComicSource(string url)
        {
            _client.BaseAddress = new Uri(url);
            ComicSourceUrl = url;
        }

        public async Task<List<ComicInfo>> GetSearchListAsync(string key, int page = 1)
        {
            string htmlStr = await GetComicDataByHtmlAsync(string.Format(SearchPageUrlFormat, key, page));
            if (string.IsNullOrEmpty(htmlStr))
            {
                return null;
            }
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlStr);
            var nodeList = htmlDoc.DocumentNode.SelectNodes(SearchListXPath);
            List<ComicInfo> ltComicInfo = new List<ComicInfo>();
            foreach (HtmlAgilityPack.HtmlNode node in nodeList)
            {
                string comicName = "", comicUrl = "", comicCover = "";
                ComicInfo comicInfo = new ComicInfo();

                if (!string.IsNullOrEmpty(SearchListComicNameXPath))
                {
                    comicName = node.SelectSingleNode(SearchListComicNameXPath)?.InnerText;
                }
                if (!string.IsNullOrEmpty(SearchListComicUrlXPath))
                {
                    comicUrl = ((HtmlAgilityPack.HtmlNodeNavigator)node.CreateNavigator()).SelectSingleNode(SearchListComicUrlXPath)?.Value;
                }
                if (!string.IsNullOrEmpty(SearchListComicCoverXPath))
                {
                    comicCover = ((HtmlAgilityPack.HtmlNodeNavigator)node.CreateNavigator()).SelectSingleNode(SearchListComicCoverXPath)?.Value;
                }
                comicInfo.ComicName = comicName;
                comicInfo.ComicUrl = comicUrl;
                comicInfo.CoverUrl = comicCover;
                ltComicInfo.Add(comicInfo);
            }
            return ltComicInfo;
        }
        public async Task<List<ComicContents>> GetComicContentsAsync(string url)
        {
            string htmlStr = await GetComicDataByHtmlAsync(url);
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlStr);
            var nodeList = htmlDoc.DocumentNode.SelectNodes(ComicContentsXPath);
            List<ComicContents> ltConmicContents = new List<ComicContents>();
            foreach (HtmlAgilityPack.HtmlNode node in nodeList)
            {
                string comicChapter = "", comicChapterUrl = "";
                if (!string.IsNullOrEmpty(ComicChapterXPath))
                {
                    comicChapter = node.SelectSingleNode(ComicChapterXPath).InnerText;
                }
                if (!string.IsNullOrEmpty(ComicChapterXPath))
                {
                    comicChapterUrl = ((HtmlAgilityPack.HtmlNodeNavigator)node.CreateNavigator()).SelectSingleNode("a/@href")?.Value;
                }
                ltConmicContents.Add(new ComicContents() { 
                    Chapter=comicChapter,
                    ChapterUrl= comicChapterUrl
                });
            }
            return ltConmicContents;
        }

        public async Task<Dictionary<int,string>> GetComicImagesAsync(string url)
        {
            string htmlStr = await GetComicDataByHtmlAsync(url);
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlStr);

            var script = htmlDoc.DocumentNode.SelectSingleNode("/html/body/script[1]");
            string base64Str = script.InnerText.Replace("var img_data = '", "").Replace("';","");
            var bytes= Convert.FromBase64String(base64Str);
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            var ja = JArray.Parse(json);
            Dictionary<int, string> imgs = new Dictionary<int, string>();
            int i = 1;
            foreach (var j in ja)
            {
                var img = j["img"]?.ToString();
                if (!string.IsNullOrEmpty(img))
                {
                    if (!string.IsNullOrEmpty(ImgBaseUrl))
                    {
                        img = ImgBaseUrl + "/" + img;
                    }
                    i++;
                }
                imgs.Add(i,img);
            }
            return imgs;
        }
        public async Task<string> GetComicDataByHtmlAsync(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();

            }
            return null;
        }
        public async Task<string> GetComicDataByJsonAsync(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");
            request.Headers.Add("Accept", "application/json");
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
    }
}
