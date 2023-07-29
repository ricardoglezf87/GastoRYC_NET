using System.Windows;

namespace GARCA.View.Views.Common
{
    /// <summary>
    /// Lógica de interacción para LoadDialog.xaml
    /// </summary>
    public partial class LoadDialog : Window
    {
        public LoadDialog(int max)
        {
            InitializeComponent();
            pbProgreso.Value = 0;
            pbProgreso.Maximum = max;
            lblProgreso.Content = "(0 / " + max.ToString() + ") Elementos procesados";
        }

        public void performeStep()
        {
            pbProgreso.Value += 1;
            lblProgreso.Content = "(" + pbProgreso.Value.ToString() + " / " + pbProgreso.Maximum.ToString() + ") Elementos procesados";
        }

    }
}
