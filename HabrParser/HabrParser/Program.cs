using System;
using System.Collections.Generic;
using List_Links;
using DataCollectionNameSpace;
using InOut;


namespace HabrParser
{
    class Program
    {
        static void Main(string[] args)
        {
            DataCollection Search = new DataCollection();
            List<string> links = new List<string>(); // Лист с ссылками
            Links ObjLinks = new Links();
            InputOutput IO = new InputOutput();
            links = ObjLinks.GetLinks(IO.GetLinkBlog(), links);
            if (links == null)
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
