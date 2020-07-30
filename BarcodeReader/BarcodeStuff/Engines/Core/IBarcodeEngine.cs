using BarcodeReader.BarcodeStuff.Models;
using System.Drawing;

namespace BarcodeReader.BarcodeStuff.Engines.Core
{
    interface IBarcodeEngine
    {
        Barcode Read(Bitmap barcodeImage);
        Bitmap Write(string text, BarcodeFormat format);
    }
}
