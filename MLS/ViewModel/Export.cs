using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLS.ViewModel
{
    /// <summary>
    /// 导出数据
    /// </summary>
    public static class Export
    {
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="fileName"></param>
        public static void Excel(ObservableCollection<TeamScore> scores, string fileName = "f:\\abc.xlsx")
        {
            Microsoft.Office.Interop.Excel.Application excel = null;
            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
            }
            catch
            {
                return;
            }
            excel.Application.Workbooks.Add(true);
            excel.Visible = false;
            Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.ActiveSheet;

            //生成字段名称
            Microsoft.Office.Interop.Excel.Range range = null;
            string[] columns = { "编号", "团队", "姓名", "起跑", "终点", "成绩" };
            var columnList = new List<string>(columns);

            for (int i = 0; i < columnList.Count; i++)
            {
                var txt = columnList[i];
                excel.Cells[1, i + 1] = txt;
                range = (Microsoft.Office.Interop.Excel.Range)excel.Cells[1, i + 1];
                range.Interior.ColorIndex = 15;
                range.Font.Bold = true;
            }
            //填充数据
            for (int i = 0; i < scores.Count; i++)
            {
                var score = scores.ElementAt(i);
                excel.Cells[i + 2, 1] = score.Code;
                excel.Cells[i + 2, 2] = score.Team;
                excel.Cells[i + 2, 3] = score.A;
                excel.Cells[i + 2, 4] = score.AStart.ToString();
                excel.Cells[i + 2, 5] = score.AEnd.ToString();
                excel.Cells[i + 2, 6] = score.AScore.ToString();
            }
            var t = sheet.get_Range("A1", "N1");
            t.EntireColumn.AutoFit();
            sheet.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            excel.Quit();
        }
    }
}
