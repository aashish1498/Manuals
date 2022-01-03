using Manuals.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Manuals
{
    public partial class AppShell : Shell
    {
        public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

        public AppShell()
        {
            InitializeComponent();
        }


        void RegisterRoutes()
        {
            Routes.Add("home", typeof(HomeView));

            foreach (var item in Routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }
    }
}