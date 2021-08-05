using FluentFTP;
using Microsoft.Win32;
using ShopWebData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using Controls = System.Windows.Controls;

namespace ShopWebApp.Windows 
{
    public partial class MenuItemImages : Window
    {
        MenuItem MenuItem;
        ShopWebContext Context;
        FtpClient client;
        string WorkingDirectory;
        public MenuItemImages(MenuItem MenuItem, ShopWebContext Context)
        {
            InitializeComponent();

            this.MenuItem = MenuItem;
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


            List<BitmapImageWithId> images = new List<BitmapImageWithId>();
            foreach (Image i in MenuItem.Images)
            {
                string ImagePath = Environment.CurrentDirectory + @"\imagesCache\" + i.Url;
                client.DownloadFile(ImagePath, i.Url);

                BitmapImage image = new BitmapImage();

                try
                {
                    using (var stream = File.OpenRead(ImagePath))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
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
            OpenFileDialog op = new OpenFileDialog
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
                for (; ; )
                {
                    NewFileName = "menu_item_img" + MenuItem.Id + "_" + Count;

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

                    if (status == FtpStatus.Failed)
                    {
                        MessageBox.Show("При загрузке изображения произошла ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MenuItem.Images.Add(new Image
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
            catch (DirectoryNotFoundException)
            {

            }
            catch
            {
                MessageBox.Show("Во время очистки временных файлов возникла ошибка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            Controls::MenuItem menuItem = sender as Controls::MenuItem;

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
            public BitmapImage Image { get; set; }
        }
    }
}
