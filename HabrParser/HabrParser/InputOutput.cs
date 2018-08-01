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
        private readonly string EMPTY = "";

        public string GetLinkBlog()
        {
            Console.WriteLine("Читаем ссылку на блог...");
            FileStream Inpt = new FileStream("Input.txt",
                                                FileMode.Open);
            StreamReader reader = new StreamReader(Inpt);
            string fileInput = reader.ReadLine();
            Inpt = null;
            reader.Close();
            Console.WriteLine("Ссылка прочитана!");
            return fileInput;

        }

        public void Output(List<InfoSite> myInfoSite)
        {
            FileStream File = new FileStream("Company.csv",
                                                FileMode.Create,
                                                FileAccess.Write);
            StreamWriter Writer = new StreamWriter(File, Encoding.Unicode);
            string Buf = EMPTY;
            Writer.WriteLine(NAME + Sym +
                             LINK + Sym +
                             RAITING + Sym +
                             BOOTMARKS + Sym +
                             VIEWS + Sym +
                             NUMBOFCOMMENTS + Sym +
                             DATEOFPUBLICATION + Sym +
                             LABEL + Sym +
                             HUBS);
            string Buf2 = EMPTY;
            for (int i = 0; i < myInfoSite.Count; i++)
            {
                for (int j = 0; j < myInfoSite[i].labels.Count; j++)
                {
                    if (j+1 < myInfoSite[i].labels.Count)
                    {
                        Buf += myInfoSite[i].labels[j] + ", ";
                    }
                    else
                    {
                        Buf += myInfoSite[i].labels[j] + ".";
                    }
                }

                for (int j = 0; j < myInfoSite[i].hubs.Count; j++)
                {
                    if (j + 1 < myInfoSite[i].hubs.Count)
                    {
                        Buf2 += myInfoSite[i].hubs[j] + ", ";
                    }
                    else
                    {
                        Buf2 += myInfoSite[i].hubs[j] + ".";
                    }
                }

                Writer.WriteLine(myInfoSite[i].name + Sym +
                                 myInfoSite[i].link + Sym +
                                 myInfoSite[i].rating + Sym +
                                 myInfoSite[i].bootmarks + Sym +
                                 myInfoSite[i].views + Sym +
                                 myInfoSite[i].numbOfComments + Sym +
                                 myInfoSite[i].dateOfPublication + Sym +
                                 Buf + Sym +
                                 Buf2);
                Buf = EMPTY;
                Buf2 = EMPTY;
            }
            Writer.Close();
        }
    }
}
