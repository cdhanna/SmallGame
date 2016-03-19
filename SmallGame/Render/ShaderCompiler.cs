using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SmallGame.Render
{
    public static class ShaderCompiler
    {
        [Conditional("DEBUG")]
        public static void Init(string contentFolder, string toolFolder)
        {
            
            var cDir = Directory.GetCurrentDirectory();
            var fDir = cDir + Path.DirectorySeparatorChar + contentFolder;

            Directory.GetFiles(fDir, "*.fx", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".fx"))
                .ToList()
                .ForEach(f =>
                {
                    //var name = f.Substring(f.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    //var path = f.Substring(0, f.Length - name.Length);
                    var output = f + ".mgfxo";
                    CompileShader(toolFolder, f);
                    Console.WriteLine(f);
                });

        }

        private static void CompileShader(string toolPath, string input)
        {
            var startInfo = new ProcessStartInfo();
            var p = new Process();

            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;

            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = toolPath + "\\";
            startInfo.Arguments = input + " " + input + ".mgfxo";
            startInfo.FileName = toolPath + "\\2MGFX.exe";
           
            p.StartInfo = startInfo;
            p.Start();

            //p.OutputDataReceived += new DataReceivedEventHandler
            //(
            //    delegate(object sender, DataReceivedEventArgs e)
            //    {
            //        using (StreamReader output = p.StandardOutput)
            //        {
            //            retMessage = output.ReadToEnd();
            //        }
            //    }
            //);

            p.WaitForExit();
        }
       

    }
}
