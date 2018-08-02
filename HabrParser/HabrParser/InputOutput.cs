using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DataCollectionNameSpace;

namespace InOut
{

    class InputOutput
    {
        private readonly string NAME = "Название";
        private readonly string LINK = "Ссылка";
        private readonly string RAITING = "Рейтинг";
        private readonly string BOOTMARKS = "Закладки";
        private readonly string VIEWS = "Просмотры";
        private readonly string NUMBOFCOMMENTS = "Коментарии";
        private readonly string DATEOFPUBLICATION = "Дата";
        private readonly string LABEL = "Метки";
        private readonly string HUBS = "Хабы";
        private readonly string Sym = ";";

        public void Input(string[] args, List<InfoMoreBlogsWithHabr> InfoMoreBlogs)
        {
            InfoMoreBlogsWithHabr InfoSingleBlog = new InfoMoreBlogsWithHabr();
            FileStream Input = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(Input);
            for (int i = 0; i < File.ReadAllLines(args[0]).Length; i++)
            {
                string bufStringData = reader.ReadLine();
                string[] result = bufStringData.Split(' ');
                foreach (string one in result)
                {
                    if (one.Contains("HREF:"))
                    {
                        string res = one.Replace("HREF:", "");
                        InfoSingleBlog.hrefBlogs = res;
                         
                    }
                    if (one.Contains("NUMBSTR:"))
                    {
                        string res = one.Replace("NUMBSTR:", "");
                        InfoSingleBlog.searchDepth = Convert.ToInt32(res);
                    }
                    if (one.Contains("PATHOUT:"))
                    {
                        string res = one.Replace("PATHOUT:", "");
                        InfoSingleBlog.pathOutFile = res;
                    }
                }
                InfoMoreBlogs.Add(InfoSingleBlog);
            }

        }

        public void Output(List<InfoSite> myInfoSite)
        {
            FileStream File = new FileStream("Company.csv",
                                                FileMode.Create,
                                                FileAccess.Write);
            StreamWriter Writer = new StreamWriter(File, Encoding.Unicode);
            Writer.WriteLine(NAME + Sym +
                             LINK + Sym +
                             RAITING + Sym +
                             BOOTMARKS + Sym +
                             VIEWS + Sym +
                             NUMBOFCOMMENTS + Sym +
                             DATEOFPUBLICATION + Sym +
                             LABEL + Sym +
                             HUBS);

            for (int i = 0; i < myInfoSite.Count; i++)
            {
                string buf_labels = string.Join(", ", myInfoSite[i].labels);
                string buf_hubs = string.Join(", ", myInfoSite[i].hubs);

                Writer.WriteLine(myInfoSite[i].name + Sym +
                                 myInfoSite[i].link + Sym +
                                 myInfoSite[i].rating + Sym +
                                 myInfoSite[i].bootmarks + Sym +
                                 myInfoSite[i].views + Sym +
                                 myInfoSite[i].numbOfComments + Sym +
                                 myInfoSite[i].dateOfPublication + Sym +
                                 buf_labels + Sym +
                                 buf_hubs);
            }
            Writer.Close();
        }
    }
}
