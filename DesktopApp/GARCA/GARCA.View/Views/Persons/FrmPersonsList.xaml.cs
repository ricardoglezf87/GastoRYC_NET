using GARCA.Models;
using GARCA.Data.IOC;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            LoadItemSource();
        }
        private void LoadItemSource()
        {
            gvPersons.ItemsSource = DependencyConfigView.PersonsServiceView.GetAll()?.ToList();
        }

        private void gvPersons_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var persons = (Persons)e.RowData;

            if (string.IsNullOrWhiteSpace(persons.Name))
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Name", "Tiene que rellenar el nombre");
            }

        }

        private void gvPersons_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var persons = (Persons)e.RowData;
            persons = DependencyConfigView.PersonsServiceView.Update(persons);
            LoadItemSource();
        }

        private void gvPersons_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Persons persons in e.Items)
            {
                DependencyConfigView.PersonsServiceView.Delete(persons);
            }
            LoadItemSource();
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
