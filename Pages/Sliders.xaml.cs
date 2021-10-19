using ShopWeb.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using MenuItem = ShopWeb.Data.MenuItem;

namespace ShopWebApp.Pages
{
    public partial class Sliders : Page
    {
        readonly ShopWebContext Context = new();
        readonly List<MenuItem> ParentsList;
        public Sliders()
        {
            InitializeComponent();


            ParentsList = (from MenuItem in Context.Menu.AsQueryable()
                           where MenuItem.Type == MenuType.SliderItem
                           where MenuItem.Parent == null
                           select MenuItem).ToList();
            ParentsList.Add(null);

            CollectionViewSource ParentsListSource = (CollectionViewSource)FindResource("ParentsListSource");
            ParentsListSource.Source = ParentsList;

            DataGrid.ItemsSource = (from MenuItem in Context.Menu.AsQueryable()
                                    where MenuItem.Type == MenuType.SliderItem
                                    select MenuItem).ToList();
        }

        void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Context.SaveChanges();
        }

        void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            MenuItem NewItem = new()
            {
                Name = "Слайд / Слайдер",
                Type = MenuType.SliderItem
            };
            e.NewItem = NewItem;
            Context.Menu.Add(NewItem);
        }
    }
}