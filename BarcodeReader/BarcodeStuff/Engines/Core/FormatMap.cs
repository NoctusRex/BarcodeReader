using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeReader.BarcodeStuff.Engines.Core
{
    public class FormatMap<T> where T : Enum
    {
        private Dictionary<BarcodeFormat, T> MappenFormats { get; set; }

        public T DefaultFormat { get; private set; }

        public FormatMap(T defaultFormat)
        {
            DefaultFormat = defaultFormat;
        }

        public void Map(BarcodeFormat format1, T format2)
        {
            if (MappenFormats is null) MappenFormats = new Dictionary<BarcodeFormat, T>();

            if (MappenFormats.ContainsKey(format1)) throw new Exception($"{format1} is already mapped to {MappenFormats[format1]} and can not be mapped to {format2}.");
            if (MappenFormats.ContainsValue(format2)) throw new Exception($"{format2} is already mapped to {Resolve(format2)} and can not be mapped to {format1}.");

            MappenFormats.Add(format1, format2);
        }

        public BarcodeFormat Resolve(T format)
        {
            try
            {
                return MappenFormats.Single(x => x.Value.Equals(format)).Key;
            }
            catch (Exception)
            {
                try
                {
                    return MappenFormats.Single(x => x.Value.Equals(DefaultFormat)).Key;
                }
                catch (Exception)
                {
                    return BarcodeFormat.CODE_128;
                }
            }
        }

        public T Resolve(BarcodeFormat format)
        {
            try
            {
                return MappenFormats[format];
            }
            catch(Exception)
            {
                return DefaultFormat;
            }
        }
    }
}
