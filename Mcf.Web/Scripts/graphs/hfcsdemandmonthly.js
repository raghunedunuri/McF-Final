var json = [{ "values": [{ "year": 2014, "value": 50000 }, { "year": 2015, "value": 80000 }, { "year": 2016, "value": 55000 }, { "year": 2017, "value": 95000 }], "month": "Jan" }, { "values": [{ "year": 2014, "value": 52000 }, { "year": 2015, "value": 40000 }, { "year": 2016, "value": 75000 }, { "year": 2017, "value": 43000 }], "month": "Feb" }, { "values": [{ "year": 2014, "value": 57000 }, { "year": 2015, "value": 70000 }, { "year": 2016, "value": 92000 }, { "year": 2017, "value": 69000 }], "month": "Mar" }, { "values": [{ "year": 2014, "value": 69000 }, { "year": 2015, "value": 78000 }, { "year": 2016, "value": 62000 }, { "year": 2017, "value": 72000 }], "month": "Apr" }, { "values": [{ "year": 2014, "value": 62000 }, { "year": 2015, "value": 63000 }, { "year": 2016, "value": 78000 }, { "year": 2017, "value": 61000 }], "month": "May" }, { "values": [{ "year": 2014, "value": 69000 }, { "year": 2015, "value": 95000 }, { "year": 2016, "value": 96000 }, { "year": 2017, "value": 90000 }], "month": "Jun" }, { "values": [{ "year": 2014, "value": 49000 }, { "year": 2015, "value": 60000 }, { "year": 2016, "value": 71000 }, { "year": 2017, "value": 82000 }], "month": "Jul" }, { "values": [{ "year": 2014, "value": 108000 }, { "year": 2015, "value": 81000 }, { "year": 2016, "value": 70000 }, { "year": 2017, "value": 82000 }], "month": "Aug" }, { "values": [{ "year": 2014, "value": 51000 }, { "year": 2015, "value": 77000 }, { "year": 2016, "value": 75000 }, { "year": 2017, "value": 0 }], "month": "Sep" }, { "values": [{ "year": 2014, "value": 86000 }, { "year": 2015, "value": 52000 }, { "year": 2016, "value": 80000 }, { "year": 2017, "value": 0 }], "month": "Oct" }, { "values": [{ "year": 2014, "value": 71000 }, { "year": 2015, "value": 50000 }, { "year": 2016, "value": 61000 }, { "year": 2017, "value": 0 }], "month": "Nov" }, { "values": [{ "year": 2014, "value": 72000 }, { "year": 2015, "value": 73000 }, { "year": 2016, "value": 52000 }, { "year": 2017, "value": 0 }], "month": "Dec" }];
var _chart, categories = [], data = { '2014': [], '2015': [], '2016': [], '2017': [] };
$(document).ready(function () {
    $.each(json, function (index, item) {
        $.each(item.values, function (index, item1) {
            data[item1['year'] + ''].push(item1.value);
        });
        categories.push(item.month + '');
        console.log(item.month)
    });

    function updateChart() {
        _chart = Highcharts.chart('mapcontainer', {
            chart: {
                type: 'bar',
            },
            title: {
                text: 'U.S monthly HFSC -55 exports to Mexico',
                style: {
                    color: '#8B3910',
                    fontSize: '36px',
                    fontWeight: 'normal',
                    fontFamily: 'Cordia New'
                }
            },
            xAxis: {
                categories: categories,
                labels: {
                    overflow: 'justify'
                }
            },
            yAxis: {
                title: {
                    text: 'Metric tons, dry, basis',
                    style: {
                        color: '#8B3910',
                        fontSize: '24px',
                        fontFamily: 'Cordia New'
                    }
                },
            },
            tooltip: {
                formatter: function () {
                    var comment = (this.point.comment) ? '<b>' + this.point.comment + '</b>' : '';
                    return '<b>' + this.x + '</b><br/>' +
                        this.series.name + ': ' + this.y + '<br/>' + comment;
                }
            },
            legend: {
                layout: 'vertical',
                symbolWidth: 20,
                itemStyle: {
                    color: '#000000',
                    fontWeight: 'normal',
                    fontSize: '13px'
                }
            },
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true
                    }
                }
            },
            series: [{
                id: '_1',
                name: 'HFCS-55 Exports to Mexico, 2014',
                data: data['2014'],
                color: '#D9D9D9',
                marker: {
                    enabled: true
                }
            }, {
                id: '_2',
                name: '2015',
                data: data['2015'],
                color: '#A6A6A6',
                marker: {
                    enabled: true
                }
            }, {
                id: '_3',
                name: '2016',
                data: data['2016'],
                color: '#DCC7ED',
                marker: {
                    enabled: true
                }
            }, {
                id: '_4',
                name: '2017',
                data: data['2017'],
                color: '#7030A0',
                marker: {
                    enabled: true
                }
            }]
        });
    }

    updateChart();
});