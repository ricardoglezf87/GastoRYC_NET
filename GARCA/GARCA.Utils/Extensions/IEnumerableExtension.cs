using GARCA.BO.Models;
using GARCA.View.ViewModels;
using GARCA.DAO.Models;
using System.Collections.Generic;

namespace GARCA.Utlis.Extensions
{
    public static class EnumerableExtension
    {
        public static HashSet<TransactionsDAO?>? ToHashSetDao(this IEnumerable<Transactions?>? source)
        {
            HashSet<TransactionsDAO?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add(obj.ToDao());
                }
            }
            return list;
        }

        public static HashSet<DateCalendar?> ToHashSetBo(this IEnumerable<DateCalendarDAO> source)
        {
            HashSet<DateCalendar?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((DateCalendar?)obj);
                }
            }
            return list;
        }

        public static SortedSet<Transactions?> ToSortedSetBo(this IEnumerable<TransactionsDAO>? source)
        {
            SortedSet<Transactions?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((Transactions?)obj);
                }
            }
            return list;
        }

        public static HashSet<PeriodsReminders?> ToHashSetBo(this IEnumerable<PeriodsRemindersDAO> source)
        {
            HashSet<PeriodsReminders?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((PeriodsReminders?)obj);
                }
            }
            return list;
        }

        public static HashSet<ExpirationsReminders?> ToHashSetBo(this IEnumerable<ExpirationsRemindersDAO> source)
        {
            HashSet<ExpirationsReminders?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((ExpirationsReminders?)obj);
                }
            }
            return list;
        }

        public static HashSet<VBalancebyCategory?> ToHashSetBo(this IEnumerable<VBalancebyCategoryDAO> source)
        {
            HashSet<VBalancebyCategory?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((VBalancebyCategory?)obj);
                }
            }
            return list;
        }

        public static HashSet<SplitsReminders?> ToHashSetBo(this IEnumerable<SplitsRemindersDAO> source)
        {
            HashSet<SplitsReminders?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((SplitsReminders?)obj);
                }
            }
            return list;
        }

        public static HashSet<Splits?> ToHashSetBo(this IEnumerable<SplitsDAO> source)
        {
            HashSet<Splits?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((Splits?)obj);
                }
            }
            return list;
        }

        public static HashSet<TransactionsReminders?> ToHashSetBo(this IEnumerable<TransactionsRemindersDAO> source)
        {
            HashSet<TransactionsReminders?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((TransactionsReminders?)obj);
                }
            }
            return list;
        }

        public static HashSet<Transactions?> ToHashSetBo(this IEnumerable<TransactionsDAO> source)
        {
            HashSet<Transactions?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((Transactions?)obj);
                }
            }
            return list;
        }

        public static HashSet<InvestmentProductsPrices?> ToHashSetBo(this IEnumerable<InvestmentProductsPricesDAO> source)
        {
            HashSet<InvestmentProductsPrices?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((InvestmentProductsPrices?)obj);
                }
            }
            return list;
        }

        public static HashSet<InvestmentProducts?> ToHashSetBo(this IEnumerable<InvestmentProductsDAO> source)
        {
            HashSet<InvestmentProducts?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((InvestmentProducts?)obj);
                }
            }
            return list;
        }

        public static HashSet<InvestmentProductsTypes?> ToHashSetBo(this IEnumerable<InvestmentProductsTypesDAO> source)
        {
            HashSet<InvestmentProductsTypes?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((InvestmentProductsTypes?)obj);
                }
            }
            return list;
        }

        public static HashSet<Categories?> ToHashSetBo(this IEnumerable<CategoriesDAO> source)
        {
            HashSet<Categories?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((Categories?)obj);
                }
            }
            return list;
        }

        public static HashSet<CategoriesTypes?> ToHashSetBo(this IEnumerable<CategoriesTypesDAO> source)
        {
            HashSet<CategoriesTypes?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((CategoriesTypes?)obj);
                }
            }
            return list;
        }

        public static HashSet<Persons?> ToHashSetBo(this IEnumerable<PersonsDAO> source)
        {
            HashSet<Persons?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((Persons?)obj);
                }
            }
            return list;
        }

        public static HashSet<Tags?> ToHashSetBo(this IEnumerable<TagsDAO> source)
        {
            HashSet<Tags?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((Tags?)obj);
                }
            }
            return list;
        }

        public static HashSet<TransactionsStatus?> ToHashSetBo(this IEnumerable<TransactionsStatusDAO> source)
        {
            HashSet<TransactionsStatus?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((TransactionsStatus?)obj);
                }
            }
            return list;
        }

        public static HashSet<Accounts?> ToHashSetBo(this IEnumerable<AccountsDAO> source)
        {
            HashSet<Accounts?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((Accounts?)obj);
                }
            }
            return list;
        }

        public static HashSet<AccountsView> ToHashSetViewBo(this IEnumerable<AccountsDAO> source)
        {
            HashSet<AccountsView> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((AccountsView)obj);
                }
            }
            return list;
        }

        public static HashSet<AccountsTypes?> ToHashSetBo(this IEnumerable<AccountsTypesDAO> source)
        {
            HashSet<AccountsTypes?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add((AccountsTypes?)obj);
                }
            }
            return list;
        }
    }
}
