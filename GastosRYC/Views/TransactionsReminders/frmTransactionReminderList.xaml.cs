using BBDDLib.Models;
using BBDDLib.Services.Interfaces;
using GastosRYC.BBDDLib.Services;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para TransactionsReminders.xaml
    /// </summary>
    public partial class FrmTransactionReminderList : Window
    {

        private readonly IAccountsService accountsService;
        private readonly ICategoriesService categoriesService;
        private readonly IPersonsService personsService;
        private readonly ITagsService tagsService;
        private readonly IPeriodsRemindersService periodsRemindersService;
        private readonly ITransactionsRemindersService transactionsRemindersService;
        private readonly ITransactionsStatusService transactionsStatusService;
        private readonly ISplitsRemindersService splitsRemindersService;

        public FrmTransactionReminderList(IAccountsService accountsService, ICategoriesService categoriesService,
            IPersonsService personsService, ITagsService tagsService,
            IPeriodsRemindersService periodsRemindersService, ITransactionsRemindersService transactionsRemindersService,
            ITransactionsStatusService transactionsStatusService, ISplitsRemindersService splitsRemindersService)
        {
            this.accountsService = accountsService;
            this.categoriesService = categoriesService;
            this.personsService = personsService;
            this.tagsService = tagsService;
            this.periodsRemindersService = periodsRemindersService;
            this.transactionsRemindersService = transactionsRemindersService;
            this.transactionsStatusService = transactionsStatusService;
            InitializeComponent();
            this.splitsRemindersService = splitsRemindersService;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadTransactions();
        }

        private void gvTransactionsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (TransactionsReminders transactionsReminders in e.Items)
            {
                transactionsRemindersService.delete(transactionsReminders);
            }
        }

        private void gvTransactionsReminders_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este tag?", "Eliminación tag", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvTransactionsReminders.SearchHelper.AllowFiltering = true;
            gvTransactionsReminders.SearchHelper.Search(txtSearch.Text);
        }

        private void gvTransactionsReminders_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gvTransactionsReminders.CurrentItem != null)
            {
                FrmTransactionReminders frm = new FrmTransactionReminders((TransactionsReminders)gvTransactionsReminders.CurrentItem,accountsService,
                    categoriesService,personsService,tagsService,periodsRemindersService,transactionsRemindersService,transactionsStatusService,
                    splitsRemindersService);
                frm.ShowDialog();
                loadTransactions();
            }
        }

        private void loadTransactions()
        {
            gvTransactionsReminders.ItemsSource = transactionsRemindersService.getAll();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminders frm = new FrmTransactionReminders(accountsService,categoriesService, personsService, tagsService, 
                periodsRemindersService, transactionsRemindersService, transactionsStatusService,splitsRemindersService);
            frm.ShowDialog();
            loadTransactions();
        }
    }
}
