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
            try
            {
                DateTime start = DateTime.Now;
                Options arguments = null;
                CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(opts => arguments = opts);

                if (arguments == null)
                    return;

                HabrParserLib.Parser parser = new HabrParserLib.Parser();
                parser.ErrorOccured += (sender, msg) => Console.WriteLine(msg);

                OutFile.Output(parser.Parse(arguments.OutputFile, arguments.InputFile, arguments.SearchDepth), arguments.OutputFile);

                TimeSpan rezult = DateTime.Now - start;
                Console.WriteLine(rezult.ToString());
                Console.WriteLine("Finished.");
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}