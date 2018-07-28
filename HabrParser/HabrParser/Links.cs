using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;

namespace List_Links
{
    public class Links
    {
        public void Get_Links(string url, List<string> links)
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var nodes = htmlDoc.DocumentNode
                               .SelectNodes("//h2[@class='post__title']/a");
            if (nodes == null)
            {
                return;
            }
            while (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    links
                        .Add(node
                                .Attributes["href"]
                                .Value);
                }

                var nextPage = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='arrows-pagination__item-link " +
                                                         "arrows-pagination__item-link_next']");

                if (nextPage == null || nextPage.Attributes["href"] == null)
                {
                    break;
                }

                htmlDoc = web.Load("https://habr.com" + nextPage.Attributes["href"].Value);
                nodes = htmlDoc.DocumentNode
                               .SelectNodes("//h2[@class='post__title']/a");
            }
        }
    }
}
