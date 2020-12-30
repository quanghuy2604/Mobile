using ClosedXML.Excel;
using DemoAuth.Models;
using DocumentFormat.OpenXml.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Document = iTextSharp.text.Document;
using Paragraph = iTextSharp.text.Paragraph;

namespace DemoAuth.Controllers
{
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {

            return View();

        }
        [AllowAnonymous]
        public IActionResult run()
        {
            List<Author> authors = new List<Author>
{
    new Author { Id = 1, FirstName = "Joydip", LastName = "Kanjilal" },
    new Author { Id = 2, FirstName = "Steve", LastName = "Smith" },
    new Author { Id = 3, FirstName = "Anand", LastName = "Narayaswamy"}
};
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "authors.xlsx";
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =
                    workbook.Worksheets.Add("Authors");
                    worksheet.Cell(1, 1).Value = "Id";
                    worksheet.Cell(1, 2).Value = "FirstName";
                    worksheet.Cell(1, 3).Value = "LastName";
                    for (int index = 1; index <= authors.Count; index++)
                    {
                        worksheet.Cell(index + 1, 1).Value =
                        authors[index - 1].Id;
                        worksheet.Cell(index + 1, 2).Value =
                        authors[index - 1].FirstName;
                        worksheet.Cell(index + 1, 3).Value =
                        authors[index - 1].LastName;
                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);

                    }
                }
            }
            catch (Exception ex)
            {
                return Error();
            }
        }
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            document.Open();
            document.Add(new Paragraph("Hello World"));
            document.Add(new Paragraph(DateTime.Now.ToString()));
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return new FileStreamResult(workStream, "application/pdf");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
