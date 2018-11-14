/// <reference path="../app.js" />
mcfapp.controller('layoutcontroller', ['$scope', 'commonservice', layoutcontroller]);

function layoutcontroller($scope, commonservice) {
    $scope.populateDataSources = function () {
        $scope.datasources = commonservice.getAllDataSources();
        $scope.datasources.$promise.then(function (data) {
            $scope.datasources = data;
        }, function (error) {
        });
    }

    $scope.getRedirectionURL = function(ds)
    {
        return '@Url.Action("Index",' + ds.dataSource + ')';
    }

}