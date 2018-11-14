/// <reference path="../app.js" />
mcfapp.factory('usweeklyservice', ['$resource', usweeklyservice]);

function usweeklyservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/USWeekly", { Category: '@category', Index: '@index', From: '@from', To: '@to' }, {
        getRawData: {
            url: location.protocol + '//' + location.host + "/Api/USWeekly/GetRawData",
            isArray: false
        },
        getUsweeklyFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/USWeekly/GetUSweeklyFormattedData",
            isArray: true
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}