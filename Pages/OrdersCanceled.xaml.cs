using Microsoft.EntityFrameworkCore;

using ShopWeb.Shared;

using ShopWebApp.Code;
using ShopWebData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace ShopWebApp.Pages
{
    public partial class OrdersCanceled : Page
    {
        ShopWebContext Context;
        CollectionViewSource CartsCollectionViewSource;
        CollectionViewSource ShopsCollectionViewSource;
        public OrdersCanceled()
        {
            InitializeComponent();

            if (int.Parse(SettingsManager.GetValue("LegalEntityEnabled")) != 1)
            {
                LegalEntityID.Visibility = Visibility.Collapsed;
                OrganizationName.Visibility = Visibility.Collapsed;
            }

            CartsCollectionViewSource = (CollectionViewSource)FindResource("Carts");

            ShopsCollectionViewSource = (CollectionViewSource)FindResource("Shops");

            RefreshOrders();
            //InitializeTimer();
        }

        /*void InitializeTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += RefreshOrders;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
        }*/

        void RefreshOrders(/*object sender, EventArgs e*/)
        {
            object LastCurrentItem = CartsCollectionViewSource?.View?.CurrentItem;

            Context = new ShopWebContext();
            
            List<Cart> ListCollection = Context.Carts.AsQueryable()
                .Where(cart => cart.Status == StatusEnum.ShopsCanceled)
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
            
            CartsCollectionViewSource.Source = new ObservableCollection<Cart>(ListCollection);
            CartsCollectionViewSource.View.MoveCurrentTo(LastCurrentItem);

            ShopsCollectionViewSource.Source = Context.Shops.ToList();
        }

        void OrdersDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count == 0) return;

            (e.AddedCells[0].Item as Cart).OperatorViewed = true;
            Context.SaveChanges();

            OrdersDataGrid.BeginEdit();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshOrders();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if ((sender as ComboBox).IsFocused)
                {
                    Shop Shop = e.AddedItems[0] as Shop;
                    Cart CurrentCart = OrdersDataGrid.SelectedCells[0].Item as Cart;
                    MessageBoxResult ApplyShopResult = MessageBox.Show("Вы хотите применить этот магазин для этого адреса?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (ApplyShopResult == MessageBoxResult.Yes)
                    {
                        CurrentCart.ShopAdress.ShopAdresses.ForEach(sa =>
                        {
                            sa.SequentialNumber++;
                        });
                        if (CurrentCart.ShopAdress.ShopAdresses.Exists(sa => sa.ShopId == Shop.Id))
                        {
                            CurrentCart.ShopAdress.ShopAdresses.First(sa => sa.ShopId == Shop.Id).SequentialNumber = CurrentCart.ShopAdress.ShopAdresses.Min(sa => sa.SequentialNumber) - 1;
                        }
                        else
                        {
                            CurrentCart.ShopAdress.ShopAdresses.Add(
                                new ShopsAdresses
                                {
                                    SequentialNumber = CurrentCart.ShopAdress.ShopAdresses.Min(sa => sa.SequentialNumber) - 1,
                                    Shop = Shop,
                                    Adress = CurrentCart.ShopAdress
                                }
                            );
                        }
                    }

                    CurrentCart.Status = StatusEnum.Formalized;
                }
            }));         
        }

        private void ProcessedManually_Click(object sender, RoutedEventArgs e)
        {
            (OrdersDataGrid.SelectedCells[0].Item as Cart).Status = StatusEnum.Formalized;
            Context.SaveChanges();
        }
    }    
}
