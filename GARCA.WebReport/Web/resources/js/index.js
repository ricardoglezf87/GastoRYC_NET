

ej.base.registerLicense("Ngo9BigBOggjHTQxAR8/V1NGaF5cXmtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWXhfdnRcRmBcVkN2XkY=");
var summaryData = generateSummary();



var ele = document.getElementById('container');
if (ele) {
    ele.style.visibility = "visible";
}
var chart = new ej.charts.Chart({
    // Tooltip for chart
    tooltip: {
        enable: true
    },
    primaryXAxis: {
        valueType: 'Category',
    },
    primaryYAxis: {
        labelFormat: '${value}'
    },
    series: [{
        dataSource: summaryData,
        name: 'Sales',
        xName: 'category',
        yName: 'amount',
        type: 'Line',
        marker: {
            // Data label for chart series
            dataLabel: {
                visible: true
            }
        }
    }],
    legendSettings: { visible: true },
    title: 'Category Analysis'
}, '#element');

var piechart = new ej.charts.AccumulationChart({
    tooltip: {
        enable: true
    },
	series: [
        {
            dataSource: summaryData,
            xName: 'category',
            yName: 'amount'			
        }
    ]
}, '#elementPie');

filterChart('2023/03');

var daterangepicker = new ej.calendars.DateRangePicker({
        placeholder: 'Select a range',
        //sets the start date in the range
        startDate: new Date("11/9/2017"),
        //sets the end date in the range
        endDate: new Date("11/21/2017")
    });
    daterangepicker.appendTo('#elementDate');
	
function filterChart(selectedMonth) 
{
    var filteredData = filterData(summaryData,selectedMonth);
    
	chart.series[0].dataSource = filteredData;
    chart.refresh();
	
	piechart.series[0].dataSource = filteredData;
    piechart.refresh();
}



