using Manuals.Models;
using Manuals.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manuals.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddProductView : ContentPage
    {
        public AddProductView(ProductItem product)
        {
            InitializeComponent();
            BindingContext = new AddProductViewModel(Navigation, product);
        }

        private void ManualsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = BindingContext as AddProductViewModel;
            var manualName = e.Item as string;
            vm?.ManualClickedCommand.Execute(manualName);
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = BindingContext as AddProductViewModel;
            if (vm.PageClosedCommand.CanExecute(""))  // You can add parameters if any
            {
                vm.PageClosedCommand.Execute(""); // You can add parameters if any
            }
            base.OnDisappearing();
            return true;

        }
    }
}