using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShopWebApp.Code
{
    public class DeleteItem
    {
        public DeleteItem(object sender, KeyEventArgs e, ref bool boolVar)
        {
            DataGrid dataGrid = (DataGrid)sender;
            if (e.Key == Key.Delete)
            {
                if (AskDeleteItem.Delete(dataGrid))
                {
                    boolVar = true;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
    }

    static public class AskDeleteItem
    {
        static public bool Delete(DataGrid dataGrid)
        {
            if (dataGrid.SelectedCells.Count > 0)
            {
                MessageBoxResult delete = MessageBox.Show("Вы точно хотите удалить выбранные элементы?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.Yes == delete)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
