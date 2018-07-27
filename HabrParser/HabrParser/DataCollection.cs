using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using System.Windows.Forms;


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
       // public void DataCollection()
        public void MySearch()
        {

            //InfoSite MySiteInfo = new InfoSite(); 
            // Создаём экземпляр класса
            HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
            // Присваиваем текстовой переменной k html-код
            string url = "https://habr.com/company/pvs-studio/blog/418143/";
            
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            List<InfoSite> myInfoSite = new List<InfoSite>();

            InfoSite infoSite = new InfoSite();

            


          //  var doc3 = new HtmlAgilityPack.HtmlDocument();
           
            infoSite.name = htmlDoc.DocumentNode
                                   .SelectSingleNode("//span[@class='post__title-text']").InnerText;


            infoSite.link = htmlDoc.DocumentNode
                                   .SelectSingleNode("//head/link[@rel='canonical']").Attributes["href"].Value;

            string buf = "";
            buf = htmlDoc.DocumentNode
                        .SelectSingleNode("//span[@class='voting-wjt__counter voting-wjt__counter_positive  js-score']").InnerText;
            infoSite.rating = Convert.ToInt32(buf);


            buf = htmlDoc.DocumentNode
                            .SelectSingleNode("//span[@class='bookmark__counter js-favs_count']").InnerText;
            infoSite.bootmarks = Convert.ToInt32(buf);

            
            buf = htmlDoc.DocumentNode
                            .SelectSingleNode("//span[@class='post-stats__views-count']").InnerText;


            buf = buf.Substring(0, buf.Length - 1);
            infoSite.views = Convert.ToDouble(buf) * 1000;     


            buf = htmlDoc.DocumentNode
                            .SelectSingleNode("//span[@class='post-stats__comments-count']").InnerText;
            infoSite.numbOfComments = Convert.ToInt32(buf);


           /* infoSite.timeOfPublication = htmlDoc.DocumentNode
                            .SelectSingleNode("//span[@class='post__title-text']").InnerText;*/


            infoSite.dateOfPublication = htmlDoc.DocumentNode
                            .SelectSingleNode("//span[@class='post__time']").InnerText;


            var nodes = htmlDoc.DocumentNode
                            .SelectNodes("//li[@class='inline-list__item inline-list__item_tag']/a");

            infoSite.labels = new List<string>();
            foreach (var node in nodes)
            {
                infoSite.labels.Add(node.InnerText) ;
            }

            myInfoSite.Add(infoSite);       //Обернуть функцию

        }
    }
}
