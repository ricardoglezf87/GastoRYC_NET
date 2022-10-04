using GastosRYCLib.Manager;
using GastosRYCLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GastosRYC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private RYCContext? rycContext;

        public MainWindow()
        {
            InitializeComponent();
            loadContext();
        }

        private void loadContext()
        {
            rycContext = new RYCContext();
            rycContext.categories?.Load();
            rycContext.persons?.Load();
            rycContext.accountsTypes?.Load();
            rycContext.accounts?.Load();
            rycContext.transactions?.Load();
        }

        private void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            lvCuentas.ItemsSource = rycContext?.accounts?.ToList();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvCuentas.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("accountsTypes");
            view.GroupDescriptions.Add(groupDescription);

            cbAccounts.ItemsSource = rycContext?.accounts?.ToList();
            cbPersons.ItemsSource = rycContext?.persons?.ToList();
            cbCategories.ItemsSource = rycContext?.categories?.ToList();

             ICollectionView viewTransaction;
             viewTransaction = CollectionViewSource.GetDefaultView(rycContext?.transactions?.ToList());
             gvMovimientos.ItemsSource = viewTransaction;

        }

        public void ApplyFilters()
        {
            ICollectionView view = (ICollectionView) gvMovimientos.ItemsSource;
            if (view != null)
            {
                view.Filter = accountFilter;
                gvMovimientos.ItemsSource = view;
            }
        }


        public bool accountFilter(object o)
        {
            Transactions p = (o as Transactions);
            if (p == null)
                return false;
            else
                if (p.account?.id == ((Accounts)lvCuentas.SelectedValue)?.id)
                    return true;
                else
                    return false;
        }

        private void lvCuentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
    }
}
