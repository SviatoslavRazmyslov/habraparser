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
    public struct DataInput
    {
        public List<string> BlogUrls;
        public string pathOutFile;
        public int searchDepth;
    }

    public struct ArticleInfo
    {
        public string name;
        public string link;
        public int rating;
        public int bookmarks;
        public double views;
        public int numbOfComments;
        public string dateOfPublication;
        public string timeOfPublication;
        public List<string> labels;
        public List<string> hubs;

    }

    public struct BlogInfo
    {
        public string hrefBlogs;
        public List<ArticleInfo> InfoSingeBlogs;
    }

    public class Parser
    {
        //Поля для обращения к нужным атрибутам/значениям на сайте
        private readonly string _name = "//span[@class='post__title-text']";
        private readonly string _link = "//head/link[@rel='canonical']";
        private readonly string _raiting = "//ul/li/div[@class='voting-wjt voting-wjt_post js-post-vote']/span";
        private readonly string _bookmarks = "//span[@class='bookmark__counter js-favs_count']";
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
        //msg для событий
        private readonly string _unableConnect = "Unable To Connect: {0}";
        private readonly string _notProcessPg = "The page is not processed : {0}";



        public event EventHandler<string> ErrorOccured;
        public event EventHandler<string> StatusProcessing;

        public List<BlogInfo> Parse(String outputFile, String inputFile, int depth)
        {
            List<BlogInfo> infoMoreBlogs = new List<BlogInfo>();
            DataInput dataAllBlogs = new DataInput();

            dataAllBlogs.pathOutFile = outputFile;
            dataAllBlogs.searchDepth = depth;
            dataAllBlogs.BlogUrls =  Input(inputFile);

            foreach (string blogUrl in dataAllBlogs.BlogUrls)
                ProcessBlog(blogUrl, dataAllBlogs, infoMoreBlogs);

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


        private List<ArticleInfo> ProcessArticleLinks(List<string> links)
        {
            ArticleInfo infoSite = new ArticleInfo();
            List<ArticleInfo> myInfoSite = new List<ArticleInfo>();
            List<Task<ArticleInfo>> tasks = new List<Task<ArticleInfo>>(10);
            foreach (string url in links)
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    infoSite = default(ArticleInfo);
                    try
                    {

                        return ProcessArticle(url, infoSite);
                    }
                    catch
                    {
                        ErrorOccured(this, String.Format(_notProcessPg, url));
                        return infoSite;
                    }

                }));

            Task.WaitAll(tasks.ToArray());

            foreach (var t in tasks)
                if (t.Result.timeOfPublication != null)
                    myInfoSite.Add(t.Result);
            return myInfoSite;
        }

        private ArticleInfo ProcessArticle(string url, ArticleInfo infoSite)
        {
            HtmlDocument htmlDoc = null;
            try
            {
                htmlDoc = new HtmlWeb().Load(url);
            }
            catch
            {
                ErrorOccured?.Invoke(this, String.Format(_unableConnect, url));
                return infoSite;
            }
            //         htmlDoc = null;

            // Поиск названия сайта       
            infoSite.name = SearchNameSite(htmlDoc);

            // Поиск ссылки сайта       
            infoSite.link = SearchHrefSite(htmlDoc);

            // Поиск общего рейтинга сайта       
            infoSite.rating = SearchRatingSite(htmlDoc);

            // Поиск колличества закладок данного сайта       
            infoSite.bookmarks = SearchBookmarks(htmlDoc);

            //Поиск колличества просмотров 
            infoSite.views = SearchNumbViews(htmlDoc);

            // Поиск колличества комментариев на сайте       
            infoSite.numbOfComments = SearchNumbComments(htmlDoc);

            // Поиск даты создания сайта       
            SearchDate(ref infoSite.dateOfPublication, ref infoSite.timeOfPublication, htmlDoc);

            // Поиск меток, присутствующих на сайте       

            infoSite.labels = SearchLabels(htmlDoc);

            //Поиск хабов, расположенных на сайте
            infoSite.hubs = SearchHubs(htmlDoc);

            return infoSite;
        }

        private List<string> SearchHubs(HtmlDocument htmlDoc)
        {
            HtmlNodeCollection nodes = htmlDoc.DocumentNode
                               .SelectNodes(_hubs);

            List<string> hubs = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                   hubs.Add(node.InnerText);
                }
            }

            return hubs;
        }

        private List<string> SearchLabels(HtmlDocument htmlDoc)
        {

            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(_labels);
            List<string> labels = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    labels.Add(node.InnerText);
                }
            }

            return labels;
        }

        private void SearchDate(ref string date, ref string time, HtmlDocument htmlDoc)
        {
            string buf = "";
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
                        DateTime resultTime = DateTime.ParseExact(buf, "сегодня в HH:mm", provider);
                        resultDate = DateTime.Now.Date.Add(new TimeSpan(resultTime.Hour, resultTime.Minute, resultTime.Second));
                    }
                    else if (buf.Contains("вчера"))
                    {
                        DateTime resultTime = DateTime.ParseExact(buf, "вчера в HH:mm", provider);
                        resultDate = DateTime.Now.AddDays(-1);
                        resultDate = resultDate.Date.Add(new TimeSpan(resultTime.Hour, resultTime.Minute, resultTime.Second));
                    }
                    break;
                default:
                    resultDate = DateTime.Now;
                    break;
            }

            date = resultDate.Date.ToShortDateString();
            time = resultDate.ToShortTimeString();

        }

        private int SearchNumbComments(HtmlDocument htmlDoc)
        {
            int numb = 0;
            int.TryParse(htmlDoc.DocumentNode.SelectSingleNode(_comments).InnerText, out numb);
            return numb;
        }

        private double SearchNumbViews(HtmlDocument htmlDoc)
        {
            string buf = "";
            buf = htmlDoc.DocumentNode.SelectSingleNode(_views).InnerText;

            if (buf.Contains('k'))
            {
                buf = buf.Substring(0, buf.Length - 1);
                return Convert.ToDouble(buf) * 1000;
            }
            else
            {
                return Convert.ToDouble(buf);
            }
        }

        private int SearchBookmarks(HtmlDocument htmlDoc)
        {
            string buf = "";
            buf = htmlDoc.DocumentNode.SelectSingleNode(_bookmarks).InnerText;
            return Convert.ToInt32(buf);
            
        }

        private int SearchRatingSite(HtmlDocument htmlDoc)
        {
            string buf = "";
            buf = htmlDoc.DocumentNode.SelectSingleNode(_raiting).InnerText;

            if (buf.StartsWith("–")
                || buf.StartsWith("‐")
                || buf.StartsWith("−")
                || buf.StartsWith("—"))
            {
                buf = buf.Remove(0, 1);
                return Convert.ToInt32(buf) * (-1);
            }
            else
                return Convert.ToInt32(buf);
        }

        private string SearchHrefSite(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode.SelectSingleNode(_link).Attributes["href"].Value;
        }

        private string SearchNameSite(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode.SelectSingleNode(_name).InnerText;
        }

        private List<string> Input(String inputFile)
        {
            List<string> listAllLinks = new List<string>();

            FileStream Input = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(Input);

            foreach (string link in File.ReadAllLines(inputFile))
            {
                listAllLinks.Add(link);
            }

            return listAllLinks;
        }

        private void ProcessBlog(string linkBlog, 
                                 DataInput Data, 
                                 List<BlogInfo> InfoMoreBlogs)
        {
            BlogInfo InfoBlog = new BlogInfo();
            InfoBlog.InfoSingeBlogs = new List<ArticleInfo>();
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
                    check++;
                    if (check >= 4)
                    {
                        ErrorOccured?.Invoke(this, String.Format(_unableConnect, linkBlog));
                        return;
                    }
                }
            }
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(_title);
                        
            List<string> links = new List<string>();
            if (nodes == null)
                return;

            int pagesCount = 0;
            if (Data.searchDepth == -1)
                Data.searchDepth = Int32.MaxValue;
            while (nodes != null && pagesCount < Data.searchDepth)
            {
                foreach (HtmlNode node in nodes)
                    links.Add(node.Attributes[_atrib].Value);

                foreach (var element in ProcessArticleLinks(links))
                    InfoBlog.InfoSingeBlogs.Add(element);

                links.Clear();
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
                        check++;
                        if (check >= 4)
                        {
                            ErrorOccured?.Invoke(this, String.Format(_unableConnect, nextPage.Attributes[_atrib].Value));
                            break;
                        }

                    }
                }
                nodes = htmlDoc.DocumentNode.SelectNodes(_title);
                pagesCount++;
            }
            InfoMoreBlogs.Add(InfoBlog);
            StatusProcessing?.Invoke(this, String.Format("The unit successfully processed : {0}", linkBlog));
            return;
            //////

        }
    }
}
