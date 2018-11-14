/// <reference path="../app.js" />
mcfapp.factory('cocoaservice', ['$resource', cocoaservice]);

function cocoaservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Common", { index: '@index', from: '@from', to: '@to' }, {
        getCocoaFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/Common/GetCocoaFormatedData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}