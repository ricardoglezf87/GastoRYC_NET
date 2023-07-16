using GARCA.BO.Models;
using GARCA.BO.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GARCA.WebReport
{
    public class clsWebReport
    {
        public async Task writeReport()
        {
            await writeData();
        }

        private async Task writeData()
        {
            StringBuilder js = new();
            try
            {
                js.AppendLine("var chartData = [");

                var transactions = await Task.Run(() => TransactionsService.Instance.getAllOpennedWithoutTransOrderByDateAsc());

                for (int i = 0; i < transactions.Count; i++)
                {
                    Transactions? trans = transactions[i];
                    js.Append(@"{ date: '" + dateToStringJS(trans.date) + "', datenum: " + dateNumberToStringJS(trans.date)
                        + ", category: '" + trans.categoryDescripGrid + "', categoryid: " + (trans.categoryid == null ? -99 : trans.categoryid) +
                        ", amount: " + decimalToStringJS(trans.amount) + " }");

                    if (i < transactions.Count - 1)
                    {
                        js.AppendLine(",");
                    }
                }
                js.Append("\n];");

                File.WriteAllText("..\\Web\\Resources\\js\\data.js", js.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string decimalToStringJS(decimal? amount)
        {
            if (amount == null)
            {
                return string.Empty;
            }
            else
            {
                return amount.ToString().Replace(".", "").Replace(",", ".");
            }
        }

        private string dateToStringJS(DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }
            else
            {
                return $"{date.Value.Year.ToString("0000")}-{date.Value.Month.ToString("00")}-{date.Value.Day.ToString("00")}";
            }
        }

        private string dateNumberToStringJS(DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }
            else
            {
                return $"{date.Value.Year.ToString("0000") + date.Value.Month.ToString("00") + date.Value.Day.ToString("00")}";
            }
        }
    }
}

/*
<html>

<head>

   	<title>clave acceso</title>

</head>

<body>

<SCRIPT>

function acceso(){
   	window.location = document.formclave.clave.value + ".html"
}

</SCRIPT>

<FORM name=formclave>

  
<INPUT type=password name=clave>

<INPUT type=button value=Acceder onclick="acceso()">

</FORM>

</body>

</html>
 
 */