using GARCA.BO.Models;
using GARCA.BO.Services;
using System.Windows;
using System.Windows.Controls;
using GARCA.Utils.IOC;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para Persons.xaml
    /// </summary>
    public partial class FrmPersonsList : Window
    {
        public FrmPersonsList()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gvPersons.ItemsSource = DependencyConfig.IPersonsService.GetAll();
        }

        private void gvPersons_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var persons = (Persons)e.RowData;

            if (persons.Name == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("name", "Tiene que rellenar el nombre");
            }

        }

        private void gvPersons_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var persons = (Persons)e.RowData;
            DependencyConfig.IPersonsService.Update(persons);
        }

        private void gvPersons_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Persons persons in e.Items)
            {
                DependencyConfig.IPersonsService.Delete(persons);
            }
        }

        private void gvPersons_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar esta persona?", "Eliminación persona", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvPersons.SearchHelper.AllowFiltering = true;
            gvPersons.SearchHelper.Search(txtSearch.Text);
        }
    }
}
