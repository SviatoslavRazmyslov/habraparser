using System;
using System.Collections.Generic;
using List_Links;
using DataCollectionNameSpace;
using InOut;
using System.IO;


namespace HabrParser
{
    struct dataInput
    {
        public string hrefBlog;
        public string pathOutFile;
        public int searchDepth;
    }

    class Program
    {
        static void Main(string[] args)
        {

            dataInput dataInputOfFile = new dataInput();
            FileStream Input = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(Input);
            List<dataInput> dataAllBlogs = new List<dataInput>();
            for (int i = 0; i < File.ReadAllLines(args[0]).Length; ++i)
            {
                string bufStringData = reader.ReadLine();
                string[] result = bufStringData.Split(' ');
                foreach (string one in result)
                {
                    if (one.Contains("HREF:"))
                    {
                        string res =  one.Replace("HREF:", "");
                        dataInputOfFile.hrefBlog = res;
                    }
                    if (one.Contains("NUMBSTR:"))
                    {
                        string res = one.Replace("NUMBSTR:", "");
                        dataInputOfFile.searchDepth = Convert.ToInt32(res);
                    }
                    if (one.Contains("PATHOUT:"))
                    {
                        string res = one.Replace("PATHOUT:", "");
                        dataInputOfFile.pathOutFile = res;
                    }
                }
                dataAllBlogs.Add(dataInputOfFile);
            }

            DataCollection Search = new DataCollection();
            List<string> links = new List<string>(); // Лист с ссылками
            Links ObjLinks = new Links();
            InputOutput IO = new InputOutput();
            ObjLinks.GetLinks(IO.GetLinkBlog(), links);
            if (links.Count == 0)
            {
                Console.Write("В данном блоге статей нет!");
            }
            else
            {
                List<InfoSite> myInfoSite = new List<InfoSite>(); // Данные с блога 
                Console.WriteLine("Начинается сбор данных...");
                Search.MainDataCollection(links, myInfoSite);
                Console.WriteLine("Данные собраны!");
                Console.WriteLine("Переносим все в CSV-файл...");
                IO.Output(myInfoSite);
                Console.Write("Готово!");
            }
            Console.ReadKey();
        }
    }
}
