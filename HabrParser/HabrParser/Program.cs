using System;
using HabrParserLib;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            
            Parser parser = new Parser();
            parser.IncorrectData += Obj_IncorrectData;
            parser.InvalidFilePath += Obj_InvalidFilePath;
            parser.UnableToConnect += Obj_UnableToConnect;
            parser.Parse(args);
            TimeSpan rezult = DateTime.Now - start;
            Console.WriteLine(rezult.ToString());
            Console.WriteLine("Finished.");
            Console.ReadKey();

        }

        private static void Obj_UnableToConnect()
        {
            //TODO убрать сообщения внутрь класса, использовать для event'а сигнатуру EventHandler<String>
            //в обработчике оставить только выдачу пришедшего сообщения в консоль
            //Т.о. можно 3 event'а заменить на 1, например "ErrorOccured"
            Console.WriteLine("Check your internet connection...");
            Console.ReadKey();
        }

        private static void Obj_InvalidFilePath()
        {
            Console.Write("Invalid File Path");
        }

        private static void Obj_IncorrectData()
        {
            Console.Write("Incorrect data in the file");
        }
    }
}
