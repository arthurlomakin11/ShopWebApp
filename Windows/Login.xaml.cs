using System.Windows;

namespace ShopWebApp.Windows
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(LoginTextBox.Text == "admin" && PasswordTextBox.Text == "2021")
            {
                new MainWindow().Show();
                Close();
            }
        }
    }
}
