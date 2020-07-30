﻿using BarcodeReader.BarcodeStuff;
using BarcodeReader.BarcodeStuff.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BarcodeReader
{
    /// <summary>
    /// Interaktionslogik für BarcodeHistoryUserControl.xaml
    /// </summary>
    public partial class BarcodeHistoryUserControl : UserControl
    {
        public Barcode Barcode { get; set; }
        public event EventHandler Deleted;

        public BarcodeHistoryUserControl()
        {
            InitializeComponent();
        }

        public BarcodeHistoryUserControl(Barcode barcode, DateTime timeStamp, string formattedFnc1Barcode = "")
        {
            InitializeComponent();

            Barcode = barcode;

            if (string.IsNullOrEmpty(formattedFnc1Barcode))
                ContentLabel.Content = barcode.Text.Replace(BarcodeConstants.FNC1.ToString(), BarcodeConstants.FNC1_DisplayPlaceholder);
            else
                ContentLabel.Content = formattedFnc1Barcode.Replace(BarcodeConstants.FNC1.ToString(), BarcodeConstants.FNC1_DisplayPlaceholder);

            InfoLabel.Content = string.Format("Barcode Type: {0} - Scannend at {1}", barcode.Format, timeStamp);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Deleted(this, e);
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            new Windows.BarcodeWindow(Barcode).Show();
        }
    }
}
