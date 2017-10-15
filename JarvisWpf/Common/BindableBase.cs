using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace JarvisWpf.Common
{
    public delegate void Navigate(BindableBase newViewModel, bool AddtoHistory = true);

    abstract public class BindableBase : INotifyPropertyChanged
    {
        Dictionary<string, List<string>> validationErrors = new Dictionary<string, List<string>>();
        ValidationContext vc = null;

        protected Action CanExecuteChangedContainer = delegate { };
        public event Navigate onNavigate;
        public event Action onGoBackInHistory;
        public event Action<string, object> ChildChanged;

        protected void RaiseChildChangedEvent(string Invoker, object parameter)
        {
            if (ChildChanged != null)
                ChildChanged.Invoke(Invoker, parameter);
        }

        private bool _isValid;

        public bool isValid
        {
            get { return _isValid; }
            set { _isValid = value; CanExecuteChangedContainer(); }
        }

        bool _isEditMode;
        public bool isEditMode { get { return _isEditMode; } set { _isEditMode = value; NotifyPropertyChanged("isEditMode"); } }

        public bool IsValidationOn { get; set; }

        private string _ValidationErrors;

        public string ValidationErrors
        {
            get { return _ValidationErrors; }
            set
            {
                _ValidationErrors = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ValidationErrors"));
            }
        }

        //Lazy Loading
        //We do not want huge string in the files to be fetched and stored in memory.
        //That should only be done when it is really absolutely necessary
        //Once the data context is set, set this property to call the overriden function BindBigData in child class.
        public bool BindBigDataNow
        {
            set
            {
                BindBigData();
            }
        }


        public bool UnBindBigDataNow { set { UnBindBigData(); } }

        protected virtual void BindBigData() { }
        protected virtual void UnBindBigData() { }

        public BindableBase()
        {
            _isValid = true;
            vc = new ValidationContext(this, null, null);
            IsValidationOn = false;
        }

        protected virtual void NavigateToView(BindableBase newViewModel)
        {
            if (onNavigate != null)
                onNavigate.Invoke(newViewModel);
        }

        protected virtual void GoBackInHistory()
        {
            if (onGoBackInHistory != null)
                onGoBackInHistory.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string Name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(Name));
            if (IsValidationOn)
                Validate();
        }

        public void Validate()
        {
            List<ValidationResult> validationresults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(this, vc, validationresults, true);
            if (isValid)
            {
                ValidationErrors = "";
            }
            else
            {
                ValidationErrors = string.Join("\n", validationresults.Select(c => c.ErrorMessage).ToArray()) + "\n";
            }
        }


    }
}
