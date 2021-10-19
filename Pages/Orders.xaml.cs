using Microsoft.EntityFrameworkCore;

using ShopWeb.Shared;

using ShopWebApp.Code;
using ShopWeb.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ShopWebApp.Pages
{
    public partial class Orders : Page
    {
        readonly ShopWebContext Context = new();
        public Orders()
        {
            InitializeComponent();

            if (int.Parse(SettingsManager.GetValue("LegalEntityEnabled")) != 1)
            {
                LegalEntityID.Visibility = Visibility.Collapsed;
                OrganizationName.Visibility = Visibility.Collapsed;
            }

            OrdersDataGrid.ItemsSource = Context.Carts.AsQueryable()
                .Where(cart => cart.Status != StatusEnum.InCart &&
                               cart.Status != StatusEnum.ShopsCanceled)
                .Include(cart => cart.CartItems)
                    .ThenInclude(item => item.Product)
                .Include(cart => cart.Buyer)
                .Include(cart => cart.Shop)
                .Include(cart => cart.CartStatus)
                .Include(cart => cart.ShopAdress)
                    .ThenInclude(cart => cart.ShopAdresses)
                .Include(cart => cart.ShopAdress)
                    .ThenInclude(cart => cart.Parent)
                .Include(cart => cart.Gifts)
                    .ThenInclude(gift => gift.Product)
                .OrderByDescending(cart => cart.CartStatus.CreationDateTime)
                .AsSingleQuery()
                .ToList();

            CollectionViewSource Shops = FindResource("Shops") as CollectionViewSource;
            Shops.Source = Context.Shops.ToList();
        }

        void Save_Click(object sender, RoutedEventArgs e)
        {
            Context.SaveChanges();
        }

        void OrdersDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count == 0) return;
            Context.SaveChanges();

            OrdersDataGrid.BeginEdit();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((sender as Button).DataContext as Cart).Status.ToString());
        }
    }
}
