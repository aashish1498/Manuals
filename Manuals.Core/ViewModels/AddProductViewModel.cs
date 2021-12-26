using Manuals.Core.Models;
using Manuals.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manuals.Core.ViewModels
{
    public class AddProductViewModel : INotifyPropertyChanged
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
        public ICommand SaveCommand => new Command(SaveAsync);
        public ICommand DeleteCommand => new Command(DeleteAsync);
        public ICommand ProductImageCommand => new Command(PickImage);

        private async void PickImage()
        {
            //Mvx.IoCProvider.RegisterSingleton<FileSelectViewModel>(new FileSelectViewModel());
            //var myvm = Mvx.IoCProvider.Resolve<FileSelectViewModel>();
            //myvm.PickImage();
            //await NavigationS Navigate<FileSelectViewModel>();
            try
            {
                await OpenImagePicker();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        private async void DeleteAsync()
        {
            var database = await ProductItemDatabase.Instance;
            await database.DeleteItemAsync(Product);
            //await _navigationService.Close(this);
        }
        private async void SaveAsync()
        {
            var database = await ProductItemDatabase.Instance;
            await database.SaveItemAsync(Product);
            //await _navigationService.Close(this);
        }
        private async Task OpenImagePicker()
        {
            var productPhoto = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Choose product image"
            });
            if (productPhoto != null)
            {
                await SavePhotoAsync(productPhoto);

            }
        }

        private async Task SavePhotoAsync(FileResult productPhoto)
        {
            var newFile = Path.Combine(Constants.ProductImagesFolder, productPhoto.FileName);
            using (var stream = await productPhoto.OpenReadAsync())
            {
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);
                ProductImage = ImageSource.FromStream(() => stream);
            }

            Product.ProductImageName = productPhoto.FileName;
            var database = await ProductItemDatabase.Instance;
            await database.SaveItemAsync(Product);
        }
        #endregion

        #region Properties
        public INavigation Navigation { get; set; }
        public ProductItem Product { get; set; }
        private string _name;

        public ImageSource ProductImage { get; set; }
        public string ProductImagePath { get; set; } = "";

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Product.Name = value;
            }
        }

        public List<string> Tags { get; set; }


        #endregion

        #region Initialise
        public AddProductViewModel(INavigation navigation, ProductItem product)
        {
            this.Navigation = navigation;
            Product = product;
            Name = Product.Name;
            Tags = product.Tags;
            if (product.ProductImageName != null)
            {
                ProductImage = ImageSource.FromFile(Path.Combine(Constants.ProductImagesFolder, product.ProductImageName));
            }
        }
        //public override void Prepare(ProductItem product)
        //{
        //    Product = product;
        //    Name = Product.Name;
        //    Tags = product.Tags;
        //    if (product.ProductImageName != null)
        //    {
        //        ProductImage = ImageSource.FromFile(Path.Combine(Constants.ProductImagesFolder, product.ProductImageName));
        //    }
        //}
        #endregion
    }
}
