/// <reference path="../app.js" />
mcfapp.controller('datasourcecontroller', ['$scope', '$timeout', 'commonservice', datasourcecontroller]);

function datasourcecontroller($scope, $timeout, commonservice) {
    $scope.gridOptions = {
        i18n: 'en',
        rowEditWaitInterval: -1,
        showGroupPanel: false,
        enableGridMenu: false,
        enableSorting: false,
        enableFiltering: false,
        //enablePaging: true,
        enableVerticalScrollbar: 1,
        enablePaginationControls: false,
        enableHorizontalScrollbar: 1,
        //enablePagination: true,
        paginationPageSize: 20,
        paginationPageSizes: [5, 10, 25, 50, 75, 100],
        rowHeight: 35,
        columnDefs: [],
        data: []
    };

    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.AddHeader = function(header)
    {
        $scope.header.name = '';
        $timeout(function () {
            var obj = {};
            obj.name = header;
            obj.displayName = header;
            obj.field = header;
            obj.enableCellEdit = true;
            obj.type = $("#selDataTypes option:selected").val();
            $scope.gridOptions.columnDefs.push(obj);
        })
    }

    $scope.AddRow = function ()
    {
        var newRow = {};
        $timeout(function () {
            $scope.gridOptions.data.push(newRow);
        });
    }

    $scope.SaveGrid = function () {
        var dt = {};
        dt.name = $scope.datasource.name;
        dt.headers = $scope.populateHeaders();
        dt.values = $scope.populateValues();
        commonservice.saveDataSource({}, dt).$promise.then(function (data) {
        }, function (error) {
        });
    }

    $scope.populateHeaders = function()
    {
        var headers = [];
        $.each($scope.gridOptions.columnDefs, function (index, item) {
            var header = {}
            header.name = item.name;
            header.type = item.type;
            headers.push(header);
        })
        return headers;
    }

    $scope.populateValues = function () {
        var values = [];
        for (var key in $scope.gridOptions.data) {
            var temparr = [];
            for (var item in $scope.gridOptions.data[key]) {
                if (Object.prototype.hasOwnProperty.call($scope.gridOptions.data[key], item)) {
                    if (item === "$$hashKey") {
                        continue;
                    }
                    temparr.push($scope.gridOptions.data[key][item]);
                }
            }
            values.push(temparr);
        }
        return values;
    }
}