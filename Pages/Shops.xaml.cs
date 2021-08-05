using ShopWebData;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ShopWebApp.Pages
{
    public partial class Shops : Page
    {
        readonly ShopWebContext Context = new ShopWebContext();
        public Shops()
        {
            InitializeComponent();

            DataGrid.ItemsSource = Context.Shops.ToList();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Shop NewShop = new Shop
            {
                Name = "Магазин",
                Adress = "Адрес"
            };
            e.NewItem = NewShop;
            Context.Shops.Add(NewShop);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Context.SaveChanges();
        }
    }
}
