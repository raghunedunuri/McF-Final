/// <reference path="../app.js" />
mcfapp.controller('wasdedomesticcontroller', ['$scope', 'jobservice', 'wasdedomesticservice', 'uiGridConstants', 'commonservice', wasdedomesticcontroller]);

function wasdedomesticcontroller($scope, jobservice, wasdedomesticservice, uiGridConstants, commonservice) {
    $scope.gridOptions = [];

    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridPageingApi = gridApi;
        gridApi.rowEdit.on.saveRow($scope, $scope.UpdateGridData);
        gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
            $scope.updatedColumnName = colDef.name;
            $scope.updatedValue = newValue;
        });
    }

    $scope.UpdateGridData = function (rowEntity) {
        var newObjArr = [];
        var headerObjArr = [];
        var newObj = {};
        $.each($scope.readOnlyHeaders, function (index, value) {
            var headerObj = {};
            headerObj.Header = $scope.readOnlyHeaders[index];
            headerObj.Value = rowEntity[value];
            headerObjArr.push(headerObj);
        })
        newObj.Header = $scope.updatedColumnName;
        newObj.Value = $scope.updatedValue;
        newObjArr.push(newObj);
        var dsUpdatedData = {
            DataSource: $scope.datasource,
            KeyData: headerObjArr,
            Table: $scope.tablename,
            ValueData: newObjArr
        }
        commonservice.saveData({}, dsUpdatedData).$promise.then(function (data) {
        }, function (error) {
        });
    };

    $scope.GetCurrentJobInfo = function (datasource) {
        blockUI();
        $scope.datasource = datasource;
        $scope.wasdedomesticjobinfo = jobservice.getCurrentJobInfo({ datasource: datasource });
        $scope.wasdedomesticjobinfo.$promise.then(function (data) {
            $scope.wasdedomesticjobinfo = data[0];
            $scope.wasdedomesticjobinfo.startTime = FormatDate($scope.wasdedomesticjobinfo.startTime);
            $scope.wasdedomesticjobinfo.endTime = FormatDate($scope.wasdedomesticjobinfo.endTime);
            $scope.GetRawData();
        }, function (error) {
            unblockUI();
        });
    }

    $scope.GetRawData = function () {
        //$scope.cotRawData = cotservice.getRawData();
        //$scope.cotRawData.$promise.then(function (data) {
        //    $scope.cotRawData.url = 'https://view.officeapps.live.com/op/embed.aspx?src=' + data.url;
        //    $scope.GetCOTFormattedData();
        //}, function (error) {
        //});
        $scope.wasdedomesticFrameUrl = 'https://view.officeapps.live.com/op/embed.aspx?src=http://usda.mannlib.cornell.edu/usda/current/wasde/wasde-07-12-2018.xls';
        $scope.GetWASDEDomesticFormattedData();
    }

    $scope.ClearFilters = function () {
        $('#startDate')[0].value = '';
        $('#endDate')[0].value = '';
        $("#selduration").val("1");
        $scope.GetWASDEDomesticFormattedData();
    }

    $scope.GetWASDEDomesticFormattedData = function () {
        $scope.gridOptions.length = 0;
        $scope.tablename = '';
        $scope.readOnlyHeaders = [];
        var i = 1;
        var from = $('#startDate')[0].value;
        var to = $('#endDate')[0].value;
        var index = $("#selduration option:selected").val();
        if (to != '')
            index = 0;
        $scope.wasdedomesticformatteddata = wasdedomesticservice.getWASDEDomesticFormattedData({ Index: index, From: from, To: to });
        $scope.wasdedomesticformatteddata.$promise.then(function (data) {
            $scope.griddata = data.dictFormatedData;
            var obj = $scope.griddata;
            for (var key in obj) {
                if (obj.hasOwnProperty(key)) {
                    var grioptionsvar = "options";
                    $scope.grioptionsvar = {
                        i18n: 'en',
                        showGroupPanel: true,
                        enableGridMenu: true,
                        enableSorting: true,
                        enableFiltering: true,
                        exporterMenuPdf: false,
                        exporterMenuExcel: false,
                        //enablePaging: true,
                        enableVerticalScrollbar: 0,
                        enablePaginationControls: true,
                        enableHorizontalScrollbar: 1,
                        //enablePagination: true,
                        paginationPageSize: 10,
                        paginationPageSizes: [5, 10, 25, 50, 75, 100],
                        rowHeight: 35,
                        columnDefs: [],
                        header: {},
                        data: []
                    };

                    $scope.grioptionsvar.onRegisterApi = function (gridApi) {
                        $scope.gridPageingApi = gridApi;
                        gridApi.rowEdit.on.saveRow($scope, $scope.UpdateGridData);
                        gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                            $scope.updatedColumnName = colDef.field;
                            $scope.updatedValue = newValue;
                            $scope.oldValue = oldValue;
                        });
                    }

                    $scope.grioptionsvar.header.name = $scope.griddata[key].commodityData.name;
                    $scope.grioptionsvar.header.unit = $scope.griddata[key].commodityData.unit;
                    $scope.grioptionsvar.header.id = i++;
                    if ($scope.tablename == '')
                        $scope.tablename = $scope.griddata[key].commodityData.tableName;
                    for (var j = 0; j < $scope.griddata[key].headers.length; j++) {
                        var units = '';
                        if ($scope.griddata[key].commodityData.unit.length == 0)
                            units = $scope.griddata[key].headers[j].unit != undefined && $scope.griddata[key].headers[j].unit != '' ? '(' + $scope.griddata[key].headers[j].unit + ')' : '';
                        $scope.grioptionsvar.columnDefs.push({
                            name: $scope.griddata[key].headers[j].displayName,
                            field: $scope.griddata[key].headers[j].name,
                            width: $scope.griddata[key].headers[j].displayName.length < 10 ? $scope.griddata[key].headers[j].displayName.length * 20 : $scope.griddata[key].headers[j].displayName.length * 10,
                            visible: j < 15 ? true : false,
                            headerCellTemplate: '<div ng-class="{ \'sortable\': sortable }">' +
                                                '<span style="padding-left:10px">' + $scope.griddata[key].headers[j].displayName + '</span>' +
                                                '<br>' + '<span style="padding-left:10px">' + units + '</span>' +
                                                '<div class="ui-grid-vertical-bar">&nbsp;</div>' +
                                                '<div col-index="renderIndex">' +
                                                '</span>' +
                                                '</div>' +
                                                '<div class="ui-grid-column-menu-button" ng-if="grid.options.enableColumnMenus && !col.isRowHeader  && col.colDef.enableColumnMenu !== false" class="ui-grid-column-menu-button" ng-click="toggleMenu($event)">' +
                                                '<i class="ui-grid-icon-angle-down">&nbsp;</i>' +
                                                '</div>' +
                                                '<div ng-if="filterable" class="ui-grid-filter-container" ng-repeat="colFilter in col.filters">' +
                                                '<input type="text" class="ui-grid-filter-input" ng-model="colFilter.term"' +
                                                '<div class="ui-grid-filter-button" ng-click="colFilter.term = null">' +
                                                '</div>' +
                                                '</div>' +
                                                '</div>',
                            type: 'string',
                            enableCellEdit: !$scope.griddata[key].headers[j].readOnly,
                            filter: { condition: uiGridConstants.filter.CONTAINS }
                        });
                        if ($scope.griddata[key].headers[j].readOnly == true) {
                            $scope.readOnlyHeaders.push($scope.griddata[key].headers[j].name);
                        }
                    }
                    for (var k = 0; k < $scope.griddata[key].values.length; k++) {
                        var valueobj = {};
                        for (var l = 0; l < $scope.griddata[key].values[k].length; l++) {
                            valueobj[$scope.griddata[key].headers[l].name] = ($scope.griddata[key].values[k][l]);
                        }
                        $scope.grioptionsvar.data.push(valueobj);
                    }
                    $scope.gridOptions.push($scope.grioptionsvar);
                    $scope.grioptionsvar = {};
                }
            }
            setTimeout(function () {
                $('#selDomesticCommodity').val($scope.gridOptions[0].header.name).change();
                unblockUI();
            }, 1000);
        }, function (error) {
            unblockUI();
        });
    }

    function ShortDate(datestr) {
        var date = new Date(datestr);
        if (!isNaN(date)) {
            var datepart = date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();
            var year = date.getFullYear();
            var hours = date.getHours();
            var minutes = date.getMinutes();
            var seconds = date.getSeconds();
            var ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            minutes = minutes < 10 ? '0' + minutes : minutes;
            var strTime = datepart //+ ' ' + hours + ':' + minutes + ':' + seconds + ' ' + ampm;}
            return strTime
        }
        return datestr;
    }

    function FormatDate(datestr) {
        var d = new Date(datestr);
        return (
            ("00" + (d.getMonth() + 1)).slice(-2) + "/" +
            ("00" + d.getDate()).slice(-2) + "/" +
            d.getFullYear() + " " +
            ("00" + d.getHours()).slice(-2) + ":" +
            ("00" + d.getMinutes()).slice(-2) + ":" +
            ("00" + d.getSeconds()).slice(-2)
        );
    }
}