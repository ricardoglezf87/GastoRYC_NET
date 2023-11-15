using GARCA.Models;
using System.Linq;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategoriesTypes.ItemsSource = iCategoriesTypesService.GetAllWithoutSpecialTransfer();
            LoadItemSource();
        }

        private void LoadItemSource()
        {
            gvCategories.ItemsSource = iCategoriesService.GetAllWithoutSpecialTransfer()?.ToList();
        }

        private void gvCategories_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var categories = (Categories)gvCategories.SelectedItem;
            if (categories != null)
            {
                switch (gvCategories.Columns[e.RowColumnIndex.ColumnIndex - 1].MappingName)
                {
                    case "categoriesTypesid":
                        categories.CategoriesTypes = iCategoriesTypesService.GetById(categories.CategoriesTypesid ?? -99);
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

            if (categories.CategoriesTypesid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("CategoriesTypesid", "Tiene que rellenar el tipo de categoría");
            }
        }

        private void gvCategories_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var categories = (Categories)e.RowData;

            if (categories.CategoriesTypes == null && categories.CategoriesTypesid != null)
            {
                categories.CategoriesTypes = iCategoriesTypesService.GetById(categories.CategoriesTypesid ?? -99);
            }

            iCategoriesService.Update(categories);
            LoadItemSource();
        }

        private void gvCategories_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Categories categories in e.Items)
            {
                iCategoriesService.Delete(categories);
            }
            LoadItemSource();
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
