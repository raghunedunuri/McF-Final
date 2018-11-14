/// <reference path="../app.js" />
mcfapp.controller('homecontroller', ['$scope', '$resource', 'jobservice', 'uiGridConstants', '$timeout', homecontroller]);

function homecontroller($scope, $resource, jobservice, uiGridConstants,$timeout) {
    $scope.graphData = [];
    $scope.graphColors = ['#ffb400','#188018','#d14019','#a40541','#8100a1','#1e92af','#f3e220','#d40f0f'];
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
            name: 'jobName', displayName: 'Job Name', field: 'jobName',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 100

        },
        {
            name: 'startTime', displayName: 'Start Time', field: 'startTime',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 200
        },

        {
            name: 'endTime', displayName: 'End Time', field: 'endTime',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 200

        },

        {
            name: 'status', displayName: 'Status', field: 'status',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 100
        },

        {
            name: 'message', displayName: 'Message', field: 'message',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 150

        },

        {
            name: 'newRecords', displayName: 'New Records', field: 'newRecords',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 130

        },

        {
            name: 'newSymbols', displayName: 'New Symbols', field: 'newSymbols',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 130

        },

        {
            name: 'updatedRecords', displayName: 'Updated Records', field: 'updatedRecords',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 130
        },

        {
            name: 'unmappedRecords', displayName: 'Unmapped Records', field: 'unmappedRecords',
            filter: { condition: uiGridConstants.filter.CONTAINS }, minWidth: 160
        }
        ],
        data: []
    };


    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridPageingApi = gridApi;
    }

    $scope.GetJobSummary = function () {
        $scope.graphData.push(['JobStatus', 'Count']);
        $scope.jobSummary = jobservice.getJobSummary();
        $scope.jobSummary.$promise.then(function (data) {
            $scope.jobSummary = data.toJSON();
            $.each($scope.jobSummary, function (key, value) {
                $scope.graphData.push([key, value]);
            })
            $timeout(function () {
                google.setOnLoadCallback(DrawJobSummaryChart())
            }, 0);
            $scope.GetCurrentJobInfo('');
        }, function (error) {

        });
    }

    $scope.GetCurrentJobInfo = function (datasource) {
        $scope.allJobInfo = jobservice.getCurrentJobInfo({ datasource: datasource });
        $scope.allJobInfo.$promise.then(function (data) {
            $scope.allJobInfo = data;            
            $scope.gridOptions.data = $scope.allJobInfo;
                            
        }, function (error) {

        });
    }

    function DrawJobSummaryChart()
    {
        var graphData = new google.visualization.DataTable();       
        graphData = google.visualization.arrayToDataTable($scope.graphData);
        var options = {            
            tooltip: { 'text': 'value', 'showColorCode': 'true' },
            backgroundColor: { fill: 'white' },
            colors:$scope.graphColors,
            chartArea: { left: 30, top: 30, width: '80%', height: '86%' }
        };
        var chart = new google.visualization.ColumnChart(document.getElementById('columnchart'));
        chart.draw(graphData, options);
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