using BarcodeReader.BarcodeStuff.Models;
using System.Windows.Controls;

namespace BarcodeReader.UserControls
{
    /// <summary>
    /// Interaktionslogik für AiUserControl.xaml
    /// </summary>
    public partial class AiUserControl : UserControl
    {
        public AiUserControl(ApplicationIdentifier ai)
        {
            InitializeComponent();
            
            AiLabel.Content = ai.AI;
            AiDescribtionLabel.Content = ai.Description;
            AiContentLabel.Content = ai.Value;
        }
    }
}
