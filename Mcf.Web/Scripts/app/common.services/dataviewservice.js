/// <reference path="../app.js" />
mcfapp.factory('dataviewservice', ['$resource', dataviewservice]);

function dataviewservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Common", { datasource: '@datasource', rootsource: 'rootsource', index: '@index', from: '@from', to: '@to' }, {
        getFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/Common/GetCompleteFormatData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}