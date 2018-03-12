using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesBlaster
{
    internal class BlasterExtract : BlasterProvider
    {
     

        public BlasterExtract(BlasterProvider BlasterProvider)
        {

        }

        public BlasterExtract(DirectoryInfo source, DirectoryInfo target,
            DirectoryInfo logDirectory, bool onlyBinaries = false)
        {
            this.targetRootDirectory = target;
            this.sourceRootDirectory = source;


            this.binariesDirectory = new DirectoryInfo(this.sourceRootDirectory.FullName.ToString() + @"\blaster_binaries");

            this.blasterLogWriter = new BlasterLogWriter(logDirectory);
            this.blasterBinariesStrategy = new BlasterBinariesStrategy(source, this.blasterLogWriter);
            this.onlyBinaries = onlyBinaries;
        }


        public override void Init(List<BlasterFilter> filters)
        {
            Split();
        }

        

        private void handleBinaries()
        {
            this.blasterBinariesStrategy.Extract(this.binariesDirectory, this.targetRootDirectory);
            Console.WriteLine("Done with binaries.");
        }

        private void Split()
        {
            handleBinaries();
            if (this.onlyBinaries)
            {
                return;
            }
            DirectoryInfo root = this.sourceRootDirectory;
            var sorted = root.GetFiles().OrderBy(f => f.Name);
            int idx = 1;
            foreach (var fi in root.GetFiles())
            {
                Console.WriteLine("Processing file no. " + idx++);
                using (StreamReader sr = fi.OpenText())
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.Contains(PATH_KEY))
                        {
                            buildDirectoryStruct(s);
                        }
                        if (s.Contains("[blaster-file-name]"))
                        {
                            var filenameAndExtension =
                                    s.Substring(s.IndexOf(FILE_KEY) + FILE_KEY.Length);
                            writeFile(filenameAndExtension, sr);

                        }
                    }
                }
            }
            Console.WriteLine("Done with text files.");
        }

        void buildDirectoryStruct(string keyAndValue)
        {
            var path = keyAndValue.Substring(keyAndValue.IndexOf(PATH_KEY) + PATH_KEY.Length);
            if (String.IsNullOrEmpty(path))
            {
                path = "";
            }

            this.contextDirectory = new DirectoryInfo(this.targetRootDirectory + @"\\" + path);
            if (!this.contextDirectory.Exists)
            {
                this.contextDirectory = Directory.CreateDirectory(this.targetRootDirectory + @"\\" + path);
            }
        }

        void writeFile(string filename, StreamReader sr)
        {
            var keyAndValue = sr.ReadLine();
            if (keyAndValue == null) return;
            if (keyAndValue.Contains(START_CONTENTS_KEY))
            {
                string s = "";
                StringBuilder sb = new StringBuilder();
                System.IO.StreamWriter file =
                    new System.IO.StreamWriter(this.contextDirectory.FullName + @"\" + filename);
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains(END_CONTENTS_KEY))
                    {
                        file.Close();
                        return;
                    }

                    if (s.Trim() != "")
                    {
                        file.WriteLine(s);
                    }
                }
            }
        }

    }
}
