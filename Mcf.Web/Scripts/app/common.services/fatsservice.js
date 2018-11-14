/// <reference path="../app.js" />
mcfapp.factory('fatsservice', ['$resource', fatsservice]);

function fatsservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Common", { Index: '@index', From: '@from', To: '@to' }, {
        getFatsFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/Common/GetFOFormatedData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}