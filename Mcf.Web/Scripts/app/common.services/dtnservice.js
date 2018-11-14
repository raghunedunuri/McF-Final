/// <reference path="../app.js" />
mcfapp.factory('dtnservice', ['$resource', dtnservice]);

function dtnservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/DTN", { index: '@index', from: '@from', to: '@to' }, {
        getDTNFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/DTN/GetDTNFormatedData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}