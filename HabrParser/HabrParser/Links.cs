using System.Collections.Generic;
using HtmlAgilityPack;

namespace List_Links
{
    
    public class Links
    {
        private readonly string TITLE = "//h2[@class='post__title']/a";
        private readonly string NEXT_PAGE = "//a[@class='arrows-pagination__item-link " +
                                            "arrows-pagination__item-link_next']";
        private readonly string ATRIB = "href";
        private readonly string START_PAGE = "https://habr.com";

        public void GetLinks(string url, List<string> links)
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var nodes = htmlDoc.DocumentNode
                               .SelectNodes(TITLE);
            if (nodes == null)
            {
                return;
            }
            while (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    links.Add(node.Attributes[ATRIB]
                                  .Value);
                }

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
            }
            return;
        }
    }
}
