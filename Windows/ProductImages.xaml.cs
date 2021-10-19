using FluentFTP;
using Microsoft.Win32;
using ShopWeb.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MenuItem = System.Windows.Controls.MenuItem;

namespace ShopWebApp.Windows
{
    public partial class ProductImages : Window
    {
        Product Product;
        ShopWebContext Context;
        FtpClient client;
        string WorkingDirectory;
        public ProductImages(Product Product, ShopWebContext Context)
        {
            InitializeComponent();

            this.Product = Product;
            this.Context = Context;

            string FtpHost = Context.Settings.First(s => s.Key == "FtpHost").Value;
            string FtpLogin = Context.Settings.First(s => s.Key == "FtpLogin").Value;
            string FtpPassword = Context.Settings.First(s => s.Key == "FtpPassword").Value;

            client = new FtpClient(FtpHost)
            {
                Credentials = new NetworkCredential(FtpLogin, FtpPassword)
            };



            int? FtpPort = null;
            string FtpPortString = Context.Settings.First(s => s.Key == "FtpPort").Value;
            if (!string.IsNullOrWhiteSpace(FtpPortString))
            {
                FtpPort = int.Parse(FtpPortString);
            }

            if (FtpPort.HasValue)
            {
                client.Port = FtpPort.Value;
            }

            WorkingDirectory = Context.Settings.First(s => s.Key == "ImagesFolderPath").Value;

            RefreshImages();
        }
        void RefreshImages()
        {            
            client.Connect();
            client.SetWorkingDirectory(WorkingDirectory);


            List<BitmapImageWithId> images = new();
            foreach (Image i in Product.Images)
            {
                string ImagePath = Environment.CurrentDirectory + @"\imagesCache\" + i.Url;
                client.DownloadFile(ImagePath, i.Url);

                BitmapImage image = new();

                try
                {
                    using (var stream = File.OpenRead(ImagePath))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;                     
                        image.EndInit();
                    }

                    images.Add(new BitmapImageWithId
                    {
                        Id = i.Id,
                        Image = image
                    });
                }
                catch
                {
                    MessageBox.Show($"Изображение {ImagePath} не загружено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            ImagesControl.ItemsSource = images;

            client.Disconnect();
        }
        

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new ()
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png"
            };
            if (op.ShowDialog() == true)
            {
                client.Connect();

                client.SetWorkingDirectory(WorkingDirectory);

                string extention = Path.GetExtension(op.FileName);

                int Count = 1;
                string NewFileName;
                for(;;)
                {
                    NewFileName = "img" + Product.Id + "_" + Count;

                    if (client.FileExists(NewFileName + extention))
                    {
                        Count++;
                    }
                    else
                    {
                        break;
                    }
                }

                NewFileName += extention;

                try
                {
                    client.RetryAttempts = 3;
                    FtpStatus status = client.UploadFile(op.FileName, NewFileName, FtpRemoteExists.NoCheck, false, FtpVerify.Retry);

                    if(status == FtpStatus.Failed)
                    {
                        MessageBox.Show("При загрузке изображения произошла ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Product.Images.Add(new Image
                        {
                            Url = NewFileName
                        });

                        Context.SaveChanges();

                        RefreshImages();
                    }
                }
                catch
                {
                    MessageBox.Show("При загрузке изображения произошла ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                Directory.Delete(Environment.CurrentDirectory + @"\imagesCache", true);
            }
            catch(DirectoryNotFoundException)
            {

            }
            catch
            {
                MessageBox.Show("Во время очистки временных файлов возникла ошибка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            int Id = int.Parse(menuItem.Tag.ToString());
            Image Image = Context.Images.Find(Id);

            client.Connect();
            client.SetWorkingDirectory(WorkingDirectory);

            client.DeleteFile(Image.Url);
            Context.Images.Remove(Image);

            Context.SaveChanges();

            RefreshImages();
        }

        class BitmapImageWithId
        {
            public int Id { get; set; }
            public ImageSource Image { get; set; }
        }
    }
}