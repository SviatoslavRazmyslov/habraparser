using System.Collections.Generic;
using HtmlAgilityPack;
using InOut;
using DataCollectionNameSpace;
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
            InfoBlog.hrefBlogs = dataSingleBlog.hrefBlog;
            InfoBlog.searchDepth = dataSingleBlog.searchDepth;
            InfoBlog.pathOutFile = dataSingleBlog.pathOutFile;
            var web = new HtmlWeb();
            var htmlDoc = web.Load(dataSingleBlog.hrefBlog);
            var nodes = htmlDoc.DocumentNode
                               .SelectNodes(TITLE);
            string namm = htmlDoc.DocumentNode.SelectSingleNode(TITLE_NAME).InnerText;
            List<string> links = new List<string>();
            if (nodes == null)
            {
                return;
            }
            int counterSearch = 0;
            while (nodes != null && counterSearch < dataSingleBlog.searchDepth)
            {
                foreach (HtmlNode node in nodes)
                {
                    links.Add(node.Attributes[ATRIB].Value);
                }
                foreach (string Buf in links)
                {
                    DataCollection transferObj = new DataCollection();
                    transferObj.MainDataCollection(links, InfoBlog.InfoSingeBlogs);
                    //передаю глобал и локал листы
                    // transferObj.MainDataCollection(InfoMoreBlogs, links);
                }
                nodes = null;
                var nextPage = htmlDoc.DocumentNode
                                      .SelectSingleNode(NEXT_PAGE);

                if (nextPage == null || nextPage.Attributes[ATRIB] == null)
                {
                    break;
                }

                htmlDoc = web.Load(START_PAGE + nextPage.Attributes[ATRIB]
                                                        .Value);
                nodes = htmlDoc.DocumentNode
                               .SelectNodes(TITLE);
                counterSearch++;
            }
            InfoMoreBlogs.Add(InfoBlog);
            return;
        }
    }
}
