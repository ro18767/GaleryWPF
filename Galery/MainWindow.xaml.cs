using Galery.Data;
using Galery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Galery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Uri TooLargeImgUri = new Uri(@"pack://application:,,,/"
    + Assembly.GetExecutingAssembly().GetName().Name
    + ";component/"
    + "Resouses/TooLargeImg.png", UriKind.Absolute);
        private static readonly Uri NotSupportedFormatImgUri = new Uri(@"pack://application:,,,/"
    + Assembly.GetExecutingAssembly().GetName().Name
    + ";component/"
    + "Resouses/NotSupportedFormatImg.png", UriKind.Absolute);

        private static CultureInfo cultureInfo = new CultureInfo("en-US");

        public ObservableCollection<ImageInfo> List { get; set; } = new();

        private DataContext DBContext { get; set; } = AppDBContext.getInstance();

        public MainWindow()
        {
            InitializeComponent();

            foreach (var imageInfo in DBContext.ImageInfos.ToList())
            {
                List.Add(imageInfo);
            }
            // DataContext = this;
        }

        private void AddImageBtn_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;
            if (openFileDialog.SafeFileName == String.Empty) return;

            var fullPath = openFileDialog.FileName;
            AddIMageToGalery(fullPath);

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             
            // записать минимальный размер
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;

            // открыть на полный экран
            this.SizeToContent = SizeToContent.Manual;
            this.WindowState = WindowState.Maximized;

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.IsLoaded) return;

        }

        #region Files

        private readonly Dictionary<string, FileInfo> FileInfoDictionary = new();

        private FileInfo GetFileInfo(Uri uri)
        {
            if (!uri.IsAbsoluteUri) uri = new Uri(System.IO.Path.GetFullPath(uri.LocalPath));
            string path = System.IO.Path.GetFullPath(uri.LocalPath);
            FileInfo fi = FileInfoDictionary.GetValueOrDefault(path) ?? new(path);

            if (!FileInfoDictionary.ContainsKey(path))
            {
                FileInfoDictionary.Add(path, fi);
            }

            return fi;
        }

        #endregion Files

        #region Images

        private readonly Dictionary<string, BitmapImage> BitmapDictionary = new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri">Must be file path</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">when file is not image </exception>
        /// <exception cref="FileNotFoundException"></exception>
        private BitmapImage GetBitmap(Uri uri)
        {
            if (!uri.IsAbsoluteUri) uri = new Uri(System.IO.Path.GetFullPath(uri.ToString()));
            string path = System.IO.Path.GetFullPath(uri.ToString());
            BitmapImage bitmap = BitmapDictionary.GetValueOrDefault(path) ?? new(uri);

            if (!BitmapDictionary.ContainsKey(path))
            {
                BitmapDictionary.Add(path, bitmap);
            }

            return bitmap;
        }


        private Image CreateImgElement(BitmapImage bitmap)
        {
            

            Image img = new();
            img.Width = 256;
            img.Height = 256;
            img.HorizontalAlignment = HorizontalAlignment.Left;
            img.Stretch = Stretch.Fill;
            img.Source = bitmap;

            return img;

        }
        private void AddIMageToGalery(string fullPath)
        {
            if (!File.Exists(fullPath)) return;

            string path = System.IO.Path.GetFullPath(fullPath);
            if (List.FirstOrDefault(i => i.FullPath == path) != null) return;
            var uri = new Uri(path, UriKind.Absolute);
            var previewUri = uri;


            var fi = GetFileInfo(uri);

            if (!fi.Exists) return;

            BitmapImage? bitmap = null;

            try
            {
                try
                {

                    bitmap = GetBitmap(uri);
                    if (bitmap.PixelWidth > 4096 || bitmap.PixelHeight > 4096)
                    {
                        previewUri = TooLargeImgUri;
                    }
                }
                catch (NotSupportedException)
                {
                    previewUri = NotSupportedFormatImgUri;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //MessageBox.Show(uri.OriginalString);
            ImageInfo imageInfo = new()
            {
                Id = Guid.NewGuid(),
                PreviewFullPath = previewUri.OriginalString,
                FullPath = uri.OriginalString,
                CreationTime = fi.CreationTime,
                FileSize = fi.Length,
                PixelWidth = bitmap?.PixelWidth ?? 0,
                PixelHeight = bitmap?.PixelHeight ?? 0,
            };
            DBContext.Add(imageInfo);
            List.Add(imageInfo);
        }


        private readonly Dictionary<string, BitmapImage> GroupDictionary = new();

        #endregion Images

        #region DragAndDrop
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (String item in files)
                {
                    AddIMageToGalery(item);
                }
            }
        }

        private void Window_Drag(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            e.Handled = true;
  
        }
        #endregion DragAndDrop

        #region DB

        #endregion DB
        private void DeleteImageBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as ImageInfo;
            if (item is null) return;
            ImageInfo info = item;

            DBContext.Remove(info);
            List.Remove(info);

        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as ImageInfo;
            if (item is null) return;
            ImageInfo info = item;

            Hide();
            new PreviewWindow(info).ShowDialog();
            Show();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsVisible) return;
            if (!this.IsLoaded) return;
            // открыть на полный экран
            this.SizeToContent = SizeToContent.Manual;
            this.WindowState = WindowState.Maximized;
        }
    }



}
