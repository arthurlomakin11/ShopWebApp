using Microsoft.EntityFrameworkCore;
using ShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopWebApp.Pages
{
    public partial class Adresses : Page
    {
        readonly ShopWebContext Context = new ShopWebContext();
        CollectionViewSource ShopsListSource;
        public Adresses()
        {
            InitializeComponent();

            ShopsListSource = (CollectionViewSource)FindResource("ShopsListSource");

            ShopsListSource.Source = Context.Shops.ToList();

            DataGrid.ItemsSource = Context.Adresses
                .Include(a => a.ShopAdresses)
                .ToList();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Adress NewAdress = new Adress
            {
                Name = "Адрес"                
            };
            e.NewItem = NewAdress;
            Context.Adresses.Add(NewAdress);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Context.SaveChanges();
        }

        private void ShopsDataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            ShopsAdresses shopsAdresses = new ShopsAdresses
            {
                Adress = DataGrid.SelectedItem as Adress,
                Shop = (ShopsListSource.Source as List<Shop>).FirstOrDefault()
            };
            e.NewItem = shopsAdresses;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //ShopsDataGrid.ItemsSource = (DataGrid.SelectedItem as Adress).ShopAdresses;
        }

        private void ShopsDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count == 0) return;
            DataGridCellInfo CurrentCell = e.AddedCells[0];
            if(CurrentCell.Column == ShopsDataGrid.Columns[0])
            {
                ShopsDataGrid.BeginEdit();
            }
        }
    }
}
