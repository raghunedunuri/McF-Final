var json = [{ "year": 1994, "hfcs_demand": 15.5, "hfcs_capacity": 16, "comment": "" }, { "year": 1995, "hfcs_demand": 16, "hfcs_capacity": 20, "comment": "" }, { "year": 1996, "hfcs_demand": 17, "hfcs_capacity": 23, "comment": "1994-97:Industry expansion for Mexico" }, { "year": 1997, "hfcs_demand": 18, "hfcs_capacity": 25, "comment": "" }, { "year": 1998, "hfcs_demand": 19, "hfcs_capacity": 23, "comment": "" }, { "year": 1999, "hfcs_demand": 20, "hfcs_capacity": 23, "comment": "1999:Cargill stops HFCS production at Dayton,Ohio" }, { "year": 2000, "hfcs_demand": 20, "hfcs_capacity": 24, "comment": "" }, { "year": 2001, "hfcs_demand": 20, "hfcs_capacity": 24, "comment": "" }, { "year": 2002, "hfcs_demand": 20, "hfcs_capacity": 24, "comment": "" }, { "year": 2002, "hfcs_demand": 20, "hfcs_capacity": 24, "comment": "" }, { "year": 2003, "hfcs_demand": 19, "hfcs_capacity": 23, "comment": "" }, { "year": 2004, "hfcs_demand": 19, "hfcs_capacity": 23, "comment": "" }, { "year": 2005, "hfcs_demand": 20, "hfcs_capacity": 22, "comment": "2005:Cargill closes Dimmitt, Tex.plant" }, { "year": 2006, "hfcs_demand": 20, "hfcs_capacity": 21, "comment": "2006:Cargill re-opens Dayton plant" }, { "year": 2007, "hfcs_demand": 20, "hfcs_capacity": 23, "comment": "" }, { "year": 2008, "hfcs_demand": 19, "hfcs_capacity": 23, "comment": "" }, { "year": 2009, "hfcs_demand": 18, "hfcs_capacity": 23, "comment": "2009:Cargill permanently closes Decatur,Ala." }, { "year": 2010, "hfcs_demand": 20, "hfcs_capacity": 22, "comment": "" }, { "year": 2011, "hfcs_demand": 20, "hfcs_capacity": 22, "comment": "" }, { "year": 2012, "hfcs_demand": 20, "hfcs_capacity": 23, "comment": "" }, { "year": 2013, "hfcs_demand": 19, "hfcs_capacity": 23, "comment": "" }, { "year": 2014, "hfcs_demand": 19, "hfcs_capacity": 23, "comment": "" }, { "year": 2015, "hfcs_demand": 18, "hfcs_capacity": 21, "comment": "2015:Cargill closes Memphis, Tenn.plant, removing 9% of HFCS capacity" }, { "year": 2016, "hfcs_demand": 18, "hfcs_capacity": 21, "comment": "2016:Ingredion sells Port Colborne, Ont., removing 2% of HFCS capacity" }, { "year": 2017, "hfcs_demand": 17, "hfcs_capacity": 20, "comment": "" }];
var _chart, categories = [], demand = [], capacity = [];
$(document).ready(function () {
    var object;
    $.each(json, function (inde, item) {
        categories.push(item.year);
        object = { y: item.hfcs_capacity };
        if (item.comment && item.comment !== '') {
            object.marker = {
                text: item.comment,
                fillColor: '#FFFF00',
                lineColor: '#d23928',
                lineWidth: 2,
                radius: 8
            };
        }
        capacity.push(object);
        demand.push(item.hfcs_demand);
    });

    function updateChart() {
        _chart = Highcharts.chart('mapcontainer', {
            chart: {
                type: 'spline',
            },
            title: {
                text: 'Theoretical North American HFCS demand',
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
                    text: 'Billion pounds, dry',
                    style: {
                        color: '#8B3910',
                        fontSize: '24px',
                        fontFamily: 'Cordia New'
                    }
                },
            },
            tooltip: {
                formatter: function () {
                    var comment = (this.point.marker) ? '<b>' + this.point.marker.text + '</b>' : '';
                    return '<b>' + this.x + '</b><br/>' +
                        this.series.name + ': ' + this.y + '<br/>' + comment;
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
                spline: {
                    lineWidth: 4,
                    states: {
                        hover: {
                            lineWidth: 5
                        }
                    },
                    marker: {
                        enabled: true,
                        lineWidth: 0,
                        radius: 0
                    }
                }
            },
            series: [{
                id: '_1',
                name: 'North American HFCS Capacity',
                data: capacity,
                color: '#007297'

            }, {
                id: '_2',
                name: 'North American HFCS Demand',
                data: demand,
                color: '#FFC000'
            }]
        });
    }

    $('#_comment').change(function () {
        var enabled = this.checked;
        _chart.update({
            plotOptions: {
                spline: {
                    marker: {
                        enabled: enabled
                    }
                }
            }
        });
    });

    updateChart();
});