using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesBlaster
{
    internal class BlasterFuse : BlasterProvider
    {
       

        public BlasterFuse(DirectoryInfo source, DirectoryInfo target,
            DirectoryInfo logDirectory, bool onlyBinaries = false)
        {
            this.targetRootDirectory = target;
            this.sourceRootDirectory = source;
            
            this.binariesDirectory = new DirectoryInfo(this.targetRootDirectory.FullName.ToString() + @"\blaster_binaries");

            this.blasterLogWriter = new BlasterLogWriter(logDirectory);
            this.blasterBinariesStrategy = new BlasterBinariesStrategy(source, this.blasterLogWriter);
            this.onlyBinaries = onlyBinaries;
        }

        public override void Init(List<BlasterFilter> filters)
        {
            this.filters = filters;
            this.fuse(this.sourceRootDirectory);
            
            this.activeIndexFile.Flush();
            this.activeIndexFile.Close();
        }

        

        private void fuse(DirectoryInfo root)
        {

            FuseFilesInDirectory(root);

            foreach (var directory in root.GetDirectories())
            {
                fuse(directory);
            }
        }


        private void FuseFilesInDirectory(DirectoryInfo directory)
        {

            var files = directory.GetFiles();
            var refDirectory = "";
            if (files.Length != 0)
            {
                refDirectory = directory.FullName.Replace(this.sourceRootDirectory.FullName, "");
                if (!String.IsNullOrEmpty(refDirectory))
                {
                    refDirectory = refDirectory.Substring(1);
                    Console.WriteLine(refDirectory);
                }
            }


            foreach (var file in files)
            {
                manageIndexFileStates();
                foreach (var filter in this.filters)
                {
                    if (!filter.Exclude(file))
                    {
                        if (file.FullName.Contains("_variables")){
                            var a = "";
                        }
                        string text = File.ReadAllText(file.FullName);
                        string filename = file.Name;
                        writeEntryInLog(PATH_KEY, refDirectory);
                        writeEntryInLog(FILE_KEY, filename);
                        writeEntryInLog(START_CONTENTS_KEY, "");
                        writeEntryInLog("", text);
                        writeEntryInLog(END_CONTENTS_KEY, "");
                    }
                    else
                    {
                        this.blasterBinariesStrategy.Fuse(file, this.binariesDirectory);
                    }
                }
            }
        }

        public void writeEntryInLog(string key, string value)
        {
            activeIndexFile.WriteLine(key + value);
        }

        void manageIndexFileStates()
        {

            this.indexFilePath = this.targetRootDirectory.FullName +
                    String.Format(indexFileNamePattern, indexCounter) + ".txt";

            if (Object.Equals(this.activeIndexFile, null))
            {
                this.activeIndexFile = new StreamWriter(this.indexFilePath);
            }

            if (!File.Exists(this.indexFilePath))
            {
                File.Create(this.indexFilePath);
            }

            if (isFileQuotaExceeded())
            {
                if (!Object.Equals(this.activeIndexFile, null))
                {
                    this.activeIndexFile.Close();
                    this.activeIndexFile.Dispose();
                    this.indexCounter++;

                    this.indexFilePath = this.targetRootDirectory.FullName +
                    String.Format(indexFileNamePattern, indexCounter) + ".txt";

                    this.activeIndexFile = new StreamWriter(this.indexFilePath);
                }
            }
        }

        bool isFileQuotaExceeded()
        {
            this.activeIndexFile.Flush();
            long fileSize = new FileInfo(this.indexFilePath).Length;

            if (fileSize > FILE_MAX_SIZE)
                return true;
            else
                return false;
        }

      
    }
}
