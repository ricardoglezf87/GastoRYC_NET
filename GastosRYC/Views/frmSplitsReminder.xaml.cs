﻿using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para SplitsReminders.xaml
    /// </summary>
    public partial class frmSplitsReminders : Window
    {

        private readonly CategoriesService categoriesService = new CategoriesService();
        private readonly SplitsRemindersService splitsRemindersService = new SplitsRemindersService();
        private readonly TransactionsRemindersService transactionsRemindersService = new TransactionsRemindersService();
        private readonly TransactionsReminders? transactionsReminders;

        public frmSplitsReminders(TransactionsReminders? transactionsReminders)
        {
            InitializeComponent();
            this.transactionsReminders = transactionsReminders;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = categoriesService.getAll();
            if (transactionsReminders != null && transactionsReminders.id > 0)
            {
                gvSplitsReminders.ItemsSource = splitsRemindersService.getbyTransactionid(transactionsReminders.id);
            }
            else
            {
                gvSplitsReminders.ItemsSource = splitsRemindersService.getbyTransactionidNull();
            }
        }

        private void gvSplitsReminders_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)gvSplitsReminders.SelectedItem;
            if (splitsReminders != null)
            {
                switch (gvSplitsReminders.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "categoryid":
                        splitsReminders.category = categoriesService.getByID(splitsReminders.categoryid);
                        break;                    
                }
            }
        }

       
        private void gvSplitsReminders_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)e.RowData;
            
            if (splitsReminders.categoryid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "Tiene que rellenar el tipo de categoría");
            }
            else if(splitsReminders.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "No se puede utilizar esta categoría en un split");
            }

            if (splitsReminders.amountIn == null && splitsReminders.amountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("amountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("amountOut", "Tiene que rellenar la cantidad");
            }
        }                
        
        private void gvSplitsReminders_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)e.RowData;

            saveChanges(splitsReminders);
        }

        private void saveChanges(SplitsReminders splitsReminders)
        {
            if (splitsReminders.category == null && splitsReminders.categoryid != null)
            {
                splitsReminders.category = categoriesService.getByID(splitsReminders.categoryid);
            }

            if (splitsReminders.amountIn == null)
                splitsReminders.amountIn = 0;

            if (splitsReminders.amountOut == null)
                splitsReminders.amountOut = 0;

            updateTranfer(splitsReminders);

            splitsRemindersService.update(splitsReminders);
        }

        private void updateTranfer(SplitsReminders splitsReminders)
        {
            if (splitsReminders.tranferid != null && 
                splitsReminders.category.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                TransactionsReminders? tContraria = transactionsRemindersService.getByID(splitsReminders.tranferid);
                if (tContraria != null)
                {
                    transactionsRemindersService.delete(tContraria);
                }
                splitsReminders.tranferid = null;
            }
            else if (splitsReminders.tranferid == null && 
                splitsReminders.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                splitsReminders.tranferid = transactionsRemindersService.getNextID();

                TransactionsReminders? tContraria = new TransactionsReminders();
                tContraria.date = transactionsReminders.date;
                tContraria.accountid = splitsReminders.category.accounts.id;
                tContraria.personid = transactionsReminders.personid;
                tContraria.categoryid = transactionsReminders.account.categoryid;
                tContraria.memo = splitsReminders.memo;
                tContraria.tagid = transactionsReminders.tagid;
                tContraria.amountIn = splitsReminders.amountOut;
                tContraria.amountOut = splitsReminders.amountIn;

                if (splitsReminders.id != 0)
                    tContraria.tranferSplitid = splitsReminders.id;
                else
                    tContraria.tranferSplitid = splitsRemindersService.getNextID() + 1;

                tContraria.transactionStatusid = transactionsReminders.transactionStatusid;

                transactionsRemindersService.update(tContraria);

            }
            else if (splitsReminders.tranferid != null && 
                splitsReminders.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                TransactionsReminders? tContraria = transactionsRemindersService.getByID(splitsReminders.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactionsReminders.date;
                    tContraria.accountid = splitsReminders.category.accounts.id;
                    tContraria.personid = transactionsReminders.personid;
                    tContraria.categoryid = transactionsReminders.account.categoryid;
                    tContraria.memo = splitsReminders.memo;
                    tContraria.tagid = transactionsReminders.tagid;
                    tContraria.amountIn = splitsReminders.amountOut??0;
                    tContraria.amountOut = splitsReminders.amountIn??0;
                    tContraria.transactionStatusid = transactionsReminders.transactionStatusid;
                    transactionsRemindersService.update(tContraria);
                }
            }
        }


        private void gvSplitsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (SplitsReminders splitsReminders in e.Items) {
                if (splitsReminders.tranferid != null)
                {
                    transactionsRemindersService.delete(transactionsRemindersService.getByID(splitsReminders.tranferid));
                }
                splitsRemindersService.delete(splitsReminders);
            }            
        }

        private void gvSplitsReminders_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if(MessageBox.Show("Esta seguro de querer eliminar esta split?","Eliminación split",MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void gvSplitsReminders_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)e.NewObject;
            splitsReminders.transactionid = transactionsReminders.id;
            splitsReminders.transaction = transactionsReminders;
        }
    }
}