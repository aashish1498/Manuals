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
        public ICommand PageClosedCommand => new Command(PageClosed);
        public ICommand ProductImageCommand => new Command(PickImage);
        public ICommand RemoveImageCommand => new Command(RemoveImage);
        public ICommand RemoveManualCommand => new Command<string>(RemoveManual);
        public ICommand AddManualCommand => new Command(AddManual);
        public ICommand ManualClickedCommand => new Command<string>(ManualClicked);
        public ICommand EditManualCommand => new Command<string>(EditClickedAsync);

        private async void SaveAsync()
        {
            var database = await ProductItemDatabase.Instance;
            await database.SaveWithChildrenAsync(Product);
            CleanUp(true);
            _ = Navigation.PopAsync();
        }
        private async void DeleteAsync()
        {
            var database = await ProductItemDatabase.Instance;
            await database.DeleteItemAsync(Product);
            Directory.Delete(Path.Combine(GetProductItemsFolder(), Product.ID.ToString()), true);
            _ = Navigation.PopAsync();
        }
        private void PageClosed()
        {
            CleanUp(false);
            _ = Navigation.PopAsync();
        }

        private void RemoveManual(string manualName)
        {
            ObservableManualLocations.Remove(manualName);
            Product.ManualNames.Remove(manualName);
        }

        private async void EditClickedAsync(string manualName)
        {
            string action = await Application.Current.MainPage.DisplayActionSheet("Edit " + manualName.Truncate(17), "Cancel", null, "Rename", "Change", "Delete");
            switch (action)
            {
                case "Rename":
                    string newName = await Application.Current.MainPage.DisplayPromptAsync("Edit", "Rename manual", initialValue: manualName);
                    if (!string.IsNullOrEmpty(newName))
                    {
                        var oldFile = Path.Combine(GetLocalFolder(FileType.Manual, Product.ID), manualName);
                        var newFile = Path.Combine(GetLocalFolder(FileType.Manual, Product.ID), newName);
                        File.Move(oldFile, newFile);
                        RemoveManual(manualName);
                        AddManualName(newName);
                    }
                    break;
                case "Change":
                    RemoveManual(manualName);
                    AddManual();
                    break;
                case "Delete":
                    RemoveManual(manualName);
                    break;
                default:
                    return;
            }
            
            //string result = await DisplayPromptAsync("Question 1", "What's your name?");
        }

        private async void ManualClicked(string manualName)
        {
            var filename = Path.Combine(GetLocalFolder(FileType.Manual, Product.ID), manualName);
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
            var newFile = Path.Combine(GetLocalFolder(fileType, Product.ID), chosenFile.FileName);
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
                AddManualName(chosenFile.FileName);
            }

        }

        private void AddManualName(string name)
        {
            ObservableManualLocations.Add(name);
            Product.ManualNames.Add(name);
            Height = ObservableManualLocations.Count * manual_height;
        }
        private void CleanUp(bool saved)
        {
            // Clean Product Photo
            var photos = Directory.GetFiles(GetLocalFolder(FileType.ProductImage, Product.ID));
            var manuals = Directory.GetFiles(GetLocalFolder(FileType.Manual, Product.ID));
            string currentPhotoName;
            List<string> currentManualNames;
            if (saved)
            {
                currentPhotoName = Product.ProductImageName;
                currentManualNames = Product.ManualNames;
            }
            else
            {
                currentPhotoName = oldImageName;
                currentManualNames = oldManualNames;
            }
            foreach (var photo in photos)
            {
                var photoName = Path.GetFileName(photo);
                if (!photoName.Equals(currentPhotoName))
                {
                    File.Delete(photo);
                }
            }

            foreach (var manual in manuals)
            {
                var manualName = Path.GetFileName(manual);
                if (!currentManualNames.Contains(manualName))
                {
                    File.Delete(manual);
                }
            }
        }

#endregion

#region Properties

        private readonly int manual_height = 30;
        private ProductItem productToSave;
        public INavigation Navigation { get; set; }
        public ProductItem Product { get; set; }
        private string _name;
        private ImageSource productImage;
        public ObservableCollection<string> ObservableManualLocations { get; set; } = new ObservableCollection<string>();
        private string oldImageName;
        private List<string> oldManualNames;
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
            productToSave = product; // TODO doesnt make a  difference
            oldImageName = string.Copy(product.ProductImageName);
            oldManualNames = new List<string>(product.ManualNames);
            CreateFolders(product.ID);
            Product = product;
            Name = Product.Name;
            Tags = product.Tags;
            if (!string.IsNullOrEmpty(product.ProductImageName))
            {
                ProductImage = ImageSource.FromFile(Path.Combine(GetLocalFolder(FileType.ProductImage, product.ID), product.ProductImageName));
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
