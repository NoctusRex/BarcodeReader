using System.Windows.Controls;
using static BarcodeReader.Misc.Fnc1Parser;

namespace BarcodeReader.UserControls
{
    /// <summary>
    /// Interaktionslogik für AiUserControl.xaml
    /// </summary>
    public partial class AiUserControl : UserControl
    {
        public AiUserControl(AII ai, string content)
        {
            InitializeComponent();
            
            AiLabel.Content = ai.AI;
            AiDescribtionLabel.Content = ai.Description;
            AiContentLabel.Content = content;
        }
    }
}
