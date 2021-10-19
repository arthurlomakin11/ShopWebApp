using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using ShopWebApp.Code;
using ShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ShopWebApp.Pages
{
    public partial class Users : Page
    {
        ShopWebContext Context = new ShopWebContext();
        public Users()
        {
            InitializeComponent();

            DataGrid.ItemsSource = Context.Users
                .Include(u => u.Carts)
                    .ThenInclude(u => u.ShopAdress)
                        .ThenInclude(a => a.Parent)
                .ToList();

            //ExcelImage.Source = new Imag/ Icons / ExportToExcel.png
        }

        void Excel_Click(object sender, RoutedEventArgs e)
        {
            string FilePath = Environment.CurrentDirectory + "/Users.xlsx";

            using XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Клиенты");

            worksheet.Cell(1, 1).Value = "ФИО";
            worksheet.Cell(1, 2).Value = "Номер телефона";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Адрес";

            int RowsCount = 2;
            foreach (User Item in DataGrid.Items)
            {
                worksheet.Cell(RowsCount, 1).Value = Item.Fio;
                worksheet.Cell(RowsCount, 2).SetValue<string>(Item.PhoneNumber);
                worksheet.Cell(RowsCount, 3).Value = Item.Email;
                worksheet.Cell(RowsCount, 4).Value = Item.LastCartFullAdress;

                RowsCount++;
            }

            worksheet.ColumnsUsed().AdjustToContents();

            workbook.SaveAs(FilePath);


            FileInfo fi = new FileInfo(FilePath);
            if (fi.Exists)
            {
                Process Process = new Process
                {
                    StartInfo = new ProcessStartInfo(FilePath)
                    {
                        UseShellExecute = true
                    }
                };
                Process.Start();
            }
        }
    }
}