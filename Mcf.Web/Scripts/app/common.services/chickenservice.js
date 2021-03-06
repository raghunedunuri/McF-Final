﻿/// <reference path="../app.js" />
mcfapp.factory('chickenservice', ['$resource', chickenservice]);

function chickenservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Common", { Index: '@index', From: '@from', To: '@to' }, {
        getFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/Common/GetFormattedData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}