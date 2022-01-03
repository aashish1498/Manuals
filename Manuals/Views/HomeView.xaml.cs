using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Manuals.ViewModels;
using Manuals.Models;

namespace Manuals.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel(Navigation);
        }

        private void ProductList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = BindingContext as HomeViewModel;
            var prodcut = e.Item as ProductItem;
            vm?.ProductItemClicked.Execute(prodcut);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = BindingContext as HomeViewModel;
            if (sender is SearchBar searchBar) vm?.PerformSearch.Execute(searchBar.Text);
        }
    }
}