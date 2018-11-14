$(document).ready(function () {
    var json = [{ "value": 7.9, "year": 2000 }, { "value": 8, "year": 2001 }, { "value": 7.9, "year": 2002 }, { "value": 8.1, "year": 2003 }, { "value": 8.2, "year": 2004 }, { "value": 8.1, "year": 2005 }, { "value": 9, "year": 2006 }, { "value": 9.2, "year": 2007 }, { "value": 9, "year": 2008 }, { "value": 8.4, "year": 2009 }, { "value": 9.5, "year": 2010 }, { "value": 10.4, "year": 2011 }, { "value": 10.2, "year": 2012 }, { "value": 11, "year": 2013 }, { "value": 11.5, "year": 2014 }, { "value": 10.3, "year": 2015 }, { "value": 10.8, "year": 2016 }, { "value": 11.8, "year": 2017 }];
    var categories = [], data = [];

    $.each(json, function (index, item) {
        categories.push(item.year);
        data.push(item.value);
    });

    function updateChart() {
        Highcharts.chart('mapcontainer', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'North American corn syrup demand',
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
                    text: 'Billion pounds, wet basis',
                    style: {
                        color: '#8B3910',
                        fontSize: '24px',
                        fontFamily: 'Cordia New'
                    },
                }
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.x + '</b><br/>' +
                        this.series.name + ': ' + this.y + '<br/>';
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
                id: '_1',
                name: 'North American corn syrup demand',
                data: data,
                color: '#469E34'
            }]
        });
    }

    updateChart();
});