using JarvisWpf.Common;
using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWpf.CommonItems
{
    public class CommonItemsViewModel: BindableBase
    {
        public static event Action CommonItemsSaved;

        IResourceProvider ResourceProvider = null;
        public ObservableCollection<string> Categories
        {
            get
            {
                return new ObservableCollection<string>(AppContextMenuPayload.Where(c => c.isCategory == true && c.DisplayName != "External Tools").Select(c => c.DisplayName).ToList());
            }
        }

        string _selectedCategory;
        public string selectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                NotifyPropertyChanged("selectedCategory");
                NotifyPropertyChanged("SelectedCategoryCommonItems");
                NotifyPropertyChanged("RichTextBoxVisibility");
                AddCommonItemCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ApplicationContextMenuPayload> SelectedCategoryCommonItems
        {
            get
            {
                var utems = AppContextMenuPayload.Where(c => c.isCategory == true && c.DisplayName == selectedCategory);
                if (utems != null)
                {
                    var category = utems.SingleOrDefault();
                    if (category != null)
                        if (category.innerList != null)
                        return new ObservableCollection<ApplicationContextMenuPayload>(category.innerList);
                }

                return new ObservableCollection<ApplicationContextMenuPayload>();
            }
        }


        private ApplicationContextMenuPayload _selectedCommonItem;

        public ApplicationContextMenuPayload selectedCommonItem
        {
            get
            {
                return _selectedCommonItem;
            }
            set
            {
                _selectedCommonItem = value;
                NotifyPropertyChanged("selectedCommonItem");
                NotifyPropertyChanged("RichTextBoxVisibility");

            }
        }

        private string _NewCategory;

        public string NewCategory
        {
            get
            {
                return _NewCategory;
            }
            set
            {
                _NewCategory = value;
                NotifyPropertyChanged("NewCategory");
                NotifyPropertyChanged("RichTextBoxVisibility");

            }
        }


        public List<string> ClipBoardFormats { get; set; }

        public RelayCommand<object> AddNewCategoryCommand { get; set; }
        public RelayCommand<object> AddCommonItemCommand { get; set; }
        public RelayCommand<ApplicationContextMenuPayload> ItemDeleteCommand { get; set; }
        public RelayCommand<object> SaveCommand { get; set; }

        public CommonItemsViewModel()
        {
            ClipBoardFormats = new List<string>()
            {
                "Rich Text Format",
                "UnicodeText",
                "Text"
            };
            AddNewCategoryCommand = new RelayCommand<object>(AddNewCategory);
            AddCommonItemCommand = new RelayCommand<object>(AddCommonItem, CanAddCommonItem);
            ItemDeleteCommand = new RelayCommand<ApplicationContextMenuPayload>(ItemDelete);
            SaveCommand = new RelayCommand<object>(Save);
            ResourceProvider = ProviderFactory.GetCurrentProvider();
            ICustomCommand<List<ApplicationContextMenuPayload>> menuRetrieveCommand = ResourceProvider.GetRetrieveCommonItemCommand();
            AppContextMenuPayload = menuRetrieveCommand.Execute();

        }

        private bool CanAddCommonItem()
        {
            return !string.IsNullOrWhiteSpace(selectedCategory);
        }

        private void Save(object obj)
        {
            string ValidationMessages = ValidatePayload();

            AppContextMenuPayload.Remove(AppContextMenuPayload.Find(c => c.DisplayName == "External Tools"));

            IUndoableCommand commonItemSaveCommand = ResourceProvider.GetSaveCommonItemCommand(AppContextMenuPayload);
            commonItemSaveCommand.Execute();

            if (CommonItemsSaved != null)
                CommonItemsSaved.Invoke();
        }

        private string ValidatePayload()
        {
            return string.Empty;
        }

        private void ItemDelete(ApplicationContextMenuPayload obj)
        {
            if (obj != null)
            {
                var utems = AppContextMenuPayload.Where(c => c.isCategory == true && c.DisplayName == selectedCategory);
                if (utems != null)
                {
                    var category = utems.SingleOrDefault();
                    if (category != null)
                        if (category.innerList != null)
                        {
                            if(category.innerList.Contains(obj))
                            {
                                category.innerList.Remove(obj);
                                NotifyPropertyChanged("SelectedCategoryCommonItems");
                                NotifyPropertyChanged("RichTextBoxVisibility");

                                if (category.innerList.Count == 0)
                                {
                                    if (AppContextMenuPayload.Contains(category))
                                    {
                                        AppContextMenuPayload.Remove(category);
                                        NotifyPropertyChanged("Categories");
                                        selectedCategory = null;
                                        NotifyPropertyChanged("selectedCategory");
                                        NotifyPropertyChanged("SelectedCategoryCommonItems");
                                        NotifyPropertyChanged("RichTextBoxVisibility");
                                    }
                                }
                            }
                        }
                }
            }
            
        }

        private void AddCommonItem(object obj)
        {
            var utems = AppContextMenuPayload.Where(c => c.isCategory == true && c.DisplayName == selectedCategory);
            if (utems != null)
            {
                var category = utems.SingleOrDefault();
                if (category != null)
                {
                    if (category.innerList == null)
                        category.innerList = new List<ApplicationContextMenuPayload>();
                    ApplicationContextMenuPayload CommonItem = new ApplicationContextMenuPayload();
                    CommonItem.isCategory = false;
                    CommonItem.Category = selectedCategory;
                    category.innerList.Add(CommonItem);
                    NotifyPropertyChanged("SelectedCategoryCommonItems");
                    NotifyPropertyChanged("RichTextBoxVisibility");
                    selectedCommonItem = CommonItem;
                }
            }
        }

        private void AddNewCategory(object obj)
        {
            string cat = Categories.ToList<string>().Find((c) => c.ToLower() == NewCategory.ToLower());
            if (cat != null)
            {
                selectedCategory = cat;
                return;
            }

            AppContextMenuPayload.Add(
                new ApplicationContextMenuPayload
                {
                    isCategory = true,
                    DisplayName = NewCategory
                }
                );
            NotifyPropertyChanged("Categories");
            NotifyPropertyChanged("RichTextBoxVisibility");
            selectedCategory = NewCategory;
        }

        public List<ApplicationContextMenuPayload> AppContextMenuPayload { get; set; }
        
    }
}
