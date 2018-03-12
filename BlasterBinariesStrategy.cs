using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesBlaster
{
    internal class BlasterBinariesStrategy
    {
        BlasterLogWriter blasterLogWriter;
        DirectoryInfo baseSourceDirectory;
        string PATH_KEY = "[blaster-path]";
        string FILE_KEY = "[blaster-file-name]";

        public BlasterBinariesStrategy(DirectoryInfo baseSourceDirectory, BlasterLogWriter blasterLogWriter)
        {
            this.baseSourceDirectory = baseSourceDirectory;
            this.blasterLogWriter = blasterLogWriter;
        }


        public void Fuse(FileInfo file, DirectoryInfo targetDirectory) {
            //copy file to target folder
            if (!targetDirectory.Exists){targetDirectory.Create();}
            this.blasterLogWriter.Write(file);

            System.IO.File.Copy(file.FullName, targetDirectory + @"\" + file.Name, true);

            appendToIndex(file,targetDirectory);
        }


        public void Extract(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory) {

            var index = this.getIndexFile(sourceDirectory);
            FileInfo file = new FileInfo(".");
            string path = "";

            using (StreamReader sr = index.OpenText())
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains(PATH_KEY)) {
                        path = sr.ReadLine();
                    }
                    s = sr.ReadLine();
                    if (s.Contains(FILE_KEY)) {
                        s = sr.ReadLine();
                        file = new FileInfo(sourceDirectory + @"\" + s);
                        if (file.Exists) {

                            var targetfolder = new DirectoryInfo(targetDirectory + @"\" + path.Replace(s, ""));

                            //create directories from path
                            Directory.CreateDirectory(targetfolder.FullName);

                            //copy binary to target folder
                            System.IO.File.Copy(file.FullName, targetfolder + @"\" + file.Name, true);
                        }
                    }
                }
            }
        }


        private void buildDirectories(DirectoryInfo directoriesPath) {
            var directories = directoriesPath.FullName.Split('/');
            
            //foreach (var directory in directories)
            //{ 
            //}

        }

        private FileInfo getIndexFile(DirectoryInfo targetDirectory) {
            return new FileInfo(targetDirectory + @"\BlasterBinaries.idx");
        }

        private void appendToIndex(FileInfo file, DirectoryInfo targetDirectory) {

            System.IO.StreamWriter sw =
                new System.IO.StreamWriter(this.getIndexFile(targetDirectory).FullName, true);

            var fileLocation = file.FullName.Replace(this.baseSourceDirectory.FullName + "\\", "");
            sw.WriteLine(this.PATH_KEY);
            sw.WriteLine(fileLocation);
            sw.WriteLine(this.FILE_KEY);
            sw.WriteLine(file.Name);
            sw.Close();
        }
    }
}
