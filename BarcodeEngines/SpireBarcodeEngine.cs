using System.Drawing;
using BarcodeReader.BarcodeStuff.Engines.Core;
using BarcodeReader.BarcodeStuff.Models;
using Spire.Barcode;

namespace BarcodeEngines
{
    public class SpireBarcodeEngine : IBarcodeEngine
    {
        private FormatMap<BarCodeType> FormatMap { get; set; }

        public string Identifier => "Spire";

        public SpireBarcodeEngine()
        {
            FormatMap = new FormatMap<BarCodeType>(BarCodeType.Code128);

            FormatMap.Map(BarcodeFormat.CODE_128, BarCodeType.Code128);
            FormatMap.Map(BarcodeFormat.CODABAR, BarCodeType.Codabar);
            FormatMap.Map(BarcodeFormat.CODE_39, BarCodeType.Code39);
            FormatMap.Map(BarcodeFormat.CODE_93, BarCodeType.Code93);
            FormatMap.Map(BarcodeFormat.QR_CODE, BarCodeType.QRCode);
            FormatMap.Map(BarcodeFormat.DATA_MATRIX, BarCodeType.QRCode);
            FormatMap.Map(BarcodeFormat.CODE_128, BarCodeType.Code128);
            FormatMap.Map(BarcodeFormat.EAN_13, BarCodeType.EAN13);
            FormatMap.Map(BarcodeFormat.EAN_8, BarCodeType.EAN8);
        }

        public Barcode Read(Bitmap barcodeImage)
        {
            string text = BarcodeScanner.ScanOne(barcodeImage);
            if (string.IsNullOrEmpty(text)) return null;

            return new Barcode(text, BarcodeFormat.CODE_128);
        }

        public Bitmap Write(string text, BarcodeFormat format)
        {
            BarCodeGenerator generator = new BarCodeGenerator(new BarcodeSettings()
            {
                AutoResize = true,
                BackColor = Color.White,
                BorderColor = Color.White,
                BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
                BorderWidth = 20,
                DpiX = 300,
                DpiY = 300,
                ForeColor = Color.Black,
                ShowText = false,
                Data = text,
                ResolutionType = ResolutionType.UseDpi,
                Type = FormatMap.Resolve(format),
                Data2D = text
            });

            return (Bitmap)generator.GenerateImage();
        }
    }
}
