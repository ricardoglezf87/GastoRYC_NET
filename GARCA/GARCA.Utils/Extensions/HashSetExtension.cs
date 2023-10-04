using GARCA.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Utils.Extensions
{
    public static class HashSetExtension
    {
        public static HashSet<Transactions?>? ToTransactionHashSet(this HashSet<TransactionsArchived?>? source)
        {
            HashSet<Transactions?>? list = new();
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

        public static void AddRange<Transactions>(this HashSet<Transactions?>? source, HashSet<Transactions?>? elements)
        {
            if (elements != null)
            {
                foreach (var obj in elements)
                {
                    if (obj != null)
                    {
                        source.Add((Transactions?) obj);
                    }
                }
            }
        }
    }
}
