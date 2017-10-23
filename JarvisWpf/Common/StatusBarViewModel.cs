using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JarvisWpf.Common
{
    public class StatusBarViewModel: BindableBase
    {
        private string _StatusText;

        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                _StatusText = value;
                NotifyPropertyChanged("StatusText");
            }
        }

    }
}
