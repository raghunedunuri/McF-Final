/// <reference path="../app.js" />
mcfapp.factory('ethanolservice', ['$resource', ethanolservice]);

function ethanolservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Ethanol", { index: '@index', from: '@from', to: '@to' }, {
        getEthanolRawData: {
            url: location.protocol + '//' + location.host + "/Api/Ethanol/GetEthanolRawData"
        },
        getEthanolFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/Ethanol/GetEthanolFormattedData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}