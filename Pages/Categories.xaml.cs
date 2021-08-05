using Microsoft.EntityFrameworkCore;
using ShopWebApp.Windows;
using ShopWebData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ShopWebApp.Pages
{
    public partial class Categories : Page
    {
        readonly ShopWebContext Context = new();
        readonly List<Category> ParentsList;
        public Categories()
        {
            InitializeComponent();


            ParentsList = Context.Categories.Include(c => c.Images).ToList();
            ParentsList.Add(null);

            CollectionViewSource ParentsListSource = (CollectionViewSource) FindResource("ParentsListSource");
            ParentsListSource.Source = ParentsList;

            DataGrid.ItemsSource = Context.Categories.Include(c => c.Images).ToList();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Context.SaveChanges();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Category NewCategory = new()
            {
                Name = "Категорія",
                Active = true
            };
            e.NewItem = NewCategory;
            Context.Categories.Add(NewCategory);
        }
        private void Images_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Category category = button.DataContext as Category;

            if (category.Images != null)
            {
                new CategoryImages(category, Context).Show();
            }
            else
            {
                MessageBox.Show("У категории отсутствуют изображения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
