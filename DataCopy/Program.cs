using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DataCopy
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("hello world");

            var argList = args.ToList();
            if (argList.Count < 2)
            {
                Console.WriteLine("Must have 2 args");
                return;
            }

            var watchFolder = argList[0];
            var outFolder = argList[1];

            var watcher = new FileSystemWatcher(watchFolder, "*.json");
            watcher.NotifyFilter = NotifyFilters.LastWrite;

            watcher.Created += (s, a) =>
            {
                Copy(a.Name, watchFolder, outFolder);
            };
            watcher.Changed += (s, a) =>
            {
                Copy(a.Name, watchFolder, outFolder);
            };

            watcher.EnableRaisingEvents = true;

            while (true)
            {
                
            }

        }

        static void Copy(string file, string from, string to)
        {
            Console.WriteLine("Changed " + file);
            File.Copy(from + "/" + file, to + "/" + file, true);
        }

    }
}
