namespace BarcodeReader.Misc
{
    public static class BarcodeConstants
    {
        public static char FNC1 => (char)29;
        public static string FNC1_DisplayPlaceholder => "[FNC1]";
        /// <summary>
        /// http://gs1md.org/wp-content/uploads/2016/06/GS1_General_Specifications.pdf
        /// 5.4.6.4
        /// </summary>
        public static string FNC1_SymbologyIdentifier => "]C1";
    }
}
