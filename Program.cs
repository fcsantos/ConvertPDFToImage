using ImageMagick;
using Tesseract;
using IronOcr;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConvertPDFToImage
{
    class Program
    {
        static void Main(string[] args)
        {
            var localPathDoc = @"C:\Users\fcsan\Downloads\";
            var localPathImage = @"C:\Users\fcsan\Source\FilesTesseractAndImage\images\";
            var localPathJson = @"E:\";
            
            //PDFToImage.GenerateImageFromPDF(localPathDoc, localPathImage, localPathJson);

            //IronOCRClass.ExtractTextToPDF(localPathImage, localPathJson);

            TesseractClass.ExtractTextToPDF(localPathImage, localPathJson);
        }
    }

    public static class PDFToImage
    {
        //3min
        public static void GenerateImageFromPDF(string path, string pathImage, string pathJson)
        {
            var dateStartExtract = DateTime.Now;
            Console.WriteLine(dateStartExtract);

            var settings = new MagickReadSettings();
            // Settings the density to 300 dpi will create an image with a better quality
            settings.Density = new Density(300, 300);

            using (var images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                images.Read(path+"entre o agora e o nunca.pdf", settings);

                var page = 1;
                foreach (var image in images)
                {
                    // Write page to file that contains the page number
                    image.Write(pathImage+"entre o agora e o nunca.Page" + page + ".png");
                    // Writing to a specific format works the same as for a single image
                    //image.Format = MagickFormat.Ptif;
                    //image.Write("Snakeware.Page" + page + ".tif");
                    page++;
                }
            }

            new JsonGenerate().generate("PDFToImage.GenerateImageFromPDF", dateStartExtract, DateTime.Now, "", "PDFToImage.GenerateImageFromPDF", pathJson);
            Console.WriteLine(DateTime.Now);
        }
    }

    public static class IronOCRClass
    {
        //15min
        public static void ExtractTextToPDF(string path, string pathJson)
        {

            var dateStartExtract = DateTime.Now;
            Console.WriteLine(dateStartExtract);
            
            var Ocr = new IronTesseract();

            var folderImages = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();

            foreach (var image in folderImages)
            {
                Console.WriteLine(image);
                // Fast Dictionary
                Ocr.Language = OcrLanguage.Portuguese;
                // Latest Engine 
                Ocr.Configuration.TesseractVersion = TesseractVersion.Tesseract5;
                // AI OCR only without font analysis
                Ocr.Configuration.EngineMode = TesseractEngineMode.LstmOnly;
                // Turn off unneeded options
                Ocr.Configuration.ReadBarCodes = false;
                Ocr.Configuration.RenderSearchablePdfsAndHocr = false;
                // Assume text is laid out neatly in an orthagonal document
                Ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.Auto;
                using (var Input = new OcrInput(image))
                {
                    Input.MinimumDPI = 100;
                    Input.TargetDPI = 150;

                    var Result = Ocr.Read(Input);                    
                    Console.WriteLine(Result.Text);
                    Console.WriteLine("-----------------------------------------------------------||-----------------------------------------------------------");
                }
            }

            new JsonGenerate().generate("IronOCRClass.ExtractTextToPDF", dateStartExtract, DateTime.Now, "", "IronOCRClass.ExtractTextToPDF", pathJson);
            Console.WriteLine("date start: " + dateStartExtract + " - date end: " + DateTime.Now);
        }
    }

    public static class TesseractClass
    {
        //30min
        public static void ExtractTextToPDF(string path, string pathJson)
        {
            var dateStartExtract = DateTime.Now;
            Console.WriteLine(dateStartExtract);

            try
            {
                var folderImages = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();

                foreach (var image in folderImages)
                {
                    Console.WriteLine(image);

                    using (var engine = new TesseractEngine(@"C:\Users\fcsan\AppData\Local\Temp\IronOcr\tessdata\tessdata", "por", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromFile(image))
                        {
                            using (var page = engine.Process(img))
                            {
                                var text = page.GetText();
                                Console.WriteLine(text);
                                Console.WriteLine("-----------------------------------------------------------||-----------------------------------------------------------");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
            }

            new JsonGenerate().generate("TesseractClass.ExtractTextToPDF", dateStartExtract, DateTime.Now, "", "TesseractClass.ExtractTextToPDF", pathJson);
            Console.WriteLine("date start: " + dateStartExtract + " - date end: " + DateTime.Now);
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }

    public class JsonGenerate
    {
        public void generate(string className, DateTime dateStart, DateTime dateEnd, string message, string fileName, string pathJson)
        {
            List<JsonData> _data = new List<JsonData>();
            _data.Add(new JsonData()
            {
                DateStart = dateStart,
                DateEnd = dateEnd,
                ClassName = className,
                Message = message
            });

            //open file stream
            using (StreamWriter file = File.CreateText(pathJson+fileName+".txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, _data);
            }
        }


    }

    public class JsonData
    {
        public JsonData()
        {
            _id = Guid.NewGuid();
        }

        public Guid _id { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string ClassName { get; set; }

        public string Message { get; set; }
    }
}
