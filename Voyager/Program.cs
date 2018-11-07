using System;

namespace Voyager
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Instance.OpenFile(); //open file to log
            var voyager = new Voyager();
            voyager.CreateVoyage();            
            Logger.Instance.CloseFile(); //close log file
            Console.ReadKey();
        }
    }
}
