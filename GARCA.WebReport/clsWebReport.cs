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
            await writeHTML();
            await writeJS();
        }

        private async Task writeHTML()
        {
            try
            {
                await createHead();
                await bodyHead();
                await footereHead();

                if (!Directory.Exists("..\\Web\\"))
                {
                    Directory.CreateDirectory("..\\Web\\");
                }

                File.WriteAllText("..\\Web\\index.html", html.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task writeJS()
        {
            try
            {
                js.AppendLine(@"
                                    var ele = document.getElementById('container');
                                    if(ele) {
                                      ele.style.visibility = ""visible"";
                                    } 
                                    var chartData = [
                                            { month: 'Jan', sales: 35 }, { month: 'Feb', sales: 28 },
                                            { month: 'Mar', sales: 34 }, { month: 'Apr', sales: 32 },
                                            { month: 'May', sales: 40 }, { month: 'Jun', sales: 32 },
                                            { month: 'Jul', sales: 35 }, { month: 'Aug', sales: 55 },
                                            { month: 'Sep', sales: 38 }, { month: 'Oct', sales: 30 },
                                            { month: 'Nov', sales: 25 }, { month: 'Dec', sales: 32 }
                                    ];
                                    var chart = new ej.charts.Chart({
                                        // Tooltip for chart
                                        tooltip: {
                                            enable: true
                                        },
                                        primaryXAxis: {
                                            valueType: 'Category',
                                        },
                                        primaryYAxis: {
                                            labelFormat: '${value}K'
                                        },
                                        series:[{
                                            dataSource: chartData,
                                            name:'Sales',
                                            xName: 'month',
                                            yName: 'sales',
                                            type: 'Line',
                                            marker: {
                                                // Data label for chart series
                                                dataLabel: {
                                                    visible: true
                                                }
                                            }
                                        }],
                                        legendSettings: {visible: true},
                                        title: 'Sales Analysis'
                                    }, '#element');
                              ");

                if (!Directory.Exists("..\\Web\\"))
                {
                    Directory.CreateDirectory("..\\Web\\");
                }

                File.WriteAllText("..\\Web\\index.js", js.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }


        private async Task createHead()
        {
            html.AppendLine(@"  <!DOCTYPE html>
                                <html>
                                    <head>
                                        <title>GARCA ReportWeb</title>
                                        <link rel=""icon"" type=""image/x-icon"" href=""Resources/favicon.ico"" />
                                        <meta charset=""utf-8"">
                                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                        <meta name=""description"" content=""GARCA Infomes de gastos e ingresos"">
                                        <meta name=""author"" content=""Ricardo Glez"">
                                        <script src='http://cdn.syncfusion.com/ej2/dist/ej2.min.js' type='text/javascript'></script>
                                    </head>
                             ");
        }

        private async Task bodyHead()
        {
            html.AppendLine(@"   
                                  <body
                                    <div id=""container"">
                                        <div id=""element""></div>
                                    </div>
                                    <script src='index.js' type='text/javascript'></script>
                                 </body>
                            ");
        }

        private async Task footereHead()
        {
            html.AppendLine(@"</html>
                            ");
        }

    }
}
