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
            lblProgreso.Content = "(0 / " + max + ") Elementos procesados";
        }

        public void setMax(int max, bool replace = false)
        {
            if (replace)
            {
                pbProgreso.Maximum = max;
            }
            else
            {
                pbProgreso.Maximum += max;
            }
        }

        public void PerformeStep()
        {
            pbProgreso.Value += 1;
            lblProgreso.Content = "(" + pbProgreso.Value + " / " + pbProgreso.Maximum + ") Elementos procesados";
        }

    }
}
