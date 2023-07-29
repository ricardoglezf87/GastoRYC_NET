using GARCA.BO.Models;
using GARCA.BO.Services;
using System.Windows;
using System.Windows.Controls;
using GARCA.IOC;

namespace GARCA.Views
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
            cbCategoriesTypes.ItemsSource = DependencyConfig.iCategoriesTypesService.getAllWithoutSpecialTransfer();
            gvCategories.ItemsSource = DependencyConfig.iCategoriesService.getAllWithoutSpecialTransfer();
        }

        private void gvCategories_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            Categories categories = (Categories)gvCategories.SelectedItem;
            if (categories != null)
            {
                switch (gvCategories.Columns[e.RowColumnIndex.ColumnIndex - 1].MappingName)
                {
                    case "categoriesTypesid":
                        categories.categoriesTypes = DependencyConfig.iCategoriesTypesService.getByID(categories.categoriesTypesid);
                        break;
                }
            }
        }


        private void gvCategories_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            Categories categories = (Categories)e.RowData;

            if (categories.description == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("description", "Tiene que rellenar la descripción");
            }

            if (categories.categoriesTypesid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoriesTypesid", "Tiene que rellenar el tipo de categoría");
            }
        }

        private void gvCategories_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            Categories categories = (Categories)e.RowData;

            if (categories.categoriesTypes == null && categories.categoriesTypesid != null)
            {
                categories.categoriesTypes = DependencyConfig.iCategoriesTypesService.getByID(categories.categoriesTypesid);
            }

            DependencyConfig.iCategoriesService.update(categories);
        }

        private void gvCategories_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Categories categories in e.Items)
            {
                DependencyConfig.iCategoriesService.delete(categories);
            }
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
