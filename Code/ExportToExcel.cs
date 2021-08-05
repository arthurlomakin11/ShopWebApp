using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace ShopWebApp.Code
{
    public static class ExportToExcel
    {
        public static void ExportFromDataGrid<T>(string WorksheetName, string FilePath, List<string> Headers, DataGrid DataGrid, Action Action, bool OpenFileAfterComplete = true)
        {
            using XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(WorksheetName);
            
            worksheet.Row(1).Value = Headers;

            int RowsCount = 2;
            foreach (T Item in DataGrid.Items)
            {
                Action.DynamicInvoke(Item, worksheet);
                RowsCount++;
            }
            workbook.SaveAs(FilePath);


            if (OpenFileAfterComplete)
            {
                FileInfo fi = new FileInfo(FilePath);
                if (fi.Exists)
                {                    
                    System.Diagnostics.Process.Start(FilePath);
                }
            }
        }
    }
}
