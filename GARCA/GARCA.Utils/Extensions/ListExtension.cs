using GARCA.BO.Models;
using GARCA.View.ViewModels;
using GARCA.DAO.Models;
using System.Collections.Generic;

namespace GARCA.Utlis.Extensions
{
    public static class ListExtension
    {
        public static List<TransactionsDAO?>? toListDAO(this List<Transactions?>? source)
        {
            List<TransactionsDAO?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add(obj.toDAO());
                }
            }
            return list;
        }

        public static List<DateCalendar?> toListBO(this List<DateCalendarDAO> source)
        {
            List<DateCalendar?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
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
                foreach (var obj in source)
                {
                    list.Add((AccountsTypes?)obj);
                }
            }
            return list;
        }
    }
}
