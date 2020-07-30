using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeReader.BarcodeStuff.Engines.Core
{
    public class FormatMap<T> where T : Enum
    {
        private Dictionary<BarcodeFormat, T> MappenFormats { get; set; }

        public void Map(BarcodeFormat format1, T format2)
        {
            if (MappenFormats is null) MappenFormats = new Dictionary<BarcodeFormat, T>();

            if (MappenFormats.ContainsKey(format1)) throw new Exception($"{format1} is already mapped to {MappenFormats[format1]} and can not be mapped to {format2}.");
            if (MappenFormats.ContainsValue(format2)) throw new Exception($"{format2} is already mapped to {Resolve(format2)} and can not be mapped to {format1}.");

            MappenFormats.Add(format1, format2);
        }

        public BarcodeFormat Resolve(T format) => MappenFormats.FirstOrDefault(x => x.Value.Equals(format)).Key;

        public T Resolve(BarcodeFormat format) => MappenFormats[format];
    }
}
