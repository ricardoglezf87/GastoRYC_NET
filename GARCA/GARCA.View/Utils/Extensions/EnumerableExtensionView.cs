using GARCA.DAO.Models;
using GARCA.Utlis.Extensions;
using GARCA.View.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.View.Utils.Extensions
{
    internal static class EnumerableExtensionView
    {
        public static HashSet<AccountsView> ToHashSetViewBo(this IEnumerable<AccountsDao> source)
        {
            HashSet<AccountsView> list = new();

            foreach (var obj in source)
            {
                list.Add((AccountsView)obj);
            }

            return list;
        }
    }
}
