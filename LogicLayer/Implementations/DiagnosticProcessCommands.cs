using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogicLayer.Implementations
{
    public class StartProcessCommand:ICustomCommand
    {
        public string Argument { get; set; }
        IResourceProvider ResourceProvider = null;

        public StartProcessCommand()
        {
            ResourceProvider = ProviderFactory.GetCurrentProvider();
        }

        public StartProcessCommand(string Argument)
            : this()
        {
            this.Argument = Argument;
        }

        public void Execute()
        {
            
            if (!String.IsNullOrWhiteSpace(Argument))
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessRequestInNewThread),Argument);
            }
        }

        private void ProcessRequestInNewThread(object state)
        {
            try
            {
                string Arg = (string)state;
                Process.Start(Arg);
            }
            catch (Exception)
            {
                //Swallow the exception
            }
        }
    }


    
}
