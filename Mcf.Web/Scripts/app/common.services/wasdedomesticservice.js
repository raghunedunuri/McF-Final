/// <reference path="../app.js" />
mcfapp.factory('wasdedomesticservice', ['$resource', wasdedomesticservice]);

function wasdedomesticservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Common", { Index: '@index', From: '@from', To: '@to' }, {
        getWASDEDomesticFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/Common/GetWASDEDomesticFormattedData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}