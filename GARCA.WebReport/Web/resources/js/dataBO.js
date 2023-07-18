function filterDataSummary(data, month) {
    return data.filter(item => item.month === month);
}

function filterData(dateF, dateT) {
    return chartData.filter(item => item.datenum >= toDateNum(dateF) && item.datenum <= toDateNum(dateT));
}

function filterDataExpenses(dateF, dateT) {
    return chartData.filter(item => item.datenum >= toDateNum(dateF) && item.datenum <= toDateNum(dateT) && item.amount < 0);
}

function toDateNum(date) {
    var lDate = date.toLocaleDateString().split('/');

    if (lDate[1].length == 1)
        lDate[1] = '0' + lDate[1];

    if (lDate[0].length == 1)
        lDate[0] = '0' + lDate[0];

    return lDate[2] + lDate[1] + lDate[0];
}

function generateSummary(factor) {
    var summary = [];

    // Iterar sobre los elementos del dataSource
    for (var i = 0; i < chartData.length; i++) {
        var item = chartData[i];

        // Verificar si el mes ya existe en el resumen
        var existingId = summary.find(function (summaryItem) {
            return summaryItem.id === (item.date.split('-')[0] + item.date.split('-')[1] + item.categoryid);
        });

        if (existingId) {
            // Si existe, sumar el valor actual al existente
            existingId.amount += item.amount * factor;
        } else {
            // Si no existe, agregar un nuevo objeto al resumen
            summary.push({ id: item.date.split('-')[0] + item.date.split('-')[1] + item.categoryid, month: item.date.split('-')[0] + '/' + item.date.split('-')[1], category: item.category, amount: item.amount * factor });
        }
    }
    return summary;
}

function generateSummaryWithData(data, factor) {
    var summary = [];

    // Iterar sobre los elementos del dataSource
    for (var i = 0; i < data.length; i++) {
        var item = data[i];

        // Verificar si el mes ya existe en el resumen
        var existingId = summary.find(function (summaryItem) {
            return summaryItem.id === (item.date.split('-')[0] + item.date.split('-')[1] + item.categoryid);
        });

        if (existingId) {
            // Si existe, sumar el valor actual al existente
            existingId.amount += item.amount * factor;
        } else {
            // Si no existe, agregar un nuevo objeto al resumen
            summary.push({ id: item.date.split('-')[0] + item.date.split('-')[1] + item.categoryid, month: item.date.split('-')[0] + '/' + item.date.split('-')[1], category: item.category, amount: item.amount * factor });
        }
    }
    return summary;
}