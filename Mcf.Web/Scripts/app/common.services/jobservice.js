/// <reference path="../app.js" />
mcfapp.factory('jobservice', ['$resource', jobservice]);

function jobservice($resource) {
    return $resource(location.protocol + '//' + location.host + "/Api/Job", { datasource: '@datasource' }, {
        getCurrentJobInfo: {
            url: location.protocol + '//' + location.host + "/Api/Job/GetCurrentJobInfo",
            isArray: true
        },
        getJobSummary: {
            url: location.protocol + '//' + location.host + "/Api/Job/GetJobSummary"
        },
        saveData: {
            url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
            method: 'POST'
        }
    });
}