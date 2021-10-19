using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.EntityFrameworkCore;

using ShopWeb.Shared;

using ShopWebApp.Code;
using ShopWebApp.Windows;
using ShopWeb.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MenuItem = ShopWeb.Data.MenuItem;

namespace ShopWebApp.Pages
{
    public partial class Menu : Page
    {
        readonly ShopWebContext Context = new();
        readonly List<MenuItem> ParentsList;
        public Menu()
        {
            InitializeComponent();

            InitializeSettings();

            ParentsList = Context.Menu.AsQueryable().Where(item => item.Type == MenuType.MenuItem).Include(item => item.Images).ToList();
            ParentsList.Add(null);

            CollectionViewSource ParentsListSource = (CollectionViewSource)FindResource("ParentsListSource");
            ParentsListSource.Source = ParentsList;

            DataGrid.ItemsSource = (from MenuItem in Context.Menu.AsQueryable()
                                    where MenuItem.Type == MenuType.MenuItem
                                    select MenuItem).ToList();
        }        

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Context.SaveChanges();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            MenuItem NewItem = new()
            {
                Name = "Елемент меню"
            };
            e.NewItem = NewItem;
            Context.Menu.Add(NewItem);
        }

        private void Images_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            MenuItem MenuItem = button.DataContext as MenuItem;

            if (MenuItem.Images != null)
            {
                new MenuItemImages(MenuItem, Context).Show();
            }
            else
            {
                MessageBox.Show("У категории отсутствуют изображения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void InitializeSettings()
        {
            if (int.Parse(SettingsManager.GetValue("ShowParentsComboBoxColumnInMenuDesktop")) == 1)
            {
                ParentsComboBoxColumn.Visibility = Visibility.Visible;
            }
            else
            {
                ParentsComboBoxColumn.Visibility = Visibility.Collapsed;
            }

            

            if (int.Parse(SettingsManager.GetValue("ShowDropDownColumnDesktop")) == 1)
            {
                ShowDropDownColumn.Visibility = Visibility.Visible;
            }
            else
            {
                ShowDropDownColumn.Visibility = Visibility.Collapsed;
            }

            if (int.Parse(SettingsManager.GetValue("HasSubItemsColumnDesktop")) == 1)
            {
                HasSubItemsColumn.Visibility = Visibility.Visible;
            }
            else
            {
                HasSubItemsColumn.Visibility = Visibility.Collapsed;
            }
        }
    }
}