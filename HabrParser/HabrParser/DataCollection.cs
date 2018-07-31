using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;


namespace DataCollectionNameSpace
{
    struct InfoSite
    {
        public string name;
        public string link;
        public int rating;
        public int bootmarks;
        public double views;
        public int numbOfComments;
        //public string timeOfPublication;
        public string dateOfPublication;
        public List<string> labels;
    }


    class DataCollection
    {
        private readonly string NAME = "//span[@class='post__title-text']";
        private readonly string LINK = "//head/link[@rel='canonical']";
        private readonly string RAITING = "//ul/li/div[@class='voting-wjt voting-wjt_post js-post-vote']/span";
        private readonly string BOOTMARKS = "//span[@class='bookmark__counter js-favs_count']";
        private readonly string VIEWS = "//span[@class='post-stats__views-count']";
        private readonly string COMMENTS = "//a[@class='post-stats__comments-link']";
        private readonly string DATE = "//span[@class='post__time']";
        private readonly string LABELS = "//li[@class='inline-list__item inline-list__item_tag']/a";

        //"//li[@class='inline-list__item inline-list__item_tag']/a"

            public List<InfoSite> MainDataCollection(List<string> links, List<InfoSite> myInfoSite)
       {

            InfoSite infoSite = new InfoSite();
            foreach (string link in links)
            {
                myInfoSite.Add(DataCollectionOnSite(link, infoSite));
            }
            return myInfoSite;
        }



        private InfoSite DataCollectionOnSite(string url, InfoSite infoSite)
        {

            var htmlDoc = new HtmlWeb().Load(url);
            // Поиск названия сайта       
            infoSite.name = htmlDoc.DocumentNode
                                   .SelectSingleNode(NAME).InnerText;


            // Поиск ссылки сайта       
            infoSite.link = htmlDoc.DocumentNode
                                   .SelectSingleNode(LINK).Attributes["href"].Value;

            string buf = "";
            // Поиск общего рейтинга сайта       
            buf = htmlDoc.DocumentNode                     
                         .SelectSingleNode(RAITING).InnerText;
            
            if (   buf.StartsWith("–")
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
                         .SelectSingleNode(BOOTMARKS).InnerText;
            infoSite.bootmarks = Convert.ToInt32(buf);

            //Поиск колличества просмотров 
            buf = htmlDoc.DocumentNode
                         .SelectSingleNode(VIEWS).InnerText;

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
                         .SelectSingleNode(COMMENTS).InnerText;

            int.TryParse(buf, out infoSite.numbOfComments);

            /* infoSite.timeOfPublication = htmlDoc.DocumentNode
                             .SelectSingleNode("//span[@class='post__title-text']").InnerText;*/

            // Поиск даты создания сайта       
            infoSite.dateOfPublication = htmlDoc.DocumentNode
                                                .SelectSingleNode(DATE).InnerText;

            // Поиск меток, присутствующих на сайте       
            var nodes = htmlDoc.DocumentNode
                               .SelectNodes(LABELS);

            infoSite.labels = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    infoSite.labels.Add(node.InnerText);
                }
            }
                       

            return infoSite;
        }
    }
}
