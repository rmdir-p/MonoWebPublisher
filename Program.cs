using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MonoWebPublisher
{
    class Program
    {
        /// <example>mono MonoWebPublisher.exe sample.csproj /var/www/sample-dest-publish-dir</example>
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Parameter not match!");
                Environment.Exit(1);
            }
            string projectFile = args[0];
            string destDir = args[1];
            string sourceDir = Path.GetDirectoryName(projectFile);

            //delete everything in destDir but .git folder
            if (Directory.Exists(destDir))
            {
                string[] destDirs = Directory.GetDirectories(destDir, "*", SearchOption.TopDirectoryOnly);
                destDirs.ToList<string>().ForEach(n =>
                {
                    if (Path.GetFileName(n) != ".git")
                    {
                        Directory.Delete(n, true);
                    }
                });
                string[] destFiles = Directory.GetFiles(destDir, "*", SearchOption.TopDirectoryOnly);
                destFiles.ToList<string>().ForEach(n =>
                {
                    File.Delete(n);
                });

            }

            //copy included files
            List<string> fileList = GetIncludedFiles(projectFile);
            fileList.ForEach(n =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(destDir, n)));
                File.Copy(Path.Combine(sourceDir, n), Path.Combine(destDir, n), true);
            });

            //copy bin folder
            string[] binFiles = Directory.GetFiles(Path.Combine(sourceDir, "bin"));
            Directory.CreateDirectory(Path.Combine(destDir, "bin"));
            binFiles.ToList<string>().ForEach(n =>
            {
                File.Copy(n, Path.Combine(Path.Combine(destDir, "bin"), Path.GetFileName(n)), true);
            });
        }

        private static List<string> GetIncludedFiles(string projectFile)
        {
            XDocument xmlDoc;
            xmlDoc = XDocument.Load(projectFile);
            var result = xmlDoc.Descendants(xmlDoc.Root.Name.Namespace + "Content")
                .Where(node => node.Attribute("Include") != null)
                .Select(node => node.Attribute("Include").Value.Replace("\\", Path.DirectorySeparatorChar.ToString()));

            return result.ToList<string>();
        }
    }
}
