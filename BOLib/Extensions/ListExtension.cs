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
    }
}
