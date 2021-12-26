﻿using Manuals.Models;
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
    }
}