using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesBlaster
{
    internal class BlasterLogWriter
    {
        DirectoryInfo logDirectory;
        public BlasterLogWriter(DirectoryInfo logDirectory)
        {
            this.logDirectory = logDirectory;
            if (!this.logDirectory.Exists){
                this.logDirectory.Create();
            }
        }


        public void Write(FileInfo info)
        {
            string message = info.FullName;
            
            System.IO.StreamWriter file =
                new System.IO.StreamWriter(this.logDirectory+ @"\log_blaster.log", true);
            file.WriteLine("Excluded from package:" + message);
            file.Close();
        }
    }
}
