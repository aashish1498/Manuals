
using Manuals.Core.Models;
using Manuals.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manuals.Core.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        #region Interface Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Commands
        public ICommand AddProductCommand => new Command(AddProductAsync);
        public ICommand ProductItemClicked => new Command<ProductItem>(ProductClickedAsync);

        private async void AddProductAsync()
        {
            var myProduct = new ProductItem();
            myProduct.Name = "New Product";
            await OpenProductPageAsync(myProduct);
        }

        private async void ProductClickedAsync(ProductItem productItem)
        {
            await OpenProductPageAsync(productItem);
        }
        #endregion

        #region Properties
        private ProductItemDatabase database;

        public ObservableCollection<ProductItem> ProductItems { get; set; }


        #endregion

        #region Initialise
        public HomeViewModel()
        {
            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            database = await ProductItemDatabase.Instance;
            RefreshProductsAsync();
        }
        #endregion

        private async void RefreshProductsAsync()
        {
            var productItemsList = await database.GetItemsAsync();
            ProductItems = new ObservableCollection<ProductItem>(productItemsList);
        }



        private async Task OpenProductPageAsync(ProductItem productItem)
        {
            var productPhoto = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Choose product image"
            });
            //await Application.Current.MainPage.Navigation.PushAsync(new AddProductView());
            //_ = await _navigationService.Navigate<AddProductViewModel, ProductItem>(productItem);
        }
    }
}
