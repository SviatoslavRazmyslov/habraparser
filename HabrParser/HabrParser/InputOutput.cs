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
        private readonly string Sym = ";";
        private readonly string EMPTY = "";

        public string GetLinkBlog()
        {
            Console.Write("Введите ссылку на блог, в файл Input.txt и нажмите Enter\n");
            FileStream Inpt = new FileStream("Input.txt",
                                                FileMode.Open);
            StreamReader reader = new StreamReader(Inpt);
            string fileInput = reader.ReadLine();
            Inpt = null;
            reader.Close();
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
                             LABEL);
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
                Writer.WriteLine(myInfoSite[i].name + Sym +
                                 myInfoSite[i].link + Sym +
                                 myInfoSite[i].rating + Sym +
                                 myInfoSite[i].bootmarks + Sym +
                                 myInfoSite[i].views + Sym +
                                 myInfoSite[i].numbOfComments + Sym +
                                 myInfoSite[i].dateOfPublication + Sym +
                                 Buf);
                Buf = EMPTY;
            }
            Writer.Close();
        }
    }
}
