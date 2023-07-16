function filterData(data,month){
    return data.filter(item => item.month === month);
}

function generateSummary(){
  var summary = [];
  
  // Iterar sobre los elementos del dataSource
  for (var i = 0; i < chartData.length; i++) {
    var item = chartData[i];
    
    // Verificar si el mes ya existe en el resumen
    var existingId = summary.find(function(summaryItem) {
      return summaryItem.id === (item.date.split('-')[0] + item.date.split('-')[1] + item.categoryid);
    });
    
    if (existingId) {
      // Si existe, sumar el valor actual al existente
      existingId.amount += item.amount;
    } else {
      // Si no existe, agregar un nuevo objeto al resumen
      summary.push({ id: item.date.split('-')[0] + item.date.split('-')[1] + item.categoryid, month: item.date.split('-')[0] + '/' + item.date.split('-')[1], category: item.category, amount: item.amount });
    }
  }
  return summary;
}