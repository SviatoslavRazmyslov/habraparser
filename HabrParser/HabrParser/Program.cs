using System;
using HabrParserLib;
using CommandLine;
using OutputInfo;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {     
            DateTime start = DateTime.Now;
            Options arguments = null;
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(opts => arguments = opts);

            if (arguments == null)
                return;

            HabrParserLib.Parser parser = new HabrParserLib.Parser();
            parser.ErrorOccured += Parser_ErrorOccured;

            OutFile.Output(parser.Parse(arguments), arguments.outputFile);

            TimeSpan rezult = DateTime.Now - start;
            Console.WriteLine(rezult.ToString());
            Console.WriteLine("Finished.");
            Console.ReadKey();
        }

        private static void Parser_ErrorOccured(object sender, string msg)
        {
            Console.WriteLine(msg);
        }
    }
}