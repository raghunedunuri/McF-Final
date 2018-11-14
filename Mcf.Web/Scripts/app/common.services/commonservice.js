/// <reference path="../app.js" />
mcfapp.factory('commonservice', ['$resource', commonservice]);

function commonservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Common", {}, {
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        },
        saveDataSource: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveDataSource',
            method: 'POST'
        },
        getAllDataSources: {
            url: location.protocol + '//' + location.host + '/Api/Common/GetAllDataSources',
            isArray: true
        }
    });
}