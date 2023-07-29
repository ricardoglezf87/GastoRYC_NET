using GARCA.BO.Models;
using GARCA.View.ViewModels;
using GARCA.DAO.Models;
using System.Collections.Generic;

namespace GARCA.Utlis.Extensions
{
    public static class IEnumerableExtension
    {
        public static HashSet<TransactionsDAO?>? toHashSetDAO(this IEnumerable<Transactions?>? source)
        {
            HashSet<TransactionsDAO?> list = new();
            if (source != null)
            {
                foreach (Transactions? obj in source)
                {
                    list.Add(obj.toDAO());
                }
            }
            return list;
        }

        public static HashSet<DateCalendar?> toHashSetBO(this IEnumerable<DateCalendarDAO> source)
        {
            HashSet<DateCalendar?> list = new();
            if (source != null)
            {
                foreach (DateCalendarDAO obj in source)
                {
                    list.Add((DateCalendar?)obj);
                }
            }
            return list;
        }

        public static SortedSet<Transactions?> toSortedSetBO(this IEnumerable<TransactionsDAO>? source)
        {
            SortedSet<Transactions?> list = new();
            if (source != null)
            {
                foreach (TransactionsDAO obj in source)
                {
                    list.Add((Transactions?)obj);
                }
            }
            return list;
        }

        public static HashSet<PeriodsReminders?> toHashSetBO(this IEnumerable<PeriodsRemindersDAO> source)
        {
            HashSet<PeriodsReminders?> list = new();
            if (source != null)
            {
                foreach (PeriodsRemindersDAO obj in source)
                {
                    list.Add((PeriodsReminders?)obj);
                }
            }
            return list;
        }

        public static HashSet<ExpirationsReminders?> toHashSetBO(this IEnumerable<ExpirationsRemindersDAO> source)
        {
            HashSet<ExpirationsReminders?> list = new();
            if (source != null)
            {
                foreach (ExpirationsRemindersDAO obj in source)
                {
                    list.Add((ExpirationsReminders?)obj);
                }
            }
            return list;
        }

        public static HashSet<VBalancebyCategory?> toHashSetBO(this IEnumerable<VBalancebyCategoryDAO> source)
        {
            HashSet<VBalancebyCategory?> list = new();
            if (source != null)
            {
                foreach (VBalancebyCategoryDAO obj in source)
                {
                    list.Add((VBalancebyCategory?)obj);
                }
            }
            return list;
        }

        public static HashSet<SplitsReminders?> toHashSetBO(this IEnumerable<SplitsRemindersDAO> source)
        {
            HashSet<SplitsReminders?> list = new();
            if (source != null)
            {
                foreach (SplitsRemindersDAO obj in source)
                {
                    list.Add((SplitsReminders?)obj);
                }
            }
            return list;
        }

        public static HashSet<Splits?> toHashSetBO(this IEnumerable<SplitsDAO> source)
        {
            HashSet<Splits?> list = new();
            if (source != null)
            {
                foreach (SplitsDAO obj in source)
                {
                    list.Add((Splits?)obj);
                }
            }
            return list;
        }

        public static HashSet<TransactionsReminders?> toHashSetBO(this IEnumerable<TransactionsRemindersDAO> source)
        {
            HashSet<TransactionsReminders?> list = new();
            if (source != null)
            {
                foreach (TransactionsRemindersDAO obj in source)
                {
                    list.Add((TransactionsReminders?)obj);
                }
            }
            return list;
        }

        public static HashSet<Transactions?> toHashSetBO(this IEnumerable<TransactionsDAO> source)
        {
            HashSet<Transactions?> list = new();
            if (source != null)
            {
                foreach (TransactionsDAO obj in source)
                {
                    list.Add((Transactions?)obj);
                }
            }
            return list;
        }

        public static HashSet<InvestmentProductsPrices?> toHashSetBO(this IEnumerable<InvestmentProductsPricesDAO> source)
        {
            HashSet<InvestmentProductsPrices?> list = new();
            if (source != null)
            {
                foreach (InvestmentProductsPricesDAO obj in source)
                {
                    list.Add((InvestmentProductsPrices?)obj);
                }
            }
            return list;
        }

        public static HashSet<InvestmentProducts?> toHashSetBO(this IEnumerable<InvestmentProductsDAO> source)
        {
            HashSet<InvestmentProducts?> list = new();
            if (source != null)
            {
                foreach (InvestmentProductsDAO obj in source)
                {
                    list.Add((InvestmentProducts?)obj);
                }
            }
            return list;
        }

        public static HashSet<InvestmentProductsTypes?> toHashSetBO(this IEnumerable<InvestmentProductsTypesDAO> source)
        {
            HashSet<InvestmentProductsTypes?> list = new();
            if (source != null)
            {
                foreach (InvestmentProductsTypesDAO obj in source)
                {
                    list.Add((InvestmentProductsTypes?)obj);
                }
            }
            return list;
        }

        public static HashSet<Categories?> toHashSetBO(this IEnumerable<CategoriesDAO> source)
        {
            HashSet<Categories?> list = new();
            if (source != null)
            {
                foreach (CategoriesDAO obj in source)
                {
                    list.Add((Categories?)obj);
                }
            }
            return list;
        }

        public static HashSet<CategoriesTypes?> toHashSetBO(this IEnumerable<CategoriesTypesDAO> source)
        {
            HashSet<CategoriesTypes?> list = new();
            if (source != null)
            {
                foreach (CategoriesTypesDAO obj in source)
                {
                    list.Add((CategoriesTypes?)obj);
                }
            }
            return list;
        }

        public static HashSet<Persons?> toHashSetBO(this IEnumerable<PersonsDAO> source)
        {
            HashSet<Persons?> list = new();
            if (source != null)
            {
                foreach (PersonsDAO obj in source)
                {
                    list.Add((Persons?)obj);
                }
            }
            return list;
        }

        public static HashSet<Tags?> toHashSetBO(this IEnumerable<TagsDAO> source)
        {
            HashSet<Tags?> list = new();
            if (source != null)
            {
                foreach (TagsDAO obj in source)
                {
                    list.Add((Tags?)obj);
                }
            }
            return list;
        }

        public static HashSet<TransactionsStatus?> toHashSetBO(this IEnumerable<TransactionsStatusDAO> source)
        {
            HashSet<TransactionsStatus?> list = new();
            if (source != null)
            {
                foreach (TransactionsStatusDAO obj in source)
                {
                    list.Add((TransactionsStatus?)obj);
                }
            }
            return list;
        }

        public static HashSet<Accounts?> toHashSetBO(this IEnumerable<AccountsDAO> source)
        {
            HashSet<Accounts?> list = new();
            if (source != null)
            {
                foreach (AccountsDAO obj in source)
                {
                    list.Add((Accounts?)obj);
                }
            }
            return list;
        }

        public static HashSet<AccountsView> toHashSetViewBO(this IEnumerable<AccountsDAO> source)
        {
            HashSet<AccountsView> list = new();
            if (source != null)
            {
                foreach (AccountsDAO obj in source)
                {
                    list.Add((AccountsView)obj);
                }
            }
            return list;
        }

        public static HashSet<AccountsTypes?> toHashSetBO(this IEnumerable<AccountsTypesDAO> source)
        {
            HashSet<AccountsTypes?> list = new();
            if (source != null)
            {
                foreach (AccountsTypesDAO obj in source)
                {
                    list.Add((AccountsTypes?)obj);
                }
            }
            return list;
        }
    }
}
