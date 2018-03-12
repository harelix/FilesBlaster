using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesBlaster
{
    internal class BlasterFilter
    {

        string[] fileTypes = new string[19] { "jar", "exe", "node", "zip", "tgz" , "gif",
            "jpeg", "jpg", "bmp" , "tiff", "png", "zip","rar","tar", "msi", "dll", "pdb", "cache", "nupkg"};

        public BlasterFilter()
        {
            //file types/extensions filter
        }


        public bool Exclude(FileInfo file) {
            foreach (var extension in fileTypes)
            {
                if (file.Extension.ToLower().Contains(extension)) {
                 return true;
                }
            }
            return false;
        }
    }
}
