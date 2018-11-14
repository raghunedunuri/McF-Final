$(document).ready(function () {
    var data = [{ "category": "Non_HFCS_Plants", "name": "Cargill", "city": "Ft.Dodge", "state_": "IA", "lat": 42.4974694, "lon": -94.1680158 }, { "category": "Non_HFCS_Plants", "name": "Cargill", "city": "Hammond", "state_": "IN", "lat": 41.5833688, "lon": -87.5000412 }, { "category": "Non_HFCS_Plants", "name": "Grain Processing", "city": "Washington", "state_": "IN", "lat": 38.6592152, "lon": -87.172789 }, { "category": "Non_HFCS_Plants", "name": "Grain Processing", "city": "Muscatine", "state_": "IA", "lat": 41.424473, "lon": -91.0432051 }, { "category": "Non_HFCS_Plants", "name": "Ingredion", "city": "Indianapolis", "state_": "IN", "lat": 39.768403, "lon": -86.158068 }, { "category": "Non_HFCS_Plants", "name": "Ingredion", "city": "CedarRapids", "state_": "IA", "lat": 41.9778795, "lon": -91.6656232 }, { "category": "Non_HFCS_Plants", "name": "Ingredion", "city": "KansasCity", "state_": "MO", "lat": 39.0997265, "lon": -94.5785667 }, { "category": "HFCS_Plants", "name": "ADM", "city": "Marshall", "state_": "MN", "lat": 44.448423, "lon": -95.7911916 }, { "category": "HFCS_Plants", "name": "ADM", "city": "Columbus", "state_": "NE", "lat": 41.4302973, "lon": -97.3593904 }, { "category": "HFCS_Plants", "name": "ADM", "city": "CedarRapids", "state_": "IA", "lat": 41.9778795, "lon": -91.6656232 }, { "category": "HFCS_Plants", "name": "ADM", "city": "Clinton", "state_": "IA", "lat": 41.8444735, "lon": -90.1887379 }, { "category": "HFCS_Plants", "name": "ADM", "city": "Decautur", "state_": "IL", "lat": 39.8403147, "lon": -88.9548001 }, { "category": "HFCS_Plants", "name": "Cargill", "city": "Wahpeton", "state_": "ND", "lat": 46.2652367, "lon": -96.6059072 }, { "category": "HFCS_Plants", "name": "Cargill", "city": "Blair", "state_": "NE", "lat": 41.5446975, "lon": -96.1350702 }, { "category": "HFCS_Plants", "name": "Cargill", "city": "Eddyville", "state_": "IA", "lat": 41.1605646, "lon": -92.6313031 }, { "category": "HFCS_Plants", "name": "Cargill", "city": "CedarRapids", "state_": "IA", "lat": 41.9778795, "lon": -91.6656232 }, { "category": "HFCS_Plants", "name": "Cargill", "city": "Dayton", "state_": "OH", "lat": 39.7589478, "lon": -84.1916069 }, { "category": "HFCS_Plants", "name": "Ingredion", "city": "Argo", "state_": "IL", "lat": 41.7880876, "lon": -87.810334 }, { "category": "HFCS_Plants", "name": "Ingredion", "city": "Winston-Salem", "state_": "NC", "lat": 36.0998596, "lon": -80.244216 }, { "category": "HFCS_Plants", "name": "Ingredion", "city": "Stockton", "state_": "CA", "lat": 37.9577016, "lon": -121.2907796 }, { "category": "HFCS_Plants", "name": "Roquette", "city": "Keokuk", "state_": "IA", "lat": 40.4044731, "lon": -91.3963966 }, { "category": "HFCS_Plants", "name": "Tate&Lyle", "city": "Decautur", "state_": "IL", "lat": 39.8403147, "lon": -88.9548001 }, { "category": "HFCS_Plants", "name": "Tate&Lyle", "city": "Lafeyette", "state_": "IN", "lat": 40.4167022, "lon": -86.8752869 }, { "category": "HFCS_Plants", "name": "Tate&Lyle", "city": "Loudon", "state_": "TN", "lat": 35.7328541, "lon": -84.3338112 }];
    var mapData = {};
    $.each(data, function (index, item) {
        var object = mapData[item.name + '  ' + item.category];
        if (!object) {
            object = [];
            mapData[item.name + '  ' + item.category] = object;
        }
        object.push(item);
    });

    var chartOptions = {
        title: {
            text: 'U.S. Corn wet mills',
            style: {
                color: '#8B3910',
                fontSize: '36px',
                fontFamily: 'Cordia New'
            }
        },
        legend: {
            symbolWidth: 10,
            itemStyle: {
                color: '#000000',
                fontWeight: 'normal',
                fontSize: '13px'
            }
        },
        tooltip: { enabled: false },
        mapNavigation: {
            enabled: true
        },
        series: [{
            name: 'Basemap',
            mapData: Highcharts.maps['countries/us/us-all'],
            showInLegend: false
        }]
    };

    $.each(mapData, function (key, value) {
        chartOptions.series.push({
            type: 'mappoint',
            name: key,
            showInLegend: true,
            data: value,
            marker: { symbol: 'circle', radius: 5 },
            dataLabels: {
                enabled: true,
                formatter: function () {
                    return this.point.city + ', ' + this.point.state_;
                },
                style: {
                    fontWeight: 'normal',
                    fontSize: '13px'
                }
            }
        });
    });

    Highcharts.mapChart('mapcontainer', chartOptions);
});