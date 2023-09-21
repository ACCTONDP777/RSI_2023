using System.IO;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.SS.Formula.Eval;

namespace RSI.Models.Utility
{
    public class ExcelHelper
    {
        #region Excel中指定worksheet（xls/xlsx）轉成DataTable
        /// <summary>
        /// 將Excel中指定WorkSheet導成DataTable
        /// </summary>
        /// <param name="excelFileStream">Excel文件流（xls/xlsx）</param>
        /// <param name="sheetIndex">sheet索引，從1開始</param>
        /// <param name="headerRowIndex">表頭行索引，從1開始計數</param>
        /// <param name="firstColumnIndex">數據起始列索引，從1開始計數</param>
        /// <returns></returns>
        public static DataTable ImportExcelToDataTable(Stream excelFileStream, int sheetIndex, int headerRowIndex, int firstColumnIndex)
        {
            //NPOI從0開始計數
            headerRowIndex--;
            firstColumnIndex--;
            sheetIndex--;

            IWorkbook workbook = WorkbookFactory.Create(excelFileStream);
            ISheet sheet = null;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            //get sheet
            sheet = workbook.GetSheetAt(sheetIndex);
            //get header row in sheet 
            IRow headerRow = sheet.GetRow(headerRowIndex);
            //get cell count
            int cellCount = headerRow.LastCellNum;
            //初始化datatable column
            for (int i = firstColumnIndex; i < cellCount; i++)
            {
                if (headerRow.GetCell(i).CellType == CellType.String)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    dt.Columns.Add(column);
                }
            }
            //get row count
            int rowCount = sheet.LastRowNum;

            for (int i = (headerRowIndex + 1); i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dr = dt.NewRow();
                if (row != null)
                {
                    for (int z = firstColumnIndex; z < cellCount; z++)
                    {
                        ICell cell = row.GetCell(z);
                        if (cell != null)
                        {
                            if (cell.CellType != CellType.Blank)
                            {
                                switch (cell.CellType)
                                {
                                    case CellType.Error:
                                        dr[z - firstColumnIndex] = ErrorEval.GetText(cell.ErrorCellValue);
                                        break;
                                    case CellType.Numeric:
                                        {
                                            if (DateUtil.IsCellDateFormatted(cell))
                                            {
                                                dr[z - firstColumnIndex] = cell.DateCellValue;
                                            }
                                            else
                                            {
                                                dr[z - firstColumnIndex] = cell.NumericCellValue;
                                            }
                                        }
                                        break;
                                    case CellType.Formula:
                                        if (cell.CachedFormulaResultType == CellType.Error)
                                        {
                                            dr[z - firstColumnIndex] = ErrorEval.GetText(cell.ErrorCellValue);
                                        }
                                        if (cell.CachedFormulaResultType == CellType.String)
                                        {
                                            dr[z - firstColumnIndex] = cell.StringCellValue;
                                        }
                                        if (cell.CachedFormulaResultType == CellType.Numeric)
                                        {
                                            dr[z - firstColumnIndex] = cell.NumericCellValue;
                                        }
                                        break;
                                    case CellType.Boolean:
                                        dr[z - firstColumnIndex] = cell.BooleanCellValue;
                                        break;
                                    case CellType.String:
                                        dr[z - firstColumnIndex] = cell.StringCellValue;
                                        break;
                                    default:
                                        dr[z - firstColumnIndex] = cell.ToString();
                                        break;
                                }

                            }
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion
    }
}
