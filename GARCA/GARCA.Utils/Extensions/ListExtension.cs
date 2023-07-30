using GARCA.BO.Models;
using GARCA.DAO.Models;
using System.Collections.Generic;

namespace GARCA.Utlis.Extensions
{
    public static class ListExtension
    {
        public static List<TransactionsDAO?> ToListDao(this List<Transactions?>? source)
        {
            List<TransactionsDAO?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add(obj.ToDao());
                }
            }
            return list;
        }


    }
}
