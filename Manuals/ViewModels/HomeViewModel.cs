
using Manuals.Models;
using Manuals.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Manuals.Views;
using System.Collections.Generic;

namespace Manuals.ViewModels
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

        public ICommand PageAppearingComand => new Command(OnAppearing);

        private void OnAppearing(object obj)
        {
            InitializeAsync();
        }

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
        public INavigation Navigation { get; set; }

        private ProductItemDatabase database;

        public ObservableCollection<ProductItem> ProductItems { get; } = new ObservableCollection<ProductItem>();


        #endregion

        #region Initialise
        public HomeViewModel(INavigation navigation)
        {
            this.Navigation = navigation;
        }

        public async void InitializeAsync()
        {
            database = await ProductItemDatabase.Instance;
            //var productItemsList = await database.GetItemsAsync();
            //ProductItems = new ObservableCollection<ProductItem>(productItemsList);
            RefreshProductsAsync();
        }
        #endregion

        private async void RefreshProductsAsync()
        {
            var productItemsList = await database.GetItemsAsync();
            ProductItems.Clear();
            ProductItems.AddRange(productItemsList);
        }

        private async Task OpenProductPageAsync(ProductItem productItem)
        {
            await Navigation.PushAsync(new AddProductView(productItem));

            //_ = await _navigationService.Navigate<AddProductViewModel, ProductItem>(productItem);
        }
    }
}
