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

using ShopWebData;

namespace ShopWebApp.Pages
{
    public partial class Messages : Page
    {
        public Messages()
        {
            InitializeComponent();

            ShopWebContext Context = new();

            DataGrid.ItemsSource = Context.Messages.ToList();
        }
    }
}
