namespace BarcodeReader.BarcodeStuff.Engines.Core
{
    public enum BarcodeFormat
    {
        //
        // Zusammenfassung:
        //     Aztec 2D barcode format.
        AZTEC = 1,
        //
        // Zusammenfassung:
        //     CODABAR 1D format.
        CODABAR = 2,
        //
        // Zusammenfassung:
        //     Code 39 1D format.
        CODE_39 = 4,
        //
        // Zusammenfassung:
        //     Code 93 1D format.
        CODE_93 = 8,
        //
        // Zusammenfassung:
        //     Code 128 1D format.
        CODE_128 = 16,
        //
        // Zusammenfassung:
        //     Data Matrix 2D barcode format.
        DATA_MATRIX = 32,
        //
        // Zusammenfassung:
        //     EAN-8 1D format.
        EAN_8 = 64,
        //
        // Zusammenfassung:
        //     EAN-13 1D format.
        EAN_13 = 128,
        //
        // Zusammenfassung:
        //     ITF (Interleaved Two of Five) 1D format.
        ITF = 256,
        //
        // Zusammenfassung:
        //     MaxiCode 2D barcode format.
        MAXICODE = 512,
        //
        // Zusammenfassung:
        //     PDF417 format.
        PDF_417 = 1024,
        //
        // Zusammenfassung:
        //     QR Code 2D barcode format.
        QR_CODE = 2048,
        //
        // Zusammenfassung:
        //     RSS 14
        RSS_14 = 4096,
        //
        // Zusammenfassung:
        //     RSS EXPANDED
        RSS_EXPANDED = 8192,
        //
        // Zusammenfassung:
        //     UPC-A 1D format.
        UPC_A = 16384,
        //
        // Zusammenfassung:
        //     UPC-E 1D format.
        UPC_E = 32768,
        //
        // Zusammenfassung:
        //     UPC_A | UPC_E | EAN_13 | EAN_8 | CODABAR | CODE_39 | CODE_93 | CODE_128 | ITF
        //     | RSS_14 | RSS_EXPANDED without MSI (to many false-positives) and IMB (not enough
        //     tested, and it looks more like a 2D)
        All_1D = 61918,
        //
        // Zusammenfassung:
        //     UPC/EAN extension format. Not a stand-alone format.
        UPC_EAN_EXTENSION = 65536,
        //
        // Zusammenfassung:
        //     MSI
        MSI = 131072,
        //
        // Zusammenfassung:
        //     Plessey
        PLESSEY = 262144,
        //
        // Zusammenfassung:
        //     Intelligent Mail barcode
        IMB = 524288,
        //
        // Zusammenfassung:
        //     Pharmacode format.
        PHARMA_CODE = 1048576
    }
}
