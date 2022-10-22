using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Exceptions
{
    public class DataBaseNotFoundException:Exception
    {
        public DataBaseNotFoundException()
        {
        }

        public DataBaseNotFoundException(string message)
            : base(message)
        {
        }

        public DataBaseNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
