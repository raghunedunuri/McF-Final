/// <reference path="../app.js" />
mcfapp.factory('broilerservice', ['$resource', broilerservice]);

function broilerservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Common", { index: '@index', from: '@from', to: '@to' }, {
        getBroilerFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/Common/GetBroilerFormatedData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}