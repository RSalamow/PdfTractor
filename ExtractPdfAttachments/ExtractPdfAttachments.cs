using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System.IO;

namespace PdfAttachments
{
    public class ExtractPdfAttachments: CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> FilePath { get; set; }

        [Category("Output")]
        public OutArgument<List<string>> Files { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<string> files = new List<string>();
            string filePath = FilePath.Get(context);
            string fileFolderPath = Path.GetDirectoryName(filePath); 
            // Get current working directory
            string currentDirectory = Directory.GetCurrentDirectory();
            //Load the PDF document
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(filePath);
            //Get first page from document
            PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;
            //Get the annotation collection from pages
            PdfLoadedAnnotationCollection annotations = page.Annotations;
            //Iterates the annotations
            foreach (PdfLoadedAnnotation annot in annotations)
            {
                //Check for the attachment annotation
                if (annot is PdfLoadedAttachmentAnnotation)
                {
                    PdfLoadedAttachmentAnnotation file = annot as PdfLoadedAttachmentAnnotation;
                    //Extracts the attachment and saves it to the disk
                    FileStream stream = new FileStream(file.FileName, FileMode.Create);
                    stream.Write(file.Data, 0, file.Data.Length);
                    stream.Dispose();
                }
            }
            //Iterates through the attachments
            if (loadedDocument.Attachments.Count != 0)
            {
                foreach (PdfAttachment attachment in loadedDocument.Attachments)
                {
                    string fullPath = fileFolderPath + "\\" + attachment.FileName;
                    //Extracts the attachment and saves it to the disk
                    FileStream stream = new FileStream(fullPath, FileMode.Create);
                    stream.Write(attachment.Data, 0, attachment.Data.Length);
                    files.Add(fullPath);
                    stream.Dispose();
                }
            }
            //Close the document
            loadedDocument.Close(true);
            Files.Set(context, files);
        }
    }
}
