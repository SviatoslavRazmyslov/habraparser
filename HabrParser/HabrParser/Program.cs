using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using List_Links;
using DataCollectionNameSpace;


namespace HabrParser
{
    class Program
    {
        static void Main(string[] args)
        {
            DataCollection Search = new DataCollection();
            List<string> links = new List<string>(); // Лист с ссылками 
            string url = "https://habr.com/company/mailru/";
            //"https://habr.com/company/playcode/blog/"; 
            //"https://habr.com/company/pvs-studio/"; 
            Links My = new Links();
            My.Get_Links(url, links);
            if (links.Count == 0)
            {
                Console.Write("Статей нет!");
            }
            else
            {
                List<InfoSite> myInfoSite = new List<InfoSite>(); // Данные с блога 
                Search.MainDataCollection(links, myInfoSite);
                Console.Write("Готово.");
                FileStream File = new FileStream("F:\\habr\\habraparser\\HabrParser\\HabrParser\\1.csv",
                                                FileMode.OpenOrCreate,
                                                FileAccess.Write);
                StreamWriter Writer = new StreamWriter(File, Encoding.Unicode);
                string Buf = "";
                Writer.WriteLine("Название" + ";" +
                    "Ссылка" + ";" +
                    "Рейтинг" + ";" +
                    "Закладки" + ";" +
                    "Просмотры" + ";" +
                    "Коментарии" + ";" +
                    "Дата" + ";" +
                    "Метки");
                for (int i = 0; i < links.Count; i++)
                {
                    for (int j = 0; j < myInfoSite[i].labels.Count; j++)
                    {
                        if (j==myInfoSite[i].labels.Count)
                        {
                            Buf += myInfoSite[i].labels[j] + ".";
                        }
                        else
                        {
                            Buf += myInfoSite[i].labels[j] + ", ";
                        } 
                    }
                    Writer.WriteLine(myInfoSite[i].name + ";" +
                                     myInfoSite[i].link + ";" +
                                     myInfoSite[i].rating + ";" +
                                     myInfoSite[i].bootmarks + ";" +
                                     myInfoSite[i].views + ";" +
                                     myInfoSite[i].numbOfComments + ";" +
                                     myInfoSite[i].dateOfPublication + ";" +
                                     Buf);
                    Buf = "";
                }
                Writer.Close();
                Console.WriteLine("Все готово!");
            }
            Console.ReadKey();
        }
    }
}
