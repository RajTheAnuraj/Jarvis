using JarvisWpf.Common;
using JarvisWpf.Document;
using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JarvisWpf.Common
{
    public class ApplicationContextViewModel: BindableBase
    {
        IResourceProvider ResourceProvider = null;
        public string Name { get; set; }
        public RelayCommand<object> ContextMenuSelectedCommand { get; set; }
        public Brush Icon { get; set; }
        public ApplicationContextMenuPayload UnderlyingObject { get; set; }

        public ObservableCollection<ApplicationContextViewModel> innerMenu { get; set; }

        public ApplicationContextViewModel(string Name)
        {
            this.Name = Name;
            this.ContextMenuSelectedCommand = new RelayCommand<object>(ContextMenuSelected);
            ResourceProvider = ProviderFactory.GetCurrentProvider();
        }


        public ApplicationContextViewModel(ApplicationContextMenuPayload payload):this(payload.DisplayName)
        {
            UnderlyingObject = payload;
            if (UnderlyingObject.innerList != null)
                innerMenu = new ObservableCollection<ApplicationContextViewModel>(
                    UnderlyingObject.innerList.Select(c => new ApplicationContextViewModel(c))
                    );
        }

        private void ContextMenuSelected(object obj)
        {
            if (UnderlyingObject != null)
            {
                string Args = null;

                if (UnderlyingObject.isAction || UnderlyingObject.Format != "Rich Text Format")
                    Args = UnderlyingObject.ActionStringText;
                else
                    Args = UnderlyingObject.ActionString;

                if (UnderlyingObject.isAction)
                {
                    ICustomCommand openCommand = ResourceProvider.GetStartProcessCommand(Args);
                    openCommand.Execute();
                }
                else
                {
                    ICustomCommand clipboardCommand = ResourceProvider.GetCopyTextToClipBoardCommand(Args, UnderlyingObject.Format);
                    clipboardCommand.Execute();
                }
            }
        }


       
    }
}
