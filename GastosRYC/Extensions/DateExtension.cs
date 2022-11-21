using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.Extensions
{
    public static class DateExtension
    {
        public static String toShortDateString(this DateTime? date)
        {
            if (date != null)
            {
                return date?.Day.ToString("00") + "/" + date?.Month.ToString("00") + "/" + date?.Year.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
