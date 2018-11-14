$(document).ready(function () {
    var data = [{ "values": [{ "name": "United", "y": 26 }, { "name": "Cargill***", "y": 15 }, { "name": "NSM (Amal., Sucden)", "y": 9 }, { "name": "Dreyfus (Imperial)", "y": 8 }, { "name": "Western*", "y": 5 }, { "name": "Michigan**", "y": 6 }, { "name": "AmCane", "y": 1 }, { "name": "Domino", "y": 30 }], "year": 2014 }, { "values": [{ "name": "United", "y": 27 }, { "name": "Cargill***", "y": 9 }, { "name": "NSM (Amal., Sucden)", "y": 14 }, { "name": "Dreyfus (Imperial)", "y": 8 }, { "name": "Western*", "y": 5 }, { "name": "Michigan**", "y": 7 }, { "name": "AmCane", "y": 0 }, { "name": "Domino", "y": 30 }], "year": 2018 }];
    var containers = ['mapcontainer', 'mapcontainer1'];

    function updateChart() {
        $.each(containers, function (index, item) {
            Highcharts.chart(item, {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: data[index].year,
                    style: {
                        color: '#8B3910',
                        fontSize: '36px',
                        fontFamily: 'Cordia New'
                    }
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        }
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
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: <br/>{point.percentage:.1f} %',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black',
                                fontWeight: 'normal',
                                fontSize: '13px'
                            }
                        }
                    }
                },
                series: [{
                    name: data[index].year,
                    colorByPoint: true,
                    data: data[index].values,
                }]
            });
        });
    }

    updateChart();
});