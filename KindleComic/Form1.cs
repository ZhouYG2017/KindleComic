using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KindleComic
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            ComicSource comicSource = new ComicSource("https://www.manhuadb.com");
            comicSource.ComicSourceName = "漫画大全";
            comicSource.SearchPageUrlFormat = "https://www.manhuadb.com/search?q={0}&p={1}";
            comicSource.SearchListXPath = "//*[@class=\"comicbook-index mb-2 mb-sm-0\"]";
            comicSource.SearchListComicNameXPath = "h2/a";
            comicSource.SearchListComicCoverXPath = "a/img/@src";
            comicSource.SearchListComicUrlXPath = "a/@href";

            var list = comicSource.GetSearchListAsync("1", 1).Result;

            comicSource.ComicContentsXPath = "//*[@class=\"sort_div fixed-wd-num\"]";
            comicSource.ComicChapterXPath = "a";
            comicSource.ComicChapterUrlXPath = "a/@href";

            List<ComicContents> comicContents = comicSource.GetComicContentsAsync(list[23].ComicUrl).Result;
            list[23].comicContents = comicContents;

            var d = comicSource.GetComicImagesAsync(comicContents[0].ChapterUrl).Result;
            InitializeComponent();
        }
    }
}
