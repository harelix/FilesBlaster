using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FilesBlaster
{
    internal class BlasterExtractParallelTaskManager : BlasterProvider
    {
        int numberOfTaskToRun = 100;
        Queue<FileInfo> indexFilesQueue = new Queue<FileInfo>();


        public BlasterExtractParallelTaskManager(DirectoryInfo source, DirectoryInfo target,
            DirectoryInfo logDirectory, bool onlyBinaries = false, int numberOfTaskToRun = 100)
        {
            //in advance divide the index files to tasks + binaries handler task
            this.targetRootDirectory = target;
            this.sourceRootDirectory = source;

            this.binariesDirectory = new DirectoryInfo(this.sourceRootDirectory.FullName.ToString() + @"\blaster_binaries");

            this.blasterLogWriter = new BlasterLogWriter(logDirectory);
            this.blasterBinariesStrategy = new BlasterBinariesStrategy(source, this.blasterLogWriter);
            this.onlyBinaries = onlyBinaries;
            numberOfTaskToRun = this.numberOfTaskToRun;
            //Tast T = Task.Factory.StartNew(code)
        }

        public override void Init(List<BlasterFilter> filters)
        {
            this.enqueueBlasterIndexFiles();
            handleBinaries();

            numberOfTaskToRun = numberOfTaskToRun > this.indexFilesQueue.Count ? this.indexFilesQueue.Count  : numberOfTaskToRun;
            Task[] taskArray = new Task[this.numberOfTaskToRun];


            for (int i = 0; i < taskArray.Length; i++)
            {
                var file = this.indexFilesQueue.Dequeue();
                taskArray[i] = Task.Factory.StartNew(() =>
                    processFile(file));
            }

            Task.WaitAll(taskArray);
            


            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Done fusing files!");
        }


        private void handleBinaries()
        {
            this.blasterBinariesStrategy.Extract(this.binariesDirectory, this.targetRootDirectory);
            Console.WriteLine("Done with binaries.");
        }



        void processFile(FileInfo file)
        {
            Console.WriteLine("Processing file: " + file.Name);

            using (StreamReader sr = file.OpenText())
            {
                string s = "";
                DirectoryInfo contextualDirectory = new DirectoryInfo(".");
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains(PATH_KEY))
                    {
                        contextualDirectory = buildDirectoryStruct(s);
                    }
                    if (s.Contains("[blaster-file-name]"))
                    {
                        var filenameAndExtension =
                                s.Substring(s.IndexOf(FILE_KEY) + FILE_KEY.Length);
                        writeFile(contextualDirectory, filenameAndExtension, sr);

                    }
                }
            }
        }

        void writeFile(DirectoryInfo directory, string filename, StreamReader sr)
        {
            var keyAndValue = sr.ReadLine();
            if (keyAndValue == null) return;
            if (keyAndValue.Contains(START_CONTENTS_KEY))
            {
                string s = "";
                StringBuilder sb = new StringBuilder();
                System.IO.StreamWriter file =
                    new System.IO.StreamWriter(directory.FullName + @"\" + filename);

                bool lineEmpty = false;
                bool consecutiveBlankLines = false;

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

        DirectoryInfo buildDirectoryStruct(string keyAndValue)
        {
            var path = keyAndValue.Substring(keyAndValue.IndexOf(PATH_KEY) + PATH_KEY.Length);
            if (String.IsNullOrEmpty(path))
            {
                path = "";
            }

            var directory = new DirectoryInfo(this.targetRootDirectory + @"\\" + path);
            if (!directory.Exists)
            {
                directory = Directory.CreateDirectory(this.targetRootDirectory + @"\\" + path);
            }
            return directory;

        }


        void enqueueBlasterIndexFiles() {

            var files = this.sourceRootDirectory.GetFiles().OrderBy(f => f.Name).Where<FileInfo>(x => x.FullName.Contains("blaster_index"));
            foreach (var file in files)
            {
                this.indexFilesQueue.Enqueue(file);
            }
        }
    }
}
