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


            List<InfoMoreBlogsWithHabr> InfoMoreBlogs = new List<InfoMoreBlogsWithHabr>();

            InputOutput IO = new InputOutput();
            IO.Input(args, InfoMoreBlogs);
            DataCollection Search = new DataCollection();
            Links ObjLinks = new Links();




            //ObjLinks.GetLinks();
            //if (links.Count == 0)
            //{
            //    Console.Write("В данном блоге статей нет!");
            //}
            //else
            //{
                List<InfoSite> myInfoSite = new List<InfoSite>(); // Данные с блога 
                Console.WriteLine("Начинается сбор данных...");
                //Search.MainDataCollection(links, myInfoSite);
                Console.WriteLine("Данные собраны!");
                Console.WriteLine("Переносим все в CSV-файл...");
                IO.Output(myInfoSite);
                Console.Write("Готово!");
           // }
            Console.ReadKey();
        }
    }
}
