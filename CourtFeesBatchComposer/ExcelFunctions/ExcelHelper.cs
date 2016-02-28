using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using OfficeOpenXml;

namespace CourtFeesBatchComposer.ExcelFunctions {
    class ExcelHelper {
        private static ExcelWorksheet CreateExcelSheet(ExcelPackage package, string workSheetName) {
            package.Workbook.Worksheets.Add(workSheetName);
            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
            workSheet.Name = workSheetName;
            workSheet.Cells.Style.Font.Size = 12;
            workSheet.Cells.Style.Font.Name = "Calibri";

            return workSheet;
        }
    }
}
