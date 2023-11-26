using GARCA.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para Categories.xaml
    /// </summary>
    public partial class FrmCategoriesList : Window
    {
        public FrmCategoriesList()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategoriesTypes.ItemsSource = await iCategoriesTypesService.GetAllWithoutSpecialTransfer();
            await LoadItemSource();
        }

        private async Task LoadItemSource()
        {
            gvCategories.ItemsSource = await iCategoriesService.GetAllWithoutSpecialTransfer();
        }

        private async void gvCategories_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var categories = (Categories)gvCategories.SelectedItem;
            if (categories != null)
            {
                switch (gvCategories.Columns[e.RowColumnIndex.ColumnIndex - 1].MappingName)
                {
                    case "categoriesTypesid":
                        categories.CategoriesTypes = await iCategoriesTypesService.GetById(categories.CategoriesTypesId ?? -99);
                        break;
                }
            }
        }


        private void gvCategories_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var categories = (Categories)e.RowData;

            if (string.IsNullOrWhiteSpace(categories.Description))
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Description", "Tiene que rellenar la descripción");
            }

            if (categories.CategoriesTypesId == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("CategoriesTypesId", "Tiene que rellenar el tipo de categoría");
            }
        }

        private async void gvCategories_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var categories = (Categories)e.RowData;

            if (categories.CategoriesTypes == null && categories.CategoriesTypesId != null)
            {
                categories.CategoriesTypes = await iCategoriesTypesService.GetById(categories.CategoriesTypesId ?? -99);
            }

            await iCategoriesService.Save(categories);
            await LoadItemSource();
        }

        private async void gvCategories_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Categories categories in e.Items)
            {
                await iCategoriesService.Delete(categories);
            }
            await LoadItemSource();
        }

        private void gvCategories_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar esta categoría?", "Eliminación categoría", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvCategories.SearchHelper.AllowFiltering = true;
            gvCategories.SearchHelper.Search(txtSearch.Text);
        }
    }
}
