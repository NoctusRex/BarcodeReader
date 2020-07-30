using System.Collections.Generic;

namespace BarcodeReader.BarcodeStuff.Engines.Core
{
    internal class BarcodeEngineLoader
    {
        public static IBarcodeEngine BarcodeEngine { get; private set; } = new ZXingBarcodeEngine();

        public static IEnumerable<IBarcodeEngine> GetAllBarcodeEngines()
        {
            return null;
        }

        public static void SetBarcodeEngine(IBarcodeEngine barcodeEngine) => BarcodeEngine = barcodeEngine;
    }
}
