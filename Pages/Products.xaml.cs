using Microsoft.EntityFrameworkCore;

using ShopWeb.Shared;

using ShopWebApp.Windows;

using ShopWebData;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ShopWebApp.Pages
{
    public partial class Products : Page
    {
        readonly ShopWebContext Context = new();
        readonly List<Category> CategoriesList;
        List<Product> ProductsList;
        ICollectionView ItemList;
        public Products()
        {
            InitializeComponent();

            InitializeSettings();

            CategoriesList = Context.Categories.ToList().Prepend(null).ToList();

            CollectionViewSource ParentsListSource = (CollectionViewSource)FindResource("CategoriesListSource");
            ParentsListSource.Source = CategoriesList;

            RefreshProductsList();

            SetUpFiltering();

            DataGrid.ItemsSource = ItemList;

            CategoryFilterComboBox.Items.Clear();
            CategoryFilterComboBox.ItemsSource = CategoriesList
            .Prepend(new Category
            {
                Name = "Все продукты",
                Id = 0
            })
            .Where(c => c != null);

            CategoryFilterComboBox.SelectedIndex = 0;
        }

        void RefreshProductsList()
        {
            ProductsList = (from product in Context.Products.Include(p => p.Images)
                            select product).ToList();
        }

        void SetUpFiltering()
        {
            CollectionViewSource ItemSourceList = new() { Source = ProductsList };

            ItemList = ItemSourceList.View;

            Predicate<object> Filter = new(item =>
            {
                Product product = item as Product;
                Category Category = CurrentCategory;

                bool SearchBool()
                {
                    return product.Name.ToLower().Contains(SearchTextBox.Text.ToLower()) ||
                    product.VendorCode.ToLower().Replace("А", "A").Contains(SearchTextBox.Text.ToLower().Replace("А", "A"));
                }


                if (Category == null)
                {
                    return SearchBool();
                }
                else
                {
                    return product.Category == Category && SearchBool();
                }
            });

            ItemList.Filter = Filter;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Context.SaveChanges();
        }

        private void Images_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Product product = button.DataContext as Product;

            if (product.Images != null)
            {
                new ProductImages(product, Context).Show();
            }
            else
            {
                MessageBox.Show("У продукта отсутствуют изображения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }        

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && !IsEditing)
            {
                MessageBoxResult DeleteResult = MessageBox.Show("Вы точно хотите удалить выбранные элементы?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.Yes == DeleteResult)
                {
                    foreach (DataGridCellInfo cell in DataGrid.SelectedCells)
                    {
                        var row = cell.Item;
                        Product selectedObject = (Product)row;
                        if (selectedObject.Id != 0)
                        {
                            Context.Products.Attach(selectedObject);
                            Context.Products.Remove(selectedObject);
                        }
                        else
                        {
                            Context.SaveChanges();
                            ProductsList.Remove(selectedObject);
                        }
                        Context.SaveChanges();
                    }

                    RefreshProductsList();
                    SetUpFiltering();
                    DataGrid.ItemsSource = ItemList;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        readonly bool NewProductIsCountable = SettingsManager.GetValueBool("NewProductIsCountable");
        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Product NewItem = new()
            {
                Name = "Продукт",
                Description = "",
                Active = true,
                Countable = NewProductIsCountable,
                Category = CurrentCategory
            };
            e.NewItem = NewItem;
            Context.Products.Add(NewItem);
        }

        Category CurrentCategory;
        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            CurrentCategory = comboBox.SelectedItem as Category;
            if (CurrentCategory.Id != 0)
            {
                CurrentCategory = comboBox.SelectedItem as Category;
            }
            else
            {
                CurrentCategory = null;
            }

            ItemList.Refresh();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ItemList.Refresh();
        }

        bool IsEditing = false;
        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsEditing = true;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsEditing = false;
        }

        void InitializeSettings()
        {            
            SetColumnVisibility("ShowCountableColumnInProductsDesktop", CountableColumn);
            SetColumnVisibility("ShowPromotionalColumnInProductsDesktop", PromotionalColumn);            
            SetColumnVisibility("ShowIsGiftColumnInProductsDesktop", IsGiftColumn);
            SetColumnVisibility("ShowGiftAmountColumnInProductsDesktop", GiftAmountColumn);
            SetColumnVisibility("ShowVendorCodeColumnInProductsDesktop", VendorCodeColumn);
            SetColumnVisibility("ShowIdColumnInProductsDesktop", IdColumn);
            SetColumnVisibility("ShowIsAvailableColumnInProductsDesktop", IsAvailableColumn);
        }

        void SetColumnVisibility(string SettingName, DataGridColumn Column)
        {
            if (int.Parse(SettingsManager.GetValue(SettingName)) == 1)
            {
                Column.Visibility = Visibility.Visible;
            }
            else
            {
                Column.Visibility = Visibility.Collapsed;
            }
        }
    }
}
