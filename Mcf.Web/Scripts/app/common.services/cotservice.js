/// <reference path="../app.js" />
mcfapp.factory('cotservice', ['$resource', cotservice]);

function cotservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/COT", { index: '@index', from: '@from', to: '@to' }, {
        getRawData: {
            url: location.protocol + '//' + location.host + "/Api/COT/GetRawData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        },
        getCOTFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/COT/GetCOTFormattedData"
        },
    });
}