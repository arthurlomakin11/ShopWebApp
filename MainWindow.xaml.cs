using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ShopWebApp.Pages;
using ShopWebData;
using Menu = ShopWebApp.Pages.Menu;
using System.Windows.Markup;
using System;
using System.Linq;
using ShopWebApp.Code;

namespace ShopWebApp
{
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {            
            InitializeComponent();            


            SetCulture();

            SetConnectionString();

            InitializeSettings();       
        }
        

        void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata
                (
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            );
        }

        void SetConnectionString()
        {
            string[] CommandLineArgs = Environment.GetCommandLineArgs();

            string GetNextValueInArray(string Command)
            {
                int serverIndex = Array.IndexOf(CommandLineArgs, Command);
                if((serverIndex + 1) < CommandLineArgs.Length)
                {
                    return CommandLineArgs[serverIndex + 1];
                }
                else
                {
                    return "";
                }
            }

            string serverCommand = CommandLineArgs.FirstOrDefault(s => s == "--server");
            string server = serverCommand?.Length > 0 ? GetNextValueInArray(serverCommand) : "";


            string databaseCommand = CommandLineArgs.FirstOrDefault(s => s == "--database");
            string database = databaseCommand?.Length > 0 ? GetNextValueInArray(databaseCommand) : "";

            string passwordCommand = CommandLineArgs.FirstOrDefault(s => s == "--password");
            string password = passwordCommand?.Length > 0 ? GetNextValueInArray(passwordCommand) : "";

            string userCommand = CommandLineArgs.FirstOrDefault(s => s == "--user");
            string user = userCommand?.Length > 0 ? GetNextValueInArray(userCommand) : "";


            if (server.Length == 0)
            {
                server = ConfigurationManager.AppSettings["server"];
            }
            if(database.Length == 0)
            {
                database = ConfigurationManager.AppSettings["database"];
            }
            if (password.Length == 0)
            {
                password = ConfigurationManager.AppSettings["password"];
            }
            if (user.Length == 0)
            {
                user = ConfigurationManager.AppSettings["user"];
            }

            ShopWebContext.ConnectionString = $"Server={server};Database={database};User Id={user};Password={password};MultipleActiveResultSets=True";               
        }

        void ApplyPage(Page page)
        {
            frame.Content = page;
            Title = page.Title;
        }

        void Products_Click(object sender, RoutedEventArgs e) => ApplyPage(new Products());

        void Categories_Click(object sender, RoutedEventArgs e) => ApplyPage(new Categories());

        void Menu_Click(object sender, RoutedEventArgs e) => ApplyPage(new Menu());

        void Shops_Click(object sender, RoutedEventArgs e) => ApplyPage(new Shops());
        void Adresses_Click(object sender, RoutedEventArgs e) => ApplyPage(new Adresses());

        void Sliders_Click(object sender, RoutedEventArgs e) => ApplyPage(new Sliders());
        void Orders_Click(object sender, RoutedEventArgs e) => ApplyPage(new Orders());
        void OrdersCanceled_Click(object sender, RoutedEventArgs e) => ApplyPage(new OrdersCanceled());
        void Users_Click(object sender, RoutedEventArgs e) => ApplyPage(new Users());

        void StatisticsByDayPage_Click(object sender, RoutedEventArgs e) => ApplyPage(new StatisticsByDayPage());

        void StatisticsByHourPage_Click(object sender, RoutedEventArgs e) => ApplyPage(new StatisticsByHourPage());

        void StatisticsByCategoryPage_Click(object sender, RoutedEventArgs e) => ApplyPage(new StatisticsByCategoryPage());

        void Messages_Click(object sender, RoutedEventArgs e) => ApplyPage(new Messages());

        void DeliveryTime_Click(object sender, RoutedEventArgs e) => ApplyPage(new DeliveryTimes());

        void ProductsCollections_Click(object sender, RoutedEventArgs e) => ApplyPage(new ProductsCollections());
        void DeliveryPriceTags_Click(object sender, RoutedEventArgs e) => ApplyPage(new DeliveryPriceTags());

        void InitializeSettings()
        {
            CheckMenuItemSetting("ShowProductsInMenuDesktop", Products);
            CheckMenuItemSetting("ShowMenuInMenuDesktop", Menu);
            CheckMenuItemSetting("ShowCategoriesInMenuDesktop", Categories);
            CheckMenuItemSetting("ShowShopsInMenuDesktop", Shops);
            CheckMenuItemSetting("ShowAdressesInMenuDesktop", Adresses);
            CheckMenuItemSetting("ShowSlidersInMenuDesktop", Sliders);
            CheckMenuItemSetting("ShowUsersInMenuDesktop", Users);
            CheckMenuItemSetting("ShowReportsInMenuDesktop", Reports);
            CheckMenuItemSetting("ShowStatisticsInMenuDesktop", Statistics);
            CheckMenuItemSetting("ShowMessagesInMenuDesktop", Messages);
            CheckMenuItemSetting("ShowProductsInMenuDesktop", Products);
            CheckMenuItemSetting("ShowProductsCollectionsInMenuDesktop", ProductsCollections);
            CheckMenuItemSetting("ShowDeliveryPriceTagsInMenuDesktop", DeliveryPriceTags);
            CheckMenuItemSetting("ShowDeliveryTimesInMenuDesktop", DeliveryTime);
        }

        void CheckMenuItemSetting(string Setting, System.Windows.Controls.MenuItem MenuItem)
        {
            if (int.Parse(SettingsManager.GetValue(Setting)) == 1)
            {
                MenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                MenuItem.Visibility = Visibility.Collapsed;
            }
        }
    }
}
