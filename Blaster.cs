using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesBlaster
{
    internal class Blaster
    {
        BlasterProvider blasterProvider;
        public Blaster(BlasterProvider BlasterProvider) {
            this.blasterProvider = BlasterProvider;
        }


        public void Init(List<BlasterFilter> blasterFilters = null) {
            this.blasterProvider.Init(blasterFilters);
        }

    }
}
