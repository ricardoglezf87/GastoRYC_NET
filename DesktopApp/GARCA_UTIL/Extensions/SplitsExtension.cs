using GARCA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Utils.Extensions
{
    public static class SplitsExtension
    {       
        public static SplitsArchived ToArchived(this Splits obj)
        {
            return new SplitsArchived
            {
                IdOriginal = obj.Id,
                Transactionid = obj.Transactionid,
                Transaction = null,
                Categoryid = obj.Categoryid,
                Category = null,
                AmountOut = obj.AmountOut,
                AmountIn = obj.AmountIn,
                Memo = obj.Memo,
                Tranferid = obj.Tranferid,
                Tagid = obj.Tagid,
                Tag = null
            };
        }      
    }
}