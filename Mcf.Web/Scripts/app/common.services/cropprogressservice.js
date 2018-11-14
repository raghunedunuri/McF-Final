/// <reference path="../app.js" />
mcfapp.factory('cropprogressservice', ['$resource', cropprogressservice]);

function cropprogressservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/CropProgress", { index: '@index', from: '@from', to: '@to' }, {
        getCropRawData: {
            url: location.protocol + '//' + location.host + "/Api/CropProgress/GetCropRawData"
        },
        getCropFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/CropProgress/GetCropFormattedData"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}