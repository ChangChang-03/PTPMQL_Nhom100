using OfficeOpenXml;
using System.Data;

namespace MVC.Models
{
    public class ExcelProcess
    {
        public DataTable ExcelToDataTable(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 
            var dt = new DataTable();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                bool hasHeader = true;

                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                    dt.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");

                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                {
                    var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                    DataRow row = dt.NewRow();
                    foreach (var cell in wsRow)
                        row[cell.Start.Column - 1] = cell.Text;
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }
    }
}
