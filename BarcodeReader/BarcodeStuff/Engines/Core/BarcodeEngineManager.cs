using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BarcodeReader.BarcodeStuff.Engines.Core
{
    internal class BarcodeEngineManager
    {
        public static IBarcodeEngine CurrentBarcodeEngine { get; internal set; }
        private static IEnumerable<IBarcodeEngine> LoadedEngines { get; set; }

        public static IEnumerable<IBarcodeEngine> GetAllBarcodeEngines()
        {
            if (LoadedEngines is null) LoadedEngines = LoadBarcodeEngines();

            return LoadedEngines;
        }

        private static IEnumerable<IBarcodeEngine> LoadBarcodeEngines()
        {
            List<IBarcodeEngine> engines = new List<IBarcodeEngine>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!typeof(IBarcodeEngine).IsAssignableFrom(type)) continue;
                if (typeof(IBarcodeEngine) == type) continue;
                IBarcodeEngine engine = (IBarcodeEngine)Activator.CreateInstance(type);

                if (engines.Any(x => x.Identifier == engine.Identifier)) continue;
                engines.Add(engine);
            }

            return engines;
        }

        internal static void SetBarcodeEngine(string barcodeEngineIdentifier)
        {
            CurrentBarcodeEngine = GetAllBarcodeEngines().Single(x => x.Identifier == barcodeEngineIdentifier);
        }
    }
}
