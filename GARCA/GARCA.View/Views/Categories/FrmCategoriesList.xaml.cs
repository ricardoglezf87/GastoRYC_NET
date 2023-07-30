using GARCA.BO.Models;
using GARCA.Utils.IOC;
using System.Windows;
using System.Windows.Controls;

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
            cbCategoriesTypes.ItemsSource = DependencyConfig.ICategoriesTypesService.GetAllWithoutSpecialTransfer();
            gvCategories.ItemsSource = DependencyConfig.ICategoriesService.GetAllWithoutSpecialTransfer();
        }

        private void gvCategories_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var categories = (Categories)gvCategories.SelectedItem;
            if (categories != null)
            {
                switch (gvCategories.Columns[e.RowColumnIndex.ColumnIndex - 1].MappingName)
                {
                    case "categoriesTypesid":
                        categories.CategoriesTypes = DependencyConfig.ICategoriesTypesService.GetById(categories.CategoriesTypesid);
                        break;
                }
            }
        }


        private void gvCategories_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var categories = (Categories)e.RowData;

            if (categories.Description == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("description", "Tiene que rellenar la descripción");
            }

            if (categories.CategoriesTypesid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoriesTypesid", "Tiene que rellenar el tipo de categoría");
            }
        }

        private void gvCategories_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var categories = (Categories)e.RowData;

            if (categories.CategoriesTypes == null && categories.CategoriesTypesid != null)
            {
                categories.CategoriesTypes = DependencyConfig.ICategoriesTypesService.GetById(categories.CategoriesTypesid);
            }

            DependencyConfig.ICategoriesService.Update(categories);
        }

        private void gvCategories_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Categories categories in e.Items)
            {
                DependencyConfig.ICategoriesService.Delete(categories);
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
