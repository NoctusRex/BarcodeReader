using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeReader.Misc
{
    /// <summary>
    /// https://stackoverflow.com/questions/9721718/ean128-or-gs1-128-decode-c-sharp
    /// </summary>
    public static class Fnc1Parser
    {
        public enum DataType
        {
            Numeric,
            Alphanumeric
        }

        /// <summary>
        /// Information Class for an Application Identifier (AI)
        /// </summary>
        public class AII
        {
            public string AI { get; set; }
            public string Description { get; set; }
            public int LengthOfAI { get; set; }
            public DataType DataDescription { get; set; }
            public int LengthOfData { get; set; }
            public bool FNC1 { get; set; }

            public AII(string AI, string Description, int LengthOfAI, DataType DataDescription, int LengthOfData, bool FNC1)
            {
                this.AI = AI;
                this.Description = Description;
                this.LengthOfAI = LengthOfAI;
                this.DataDescription = DataDescription;
                this.LengthOfData = LengthOfData;
                this.FNC1 = FNC1;
            }

            public override string ToString()
            {
                return string.Format("{0} [{1}]", AI, Description);
            }
        }

        private static readonly SortedDictionary<string, AII> aiiDict = new SortedDictionary<string, AII>();
        private static readonly string[] aiis;
        private static readonly int minLengthOfAI = 1;
        private static readonly int maxLengthOfAI = 4;
        private static char groupSeperator = BarcodeConstants.FNC1;
        private static string ean128StartCode = BarcodeConstants.FNC1_SymbologyIdentifier;
        private static bool hasCheckSum = false;

        public static bool HasCheckSum
        {
            get => Fnc1Parser.hasCheckSum;
            set => Fnc1Parser.hasCheckSum = value;
        }

        public static char GroutSeperator
        {
            get => Fnc1Parser.groupSeperator;
            set => Fnc1Parser.groupSeperator = value;
        }

        public static string EAN128StartCode
        {
            get => Fnc1Parser.ean128StartCode;
            set => Fnc1Parser.ean128StartCode = value;
        }

        public static string[] Aiis => aiis;

        static Fnc1Parser()
        {
            Add("00", "SSCC", 2, DataType.Numeric, 18, false);
            Add("01", "EAN Number Of Trading Unit", 2, DataType.Numeric, 14, false);
            Add("02", "EAN Number Of The Wares In The Shipping Unit", 2, DataType.Numeric, 14, false);
            Add("10", "Lot", 2, DataType.Alphanumeric, 20, true);
            Add("11", "Producer Date", 2, DataType.Numeric, 6, false);
            Add("12", "Due Date", 2, DataType.Numeric, 6, false);
            Add("13", "Packing Date", 2, DataType.Numeric, 6, false);
            Add("15", "Best Before Date", 2, DataType.Numeric, 6, false);
            Add("17", "Expiry Date", 2, DataType.Numeric, 6, false);
            Add("20", "Product Model", 2, DataType.Numeric, 2, false);
            Add("21", "Serial Number", 2, DataType.Alphanumeric, 20, true);
            Add("22", "HIBCC Number", 2, DataType.Alphanumeric, 29, false);
            Add("240", "Pruduct Identification Of Producer", 3, DataType.Alphanumeric, 30, true);
            Add("241", "Customer Parts Number", 3, DataType.Alphanumeric, 30, true);
            Add("250", "Serial Number Of A Integrated Module", 3, DataType.Alphanumeric, 30, true);
            Add("251", "Reference To The BasisUnit", 3, DataType.Alphanumeric, 30, true);
            Add("252", "Global Identifier Serialised For Trade", 3, DataType.Numeric, 2, false);
            Add("30", "Amount In Parts", 2, DataType.Numeric, 8, true);
            Add("310d", "Net Weight KG", 4, DataType.Numeric, 6, false);
            Add("311d", "Length Meter", 4, DataType.Numeric, 6, false);
            Add("312d", "Width Meter", 4, DataType.Numeric, 6, false);
            Add("313d", "Heigth Meter", 4, DataType.Numeric, 6, false);
            Add("314d", "Surface SquareMeter", 4, DataType.Numeric, 6, false);
            Add("315d", "Net Volume Liters", 4, DataType.Numeric, 6, false);
            Add("316d", "Net Volume Meters³", 4, DataType.Numeric, 6, false);
            Add("320d", "Net Weight Pounds", 4, DataType.Numeric, 6, false);
            Add("321d", "Length Inches", 4, DataType.Numeric, 6, false);
            Add("322d", "Length Feet", 4, DataType.Numeric, 6, false);
            Add("323d", "Length Yards", 4, DataType.Numeric, 6, false);
            Add("324d", "Width Inches", 4, DataType.Numeric, 6, false);
            Add("325d", "Width Feed", 4, DataType.Numeric, 6, false);
            Add("326d", "Width Yards", 4, DataType.Numeric, 6, false);
            Add("327d", "Heigth Inches", 4, DataType.Numeric, 6, false);
            Add("328d", "Heigth Feed", 4, DataType.Numeric, 6, false);
            Add("329d", "Heigth Yards", 4, DataType.Numeric, 6, false);
            Add("330d", "Gross Weight KG", 4, DataType.Numeric, 6, false);
            Add("331d", "Length Meter", 4, DataType.Numeric, 6, false);
            Add("332d", "Width Meter", 4, DataType.Numeric, 6, false);
            Add("333d", "Heigth Meter", 4, DataType.Numeric, 6, false);
            Add("334d", "Surface Meter²", 4, DataType.Numeric, 6, false);
            Add("335d", "Gross Volume Liters", 4, DataType.Numeric, 6, false);
            Add("336d", "Gross Volume Meters³", 4, DataType.Numeric, 6, false);
            Add("337d", "KG Per Meter²", 4, DataType.Numeric, 6, false);
            Add("340d", "Gross Weight Pounds", 4, DataType.Numeric, 6, false);
            Add("341d", "Length Inches", 4, DataType.Numeric, 6, false);
            Add("342d", "Length Feet", 4, DataType.Numeric, 6, false);
            Add("343d", "Length Yards", 4, DataType.Numeric, 6, false);
            Add("344d", "Width Inches", 4, DataType.Numeric, 6, false);
            Add("345d", "Width Feed", 4, DataType.Numeric, 6, false);
            Add("346d", "Width Yards", 4, DataType.Numeric, 6, false);
            Add("347d", "Heigtg Inches", 4, DataType.Numeric, 6, false);
            Add("348d", "Heigth Feed", 4, DataType.Numeric, 6, false);
            Add("349d", "Heigth Yards", 4, DataType.Numeric, 6, false);
            Add("350d", "Surface Inches²", 4, DataType.Numeric, 6, false);
            Add("351d", "Surface Feet²", 4, DataType.Numeric, 6, false);
            Add("352d", "Surface Yards²", 4, DataType.Numeric, 6, false);
            Add("353d", "Surface Inches²", 4, DataType.Numeric, 6, false);
            Add("354d", "Surface Feed²", 4, DataType.Numeric, 6, false);
            Add("355d", "Surface_ Yards2", 4, DataType.Numeric, 6, false);
            Add("356d", "Net Weight Troy Ounces", 4, DataType.Numeric, 6, false);
            Add("357d", "Net Volume Ounces", 4, DataType.Numeric, 6, false);
            Add("360d", "Net Volume Quarts", 4, DataType.Numeric, 6, false);
            Add("361d", "Net Volume Gallonen", 4, DataType.Numeric, 6, false);
            Add("362d", "Gross Volume Quarts", 4, DataType.Numeric, 6, false);
            Add("363d", "Gross Volume Gallonen", 4, DataType.Numeric, 6, false);
            Add("364d", "Net Volume Inches²", 4, DataType.Numeric, 6, false);
            Add("365d", "Net Volume Feet²", 4, DataType.Numeric, 6, false);
            Add("366d", "Net Volume Yards²", 4, DataType.Numeric, 6, false);
            Add("367d", "Gross Volume Inches2", 4, DataType.Numeric, 6, false);
            Add("368d", "Gross Volume Feet²", 4, DataType.Numeric, 6, false);
            Add("369d", "Gross Volume Yards²", 4, DataType.Numeric, 6, false);
            Add("37", "Quantity In Parts", 2, DataType.Numeric, 8, true);
            Add("390d", "Amount Due Defined Valuta Band", 4, DataType.Numeric, 15, true);
            Add("391d", "Amount Due With ISO Valuta Code", 4, DataType.Numeric, 18, true);
            Add("392d", "Be Paying Amount Defined Valuta Band", 4, DataType.Numeric, 15, true);
            Add("393d", "Be Paying Amount With ISO Valuta Code", 4, DataType.Numeric, 18, true);
            Add("400", "Job Number Of Goods Recipient", 3, DataType.Alphanumeric, 30, true);
            Add("401", "Shipping Number", 3, DataType.Alphanumeric, 30, true);
            Add("402", "Delivery Number", 3, DataType.Numeric, 17, false);
            Add("403", "Routing Code", 3, DataType.Alphanumeric, 30, true);
            Add("410", "EAN-UCC-GLN Goods Recipient", 3, DataType.Numeric, 13, false);
            Add("411", "EAN-UCC-GLN InvoiceRecipient", 3, DataType.Numeric, 13, false);
            Add("412", "EAN-UCC-GLN Distributor", 3, DataType.Numeric, 13, false);
            Add("413", "EAN-UCC-GLN Final Recipient", 3, DataType.Numeric, 13, false);
            Add("414", "EAN-UCC-GLN Physical Location", 3, DataType.Numeric, 13, false);
            Add("415", "EAN-UCC-GLN To Billig Participant", 3, DataType.Numeric, 13, false);
            Add("420", "Zip Code Of Recipient Without Country Code", 3, DataType.Alphanumeric, 20, true);
            Add("421", "Zip Code Of Recipient WithCountryCode", 3, DataType.Alphanumeric, 12, true);
            Add("422", "Basis Country Of The Ware", 3, DataType.Numeric, 3, false);
            Add("7001", "Nato Stock Number", 4, DataType.Numeric, 13, false);
            Add("8001", "Roles Products", 4, DataType.Numeric, 14, false);
            Add("8002", "Serial Number For Mobile Phones", 4, DataType.Alphanumeric, 20, true);
            Add("8003", "Global Returnable Asset Identifier", 4, DataType.Alphanumeric, 34, true);
            Add("8004", "Global Individual Asset Identifier", 4, DataType.Numeric, 30, true);
            Add("8005", "Sales Price Per Unit", 4, DataType.Numeric, 6, false);
            Add("8006", "Identifikation Of A ProductComponent", 4, DataType.Numeric, 18, false);
            Add("8007", "IBAN", 4, DataType.Alphanumeric, 30, true);
            Add("8008", "Data And Time Of Manufacturing", 4, DataType.Numeric, 12, true);
            Add("8018", "Global Service Relation Number", 4, DataType.Numeric, 18, false);
            Add("8020", "Number Bill Cover Number", 4, DataType.Alphanumeric, 25, false);
            Add("8100", "Coupon Extended Code NSC Offer Code", 4, DataType.Numeric, 10, false);
            Add("8101", "Coupon Extended Code NSC Offer Code End Of Offer Code", 4, DataType.Numeric, 14, false);
            Add("8102", "Coupon Extended Code NSC", 4, DataType.Numeric, 6, false);
            Add("90", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("91", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("92", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("93", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("94", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("95", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("96", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("97", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("98", "Company specific", 2, DataType.Alphanumeric, 30, true);
            Add("99", "Company specific", 2, DataType.Alphanumeric, 30, true);
            aiis = aiiDict.Keys.ToArray();
            minLengthOfAI = aiiDict.Values.Min(el => el.LengthOfAI);
            maxLengthOfAI = aiiDict.Values.Max(el => el.LengthOfAI);
        }

        /// <summary>
        /// Add an Application Identifier (AI)
        /// </summary>
        /// <param name="AI">Number of the AI</param>
        /// <param name="Description"></param>
        /// <param name="LengthOfAI"></param>
        /// <param name="DataDescription">The type of the content</param>
        /// <param name="LengthOfData">The max lenght of the content</param>
        /// <param name="FNC1">Support a group seperator</param>
        public static void Add(string AI, string Description, int LengthOfAI, DataType DataDescription, int LengthOfData, bool FNC1)
        {
            aiiDict[AI] = new AII(AI, Description, LengthOfAI, DataDescription, LengthOfData, FNC1);
        }

        /// <summary>
        /// Parse the ean128 code
        /// </summary>
        /// <param name="data">The raw scanner data</param>
        /// <param name="throwException">If an exception will be thrown if an AI cannot be found</param>
        /// <returns>The different parts of the ean128 code</returns>
        public static Dictionary<AII, string> Parse(string data, bool throwException = false)
        {
            // cut off the EAN128 start code 
            if (data.StartsWith(EAN128StartCode)) data = data.Substring(EAN128StartCode.Length);

            // cut off the check sum
            if (HasCheckSum) data = data.Substring(0, data.Length - 2);

            Dictionary<AII, string> result = new Dictionary<AII, string>();
            int index = 0;
            // walk through the EAN128 code
            while (index < data.Length)
            {
                // try to get the AI at the current position
                var ai = GetAI(data, ref index);
                if (ai == null)
                {
                    if (throwException) throw new InvalidOperationException("AI not found");
                    return result;
                }
                // get the data to the current AI
                string code = GetCode(data, ai, ref index);
                result[ai] = code;
            }

            return result;
        }

        /// <summary>
        /// Try to get the AI at the current position
        /// </summary>
        /// <param name="data">The row data from the scanner</param>
        /// <param name="index">The refrence of the current position</param>
        /// <param name="usePlaceHolder">Sets if the last character of the AI should replaced with a placehoder ("d")</param>
        /// <returns>The current AI or null if no match was found</returns>
        private static AII GetAI(string data, ref int index, bool usePlaceHolder = false, bool trimGs1 = false)
        {
            AII result = null;
            // Step through the different lenghts of the AIs
            for (int i = minLengthOfAI; i <= maxLengthOfAI; i++)
            {
                // get the AI sub string
                string ai = data.Substring(index, i);
                if (trimGs1) ai = ai.TrimStart(BarcodeConstants.FNC1);

                // store ai without placeholder 'd'
                string originalAi = ai;
                if (usePlaceHolder) ai = ai.Remove(ai.Length - 1) + "d";

                // try to get the ai from the dictionary
                if (aiiDict.TryGetValue(ai, out result))
                {
                    // Shift the index to the next
                    index += i;
                    // restore Placeholder
                    result.AI = originalAi;
                    return result;
                }
                // if no AI found, try it with the next lenght
            }

            // if no AI found here, than try it with placeholders. Assumed that is the first sep where usePlaceHolder is false
            if (!usePlaceHolder) result = GetAI(data, ref index, true);

            // if no AI found remove potentially leading FNC1
            if(result is null && data.Contains(BarcodeConstants.FNC1.ToString())) result = GetAI(data, ref index, false, true);

            // try with placeholder and without leading FNC1
            if (result is null && data.Contains(BarcodeConstants.FNC1.ToString())) result = GetAI(data, ref index, true, true);

            return result;
        }

        /// <summary>
        /// Get the current code to the AI
        /// </summary>
        /// <param name="data">The row data from the scanner</param>
        /// <param name="ai">The current AI</param>
        /// <param name="index">The refrence of the current position</param>
        /// <returns>the data to the current AI</returns>
        private static string GetCode(string data, AII ai, ref int index)
        {
            // get the max lenght to read.
            int lenghtToRead = Math.Min(ai.LengthOfData, data.Length - index);
            // get the data of the current AI
            string result = data.Substring(index, lenghtToRead);
            // check if the AI support a group seperator
            if (ai.FNC1)
            {
                // try to find the index of the group seperator
                int indexOfGroupTermination = result.IndexOf(GroutSeperator);
                if (indexOfGroupTermination >= 0) lenghtToRead = indexOfGroupTermination + 1;

                // get the data of the current AI till the gorup seperator
                result = data.Substring(index, lenghtToRead);
            }

            // Shift the index to the next
            index += lenghtToRead;
            return result;
        }
    }
}
