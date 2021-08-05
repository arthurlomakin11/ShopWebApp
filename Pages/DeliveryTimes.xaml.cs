using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ShopWebData;

namespace ShopWebApp.Pages
{
    public partial class DeliveryTimes : Page
    {
        public ShopWebContext Context = new();
        public DeliveryTimes()
        {
            InitializeComponent();

            DataGrid.ItemsSource = Context.DeliveryTimes.ToList();
        }

        void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            DeliveryTime NewTime = new()
            {
                Name = "",
                CustomTime = false,
                Active = true                
            };
            e.NewItem = NewTime;
            Context.DeliveryTimes.Add(NewTime);
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Context.SaveChanges();
        }
    }
}
