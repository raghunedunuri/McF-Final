/// <reference path="../app.js" />
mcfapp.factory('cattleservice', ['$resource', cattleservice]);

function cattleservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Common", { index: '@index', from: '@from', to: '@to' }, {
        getCattleFormattedData: {
            url: location.protocol + '//' + location.host + "/Api/Common/GetCattleFormatedData"
        }
    });
}