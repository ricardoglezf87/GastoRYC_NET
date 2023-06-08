using BOLib.Models;
using BOLib.ModelsView;
using DAOLib.Models;
using System.Collections.Generic;

namespace BOLib.Extensions
{
    public static class ListExtension
    {
        public static List<DateCalendar?> toListBO(this List<DateCalendarDAO> source)
        {
            List<DateCalendar?> list = new();
            if (source != null)
            {
                foreach (DateCalendarDAO obj in source)
                {
                    list.Add((DateCalendar?)obj);
                }
            }
            return list;
        }

        public static List<PeriodsReminders?> toListBO(this List<PeriodsRemindersDAO> source)
        {
            List<PeriodsReminders?> list = new();
            if (source != null)
            {
                foreach (PeriodsRemindersDAO obj in source)
                {
                    list.Add((PeriodsReminders?)obj);
                }
            }
            return list;
        }

        public static List<ExpirationsReminders?> toListBO(this List<ExpirationsRemindersDAO> source)
        {
            List<ExpirationsReminders?> list = new();
            if (source != null)
            {
                foreach (ExpirationsRemindersDAO obj in source)
                {
                    list.Add((ExpirationsReminders?)obj);
                }
            }
            return list;
        }

        public static List<VBalancebyCategory?> toListBO(this List<VBalancebyCategoryDAO> source)
        {
            List<VBalancebyCategory?> list = new();
            if (source != null)
            {
                foreach (VBalancebyCategoryDAO obj in source)
                {
                    list.Add((VBalancebyCategory?)obj);
                }
            }
            return list;
        }

        public static List<SplitsReminders?> toListBO(this List<SplitsRemindersDAO> source)
        {
            List<SplitsReminders?> list = new();
            if (source != null)
            {
                foreach (SplitsRemindersDAO obj in source)
                {
                    list.Add((SplitsReminders?)obj);
                }
            }
            return list;
        }

        public static List<Splits?> toListBO(this List<SplitsDAO> source)
        {
            List<Splits?> list = new();
            if (source != null)
            {
                foreach (SplitsDAO obj in source)
                {
                    list.Add((Splits?)obj);
                }
            }
            return list;
        }

        public static List<TransactionsReminders?> toListBO(this List<TransactionsRemindersDAO> source)
        {
            List<TransactionsReminders?> list = new();
            if (source != null)
            {
                foreach (TransactionsRemindersDAO obj in source)
                {
                    list.Add((TransactionsReminders?)obj);
                }
            }
            return list;
        }

        public static List<Transactions?> toListBO(this List<TransactionsDAO> source)
        {
            List<Transactions?> list = new();
            if (source != null)
            {
                foreach (TransactionsDAO obj in source)
                {
                    list.Add((Transactions?)obj);
                }
            }
            return list;
        }

        public static List<InvestmentProductsPrices?> toListBO(this List<InvestmentProductsPricesDAO> source)
        {
            List<InvestmentProductsPrices?> list = new();
            if (source != null)
            {
                foreach (InvestmentProductsPricesDAO obj in source)
                {
                    list.Add((InvestmentProductsPrices?)obj);
                }
            }
            return list;
        }

        public static List<InvestmentProducts?> toListBO(this List<InvestmentProductsDAO> source)
        {
            List<InvestmentProducts?> list = new();
            if (source != null)
            {
                foreach (InvestmentProductsDAO obj in source)
                {
                    list.Add((InvestmentProducts?)obj);
                }
            }
            return list;
        }

        public static List<InvestmentProductsTypes?> toListBO(this List<InvestmentProductsTypesDAO> source)
        {
            List<InvestmentProductsTypes?> list = new();
            if (source != null)
            {
                foreach (InvestmentProductsTypesDAO obj in source)
                {
                    list.Add((InvestmentProductsTypes?)obj);
                }
            }
            return list;
        }

        public static List<Categories?> toListBO(this List<CategoriesDAO> source)
        {
            List<Categories?> list = new();
            if (source != null)
            {
                foreach (CategoriesDAO obj in source)
                {
                    list.Add((Categories?)obj);
                }
            }
            return list;
        }

        public static List<CategoriesTypes?> toListBO(this List<CategoriesTypesDAO> source)
        {
            List<CategoriesTypes?> list = new();
            if (source != null)
            {
                foreach (CategoriesTypesDAO obj in source)
                {
                    list.Add((CategoriesTypes?)obj);
                }
            }
            return list;
        }

        public static List<Persons?> toListBO(this List<PersonsDAO> source)
        {
            List<Persons?> list = new();
            if (source != null)
            {
                foreach (PersonsDAO obj in source)
                {
                    list.Add((Persons?)obj);
                }
            }
            return list;
        }

        public static List<Tags?> toListBO(this List<TagsDAO> source)
        {
            List<Tags?> list = new();
            if (source != null)
            {
                foreach (TagsDAO obj in source)
                {
                    list.Add((Tags?)obj);
                }
            }
            return list;
        }

        public static List<TransactionsStatus?> toListBO(this List<TransactionsStatusDAO> source)
        {
            List<TransactionsStatus?> list = new();
            if (source != null)
            {
                foreach (TransactionsStatusDAO obj in source)
                {
                    list.Add((TransactionsStatus?)obj);
                }
            }
            return list;
        }

        public static List<Accounts?> toListBO(this List<AccountsDAO> source)
        {
            List<Accounts?> list = new();
            if (source != null)
            {
                foreach (AccountsDAO obj in source)
                {
                    list.Add((Accounts?)obj);
                }
            }
            return list;
        }

        public static List<AccountsView> toListViewBO(this List<AccountsDAO> source)
        {
            List<AccountsView> list = new();
            if (source != null)
            {
                foreach (AccountsDAO obj in source)
                {
                    list.Add((AccountsView)obj);
                }
            }
            return list;
        }

        public static List<AccountsTypes?> toListBO(this List<AccountsTypesDAO> source)
        {
            List<AccountsTypes?> list = new();
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
