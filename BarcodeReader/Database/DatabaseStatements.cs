using BarcodeReader.BarcodeStuff.Engines.Core;
using BarcodeReader.Misc;
using NoRe.Database.Core.Models;
using NoRe.Database.SqLite;
using System;
using System.Collections.Generic;

namespace BarcodeReader.Database
{
    internal class DatabaseStatements: IDisposable
    {
        private SqLiteWrapper Database { get; set; }

        public DatabaseStatements() => Database = new SqLiteWrapper(PathManager.DatabasePath, "3");

        public void CreateTable() => Database.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS history (id INTEGER PRIMARY KEY AUTOINCREMENT, value TEXT, type TEXT, date TEXT)");

        public List<Row> GetHistory() => Database.ExecuteReader("SELECT * FROM history ORDER BY id DESC").Rows;

        public void InsertBarcode(string barcodeText, BarcodeFormat barcodeFormat) => Database.ExecuteNonQuery("INSERT INTO history (value, type, date) VALUES (@0, @1, @2)", barcodeText, barcodeFormat, DateTime.Now.ToString());

        public void DeleteHistory(string barcodeText, BarcodeFormat barcodeFormat) => Database.ExecuteNonQuery("DELETE FROM history WHERE value=@0 AND type=@1", barcodeText, barcodeFormat);

        public void Dispose() => Database?.Dispose();
    }
}
