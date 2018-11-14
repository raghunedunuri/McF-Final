$(document).ready(function () {
  
    var data = [{ "name": "National Sugar Marketing", "city": "Brawley", "state_": "CA", "country": "US", "lat": 32.9786566, "lon": -115.530267 }, { "name": "National Sugar Marketing", "city": "Nampa", "state_": "ID", "country": "US", "lat": 43.5788175, "lon": -116.55978 }, { "name": "National Sugar Marketing", "city": "Twin Falls", "state_": "ID", "country": "US", "lat": 42.5558381, "lon": -114.4700518 }, { "name": "National Sugar Marketing", "city": "Mini-Cassia", "state_": "ID", "country": "US", "lat": 42.532491, "lon": -113.80417 }, { "name": "National Sugar Marketing", "city": "Renville", "state_": "MN", "country": "US", "lat": 44.7891264, "lon": -95.2116731 }, { "name": "Cargill", "city": "Gramercy", "state_": "LA", "country": "US", "lat": 30.0474239, "lon": -90.6898128 }, { "name": "Domino Foods", "city": "Crockett", "state_": "CA", "country": "US", "lat": 38.0524208, "lon": -122.2130236 }, { "name": "Domino Foods", "city": "Chalmette", "state_": "LA", "country": "US", "lat": 29.942783, "lon": -89.9635162 }, { "name": "Domino Foods", "city": "Baltimore", "state_": "MD", "country": "US", "lat": 39.2903848, "lon": -76.6121893 }, { "name": "Domino Foods", "city": "Yonkers", "state_": "NY", "country": "US", "lat": 40.9312099, "lon": -73.8987469 }, { "name": "Domino Foods", "city": "Okeelanta", "state_": "FL", "country": "US", "lat": 26.609444, "lon": -80.711667 }, { "name": "Louis Dreyfus (Imperial Sugar)", "city": "Savannah", "state_": "GA", "country": "US", "lat": 32.0808989, "lon": -81.091203 }, { "name": "Michigan Sugar", "city": "Bay City", "state_": "MI", "country": "US", "lat": 43.5944677, "lon": -83.8888647 }, { "name": "Michigan Sugar", "city": "Caro", "state_": "MI", "country": "US", "lat": 43.4911322, "lon": -83.396897 }, { "name": "Michigan Sugar", "city": "Sebewaing", "state_": "MI", "country": "US", "lat": 43.7322394, "lon": -83.4510724 }, { "name": "Michigan Sugar", "city": "Croswell", "state_": "MI", "country": "US", "lat": 43.2755809, "lon": -82.621038 }, { "name": "Michigan Sugar", "city": "Taylor", "state_": "MI", "country": "US", "lat": 42.240872, "lon": -83.2696509 }, { "name": "United Sugars", "city": "Worland", "state_": "WY", "country": "US", "lat": 44.0169014, "lon": -107.9553721 }, { "name": "United Sugars", "city": "Sidney", "state_": "MT", "country": "US", "lat": 47.7166836, "lon": -104.1563253 }, { "name": "United Sugars", "city": "East Grand Forks", "state_": "MN", "country": "US", "lat": 47.9311871, "lon": -97.009347 }, { "name": "United Sugars", "city": "Hillsboro", "state_": "ND", "country": "US", "lat": 47.371029, "lon": -97.0315309 }, { "name": "United Sugars", "city": "Moorhead", "state_": "MN", "country": "US", "lat": 46.8737648, "lon": -96.7678039 }, { "name": "United Sugars", "city": "Wahpeton", "state_": "ND", "country": "US", "lat": 46.1546779, "lon": -96.8958656 }, { "name": "United Sugars", "city": "Crookston", "state_": "MN", "country": "US", "lat": 47.7745617, "lon": -96.6093911, }, { "name": "United Sugars", "city": "Drayton", "state_": "ND", "country": "US", "lat": 48.5950979, "lon": -97.1940786 }, { "name": "United Sugars", "city": "Clewiston", "state_": "LA", "country": "US", "lat": 26.7542312, "lon": -80.9336753 }, { "name": "Western Sugar", "city": "Billings", "state_": "MT", "country": "US", "lat": 45.7832856, "lon": -108.5006904 }, { "name": "Western Sugar", "city": "Lovell", "state_": "WY", "country": "US", "lat": 44.8374532, "lon": -108.3895614 }, { "name": "Western Sugar", "city": "Scottsbluff", "state_": "NE", "country": "US", "lat": 41.8666341, "lon": -103.6671662 }, { "name": "Western Sugar", "city": "Fort Morgan", "state_": "CO", "country": "US", "lat": 40.2502582, "lon": -103.7999509 }];
    var mapData = {};
    $.each(data, function (index, item) {
        var object = mapData[item.name];
        if (!object) {
            object = [];
            mapData[item.name] = object;
        }
        object.push(item);
    });

    var chartOptions = {
        title: {
            text: 'U.S Sugar Companies',
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
            marker: {
                symbol: 'circle',
                radius: 5
            },
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
