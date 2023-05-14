using BOLib.Services;
using BOLib.Models;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para Persons.xaml
    /// </summary>
    public partial class FrmPersonsList : Window
    {
        private readonly PersonsService personsService;
   
        public FrmPersonsList()
        {
            InitializeComponent();
            personsService = InstanceBase<PersonsService>.Instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gvPersons.ItemsSource = personsService.getAll();
        }

        private void gvPersons_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            Persons persons = (Persons)e.RowData;

            if (persons.name == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("name", "Tiene que rellenar el nombre");
            }

        }

        private void gvPersons_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            Persons persons = (Persons)e.RowData;
            personsService.update(persons);
        }

        private void gvPersons_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Persons persons in e.Items)
            {
                personsService.delete(persons);
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
