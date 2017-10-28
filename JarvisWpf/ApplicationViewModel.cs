using LogicLayer;
using LogicLayer.Common;
using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Implementations;
using LogicLayer.Payloads;
using JarvisWpf.Common;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using JarvisWpf.Project;
using JarvisWpf.Common;
using System.Collections.ObjectModel;

namespace JarvisWpf
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        Stack<BindableBase> _History = null;
        BindableBase _CurrentViewModel;
        IResourceProvider ResourceProvider = null;

        ProjectListViewModel _ProjectList;

        public RelayCommand<object> MaximizeMainWindowCommand { get; set; }

        public ProjectListViewModel ProjectList
        {
            get
            {
                return _ProjectList;
            }
            set
            {
                _ProjectList = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProjectList"));
            }
        }

        public List<ApplicationContextMenuPayload> AppContextMenuPayload { get; set; }

        public ObservableCollection<ApplicationContextViewModel> ApplicationContextMenu
        {
            get
            {
                return new ObservableCollection<Common.ApplicationContextViewModel>(
                    AppContextMenuPayload.Select(c => new ApplicationContextViewModel(c))
                    );
            }
        }


        public Stack<BindableBase> History
        {
            get { return _History; }
            set
            {
                _History = value;
            }
        }

        public BindableBase CurrentViewModel
        {
            get
            {
                return _CurrentViewModel;
            }
            set
            {
                _CurrentViewModel = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentViewModel"));
            }
        }

        public StatusBarViewModel statusBarData { get; set; }

        public ApplicationViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            ResourceProvider = ProviderFactory.GetCurrentProvider();
            statusBarData = new Common.StatusBarViewModel();
            ResourceProvider.CrosstalkService.RegisterCallback("StatusBar", statusBarData);
            AppContextMenuPayload = new List<ApplicationContextMenuPayload>();
            FillAppContextMenu();
            MaximizeMainWindowCommand = new Common.RelayCommand<object>(MaximizeMainWindow);
            History = new Stack<BindableBase>();
            ProjectList = new ProjectListViewModel();
            CommonItems.CommonItemsViewModel.CommonItemsSaved += CommonItemsViewModel_CommonItemsSaved;
            NavigateToView(ProjectList);
        }

        void CommonItemsViewModel_CommonItemsSaved()
        {
            FillAppContextMenu();
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ApplicationContextMenu"));
        }


        private void MaximizeMainWindow(object sender)
        {
            System.Windows.Window window = sender as System.Windows.Window;
            if (window == null) return;

            window.Show();
            window.Activate();
        }

        public void WindowClosing(object sender, object e)
        {
            System.Windows.Window window = sender as System.Windows.Window;
            System.ComponentModel.CancelEventArgs cancelEvtArg = e as System.ComponentModel.CancelEventArgs;

            if (sender != null || cancelEvtArg != null)
            {
                window.Hide();
                cancelEvtArg.Cancel = true;
            }
        }

        private void FillAppContextMenu()
        {
            ICustomCommand<List<ApplicationContextMenuPayload>> menuRetrieveCommand = ResourceProvider.GetRetrieveCommonItemCommand();
            AppContextMenuPayload = menuRetrieveCommand.Execute();
        }

        private bool ShowHistory()
        {
            return History.Count > 0;
        }

        

        private void NavigateToView(BindableBase newViewModel,bool AddtoHistory=true)
        {
            CurrentViewModel = newViewModel;
            if(newViewModel.IsValidationOn) newViewModel.Validate();
            newViewModel.onNavigate += NavigateToView;
        }


        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
