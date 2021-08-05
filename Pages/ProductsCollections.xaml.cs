using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ShopWebData;

namespace ShopWebApp.Pages
{
    public partial class ProductsCollections
    {
        readonly ShopWebContext Context = new();
        ObservableCollection<ProductCollection> Collections { get; set; }

        public ProductsCollections()
        {
            InitializeComponent();

            ProductsListDataGrid.ItemsSource = Context.Products.AsQueryable().Where(p => p.Active).Include(p => p.Category).ToList();

            IEnumerable<ProductCollection> CollectionsList = Context.ProductCollections.Include(a => a.Products);
            Collections = new ObservableCollection<ProductCollection>(CollectionsList);
            CollectionsDataGrid.ItemsSource = Collections;
        }

        void CollectionsDataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            ProductCollection NewCollection = new()
            {
                Name = "Коллекция"
            };
            e.NewItem = NewCollection;
            Context.ProductCollections.Add(NewCollection);
        }

        void SaveChanges(object sender, RoutedEventArgs e)
        {
            Context.SaveChanges();
        }

        void AddProductToCollection_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListDataGrid.SelectedItem is Product SelectedProduct)
            {
                ProductInCollection ProductInCollection = new()
                {
                    Product = SelectedProduct
                };

                if (CollectionsDataGrid.SelectedItem is ProductCollection SelectedCollection)
                {
                    SelectedCollection.Products.Add(ProductInCollection);
                }
                else
                {
                    MessageBox.Show("Выберите коллекцию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите продукт!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
