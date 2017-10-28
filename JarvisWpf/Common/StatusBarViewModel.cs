using LogicLayer.Factories;
using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JarvisWpf.Common
{
    public class StatusBarViewModel: BindableBase, ICrosstalk
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


        public object IncomingCrosstalk(string ActionName, object[] Parameters)
        {
            if (Parameters != null)
            {
                if (Parameters.Length > 0)
                {
                    StatusText = Convert.ToString(Parameters[0]);
                }
            }
            return null;
        }
    }
}
