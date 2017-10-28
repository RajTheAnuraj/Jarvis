using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWpf.Common
{
    public class MessageBoxService: ICrosstalk
    {
        private MessageBoxService()
        {

        }

        static MessageBoxService staticInstance;

        public static MessageBoxService CreateInstance()
        {
            if (staticInstance == null)
                staticInstance = new MessageBoxService();
            return staticInstance;
        }

        public object IncomingCrosstalk(string ActionName, object[] Parameters)
        {
            if (ActionName == "Show")
            {
                if (Parameters != null)
                    if (Parameters.Length > 0)
                    {
                        System.Windows.MessageBox.Show(Convert.ToString(Parameters[0]));
                    }
            }
            return null;
        }
    }
}
