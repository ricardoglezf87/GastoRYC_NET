using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA_UTIL.Exceptions
{
    public class DonwloadPricesException : Exception
    {
        public DonwloadPricesException()
        {
        }

        public DonwloadPricesException(string message)
            : base(message)
        {
        }

        public DonwloadPricesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
