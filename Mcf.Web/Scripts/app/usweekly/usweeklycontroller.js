/// <reference path="../app.js" />
mcfapp.controller('usweeklycontroller', ['$scope', '$resource', 'jobservice', 'usweeklyservice', 'uiGridConstants', 'commonservice', usweeklycontroller]);

function usweeklycontroller($scope, $resource, jobservice, usweeklyservice, uiGridConstants, commonservice) {
    $scope.tablename = 'McF_Populate_USWeekly_Data';
    $scope.readOnlyHeaders = ['date','symbol','category'];
    $scope.gridOptions = {
        i18n: 'en',
        showGroupPanel: true,
        enableGridMenu: false,
        enableSorting: true,
        enableFiltering: true,
        //enablePaging: true,
        enableVerticalScrollbar: 0,
        enablePaginationControls: true,
        enableHorizontalScrollbar: 1,
        //enablePagination: true,
        paginationPageSize: 20,
        paginationPageSizes: [5, 10, 25, 50, 75, 100],
        rowHeight: 35,
        columnDefs: [
        
        {
            name: 'date', displayName: 'Date', field: 'date', enableCellEdit: false,
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 100

        },

        {
            name: 'symbol', displayName: 'Symbol', field: 'symbol', enableCellEdit: false,
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 200

        },
        {
            name: 'category', displayName: 'Category', field: 'category', enableCellEdit: false,
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 100
        },

        {
            name: 'net_Sales', displayName: 'Net Sales', field: 'net_Sales',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 100

        },

        {
            name: 'nxt_Mkt_year_Outstanding_Sales', displayName: 'Nxt Mkt Year Outstanding Sales', field: 'nxt_Mkt_year_Outstanding_Sales',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 300
        },

        {
            name: 'nxt_mkt_year_Net_Sales', displayName: 'Nxt Mkt Year Net Sales', field: 'nxt_mkt_year_Net_Sales',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 250

        },

        {
            name: 'outstanding_Sales', displayName: 'Outstanding Sales', field: 'outstanding_Sales',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 200

        },

        {
            name: 'weekly_Exports', displayName: 'Weekly Exports', field: 'weekly_Exports',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 200

        },
        {
            name: 'accumulated_Exports', displayName: 'Accumulated Exports', field: 'accumulated_Exports',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 200
        }
        ],
        data: []
    };


    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridPageingApi = gridApi;
        gridApi.rowEdit.on.saveRow($scope, $scope.UpdateGridData);
        gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
            $scope.updatedColumnName = colDef.name;
            $scope.updatedValue = newValue;
            $scope.oldValue = oldValue;
        });
    }

    $scope.GetCurrentJobInfo = function (datasource) {
        blockUI();
        $scope.datasource = datasource;
        $scope.usweeklyjobinfo = jobservice.getCurrentJobInfo({ datasource: datasource });
        $scope.usweeklyjobinfo.$promise.then(function (data) {
            $scope.usweeklyjobinfo = data[0];
            if ($scope.usweeklyjobinfo != undefined)
                $scope.usweeklyjobinfo.startTime = FormatDate($scope.usweeklyjobinfo.startTime);
            $scope.GetRawData();
        }, function (error) {
            unblockUI();
        });
    }

    $scope.GetUSWeeklyFormattedData = function () {
        $scope.category = $scope.category != undefined && $scope.category != '' ? $scope.category : "All";
        var from = $('#startDate')[0].value;
        var to = $('#endDate')[0].value;
        var index = $("#selduration option:selected").val();
        if (to != '')
            index = 0;
        $scope.getUsweeklyFormattedData = usweeklyservice.getUsweeklyFormattedData({ Category: $scope.category, Index: index, From: from, To: to });
        $scope.getUsweeklyFormattedData.$promise.then(function (data) {            
            $scope.gridOptions.data = data;
            unblockUI();
        }, function (error) {
            unblockUI();
        });
        $('#selduration option:eq(0)').prop('selected', true);
    }

    $scope.showItemData = function (url, cat) {
        $scope.category = cat;
        $scope.usweeklyframeurl = url;
        $scope.GetUSWeeklyFormattedData();
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

    $scope.ResetGridFilter = function()
    {
        $scope.category = 'All';
        $scope.GetUSWeeklyFormattedData();
    }

    $scope.GetRawData = function () {
        var arr = [];
        var count = 1;
        var tempArray = [];
        $scope.usweeklyRawData = usweeklyservice.getRawData();
        $scope.usweeklyRawData.$promise.then(function (data) {
            var arrData = Object.values(data);
            for (var i = 0; i < arrData.length; i++) {
                if (arrData[i].commodity_Name != undefined && arrData[i].commodity_Name != '')
                    tempArray.push(arrData[i]);
                if (count == 2) {
                    arr.push(tempArray);
                    tempArray = [];
                    count = 0;
                }
                count++;
            }
            $scope.usData = arr;
            $scope.GetUSWeeklyFormattedData();
        }, function (error) {
            console.log(error);
            unblockUI();
        });
    }

    function formatAMPM(datestr) {
        var date = new Date(datestr);
        var datepart = date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();
        var year = date.getFullYear();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = datepart + ' ' + hours + ':' + minutes + ':' + seconds + ' ' + ampm;
        return strTime;
    }

    function ShortDate(datestr) {
        var date = new Date(datestr);
        var datepart = date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();
        var year = date.getFullYear();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = datepart //+ ' ' + hours + ':' + minutes + ':' + seconds + ' ' + ampm;
        return strTime;
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