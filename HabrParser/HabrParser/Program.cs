using System;
using System.Collections.Generic;
using List_Links;
using DataCollectionNameSpace;
using InOut;
using System.IO;


namespace HabrParser
{

    class Program
    {
        static void Main(string[] args)
        {
            List<dataInput> dataAllBlogs = new List<dataInput>();
            InputOutput IO = new InputOutput();
            IO.Input(args, dataAllBlogs);
            DataCollection Search = new DataCollection();
            List<string> links = new List<string>(); // Лист с ссылками
            Links ObjLinks = new Links();
            ObjLinks.GetLinks(dataAllBlogs);
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
