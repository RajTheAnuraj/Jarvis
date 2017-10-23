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

namespace JarvisWpf
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        Stack<BindableBase> _History = null;
        BindableBase _CurrentViewModel;
        
        ProjectListViewModel _ProjectList;

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

        public StatusBarViewModel statusBarData { get; set; }

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

        public ApplicationViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            History = new Stack<BindableBase>();
            ProjectList = new ProjectListViewModel();
            statusBarData = new StatusBarViewModel();
            ProjectList.statusBarData = statusBarData;
            NavigateToView(ProjectList);
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
