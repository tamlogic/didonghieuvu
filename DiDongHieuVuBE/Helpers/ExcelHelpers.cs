using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.Helpers
{
    public class ExcelHelpers
    {
        public static void Format_Border_Excel_Range(ExcelRange excelRange)
        {
            try
            {
                excelRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                excelRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                excelRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                excelRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                excelRange.Style.Border.Top.Color.SetColor(Color.Gray);
                excelRange.Style.Border.Bottom.Color.SetColor(Color.Gray);
            }
            catch (Exception ex)
            {

            }
        }

        public static string ConvertUseDink(string _html, IConverter converterPDF, string contentRootPath, string prefixFile)
        {
            try
            {
                string fileName = prefixFile.Trim()+"_" + DateTime.Now.Ticks.ToString() + ".pdf",
                    folder = Path.Combine(contentRootPath, @"ExportHistory\PDF"),
                    fileSave = Path.Combine(folder, fileName);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                MarginSettings marginNew = new MarginSettings();
                marginNew.Left = 15;
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Out = fileSave,
                        Margins = marginNew,
                    },
                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = _html,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            //HeaderSettings = { FontSize = 9, Right = "Trang [page] / [toPage]", Line = true, Spacing = 2.812 },
                            FooterSettings = { FontSize = 9, Right = "Trang [page] / [toPage]", Line = false, Spacing = 2.812 },
                    }
                }
                };

                byte[] pdf = converterPDF.Convert(doc);
                return fileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string ConvertUseDinkLandscape(string _html, IConverter converterPDF, string contentRootPath, string prefixFile)
        {
            try
            {
                string fileName = prefixFile.Trim() + "_" + DateTime.Now.Ticks.ToString() + ".pdf",
                    folder = Path.Combine(contentRootPath, @"ExportHistory\PDF"),
                    fileSave = Path.Combine(folder, fileName);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                MarginSettings marginNew = new MarginSettings();
                marginNew.Left = 15;
                marginNew.Bottom = 15;
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Landscape,
                        PaperSize = PaperKind.A4,
                        Out = fileSave,
                        Margins = marginNew,
                    },
                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = _html,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            //HeaderSettings = { FontSize = 9, Right = "Trang [page] / [toPage]", Line = true, Spacing = 2.812 },
                            FooterSettings = { FontSize = 9, Right = "Trang [page] / [toPage]", Line = false, Spacing = 2.812 },
                    }
                }
                };

                byte[] pdf = converterPDF.Convert(doc);
                return fileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string SaveFileExcel(ExcelPackage package, string contentRootPath, string prefixFile)
        {
            try
            {
                string _fileMapServer = prefixFile+"_" + DateTime.Now.Ticks.ToString() + ".xlsx",
                       folder = Path.Combine(contentRootPath, @"ExportHistory\Excel"),
                       _pathSave = Path.Combine(folder, _fileMapServer);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                {
                    package.SaveAs(fs);
                }
                byte[] _result; _result = package.GetAsByteArray();
                return _fileMapServer;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


    }
}
