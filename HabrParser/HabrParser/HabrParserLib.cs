using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using HtmlAgilityPack;
using CommandLine;

namespace HabrParserLib
{

    public class Options
    {
        [Option('o', "output", Required = true, HelpText = "Output files to be processed.")]
        public string outputFile { get; set; }

        [Option('d', "depth", Required = true, HelpText = "Output files to be processed.")]
        public int depthSearch { get; set; }

        [Option('i', "input",Required = true,HelpText = "Input files to be processed.")]
        public string inputFile { get; set; }
    }

    public struct dataInput
    {
        public List<string> hrefBlog;
        public string pathOutFile;
        public int searchDepth;
    }

    public struct InfoSite
    {
        public string name;
        public string link;
        public int rating;
        public int bootmarks;
        public double views;
        public int numbOfComments;
        public string dateOfPublication;
        public string timeOfPublication;
        public List<string> labels;
        public List<string> hubs;

    }

    public struct InfoMoreBlogsWithHabr
    {
        public string hrefBlogs;
        public List<InfoSite> InfoSingeBlogs;
    }

    public class Parser
    {
        //Поля для обращения к нужным атрибутам/значениям на сайте
        private readonly string _name = "//span[@class='post__title-text']";
        private readonly string _link = "//head/link[@rel='canonical']";
        private readonly string _raiting = "//ul/li/div[@class='voting-wjt voting-wjt_post js-post-vote']/span";
        private readonly string _bootmarks = "//span[@class='bookmark__counter js-favs_count']";
        private readonly string _views = "//span[@class='post-stats__views-count']";
        private readonly string _comments = "//a[@class='post-stats__comments-link']";
        private readonly string _date = "//span[@class='post__time']";
        private readonly string _labels = "//li[@class='inline-list__item inline-list__item_tag']/a";
        private readonly string _hubs = "//li[@class='inline-list__item inline-list__item_hub']/a";
        private readonly string _title = "//h2[@class='post__title']/a";
        private readonly string _next_page = "//a[@class='arrows-pagination__item-link "
                                                + "arrows-pagination__item-link_next']";
        //------------------------------------------------------------------
        private readonly string _start_page = "https://habr.com";
        private readonly string _atrib = "href";

        public event EventHandler<string> ErrorOccured;

        public List<InfoMoreBlogsWithHabr> Parse(Options arguments)
        {
            List<InfoMoreBlogsWithHabr> infoMoreBlogs = new List<InfoMoreBlogsWithHabr>();
            dataInput DataAllBlogs = new dataInput();

            DataAllBlogs.pathOutFile = arguments.outputFile;
            DataAllBlogs.searchDepth = arguments.depthSearch;
            DataAllBlogs.hrefBlog =  Input(arguments);

            foreach (string dataSingleBlog in DataAllBlogs.hrefBlog)
                    GetLinks(dataSingleBlog, DataAllBlogs, infoMoreBlogs);

            return infoMoreBlogs;
        }

           
        private readonly Dictionary<string, string> _months = new Dictionary<string, string>()
                    {
                        {"января", "01"},
                        {"февраля", "02"},
                        {"марта", "03"},
                        {"апреля", "04" },
                        {"мая", "05" },
                        {"июня", "06" },
                        {"июля", "07" },
                        {"августа", "08" },
                        {"сентября", "09" },
                        {"октября", "10" },
                        {"ноября", "11" },
                        {"декабря", "12" }
                    };


        private List<InfoSite> ProcessArticleLinks(List<string> links)
        {
            InfoSite infoSite = new InfoSite();
            List<InfoSite> myInfoSite = new List<InfoSite>();
            List<Task<InfoSite>> tasks = new List<Task<InfoSite>>(10);
            foreach (string url in links)
                tasks.Add(Task.Factory.StartNew(() => ProcessArticle(url, infoSite)));
            
            Task.WaitAll(tasks.ToArray());

            foreach (var t in tasks)
                if (t.Result.hubs != null)
                    myInfoSite.Add(t.Result);
            return myInfoSite;
        }

        private InfoSite ProcessArticle(string url, InfoSite infoSite)
        {
            HtmlDocument htmlDoc = null;
                try
                {
                    htmlDoc = new HtmlWeb().Load(url);
                }
                catch
                {
                    ErrorOccured?.Invoke(this, "UnableToConnect");
                    return infoSite;
                }

            // Поиск названия сайта       
            infoSite.name = htmlDoc.DocumentNode.SelectSingleNode(_name).InnerText;

            // Поиск ссылки сайта       
            infoSite.link = htmlDoc.DocumentNode.SelectSingleNode(_link).Attributes["href"].Value;

            string buf = "";
            // Поиск общего рейтинга сайта       
            buf = htmlDoc.DocumentNode.SelectSingleNode(_raiting).InnerText;

            if (buf.StartsWith("–")
                || buf.StartsWith("‐")
                || buf.StartsWith("−")
                || buf.StartsWith("—"))
            {
                buf = buf.Remove(0, 1);
                infoSite.rating = Convert.ToInt32(buf) * (-1);
            }
            else
                infoSite.rating = Convert.ToInt32(buf);

            // Поиск колличества закладок данного сайта       
            buf = htmlDoc.DocumentNode.SelectSingleNode(_bootmarks).InnerText;
            infoSite.bootmarks = Convert.ToInt32(buf);

            //Поиск колличества просмотров 
            buf = htmlDoc.DocumentNode.SelectSingleNode(_views).InnerText;

            if (buf.Contains('k'))
            {
                buf = buf.Substring(0, buf.Length - 1);
                infoSite.views = Convert.ToDouble(buf) * 1000;
            }
            else
            {
                infoSite.views = Convert.ToDouble(buf);
            }


            // Поиск колличества комментариев на сайте       
            buf = htmlDoc.DocumentNode.SelectSingleNode(_comments).InnerText;

            int.TryParse(buf, out infoSite.numbOfComments);

            // Поиск даты создания сайта       
            buf = htmlDoc.DocumentNode.SelectSingleNode(_date).InnerText;
  
            //Замена буквенного представления месяца на численное

            buf = buf.TrimStart(' ').TrimEnd(' ');
            string[] res = buf.Split(' ');

            string indexMonth;
            _months.TryGetValue(res[1], out indexMonth);
            //----------------------------------------------------

            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime resultDate = DateTime.Now;
            switch (res.Length)
            {
                case 5:
                    buf = buf.Replace(res[1], indexMonth);
                    resultDate = DateTime.ParseExact(buf, "d MM yyyy в HH:mm", provider);
                    break;
                case 4:
                    buf = buf.Replace(res[1], indexMonth);
                    resultDate = DateTime.ParseExact(buf, "d MM в HH:mm", provider);
                    break;
                case 3:
                    if (buf.Contains("сегодня"))
                    {
                        DateTime time = DateTime.ParseExact(buf, "сегодня в HH:mm", provider);
                        resultDate = DateTime.Now.Date.Add(new TimeSpan(time.Hour, time.Minute, time.Second));
                    }
                    else if (buf.Contains("вчера"))
                    {
                        DateTime time = DateTime.ParseExact(buf, "вчера в HH:mm", provider);
                        resultDate = DateTime.Now.AddDays(-1);
                        resultDate = resultDate.Date.Add(new TimeSpan(time.Hour, time.Minute, time.Second));
                    }
                    break;
                default:
                    resultDate = DateTime.Now;
                    break;
            }

            infoSite.dateOfPublication = resultDate.Date.ToShortDateString();
            infoSite.timeOfPublication = resultDate.ToShortTimeString();

            // Поиск меток, присутствующих на сайте       
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(_labels);

            infoSite.labels = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    infoSite.labels.Add(node.InnerText);
                }
            }

            //Поиск хабов, расположенных на сайте
            nodes = htmlDoc.DocumentNode
                               .SelectNodes(_hubs);

            infoSite.hubs = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    infoSite.hubs.Add(node.InnerText);
                }
            }

            return infoSite;
        }

        private List<string> Input(Options args)
        {
            List<string> listAllLinks = new List<string>();

            FileStream Input = new FileStream(args.inputFile, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(Input);

            foreach (string link in File.ReadAllLines(args.inputFile))
            {
                listAllLinks.Add(link);
            }

            return listAllLinks;
        }

        private void GetLinks(string linkBlog, 
                              dataInput Data, 
                              List<InfoMoreBlogsWithHabr> InfoMoreBlogs)
        {
            InfoMoreBlogsWithHabr InfoBlog = new InfoMoreBlogsWithHabr();
            InfoBlog.InfoSingeBlogs = new List<InfoSite>();
            InfoBlog.hrefBlogs = linkBlog;

            HtmlWeb web = new HtmlWeb();
            int check = 0;
            HtmlDocument htmlDoc = null;
            while (check >= 0)
            {
                try
                {
                    htmlDoc = web.Load(linkBlog);
                    check = -1;
                }
                catch
                {
                    ErrorOccured?.Invoke(this, "Unable to load blog page");
                    check++;
                    if (check >= 4)
                        return;

                }
            }
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(_title);
                        
            List<string> links = new List<string>();
            if (nodes == null)
                return;

            int counterSearch = 0;
            while (nodes != null && counterSearch < Data.searchDepth)
            {
                foreach (HtmlNode node in nodes)
                    links.Add(node.Attributes[_atrib].Value);

                foreach (var element in ProcessArticleLinks(links))
                    InfoBlog.InfoSingeBlogs.Add(element);

                links.RemoveRange(0, links.Count);
                nodes = null;
                var nextPage = htmlDoc.DocumentNode.SelectSingleNode(_next_page);

                if (nextPage == null || nextPage.Attributes[_atrib] == null)
                    break;

                check = 0;
                while (check >= 0)
                {
                    try
                    {
                        htmlDoc = web.Load(_start_page + nextPage.Attributes[_atrib].Value);
                        check = -1;
                    }
                    catch
                    {
                        ErrorOccured?.Invoke(this, "UnableToConnect");
                        check++;
                        if (check >= 4)
                        {
                            ErrorOccured(this, "Page load failed");
                            break;
                        }

                    }
                }
                nodes = htmlDoc.DocumentNode.SelectNodes(_title);
                counterSearch++;
            }
            InfoMoreBlogs.Add(InfoBlog);
            return;
        }
    }
}
