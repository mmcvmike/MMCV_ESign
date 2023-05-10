using System;
using System.Data;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MMCV_Common.Helper
{
    public class ExcelHelper
    {
        public static void DataTableToExcel(DataTable tb, string fileName)
        {
            if (!fileName.ToLower().EndsWith(".xls"))
            {
                fileName += ".xls";
            }
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = (DataTableRenderToExcel.RenderDataTableToStream(tb) as MemoryStream);
            }
            finally
            {
                memoryStream.Close();
                memoryStream.Dispose();
            }
        }

        protected static object GetCellValue(ICell cell)
        {
            object result;
            switch (cell.CellType)
            {
                case (CellType)0:
                    if (!DateUtil.IsCellDateFormatted(cell))
                    {
                        result = cell.NumericCellValue;
                    }
                    else
                    {
                        result = cell.DateCellValue;
                    }
                    break;
                case (CellType)1:
                    result = cell.StringCellValue.Trim();
                    break;
                case (CellType)2:
                    result = cell.CellFormula;
                    break;
                case (CellType)3:
                    result = string.Empty;
                    break;
                case (CellType)4:
                    result = cell.BooleanCellValue;
                    break;
                case (CellType)5:
                    result = cell.ErrorCellValue;
                    break;
                default:
                    result = cell.RichStringCellValue;
                    break;
            }
            return result;
        }

        public static DataSet GetDataFromExcel(string path)
        {
            DataSet dataSet = new DataSet();
            FileStream fileStream = null;
            DataSet result;
            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                string extension = Path.GetExtension(path);
                IWorkbook workbook = null;
                if (extension == ".xls")
                {
                    workbook = new HSSFWorkbook(fileStream);
                }
                else if (extension == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheetAt = workbook.GetSheetAt(i);
                    if (sheetAt.LastRowNum > 0)
                    {
                        DataTable dataTable = new DataTable();
                        int lastCellNum = (int)sheetAt.GetRow(0).LastCellNum;
                        for (int j = 0; j < lastCellNum; j++)
                        {
                            dataTable.Columns.Add(new DataColumn(sheetAt.GetRow(0).GetCell(j).StringCellValue.Trim()));
                        }
                        for (int j = sheetAt.FirstRowNum + 1; j < sheetAt.LastRowNum + 1; j++)
                        {
                            IRow row = sheetAt.GetRow(j);
                            DataRow dataRow = dataTable.NewRow();
                            for (int k = 0; k < lastCellNum; k++)
                            {
                                if (row.GetCell(k) != null)
                                {
                                    dataRow[k] = ExcelHelper.GetCellValue(row.GetCell(k));
                                }
                            }
                            dataTable.Rows.Add(dataRow);
                        }
                        dataSet.Tables.Add(dataTable);
                    }
                }
                result = dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return result;
        }
    }

    public static class DataTableRenderToExcel
    {
        public static Stream RenderDataTableToStream(DataTable tb)
        {
            IWorkbook workbook;
            if (tb.Rows.Count < 32767)
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                workbook = new XSSFWorkbook();
            }
            MemoryStream memoryStream = new MemoryStream();
            ISheet sheet = workbook.CreateSheet();
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < tb.Columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(tb.Columns[i].ColumnName);
            }
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                IRow row2 = sheet.CreateRow(i + 1);
                for (int j = 0; j < tb.Columns.Count; j++)
                {
                    row2.CreateCell(j).SetCellValue(tb.Rows[i][j].ToString().Trim());
                }
            }
            if (tb.Rows.Count > 0)
            {
                int num = Math.Min(16, tb.Rows.Count);
                for (int i = 0; i < tb.Columns.Count; i++)
                {
                    int num2 = Encoding.GetEncoding(936).GetBytes(tb.Columns[i].ColumnName).Length;
                    for (int j = 0; j < num; j++)
                    {
                        num2 = Math.Max(num2, Encoding.GetEncoding(936).GetBytes(tb.Rows[j][i].ToString().Trim()).Length);
                    }
                    sheet.SetColumnWidth(i, (num2 + 1) * 512);
                }
            }
            workbook.Write(memoryStream);
            memoryStream.Flush();
            memoryStream.Position = 0L;
            return memoryStream;
        }

        public static void RenderDataTableToExcel(DataTable SourceTable, string FileName)
        {
            using (MemoryStream memoryStream = DataTableRenderToExcel.RenderDataTableToStream(SourceTable) as MemoryStream)
            {
                using (FileStream fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] array = memoryStream.ToArray();
                    fileStream.Write(array, 0, array.Length);
                    fileStream.Flush();
                }
            }
        }

        public static DataTable RenderDataTableFromExcel(DataTableRenderToExcel.ExcelType excelType, Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
        {
            DataTable result;
            try
            {
                IWorkbook workbook;
                if (excelType != DataTableRenderToExcel.ExcelType.Xlsx)
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    workbook = new XSSFWorkbook();
                }
                ISheet sheet = workbook.GetSheet(SheetName);
                DataTable dataTable = new DataTable();
                IRow row = sheet.GetRow(HeaderRowIndex);
                int lastCellNum = (int)row.LastCellNum;
                for (int i = (int)row.FirstCellNum; i < lastCellNum; i++)
                {
                    DataColumn column = new DataColumn(row.GetCell(i).StringCellValue);
                    dataTable.Columns.Add(column);
                }
                int lastRowNum = sheet.LastRowNum;
                for (int i = sheet.FirstRowNum + 1; i < sheet.LastRowNum; i++)
                {
                    IRow row2 = sheet.GetRow(i);
                    DataRow dataRow = dataTable.NewRow();
                    for (int j = (int)row2.FirstCellNum; j < lastCellNum; j++)
                    {
                        dataRow[j] = row2.GetCell(j).ToString();
                    }
                }
                result = dataTable;
            }
            finally
            {
                ExcelFileStream.Close();
            }
            return result;
        }

        public static DataTable RenderDataTableFromExcel(DataTableRenderToExcel.ExcelType excelType, Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
        {
            IWorkbook workbook;
            if (excelType != DataTableRenderToExcel.ExcelType.Xlsx)
            {
                workbook = new HSSFWorkbook(ExcelFileStream);
            }
            else
            {
                workbook = new XSSFWorkbook(ExcelFileStream);
            }
            ISheet sheetAt = workbook.GetSheetAt(SheetIndex);
            DataTable dataTable = new DataTable();
            IRow row = sheetAt.GetRow(HeaderRowIndex);
            int lastCellNum = (int)row.LastCellNum;
            for (int i = (int)row.FirstCellNum; i < lastCellNum; i++)
            {
                DataColumn column = new DataColumn(row.GetCell(i).StringCellValue);
                dataTable.Columns.Add(column);
            }
            for (int i = sheetAt.FirstRowNum + 1; i < sheetAt.LastRowNum; i++)
            {
                IRow row2 = sheetAt.GetRow(i);
                DataRow dataRow = dataTable.NewRow();
                for (int j = (int)row2.FirstCellNum; j < lastCellNum; j++)
                {
                    if (row2.GetCell(j) != null)
                    {
                        dataRow[j] = row2.GetCell(j).ToString();
                    }
                }
                dataTable.Rows.Add(dataRow);
            }
            ExcelFileStream.Close();
            return dataTable;
        }
        public enum ExcelType
        {
            Xls = 1,
            Xlsx
        }
    }
}