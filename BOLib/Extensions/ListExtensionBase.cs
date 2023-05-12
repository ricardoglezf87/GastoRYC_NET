using BOLib.Models;
using BOLib.ModelsView;
using DAOLib.Models;
using System;
using System.Collections.Generic;

namespace BOLib.Extensions
{
    public static class ListExtensionBase<X,Z>
        where X : ModelBase
        where Z : ModelBaseDAO
    {

        public static List<X> toListBO(List<Z>? source)
        {
            List<X> list = new();
            if (source != null)
            {
                foreach (Z obj in source)
                {
                    list.Add((X)obj);
                }
            }
            return list;
        }
    }
}
