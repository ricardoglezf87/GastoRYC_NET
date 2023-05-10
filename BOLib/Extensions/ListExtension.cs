using BOLib.Models;
using DAOLib.Models;
using System;
using System.Collections.Generic;

namespace BOLib.Extensions
{
    public static class ListExtension
    {
        public static List<TransactionsStatus> toListTransactionsStatus(this List<TransactionsStatusDAO> source)
        {
            List<TransactionsStatus> list = new();
            if (source != null)
            {
                foreach (TransactionsStatusDAO obj in source)
                {
                    list.Add((TransactionsStatus)obj);
                }
            }
            return list;
        }

        public static List<Accounts> toListAccounts(this List<AccountsDAO> source)
        {
            List<Accounts> list = new();
            if (source != null)
            {
                foreach (AccountsDAO obj in source)
                {
                    list.Add((Accounts)obj);
                }
            }
            return list;
        }

        public static List<AccountsTypes> toListAccountsTypes(this List<AccountsTypesDAO> source)
        {
            List<AccountsTypes> list = new();
            if (source != null)
            {
                foreach (AccountsTypesDAO obj in source)
                {
                    list.Add((AccountsTypes)obj);
                }
            }
            return list;
        }
    }
}
