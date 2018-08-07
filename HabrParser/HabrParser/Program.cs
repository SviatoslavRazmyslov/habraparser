using System;
using HabrParserLib;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            
            Parser obj = new Parser();
            obj.IncorrectData += Obj_IncorrectData;
            obj.InvalidFilePath += Obj_InvalidFilePath;
            obj.UnableToConnect += Obj_UnableToConnect;
            obj.CallFunctions(args);
            TimeSpan rezult = DateTime.Now - start;
            Console.WriteLine(rezult.ToString());
            Console.WriteLine("Готово.");
            Console.ReadKey();

        }

        private static void Obj_UnableToConnect()
        {
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
