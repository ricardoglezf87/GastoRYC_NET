using GARCA.BO.Models;
using GARCA.DAO.Models;
using GARCA.View.ViewModels;
using System.Collections.Generic;

namespace GARCA.Utlis.Extensions
{
    public static class EnumerableExtension
    {
        public static SortedSet<Transactions> ToSortedSetBo(this IEnumerable<TransactionsDAO>? source)
        {
            SortedSet<Transactions> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    Transactions? t = (Transactions?)obj;
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        public static HashSet<PeriodsReminders?> ToHashSetBo(this IEnumerable<PeriodsRemindersDAO> source)
        {
            HashSet<PeriodsReminders?> list = new();

            foreach (var obj in source)
            {
                list.Add((PeriodsReminders?)obj);
            }

            return list;
        }

        public static HashSet<ExpirationsReminders?> ToHashSetBo(this IEnumerable<ExpirationsRemindersDAO> source)
        {
            HashSet<ExpirationsReminders?> list = new();

            foreach (var obj in source)
            {
                list.Add((ExpirationsReminders?)obj);
            }

            return list;
        }

        public static HashSet<VBalancebyCategory?> ToHashSetBo(this IEnumerable<VBalancebyCategoryDAO> source)
        {
            HashSet<VBalancebyCategory?> list = new();

            foreach (var obj in source)
            {
                list.Add((VBalancebyCategory?)obj);
            }

            return list;
        }

        public static HashSet<SplitsReminders?> ToHashSetBo(this IEnumerable<SplitsRemindersDAO> source)
        {
            HashSet<SplitsReminders?> list = new();

            foreach (var obj in source)
            {
                list.Add((SplitsReminders?)obj);
            }

            return list;
        }

        public static HashSet<Splits?> ToHashSetBo(this IEnumerable<SplitsDAO> source)
        {
            HashSet<Splits?> list = new();

            foreach (var obj in source)
            {
                list.Add((Splits?)obj);
            }

            return list;
        }

        public static HashSet<TransactionsReminders?> ToHashSetBo(this IEnumerable<TransactionsRemindersDAO> source)
        {
            HashSet<TransactionsReminders?> list = new();

            foreach (var obj in source)
            {
                list.Add((TransactionsReminders?)obj);
            }

            return list;
        }

        public static HashSet<Transactions?> ToHashSetBo(this IEnumerable<TransactionsDAO> source)
        {
            HashSet<Transactions?> list = new();

            foreach (var obj in source)
            {
                list.Add((Transactions?)obj);
            }

            return list;
        }

        public static HashSet<InvestmentProducts?> ToHashSetBo(this IEnumerable<InvestmentProductsDAO> source)
        {
            HashSet<InvestmentProducts?> list = new();

            foreach (var obj in source)
            {
                list.Add((InvestmentProducts?)obj);
            }

            return list;
        }

        public static HashSet<InvestmentProductsTypes?> ToHashSetBo(this IEnumerable<InvestmentProductsTypesDAO> source)
        {
            HashSet<InvestmentProductsTypes?> list = new();

            foreach (var obj in source)
            {
                list.Add((InvestmentProductsTypes?)obj);
            }

            return list;
        }

        public static HashSet<Categories?> ToHashSetBo(this IEnumerable<CategoriesDAO> source)
        {
            HashSet<Categories?> list = new();

            foreach (var obj in source)
            {
                list.Add((Categories?)obj);
            }

            return list;
        }

        public static HashSet<CategoriesTypes?> ToHashSetBo(this IEnumerable<CategoriesTypesDAO> source)
        {
            HashSet<CategoriesTypes?> list = new();

            foreach (var obj in source)
            {
                list.Add((CategoriesTypes?)obj);
            }

            return list;
        }

        public static HashSet<Persons?> ToHashSetBo(this IEnumerable<PersonsDAO> source)
        {
            HashSet<Persons?> list = new();

            foreach (var obj in source)
            {
                list.Add((Persons?)obj);
            }

            return list;
        }

        public static HashSet<Tags?> ToHashSetBo(this IEnumerable<TagsDAO> source)
        {
            HashSet<Tags?> list = new();

            foreach (var obj in source)
            {
                list.Add((Tags?)obj);
            }

            return list;
        }

        public static HashSet<TransactionsStatus?> ToHashSetBo(this IEnumerable<TransactionsStatusDAO> source)
        {
            HashSet<TransactionsStatus?> list = new();

            foreach (var obj in source)
            {
                list.Add((TransactionsStatus?)obj);
            }

            return list;
        }

        public static HashSet<Accounts?> ToHashSetBo(this IEnumerable<AccountsDAO> source)
        {
            HashSet<Accounts?> list = new();

            foreach (var obj in source)
            {
                list.Add((Accounts?)obj);
            }

            return list;
        }

        public static HashSet<AccountsView> ToHashSetViewBo(this IEnumerable<AccountsDAO> source)
        {
            HashSet<AccountsView> list = new();

            foreach (var obj in source)
            {
                list.Add((AccountsView)obj);
            }

            return list;
        }

        public static HashSet<AccountsTypes?> ToHashSetBo(this IEnumerable<AccountsTypesDAO> source)
        {
            HashSet<AccountsTypes?> list = new();

            foreach (var obj in source)
            {
                list.Add((AccountsTypes?)obj);
            }

            return list;
        }
    }
}
