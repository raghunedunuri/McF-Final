/// <reference path="../app.js" />
mcfapp.controller('hogscontroller', ['$scope', '$resource', 'jobservice', 'hogsservice', 'uiGridConstants', 'commonservice', hogscontroller]);
function hogscontroller($scope, $resource, jobservice, hogsservice, uiGridConstants, commonservice) {
    $scope.gridOptions = [];

    $scope.GetCurrentJobInfo = function (datasource) {
        blockUI();
        $scope.datasource = datasource;
		$scope.hogsjobinfo = jobservice.getCurrentJobInfo({ datasource: datasource });
		$scope.hogsjobinfo.$promise.then(function (data) {
		    $scope.hogsjobinfo = data[0];
		    if ($scope.hogsjobinfo != undefined)
		        $scope.hogsjobinfo.startTime = FormatDate($scope.hogsjobinfo.startTime);
		    $scope.GetRawData();
		}, function (error) {
		    unblockUI();
		});
	}

	$scope.GetHogsFormattedData = function () {
	    $scope.gridOptions.length = 0;
	    $scope.tablename = '';
	    $scope.readOnlyHeaders = [];
	    var from = $('#startDate')[0].value;
	    var to = $('#endDate')[0].value;
	    var index = $("#selduration option:selected").val();
	    if (to != '')
	        index = 0;
	    $scope.hogsformatteddata = hogsservice.getHogsFormattedData({ Index: index, From: from, To: to });
	    $scope.hogsformatteddata.$promise.then(function (data) {
	        $scope.griddata = data.dictFormatedData;
	        var obj = $scope.griddata;
	        for (var key in obj) {
	            if (obj.hasOwnProperty(key)) {
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
	                if ($scope.tablename == '')
	                    $scope.tablename = $scope.griddata[key].commodityData.tableName;
	                for (var j = 0; j < $scope.griddata[key].headers.length; j++) {
	                    var units = '';
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
	        unblockUI();
	    }, function (error) {
            unblockUI();
	    });
	    $('#selduration option:eq(0)').prop('selected', true);
	}

	$scope.ClearFilters = function () {
	    $('#startDate')[0].value = '';
	    $('#endDate')[0].value = '';
	    $("#selduration").val("1");
	    $scope.GetHogsFormattedData();
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

	$scope.GetRawData = function () {
	    $('#table-container').jexcel({
	        // Full CSV URL
	        csv: '/DataFiles/Hogs/hgpg_all_tables.csv',
	        // Use the first row of your CSV as the headers
	        csvHeaders: false,
	        tableOverflow: true,
	        // Headers
	        colWidths: [70, 200, 1000, 100, 100, 100, 100, 100, 100, 100, 70, 200, 300, 100, 100, 100, 100, 100, 100, 100],
	    });
	    //Set readonly a specific column
	    $('#table-container').jexcel('updateSettings', {
	        cells: function (cell, col, row) {
	            $(cell).addClass('readonly');
	        }
	    });
	    $scope.GetHogsFormattedData();
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