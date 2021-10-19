using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ShopWeb.Data;

namespace ShopWebApp.Pages
{
    public partial class DeliveryPriceTags : Page
    {
        readonly ShopWebContext Context = new();
        public DeliveryPriceTags()
        {
            InitializeComponent();

            DataGrid.ItemsSource = Context.DeliveryPriceTags.ToList();
        }

        void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            DeliveryPriceTag NewItem = new()
            {
                Price = 0,
                StartCartPrice = 0
            };
            e.NewItem = NewItem;
            Context.DeliveryPriceTags.Add(NewItem);
        }

        void Save_Click(object sender, RoutedEventArgs e)
        {
            Context.SaveChanges();
        }
    }
}
