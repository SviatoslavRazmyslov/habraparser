using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DataCollectionNameSpace;

namespace InOut
{
    public struct dataInput
    {
        public string hrefBlog;
        public string pathOutFile;
        public int searchDepth;
    }

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

        public void Input(string[] args, List<dataInput> dataAllBlogs)
        {
            dataInput dataInputSingle = new dataInput();
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
                        dataInputSingle.hrefBlog = res;
                         
                    }
                    if (one.Contains("NUMBSTR:"))
                    {
                        string res = one.Replace("NUMBSTR:", "");
                        dataInputSingle.searchDepth = Convert.ToInt32(res);
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

        public void Output(InfoMoreBlogsWithHabr myInfoBlog)
        {
            FileStream File = new FileStream(myInfoBlog.pathOutFile,
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

            for (int i = 0; i < myInfoBlog.InfoSingeBlogs.Count; i++)
            {
                string buf_labels = string.Join(", ", myInfoBlog.InfoSingeBlogs[i].labels);
                string buf_hubs = string.Join(", ", myInfoBlog.InfoSingeBlogs[i].hubs);

                Writer.WriteLine(myInfoBlog.InfoSingeBlogs[i].name + Sym +
                                 myInfoBlog.InfoSingeBlogs[i].link + Sym +
                                 myInfoBlog.InfoSingeBlogs[i].rating + Sym +
                                 myInfoBlog.InfoSingeBlogs[i].bootmarks + Sym +
                                 myInfoBlog.InfoSingeBlogs[i].views + Sym +
                                 myInfoBlog.InfoSingeBlogs[i].numbOfComments + Sym +
                                 myInfoBlog.InfoSingeBlogs[i].dateOfPublication + Sym +
                                 buf_labels + Sym +
                                 buf_hubs);
            }
            Writer.Close();
        }
    }
}
