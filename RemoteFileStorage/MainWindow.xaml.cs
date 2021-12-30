using Azure.Storage.Blobs.Models;
using Microsoft.Win32;
using RemoteFileStorage.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RemoteFileStorage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ItemsViewModel itemsViewModel;
        public MainWindow()
        {
            InitializeComponent();
            itemsViewModel = new ItemsViewModel();
            Init();
        }

        private void Init()
        {
            LbBlobs.ItemsSource = itemsViewModel.Items;
            CbDirectories.ItemsSource = itemsViewModel.Directories;
        }

        private void LbBlobs_SelectionChanged(object sender, SelectionChangedEventArgs e) => DataContext = LbBlobs.SelectedItem as BlobItem;
   

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (JPEG,JPG,TIFF,PNG,SVG,GIF)| *.JPEG;*.PNG;*.JPG;*.TIFF;*.SVG;*.GIF"
            };
            string ext = string.Empty;
            if (openFileDialog.ShowDialog() == true)
            {
                 ext = Path.GetExtension(openFileDialog.FileName);
                await itemsViewModel.UploadAsync(openFileDialog.FileName,ext);
            }
            CbDirectories.Text = itemsViewModel.Directory = ext;

        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(LbBlobs.SelectedItem is BlobItem blobItem))
            {
                return;
            }
            await itemsViewModel.DeleteAsync(blobItem);
            CbDirectories.Text = itemsViewModel.Directory;
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (!(LbBlobs.SelectedItem is BlobItem blobItem))
            {
                return;
            }
            var saveFileDialog = new SaveFileDialog()
            {
                FileName = blobItem.Name.Contains(ItemsViewModel.ForwardSlash) 
                ? blobItem.Name.Substring(blobItem.Name.LastIndexOf(ItemsViewModel.ForwardSlash) + 1) 
                : blobItem.Name
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                await itemsViewModel.DownloadAsync(blobItem, saveFileDialog.FileName);
            }
            CbDirectories.Text = itemsViewModel.Directory;
        }

 
        private void CbDirectories_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (itemsViewModel.Directories.Contains(CbDirectories.Text))
            {
                itemsViewModel.Directory = CbDirectories.Text;
                CbDirectories.SelectedItem = itemsViewModel.Directory;

            }
        }

      
    }
}
