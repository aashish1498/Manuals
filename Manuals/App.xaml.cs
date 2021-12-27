using Manuals.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Manuals.Constants;

namespace Manuals
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new HomeView());
        }

        protected override void OnStart()
        {
            Directory.CreateDirectory(GetLocalFolder(FileType.Manual));
            Directory.CreateDirectory(GetLocalFolder(FileType.ProductImage));

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
