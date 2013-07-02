using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S7_DMCToolbox
{
    class ApplicationModel
    {
        private String _sCurrentProjectPath;
        public String sCurrentProjectPath
        {
            get
            {
                if (_sCurrentProjectPath == null)
                {
                    _sCurrentProjectPath = Properties.Settings.Default.ProjectPath;
                }
                if (_sCurrentProjectPath == null)
                {
                    _sCurrentProjectPath = String.Empty;
                }
                return _sCurrentProjectPath;
            }
            set
            {
                _sCurrentProjectPath = value;
            }
        }
    }
}
