using Manuals.Models;
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
using System.Collections.ObjectModel;
using static Manuals.Constants;

namespace Manuals.ViewModels
{
    public class AddProductViewModel : INotifyPropertyChanged
    {
        #region Interface Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        #region Commands
        public ICommand SaveCommand => new Command(SaveAsync);
        public ICommand DeleteCommand => new Command(DeleteAsync);
        public ICommand ProductImageCommand => new Command(PickImage);
        public ICommand RemoveImageCommand => new Command(RemoveImage); 
        public ICommand RemoveManualCommand => new Command<string>(RemoveManual);

        private void RemoveManual(string manualName)
        {
            ObservableManualLocations.Remove(manualName);
            Product.ManualNames.Remove(manualName);
        }

        public ICommand AddManualCommand => new Command(AddManual);
        public ICommand ManualClickedCommand => new Command<string>(ManualClicked);

        private async void ManualClicked(string manualName)
        {
            var filename = Path.Combine(GetLocalFolder(FileType.Manual), manualName);
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filename)
            }
    );
            return;
        }

        private async void AddManual()
        {
            try
            {
                await OpenFilePicker(FileType.Manual);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
            
        }

        private void RemoveImage()
        {
            ProductImage = "";
            Product.ProductImageName = "";
            //TODO: handle deletion of actual image too
        }

        private async void PickImage()
        {
            try
            {
                await OpenFilePicker(FileType.ProductImage);
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
            _ = Navigation.PopAsync();
        }
        private async void SaveAsync()
        {
            var database = await ProductItemDatabase.Instance;
            await database.SaveWithChildrenAsync(Product);
            _ = Navigation.PopAsync();
        }
        private async Task OpenFilePicker(FileType fileType)
        {
            PickOptions pickOptions;
            if (fileType == FileType.ProductImage)
            {
                pickOptions = new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Choose product image"
                };
            }
            else
            {
                var manualFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                  {
                   DevicePlatform.Android, new [] { "image/png", "image/jpeg", "application/pdf" }
                  },
                  {
                     DevicePlatform.iOS, new [] { "public.png" , "public.jpeg", "com.adobe.pdf", "com.microsoft.word.doc" }
                  }
                });
                pickOptions = new PickOptions
                {
                    FileTypes = manualFileType,
                    PickerTitle = "Choose product manual"
                };
            }
            var chosenFile = await FilePicker.PickAsync(pickOptions);
            if (chosenFile != null)
            {
                await SaveFileAsync(chosenFile, fileType);
            }
        }

        private async Task SaveFileAsync(FileResult chosenFile, FileType fileType)
        {
            var newFile = Path.Combine(GetLocalFolder(fileType), chosenFile.FileName);
            using (var stream = await chosenFile.OpenReadAsync())
            {
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);
            }

            if (fileType == FileType.ProductImage)
            {
                var secondstream = await chosenFile.OpenReadAsync();
                ProductImage = ImageSource.FromStream(() => secondstream);
                Product.ProductImageName = chosenFile.FileName;
            }
            else
            {
                ObservableManualLocations.Add(chosenFile.FileName);
                Product.ManualNames.Add(chosenFile.FileName);
                Height = (ObservableManualLocations.Count * manual_height);
            }

        }
        #endregion

        #region Properties
        private int manual_height = 30;
        public INavigation Navigation { get; set; }
        public ProductItem Product { get; set; }
        private string _name;
        private ImageSource productImage;
        public ObservableCollection<string> ObservableManualLocations { get; set; } = new ObservableCollection<string>();

        public ImageSource ProductImage
        {
            get => productImage; set
            {
                productImage = value;
                OnPropertyChanged();
            }
        }
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

        private int _height;

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        #endregion

        #region Initialise
        public AddProductViewModel(INavigation navigation, ProductItem product)
        {
            this.Navigation = navigation;
            Product = product;
            Name = Product.Name;
            Tags = product.Tags;
            if (!string.IsNullOrEmpty(product.ProductImageName))
            {
                ProductImage = ImageSource.FromFile(Path.Combine(GetLocalFolder(FileType.ProductImage), product.ProductImageName));
            }
            if (product.ManualNames != null)
            {
                ObservableManualLocations.AddRange(product.ManualNames);
            }
            Height = (ObservableManualLocations.Count * manual_height);

        }

        #endregion
    }
}
