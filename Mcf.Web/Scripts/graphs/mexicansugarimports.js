$(document).ready(function () {
    var data = { "years": ["2005/06", "2006/07", "2007/08", "2008/09", "2009/10", "2010/11", "2011/12", "2012/13", "2013/14", "2014/15", "2015/16", "2016/17", "2017/18"], "esandor": [0.4, 0.1, 0.2, 0.4, 0.2, 0.5, 0.2, 1.3, 1.2, 0.7, 0.6, 0.55, 0.8], "refinado": [0.4, 0.1, 0.2, 0.7, 0.5, 0.9, 0.6, 0.6, 0.6, 0.6, 0.6, 0.5, 0.3], "total": [3.1, 1.9, 2.4, 2.8, 3.0, 3.4, 3.3, 2.9, 3.4, 3.3, 3.0, 2.9, 3.1] };
    $.each(data.years, function (index, item) {

    });

    var categories = data['years'];

    function updateChart() {
        Highcharts.chart('mapcontainer', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'U.S. Imports of Mexican sugar',
                style: {
                    color: '#8B3910',
                    fontSize: '36px',
                    fontFamily: 'Cordia New'
                }
            },
            xAxis: {
                categories: categories,

            },
            yAxis: {
                allowDecimals: false,
                min: 0,
                title: {
                    text: 'Million Metric tons, raw',
                    style: {
                        color: '#8B3910',
                        fontSize: '24px',
                        fontFamily: 'Cordia New'
                    },
                },
                stackLabels: {
                    enabled: true,
                    style: {
                        fontWeight: 'bold',
                        color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                    },
                    formatter: function () {
                        // var firstColumnValue = this.y;
                        // var secondColumnValue = this.series.chart.series[1].yData[this.point.index];
                        // var yourCalculation = (firstColumnValue - secondColumnValue) / firstColumnValue * 100;
                        // return yourCalculation.toFixed(2) + '%';
                        console.log(this);
                        return this.total;
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.x + '</b><br/>' +
                        this.series.name + ': ' + this.y + '<br/>' +
                        'Total: ' + this.point.stackTotal;
                }
            },
            legend: {
                symbolWidth: 20,
                itemStyle: {
                    color: '#000000',
                    fontWeight: 'normal',
                    fontSize: '13px'
                }
            },
            plotOptions: {
                column: {
                    stacking: 'normal',
                    dataLabels: {
                        enabled: false,
                        color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white'
                    }
                }
            },
            series: [{
                id: 'esandor',
                name: 'Estandar Imported from Mexico',
                data: data['esandor'],
                stack: 'one',
                color: '#DEB40B'
            }, {
                id: 'refinado',
                name: 'Refinado Imported from Mexico',
                data: data['refinado'],
                stack: 'one',
                color: '#F59184'
            }, {
                id: 'total',
                name: 'Total U.S Sugar Imports',
                data: data['total'],
                stack: 'total',
                color: '#98A05F'
            }]
        });
    }

    updateChart();
});