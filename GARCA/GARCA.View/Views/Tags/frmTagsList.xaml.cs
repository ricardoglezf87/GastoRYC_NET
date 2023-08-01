using GARCA.BO.Models;
using GARCA.Utils.IOC;
using Syncfusion.Windows.Shared;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GARCA.View.Views
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
            gvTags.ItemsSource = DependencyConfig.TagsService.GetAll()?.ToList();
        }

        private void gvTags_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var tags = (Tags)e.RowData;

            if (string.IsNullOrWhiteSpace(tags.Description))
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Descrìption", "Tiene que rellenar la descripción");
            }

        }

        private void gvTags_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var tags = (Tags)e.RowData;
            DependencyConfig.TagsService.Update(tags);
        }

        private void gvTags_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Tags tags in e.Items)
            {
                DependencyConfig.TagsService.Delete(tags);
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
