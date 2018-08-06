using System.Collections.Generic;
using HtmlAgilityPack;
using InOut;
using DataCollectionNameSpace;
using System;
namespace List_Links
{

    public class Links
    {
        private readonly string TITLE = "//h2[@class='post__title']/a";
        private readonly string NEXT_PAGE = "//a[@class='arrows-pagination__item-link " +
                                            "arrows-pagination__item-link_next']";
        private readonly string ATRIB = "href";
        private readonly string START_PAGE = "https://habr.com";
        private readonly string TITLE_NAME = "//a[@class='page-header__info-title']";

        public void GetLinks(dataInput dataSingleBlog, List<InfoMoreBlogsWithHabr> InfoMoreBlogs)
        {
            InfoMoreBlogsWithHabr InfoBlog = new InfoMoreBlogsWithHabr();
            InfoBlog.InfoSingeBlogs = new List<InfoSite>();
            InfoBlog.hrefBlogs = dataSingleBlog.hrefBlog;
            InfoBlog.searchDepth = dataSingleBlog.searchDepth;
            InfoBlog.pathOutFile = dataSingleBlog.pathOutFile;
            HtmlWeb web = new HtmlWeb();
            bool check = false;
            HtmlDocument htmlDoc = null;
            while (!check)
            {
                try
                {
                    htmlDoc = web.Load(dataSingleBlog.hrefBlog);
                    check = true;
                }
                catch
                {
                    check = false;
                    Console.WriteLine("Проверьте соединение с интернетом и нажмите Enter");
                    Console.ReadKey();
                }
            }
            HtmlNodeCollection nodes = htmlDoc.DocumentNode
                               .SelectNodes(TITLE);
            string namm = htmlDoc.DocumentNode.SelectSingleNode(TITLE_NAME).InnerText;
            List<string> links = new List<string>();
            if (nodes == null)
            {
                return;
            }
            int counterSearch = 0, u = 0 ;
            while (nodes != null && counterSearch < dataSingleBlog.searchDepth)
            {
                //Console.Clear();
                //Console.WriteLine((u++));
                foreach (HtmlNode node in nodes)
                {
                    links.Add(node.Attributes[ATRIB].Value);
                }
                DataCollection transferObj = new DataCollection();
                foreach (var element in transferObj.MainDataCollection(links))
                    InfoBlog.InfoSingeBlogs.Add(element);
                //передаю глобал и локал листы
                // transferObj.MainDataCollection(InfoMoreBlogs, links);    
                links.RemoveRange(0, links.Count);
                nodes = null;
                var nextPage = htmlDoc.DocumentNode
                                      .SelectSingleNode(NEXT_PAGE);

                if (nextPage == null || nextPage.Attributes[ATRIB] == null)
                {
                    break;
                }

                check = false;
                while (!check)
                {
                    try
                    {
                        htmlDoc = web.Load(START_PAGE + nextPage.Attributes[ATRIB]
                                                                .Value);
                        check = true;
                    }
                    catch
                    {
                        check = false;
                        Console.WriteLine("Проверьте соединение с интернетом и нажмите Enter");
                        Console.ReadKey();
                    }
                }

                nodes = htmlDoc.DocumentNode
                               .SelectNodes(TITLE);
                counterSearch++;
            }
            InfoMoreBlogs.Add(InfoBlog);
            return;
        }
    }
}
