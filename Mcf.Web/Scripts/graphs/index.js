$(document).ready(function () {
    var jsonData = [{ "date": "2016/17", "region": "US", "a": 44.12, "b": 384.78, "c": 1.45, "d": 139.01, "e": 313.86, "f": 58.24, "g": 58.25 }, { "date": "2016/17", "region": "AR", "a": 1.46, "b": 41, "c": 0, "d": 7.5, "e": 11.2, "f": 25.99, "g": 5.28 }, { "date": "2016/17", "region": "BR", "a": 6.77, "b": 98.5, "c": 0.85, "d": 51, "e": 60.5, "f": 31.6, "g": 14.02 }, { "date": "2016/17", "region": "ZA", "a": 1.1, "b": 17.55, "c": 0, "d": 7.46, "e": 12.66, "f": 2.29, "g": 3.7 }, { "date": "2016/17", "region": "EG", "a": 2.22, "b": 6, "c": 8.77, "d": 12.7, "e": 15.1, "f": 0.01, "g": 1.89 }, { "date": "2016/17", "region": "JP", "a": 1.35, "b": 0, "c": 15.17, "d": 11.6, "e": 15.2, "f": 0, "g": 1.32 }, { "date": "2016/17", "region": "MX", "a": 5.21, "b": 27.58, "c": 14.57, "d": 22.5, "e": 40.4, "f": 1.54, "g": 5.42 }, { "date": "2016/17", "region": "KR", "a": 1.94, "b": 0.08, "c": 9.23, "d": 7.21, "e": 9.41, "f": 0, "g": 1.83 }, { "date": "2016/17", "region": "CA", "a": 2.45, "b": 13.89, "c": 0.85, "d": 7.5, "e": 13.1, "f": 1.52, "g": 2.57 }, { "date": "2016/17", "region": "CN", "a": 110.77, "b": 219.55, "c": 2.46, "d": 162, "e": 232, "f": 0.08, "g": 100.71 }, { "date": "2016/17", "region": "UA", "a": 1.39, "b": 27.97, "c": 0.03, "d": 5.1, "e": 6.5, "f": 21.33, "g": 1.55 }];
    var mapKey = 'a', mapDesc = 'Beginning Stocks';

    $("#mapDropdown").change(function () {
        var $selectedItem = $("option:selected", this);
        mapDesc = $selectedItem.text(),
        mapKey = this.value
        mapReady();
    });

    function mapReady() {
        var mapGeoJSON = Highcharts.maps['custom/world'];
        console.log(Highcharts, mapGeoJSON)
        var data = [];
        $.each(jsonData, function (index, item) {
            data.push({
                key: item['region'].toLowerCase(),
                value: item[mapKey]
            });
        });
        // Instantiate chart
        $("#mapcontainer").highcharts('Map', {
            title: {
                text: 'Corn ' + mapDesc + ' - 2016/17'
            },
            mapNavigation: {
                enabled: true
            },
            colorAxis: {
                min: 0,
                stops: [
					[0, '#FFFFBE'],
				    [0.25, '#FAD155'],
                    [0.5, '#FFA77F'],
                    [0.75, '#E64C00'],
                    [1, '#732600']
                ]
            },
            legend: {
                layout: 'vertical',
                align: 'left',
                verticalAlign: 'bottom'
            },
            series: [{
                data: data,
                mapData: mapGeoJSON,
                joinBy: ['hc-key', 'key'],
                name: mapDesc,
                states: {
                    hover: {
                        color: Highcharts.getOptions().colors[2]
                    }
                },
                dataLabels: {
                    enabled: true
                }
            }, {
                type: 'mapline',
                name: "Separators",
                data: Highcharts.geojson(mapGeoJSON, 'mapline'),
                nullColor: 'gray',
                showInLegend: false,
                enableMouseTracking: false
            }]
        });
    }


    if (Highcharts.maps['custom/world']) {
        mapReady();
    } else {
        $.getScript('https://code.highcharts.com/mapdata/custom/world.js', mapReady);
    }
});
