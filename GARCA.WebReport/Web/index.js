
                                    var ele = document.getElementById('container');
                                    if(ele) {
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
                              
