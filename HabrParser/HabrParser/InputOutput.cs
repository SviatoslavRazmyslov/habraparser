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
                var buf_labels = string.Join(", ", myInfoSite[i].labels);
                var buf_hubs = string.Join(", ", myInfoSite[i].hubs);

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
