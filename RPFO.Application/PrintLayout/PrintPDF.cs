using DevExpress.XtraReports.UI;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;

namespace RPFO.Application.PrintLayout
{
    public partial class PrintPDF : DevExpress.XtraReports.UI.XtraReport
    {
         
        string pdfFileName = "";
        string PrintSize = "";
        public PrintPDF()
        {
            
            InitializeComponent(); 
        
        }
        public void SetModel(string pdfFileName, string PrintSize)
        { 
            this.pdfFileName = pdfFileName;
            this.PrintSize = PrintSize;
            

        }
        protected override void OnBeforePrint(PrintEventArgs e)
        {
            addPDFContent(pdfFileName, PrintSize);

            //base.PaperKind = PaperKind.Custom;
            //if (PrintSize == "57")
            //{
            //    base.PageWidth = 550;
            //}
            //else
            //{
            //    base.PageWidth = 700;
            //}
            //PrintingSystem.ExecCommand(PrintingSystemCommand.ZoomIn);
            base.OnBeforePrint(e);
        }
        public void addPDFContent(string pdfFileName, string PrintSize)
        {
         
            //string Folder = Path.Combine(PrintFolder, "");
            string Flag = "";
            // Create an XRPdfContent class instance.
            XRPdfContent pdfContent = this.xrPdfContent1;
            
            // Assign binary PDF content to the XRPdfContent's Source property.
            string FilePath = pdfFileName;
            //Folder + @"\" + pdfFileName + ".pdf";
            pdfContent.Source = System.IO.File.ReadAllBytes(FilePath);

            //ReportPrintTool printTool = new ReportPrintTool(report);
            //printTool.Print("Printer Name");

         
        }


    }
}
