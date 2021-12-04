using FST.Infrastructure.Services.Interfaces;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;

namespace FST.Infrastructure.Services
{
    public class ExcelExportService : IExcelExportService
    {
        public void Export(string pathToSave, IEnumerable<string> columnTitles, IEnumerable<string[]> dataRows)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var titleColumnNum = 0;

            var titleRow = sheet.CreateRow(0);
            foreach (var columnTitle in columnTitles)
            {
                titleRow.CreateCell(titleColumnNum).SetCellValue(columnTitle);
                titleColumnNum++;
            }

            var rowNum = 1;
            var columnNum = 0;
            foreach (var dataRow in dataRows)
            {
                var sheetRow = sheet.CreateRow(rowNum);
                foreach (var data in dataRow)
                {
                    sheetRow.CreateCell(columnNum).SetCellValue(data);
                    columnNum++;
                }
                columnNum = 0;
                rowNum++;
            }

            for (int i = 0; i < titleColumnNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            using (var fileData = new FileStream(pathToSave, FileMode.Create))
            {
                workbook.Write(fileData);
            }
        }
    }
}
