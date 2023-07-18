

ej.base.registerLicense("Ngo9BigBOggjHTQxAR8/V1NGaF5cXmtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWXhfdnRcRmBcVkN2XkY=");

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
        dataSource: null,
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
            dataSource: null,
            xName: 'category',
            yName: 'amount'
        }
    ]
}, '#elementPie');

var daterangepicker = new ej.calendars.DateRangePicker({

    placeholder: 'Selecciona un rango',
    format: 'dd/MM/yyyy',
    startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1),
    endDate: new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0),
    presets: [
        { label: 'Hoy', start: new Date(), end: new Date() },
        { label: 'Este mes', start: new Date(new Date().setDate(1)), end: new Date() },
        { label: 'Mes anterior', start: new Date(new Date(new Date().setMonth(new Date().getMonth() - 1)).setDate(1)), end: new Date() },
        { label: 'Año Actual', start: new Date(new Date().getFullYear(), 0, 1), end: new Date() },
        { label: 'Año Anterior', start: new Date(new Date().getFullYear() - 1, 0, 1), end: new Date() },

    ],
    change: dateChange
});
daterangepicker.appendTo('#elementDate');

filterChart();

function dateChange(args) {
    filterChart();
}

function filterChart() {
    var filteredData = generateSummaryWithData(filterDataExpenses(daterangepicker.startDate, daterangepicker.endDate), -1);

    chart.series[0].dataSource = filteredData;
    chart.refresh();

    piechart.series[0].dataSource = filteredData;
    piechart.refresh();
}



