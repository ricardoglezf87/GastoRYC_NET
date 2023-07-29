using GARCA.BO.Models;
using GARCA.BO.Services;
using System.Windows;
using System.Windows.Controls;
using GARCA.IOC;

namespace GARCA.Views
{
    /// <summary>
    /// Lógica de interacción para Tags.xaml
    /// </summary>
    public partial class FrmTagsList : Window
    {
        public FrmTagsList()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gvTags.ItemsSource = DependencyConfig.iTagsService.getAll();
        }

        private void gvTags_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            Tags tags = (Tags)e.RowData;

            if (tags.description == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("descrìption", "Tiene que rellenar la descripción");
            }

        }

        private void gvTags_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            Tags tags = (Tags)e.RowData;
            DependencyConfig.iTagsService.update(tags);
        }

        private void gvTags_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Tags tags in e.Items)
            {
                DependencyConfig.iTagsService.delete(tags);
            }
        }

        private void gvTags_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este tag?", "Eliminación tag", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvTags.SearchHelper.AllowFiltering = true;
            gvTags.SearchHelper.Search(txtSearch.Text);
        }
    }
}
