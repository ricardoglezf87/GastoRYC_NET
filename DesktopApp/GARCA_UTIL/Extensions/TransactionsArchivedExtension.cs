using GARCA.Models;

namespace GARCA.Utils.Extensions
{
    public static class TransactionsArchivedExtension
    {
        public static void AddRange(this HashSet<TransactionsArchived>? source, HashSet<Transactions>? elements)
        {
            if (elements != null)
            {
                foreach (var obj in elements)
                {
                    if (obj != null)
                    {
                        source.Add(obj.ToArchived());
                    }
                }
            }
        }

        public static void AddRange(this HashSet<TransactionsArchived>? source, HashSet<TransactionsArchived>? elements)
        {
            if (elements != null)
            {
                foreach (var obj in elements)
                {
                    if (obj != null)
                    {
                        source.Add(obj);
                    }
                }
            }
        }
    }
}
