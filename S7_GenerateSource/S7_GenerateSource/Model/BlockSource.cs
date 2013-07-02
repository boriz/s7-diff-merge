using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S7_GenerateSource
{
    class BlockSource : Block
    {
        public String Filename { get; set; }
        public String SourceText { get; set; }
    }
}
