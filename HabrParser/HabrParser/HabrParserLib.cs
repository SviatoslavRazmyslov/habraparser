using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using HtmlAgilityPack;

namespace HabrParserLib
{
    public struct dataInput
    {
        public string hrefBlog;
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
        public double dateOfPublication;
        public List<string> labels;
        public List<string> hubs;

    }

    public struct InfoMoreBlogsWithHabr
    {
        public string hrefBlogs;
        public int searchDepth;
        public List<InfoSite> InfoSingeBlogs;
        public string pathOutFile;
    }

    public class Parser
    {
        public delegate void StatusBar();

        public event StatusBar UnableToConnect;

        public event StatusBar InvalidFilePath;

        public event StatusBar IncorrectData;

        public void Parse(string[] args)
        {
            List<InfoMoreBlogsWithHabr> infoMoreBlogs = new List<InfoMoreBlogsWithHabr>();
            List<dataInput> DataAllBlogs = new List<dataInput>();
            //TODO обработка args должна быть вне класса Parser
            if (args.Length == 0)
            {
                InvalidFilePath();
                //TODO Убрать отовсюду использование Environment.Exit.
                //Если класс не может продолжать работу после какой-либо ошибки, то он должен кинуть исключение, которое нужно поймать в месте, где
                //был создан экземпляр класса, и выдать соответствующее сообщение об ошибке.
                Environment.Exit(04);
            }
                

            Input(args, DataAllBlogs);

            foreach (var dataSingleBlog in DataAllBlogs)
            {
                GetLinks(dataSingleBlog, infoMoreBlogs);
            }

            foreach (var element in infoMoreBlogs)
            {
                Output(element);
            }
        }


        private readonly string _name = "//span[@class='post__title-text']";
        private readonly string _link = "//head/link[@rel='canonical']";
        private readonly string _raiting = "//ul/li/div[@class='voting-wjt voting-wjt_post js-post-vote']/span";
        private readonly string _bootmarks = "//span[@class='bookmark__counter js-favs_count']";
        private readonly string _views = "//span[@class='post-stats__views-count']";
        private readonly string _comments = "//a[@class='post-stats__comments-link']";
        private readonly string _date = "//span[@class='post__time']";
        private readonly string _labels = "//li[@class='inline-list__item inline-list__item_tag']/a";
        private readonly string _hubs = "//li[@class='inline-list__item inline-list__item_hub']/a";

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

            for (int index = 0; index < links.Count; index++)
            {
                string url = links[index];
                tasks.Add(Task.Factory.StartNew(() => ProcessArticle(url, infoSite)));
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var t in tasks)
                myInfoSite.Add(t.Result);

            return myInfoSite;
        }

        private InfoSite ProcessArticle(string url, InfoSite infoSite)
        {
            //TODO сделать здесь обработку потенциальных исключений (try\catch),
            //с выдачей сообщений через event - чтобы исключение не шло выше (из task'а)
            //и остальные task'и продолжали работать - если произошла ошибка на обработке одного task'а
            //программа не должна завершить работу - она должна выдать сообщение об ошибке, и продолжать работать
            int check = 0;
            HtmlDocument htmlDoc = null;
            while (check >= 0)
            {
                try
                {
                    htmlDoc = new HtmlWeb().Load(url);
                    check = -1;
                }
                catch
                {
                    //TODO Небезопасный вызов event'а - будет NullreferenceException если никто не подписан на него
                    //нужно писать EventName?.Invoke().
                    UnableToConnect();
                    check++;
                    if (check >= 4)
                    {
                        Environment.Exit(05);
                    }
                }
            }

            // Поиск названия сайта       
            infoSite.name = htmlDoc.DocumentNode
                                   .SelectSingleNode(_name).InnerText;

            // Поиск ссылки сайта       
            infoSite.link = htmlDoc.DocumentNode
                                   .SelectSingleNode(_link).Attributes["href"].Value;

            string buf = "";
            // Поиск общего рейтинга сайта       
            buf = htmlDoc.DocumentNode
                         .SelectSingleNode(_raiting).InnerText;

            if (buf.StartsWith("–")
                || buf.StartsWith("‐")
                || buf.StartsWith("−")
                || buf.StartsWith("—"))
            {
                buf = buf.Remove(0, 1);
                infoSite.rating = Convert.ToInt32(buf) * (-1);
            }
            else
            {
                infoSite.rating = Convert.ToInt32(buf);
            }

            // Поиск колличества закладок данного сайта       
            buf = htmlDoc.DocumentNode
                         .SelectSingleNode(_bootmarks).InnerText;
            infoSite.bootmarks = Convert.ToInt32(buf);

            //Поиск колличества просмотров 
            buf = htmlDoc.DocumentNode
                         .SelectSingleNode(_views).InnerText;

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
            buf = htmlDoc.DocumentNode
                         .SelectSingleNode(_comments).InnerText;

            int.TryParse(buf, out infoSite.numbOfComments);

            // Поиск даты создания сайта       
            buf = htmlDoc.DocumentNode
                         .SelectSingleNode(_date).InnerText;
            //Замена буквенного представления месяца на численное

            buf = buf.TrimStart(' ').TrimEnd(' ');
            string[] res = buf.Split(' ');

            string indexMonth;
            _months.TryGetValue(res[1], out indexMonth);
            //----------------------------------------------------

            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime result = DateTime.Now;

            switch (res.Length)
            {
                case 5:
                    buf = buf.Replace(res[1], indexMonth);
                    result = DateTime.ParseExact(buf, "d MM yyyy в HH:mm", provider);
                    break;
                case 4:
                    buf = buf.Replace(res[1], indexMonth);
                    result = DateTime.ParseExact(buf, "d MM в HH:mm", provider);
                    break;
                case 3:
                    if (buf.Contains("сегодня"))
                    {
                        DateTime time = DateTime.ParseExact(buf, "сегодня в HH:mm", provider);
                        result = DateTime.Now.Date.Add(new TimeSpan(time.Hour, time.Minute, time.Second));
                    }
                    else if (buf.Contains("вчера"))
                    {
                        DateTime time = DateTime.ParseExact(buf, "вчера в HH:mm", provider);
                        result = DateTime.Now.AddDays(-1);
                        result = DateTime.Now.Date.Add(new TimeSpan(time.Hour, time.Minute, time.Second));
                    }
                    break;
                default:
                    result = DateTime.Now;
                    break;
            }


            //Перевод даты в UNIXTIME число в секундах
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan unixTimeDate = result - origin;
            infoSite.dateOfPublication = unixTimeDate.TotalSeconds;

            // Поиск меток, присутствующих на сайте       
            HtmlNodeCollection nodes = htmlDoc.DocumentNode
                               .SelectNodes(_labels);

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


        private readonly string _nameIo = "Название";
        private readonly string _linkIo = "Ссылка";
        private readonly string _raitingIo = "Рейтинг";
        private readonly string _bootmarksIo = "Закладки";
        private readonly string _viewsIo = "Просмотры";
        private readonly string _numbofcommentsIo = "Коментарии";
        private readonly string _dateofpublicationIo = "Дата";
        private readonly string _labelIo = "Метки";
        private readonly string _hubsIo = "Хабы";
        private readonly string _symIo = ";";

        private void Input(string[] args, List<dataInput> dataAllBlogs)
        {
            dataInput dataInputSingle = new dataInput();
            FileStream Input = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(Input);
            for (int i = 0; i < File.ReadAllLines(args[0]).Length; i++)
            {
                string bufStringData = reader.ReadLine();
                string[] result = bufStringData.Split(' ');
                if (result.Length != 3)
                {
                    IncorrectData();
                    Environment.Exit(02);
                }

                foreach (string one in result)
                {
                    if (one.Contains("HREF:"))
                    {
                        string res = one.Replace("HREF:", "");
                        dataInputSingle.hrefBlog = res;

                    }
                    if (one.Contains("NUMBSTR:"))
                    {
                        string res = one.Replace("NUMBSTR:", "");
                        if (!(Int32.TryParse(res, out dataInputSingle.searchDepth)))
                        {
                            IncorrectData();
                            Environment.Exit(03);
                        }
                        dataInputSingle.searchDepth = Math.Abs(dataInputSingle.searchDepth);
                    }
                    if (one.Contains("PATHOUT:"))
                    {
                        string res = one.Replace("PATHOUT:", "");
                        dataInputSingle.pathOutFile = res;
                    }
                }
                dataAllBlogs.Add(dataInputSingle);
            }

        }

        private void Output(InfoMoreBlogsWithHabr myInfoBlog)
        {
            FileStream File = new FileStream(myInfoBlog.pathOutFile,
                                                FileMode.Create,
                                                FileAccess.Write);
            StreamWriter Writer = new StreamWriter(File, Encoding.Unicode);
            Writer.WriteLine(_nameIo + _symIo +
                             _linkIo + _symIo +
                             _raitingIo + _symIo +
                             _bootmarksIo + _symIo +
                             _viewsIo + _symIo +
                             _numbofcommentsIo + _symIo +
                             _dateofpublicationIo + _symIo +
                             _labelIo + _symIo +
                             _hubsIo);

            for (int i = 0; i < myInfoBlog.InfoSingeBlogs.Count; i++)
            {
                string bufLabels = string.Join(", ", myInfoBlog.InfoSingeBlogs[i].labels);
                string bufHubs = string.Join(", ", myInfoBlog.InfoSingeBlogs[i].hubs);

                Writer.WriteLine(myInfoBlog.InfoSingeBlogs[i].name + _symIo +
                                 myInfoBlog.InfoSingeBlogs[i].link + _symIo +
                                 myInfoBlog.InfoSingeBlogs[i].rating + _symIo +
                                 myInfoBlog.InfoSingeBlogs[i].bootmarks + _symIo +
                                 myInfoBlog.InfoSingeBlogs[i].views + _symIo +
                                 myInfoBlog.InfoSingeBlogs[i].numbOfComments + _symIo +
                                 myInfoBlog.InfoSingeBlogs[i].dateOfPublication + _symIo +
                                 bufLabels + _symIo +
                                 bufHubs);
            }
            Writer.Close();
        }

        private readonly string _title = "//h2[@class='post__title']/a";
        private readonly string _next_page = "//a[@class='arrows-pagination__item-link " +
                                            "arrows-pagination__item-link_next']";
        private readonly string _atrib = "href";
        private readonly string _start_page = "https://habr.com";
        private readonly string _title_name = "//a[@class='page-header__info-title']";

        private void GetLinks(dataInput dataSingleBlog, List<InfoMoreBlogsWithHabr> InfoMoreBlogs)
        {
            InfoMoreBlogsWithHabr InfoBlog = new InfoMoreBlogsWithHabr();
            InfoBlog.InfoSingeBlogs = new List<InfoSite>();
            InfoBlog.hrefBlogs = dataSingleBlog.hrefBlog;
            InfoBlog.searchDepth = dataSingleBlog.searchDepth;
            InfoBlog.pathOutFile = dataSingleBlog.pathOutFile;
            HtmlWeb web = new HtmlWeb();
            int check = 0;
            HtmlDocument htmlDoc = null;
            while (check >= 0)
            {
                try
                {
                    htmlDoc = web.Load(dataSingleBlog.hrefBlog);
                    check = -1;
                }
                catch
                {
                    UnableToConnect();
                    check++;
                    if (check >= 4)
                    {
                        Environment.Exit(05);
                    }

                }
            }
            HtmlNodeCollection nodes = htmlDoc.DocumentNode
                               .SelectNodes(_title);
            string namm = "";
            try
            {
                namm = htmlDoc.DocumentNode.SelectSingleNode(_title_name).InnerText;
            }
            catch
            {
                IncorrectData();
                Environment.Exit(01);
            }

            List<string> links = new List<string>();
            if (nodes == null)
                return;

            int counterSearch = 0;
            while (nodes != null && counterSearch < dataSingleBlog.searchDepth)
            {
                foreach (HtmlNode node in nodes)
                    links.Add(node.Attributes[_atrib].Value);

                foreach (var element in ProcessArticleLinks(links))
                    InfoBlog.InfoSingeBlogs.Add(element);
                //передаю глобал и локал листы
                // transferObj.MainDataCollection(InfoMoreBlogs, links);    
                links.RemoveRange(0, links.Count);
                nodes = null;
                var nextPage = htmlDoc.DocumentNode
                                      .SelectSingleNode(_next_page);

                if (nextPage == null || nextPage.Attributes[_atrib] == null)
                {
                    break;
                }

                check = 0;
                while (check >= 0)
                {
                    try
                    {
                        htmlDoc = web.Load(_start_page + nextPage.Attributes[_atrib]
                                                                .Value);
                        check = -1;
                    }
                    catch
                    {
                        UnableToConnect();

                        check++;
                        if (check >= 4)
                        {
                            //TODO Если одну страницу не смогли открыть после нескольких попыток,
                            //не завершаем работу (не кидаем исключение), а выдаём через event сообщение
                            //и пробуем работать дальше, со следующей страницей \ блогом
                            Environment.Exit(05);
                    }

                    }
                }

                nodes = htmlDoc.DocumentNode
                               .SelectNodes(_title);
                counterSearch++;
            }
            InfoMoreBlogs.Add(InfoBlog);
            return;
        }
    }
}
