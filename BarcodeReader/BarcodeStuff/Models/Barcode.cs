using BarcodeReader.BarcodeStuff.Engines.Core;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeReader.BarcodeStuff.Models
{
    public class Barcode
    {
        private string text;

        public string Text
        {
            get
            {
                if (!IsGS1) return OriginalText;
                if (ApplicationIdentifer is null || !ApplicationIdentifer.Any()) return OriginalText;
                if (!string.IsNullOrEmpty(text)) return text;

                foreach (ApplicationIdentifier ai in ApplicationIdentifer)
                {
                    text += BarcodeConstants.FNC1 + ai.AI + ai.Value;
                }

                return text;
            }
        }
        public string OriginalText { get; private set; }
        public BarcodeFormat Format { get; private set; }
        public bool IsGS1 => OriginalText.Contains(BarcodeConstants.FNC1.ToString()) || OriginalText.StartsWith(BarcodeConstants.FNC1_SymbologyIdentifier);

        public List<ApplicationIdentifier> ApplicationIdentifer { get; private set; }

        public Barcode(string text, BarcodeFormat format)
        {
            OriginalText = text;
            Format = format;

            if (IsGS1) ParseGS1(OriginalText);
        }

        private void ParseGS1(string barcode)
        {
            // restore orignal barcode
            if (!barcode.StartsWith(BarcodeConstants.FNC1_SymbologyIdentifier))
                barcode = BarcodeConstants.FNC1_SymbologyIdentifier + barcode.TrimStart(BarcodeConstants.FNC1);

            ApplicationIdentifer = Fnc1Parser.Parse(barcode, false);
        }

    }
}
