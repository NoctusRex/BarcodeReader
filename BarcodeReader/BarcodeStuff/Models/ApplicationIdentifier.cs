namespace BarcodeReader.BarcodeStuff.Models
{
    public enum DataType
    {
        Numeric,
        Alphanumeric
    }

    /// <summary>
    /// Information Class for an Application Identifier (AI)
    /// </summary>
    public class ApplicationIdentifier
    {
        public string AI { get; set; }
        public string Description { get; set; }
        public int LengthOfAI { get; set; }
        public DataType DataDescription { get; set; }
        public int LengthOfData { get; set; }
        public bool FNC1 { get; set; }

        public string Value { get; set; }

        public ApplicationIdentifier(string AI, string Description, int LengthOfAI, DataType DataDescription, int LengthOfData, bool FNC1)
        {
            this.AI = AI;
            this.Description = Description;
            this.LengthOfAI = LengthOfAI;
            this.DataDescription = DataDescription;
            this.LengthOfData = LengthOfData;
            this.FNC1 = FNC1;
        }

        public override string ToString() => string.Format("{0} [{1}]", AI, Description);
    }
}
