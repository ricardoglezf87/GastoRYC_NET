using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Utils.Enums
{
    public static class EnumCategories
    {
        public enum ECategoriesTypes
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public enum ESpecialCategories
        {
            Split = -1,
            WithoutCategory = 0
        }
    }
}
