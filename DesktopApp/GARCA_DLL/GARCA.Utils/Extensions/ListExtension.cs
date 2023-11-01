using GARCA.BO.Models;
using GARCA.DAO.Models;

namespace GARCA.Utlis.Extensions
{
    public static class ListExtension
    {
        public static List<TransactionsDao?> ToListDao(this List<Transactions?>? source)
        {
            List<TransactionsDao?> list = new();
            if (source != null)
            {
                foreach (var obj in source)
                {
                    list.Add(obj.ToDao());
                }
            }
            return list;
        }

        public static List<TransactionsArchivedDao?> ToListDao(this List<TransactionsArchived?>? source)
        {
            List<TransactionsArchivedDao?> list = new();
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
