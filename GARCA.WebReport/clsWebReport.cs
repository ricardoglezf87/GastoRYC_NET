using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.WebReport
{
    public class clsWebReport
    {
        private readonly StringBuilder html;
        private readonly StringBuilder js;

        public clsWebReport()
        {
            html = new();
            js = new();
        }

        public async Task writeReport()
        {
            await writeData();
        }

        private async Task writeData()
        {
            try
            {
                js.AppendLine(@"
                                    var chartData = [
                                            { month: 'Jan', sales: 35 }, { month: 'Feb', sales: 28 },
                                            { month: 'Mar', sales: 34 }, { month: 'Apr', sales: 32 },
                                            { month: 'May', sales: 40 }, { month: 'Jun', sales: 32 },
                                            { month: 'Jul', sales: 35 }, { month: 'Aug', sales: 55 },
                                            { month: 'Sep', sales: 38 }, { month: 'Oct', sales: 30 },
                                            { month: 'Nov', sales: 25 }, { month: 'Dec', sales: 32 }
                                    ];                                   
                              ");
                File.WriteAllText("..\\Web\\data.js", js.ToString());
            }
            catch (Exception)
            {
                throw;
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