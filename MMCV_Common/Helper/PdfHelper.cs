using iTextSharp.text.pdf;
using System.IO;
using System.Linq;
using System.Text;

namespace MMCV_Common.Helper
{
    public static class PdfHelper
    {
        public static void RemoveXObject(string inputFile, string outputFile)
        {
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(inputFile);
            var numberOfPage = reader.NumberOfPages;

            for (int i = 1; i < numberOfPage + 1; i++)
            {
                var resource = reader.GetPageResources(i);
                resource.Remove(new PdfName("XObject"));
            }

            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(reader, fs, iTextSharp.text.pdf.PdfWriter.VERSION_1_5);
                iTextSharp.text.pdf.PdfWriter writer = stamper.Writer;
                writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_5);
                writer.CompressionLevel = iTextSharp.text.pdf.PdfStream.BEST_COMPRESSION;
                //reader.RemoveFields();
                reader.RemoveUnusedObjects();
                stamper.Reader.RemoveUnusedObjects();

                stamper.SetFullCompression();
                //stamper.Writer.SetFullCompression();
                stamper.Close();
            }

            reader.Close();

            System.IO.File.Delete(inputFile);
            System.IO.File.Move(outputFile, inputFile);
        }
    }
}
