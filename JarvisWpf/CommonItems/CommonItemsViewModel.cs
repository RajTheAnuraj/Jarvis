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
                NotifyPropertyChanged("SelectedCategoryCommonItems");
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
            }
        }


        public CommonItemsViewModel()
        {
            ResourceProvider = ProviderFactory.GetCurrentProvider();
            ICustomCommand<List<ApplicationContextMenuPayload>> menuRetrieveCommand = ResourceProvider.GetRetrieveCommonItemCommand();
            AppContextMenuPayload = menuRetrieveCommand.Execute();
        }

        public List<ApplicationContextMenuPayload> AppContextMenuPayload { get; set; }
        
    }
}
