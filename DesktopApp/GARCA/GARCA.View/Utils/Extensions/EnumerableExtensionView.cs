using GARCA.Models;
using GARCA.View.ViewModels;
using System.Collections.Generic;

namespace GARCA.View.Utils.Extensions
{
    internal static class EnumerableExtensionView
    {
        public static HashSet<AccountsView> ToHashSetViewBo(this IEnumerable<Accounts> source)
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
