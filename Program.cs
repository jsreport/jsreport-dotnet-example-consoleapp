using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {         
            Console.WriteLine("Initializing local jsreport.exe utility");
            var rs = new LocalReporting()
                .KillRunningJsReportProcesses()
                .UseBinary(JsReportBinary.GetBinary())
                .Configure(cfg => cfg.AllowLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory())
                .AsUtility()
                .Create();
            //.AsWebServer()
            //.RedirectOutputToConsole()


            /*            rs.StartAsync().Wait();
                        Console.ReadLine();
                        rs.KillAsync().Wait();
                        return;*/

            /* Console.WriteLine("Rendering localy stored template jsreport/data/templates/Invoice into invoice.pdf");
             var invoiceReport = rs.RenderByNameAsync("Invoice", InvoiceData).Result;

             var logs = "";            
             long startTime = invoiceReport.Meta.Logs.First().Timestamp.Ticks;
             foreach (var l in invoiceReport.Meta.Logs)
             {               
                 logs += $"+{Math.Round((double)(l.Timestamp.Ticks - startTime)/1000)} {l.Message}\n";
             }            

             invoiceReport.Content.CopyTo(File.OpenWrite("invoice.pdf"));*/

            Console.WriteLine("Rendering custom report fully described through the request object into customReport.pdf");
            var customReport = rs.RenderAsync(new RenderRequest()
            {
                Template = new Template()
                {
                    Content = File.ReadAllText("test.html"),
                    Engine = Engine.None,
                    Recipe = Recipe.PhantomPdf
                }
            }).Result;

            customReport = rs.RenderAsync(new RenderRequest()
            {
                Template = new Template()
                {
                    Content = /*File.ReadAllText("test.html")*/"Hello",
                    Engine = Engine.None,
                    Recipe = Recipe.PhantomPdf
                }
            }).Result;

            var logs = "";
            long startTime = customReport.Meta.Logs.First().Timestamp.Ticks;
            foreach (var l in customReport.Meta.Logs)
            {
                logs += $"+{Math.Round((double)(l.Timestamp.Ticks - startTime) / 1000)} {l.Message}\n";
            }

            customReport.Content.CopyTo(File.OpenWrite("customReport.pdf"));
        }   

        private static RenderRequest CustomRenderRequest = new RenderRequest()
        {
            Template = new Template()
            {
                Content = "Helo world from {{message}}",
                Engine = Engine.Handlebars,
                Recipe = Recipe.PhantomPdf
            },
            Data = new
            {
                message = "jsreport for .NET!!!"
            }
        };

        static object InvoiceData = new
        {
            number = "123",
            seller = new
            {
                name = "Next Step Webs, Inc.",
                road = "12345 Sunny Road",
                country = "Sunnyville, TX 12345"
            },
            buyer = new
            {
                name = "Acme Corp.",
                road = "16 Johnson Road",
                country = "Paris, France 8060"
            },
            items = new[]
            {
                new { name = "Website design", price = 300 }
            }
        };
    }
}
