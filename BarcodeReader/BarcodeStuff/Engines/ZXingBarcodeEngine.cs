using BarcodeReader.BarcodeStuff.Engines.Core;
using BarcodeReader.BarcodeStuff.Models;
using System.Drawing;
using ZXing.Rendering;

namespace BarcodeReader.BarcodeStuff.Engines
{
    public class ZXingBarcodeEngine : IBarcodeEngine
    {
        private FormatMap<ZXing.BarcodeFormat> FormatMap { get; set; }

        public string Identifier => "ZXing";

        public ZXingBarcodeEngine()
        {
            FormatMap = new FormatMap<ZXing.BarcodeFormat>(ZXing.BarcodeFormat.DATA_MATRIX);

            FormatMap.Map(BarcodeFormat.All_1D, ZXing.BarcodeFormat.All_1D);
            FormatMap.Map(BarcodeFormat.AZTEC, ZXing.BarcodeFormat.AZTEC);
            FormatMap.Map(BarcodeFormat.CODABAR, ZXing.BarcodeFormat.CODABAR);
            FormatMap.Map(BarcodeFormat.CODE_128, ZXing.BarcodeFormat.CODE_128);
            FormatMap.Map(BarcodeFormat.CODE_39, ZXing.BarcodeFormat.CODE_39);
            FormatMap.Map(BarcodeFormat.CODE_93, ZXing.BarcodeFormat.CODE_93);
            FormatMap.Map(BarcodeFormat.DATA_MATRIX, ZXing.BarcodeFormat.DATA_MATRIX);
            FormatMap.Map(BarcodeFormat.EAN_13, ZXing.BarcodeFormat.EAN_13);
            FormatMap.Map(BarcodeFormat.EAN_8, ZXing.BarcodeFormat.EAN_8);
            FormatMap.Map(BarcodeFormat.IMB, ZXing.BarcodeFormat.IMB);
            FormatMap.Map(BarcodeFormat.ITF, ZXing.BarcodeFormat.ITF);
            FormatMap.Map(BarcodeFormat.MAXICODE, ZXing.BarcodeFormat.MAXICODE);
            FormatMap.Map(BarcodeFormat.MSI, ZXing.BarcodeFormat.MSI);
            FormatMap.Map(BarcodeFormat.PDF_417, ZXing.BarcodeFormat.PDF_417);
            FormatMap.Map(BarcodeFormat.PHARMA_CODE, ZXing.BarcodeFormat.PHARMA_CODE);
            FormatMap.Map(BarcodeFormat.PLESSEY, ZXing.BarcodeFormat.PLESSEY);
            FormatMap.Map(BarcodeFormat.QR_CODE, ZXing.BarcodeFormat.QR_CODE);
            FormatMap.Map(BarcodeFormat.RSS_14, ZXing.BarcodeFormat.RSS_14);
            FormatMap.Map(BarcodeFormat.RSS_EXPANDED, ZXing.BarcodeFormat.RSS_EXPANDED);
            FormatMap.Map(BarcodeFormat.UPC_A, ZXing.BarcodeFormat.UPC_A);
            FormatMap.Map(BarcodeFormat.UPC_E, ZXing.BarcodeFormat.UPC_E);
            FormatMap.Map(BarcodeFormat.UPC_EAN_EXTENSION, ZXing.BarcodeFormat.UPC_EAN_EXTENSION);
        }

        public Barcode Read(Bitmap barcodeImage)
        {
            if (barcodeImage is null) return null;
            barcodeImage = AddWhiteBorder(barcodeImage);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.Options.AssumeGS1 = true;
            reader.Options.TryHarder = true;
            reader.TryInverted = true;
            reader.AutoRotate = true;

            ZXing.Result result = reader.Decode(barcodeImage);
            if (result is null) return null;

            return new Barcode(result.Text, FormatMap.Resolve(result.BarcodeFormat));
        }

        public Bitmap Write(string text, BarcodeFormat format)
        {
            ZXing.BarcodeWriter bcWriter = new ZXing.BarcodeWriter
            {
                Format = FormatMap.Resolve(format),
                Renderer = new BitmapRenderer()
                {
                    Background = Color.White,
                    Foreground = Color.Black,
                    DpiX = 300,
                    DpiY = 300
                }
            };
            bcWriter.Options.PureBarcode = true;

            return AddWhiteBorder(bcWriter.Write(text));
        }

        private Bitmap AddWhiteBorder(Bitmap bmp)
        {
            int borderSize = 20;
            int newWidth = bmp.Width + (borderSize * 2);
            int newHeight = bmp.Height + (borderSize * 2);

            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics gfx = Graphics.FromImage(newImage))
            {
                using (Brush border = new SolidBrush(Color.White)) gfx.FillRectangle(border, 0, 0, newWidth, newHeight);

                gfx.DrawImage(bmp, new Rectangle(borderSize, borderSize, bmp.Width, bmp.Height));
            }
            return (Bitmap)newImage;
        }
    }

}
