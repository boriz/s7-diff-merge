using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMCBase;

namespace S7_GenerateSource
{
    partial class Blocks : NotifyPropertyChangedBase
    {
        private Block _LeftBlock;
        public Block LeftBlock
        {
            get
            {
                return _LeftBlock;
            }
            set
            {
                _LeftBlock = value;
                NotifyPropertyChanged("LeftBlock");
            }
        }
        private Block _RightBlock;
        public Block RightBlock
        {
            get
            {
                return _RightBlock;
            }
            set
            {
                _RightBlock = value;
                NotifyPropertyChanged("RightBlock");
            }
        }
    }
}
