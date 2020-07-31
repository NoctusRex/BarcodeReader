using BarcodeReader.BarcodeStuff.Models;
using System.Drawing;

namespace BarcodeReader.BarcodeStuff.Engines.Core
{
    public interface IBarcodeEngine
    {
        string Identifier { get; }
        Barcode Read(Bitmap barcodeImage);
        Bitmap Write(string text, BarcodeFormat format);
    }
}
