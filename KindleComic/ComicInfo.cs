using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindleComic
{
    public class ComicInfo
    {
        public string ComicName { get; set; }
        public string CoverUrl { get; set; }
        public string ComicUrl { get; set; }
        public List<ComicContents> comicContents { get; set; }
    }
    public class ComicContents
    {
        public string Chapter { get; set; }
        public string ChapterUrl { get; set; }
        public Dictionary<int,string> Images { get; set; }
    }
}
