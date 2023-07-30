using GARCA.BO.Models;
using GARCA.View.ViewModels;
using GARCA.DAO.Models;
using System.Collections.Generic;

namespace GARCA.Utlis.Extensions
{
    public static class DoubleExtension
    {
        public static int CompareTo(this double? source, double? other)
        {
            if (source < other)
            {
                return -1;
            }
            else if (source > other)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
