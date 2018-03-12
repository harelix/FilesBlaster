using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesBlaster
{
    abstract class BlasterProvider
    {
        public DirectoryInfo contextDirectory;
        public const string PATH_KEY = "[blaster-path]";
        public const string FILE_KEY = "[blaster-file-name]";
        public const string START_CONTENTS_KEY = "[blaster-contents]";
        public const string END_CONTENTS_KEY = "[/blaster-contents]";
        public const long FILE_MAX_SIZE = 5000000;
        public DirectoryInfo targetRootDirectory;
        public DirectoryInfo sourceRootDirectory;
        public DirectoryInfo binariesDirectory;

        protected StreamWriter activeIndexFile;
        protected string indexFileNamePattern = "\\blaster_index_{0}";
        protected int indexCounter = 0;
        protected string indexFilePath;
        protected List<BlasterFilter> filters;
        protected BlasterLogWriter blasterLogWriter;
        protected BlasterBinariesStrategy blasterBinariesStrategy;
        protected bool onlyBinaries = false;
        protected BlasterProvider blasterProvider;

        abstract public void Init(List<BlasterFilter> filters);
    }
}
