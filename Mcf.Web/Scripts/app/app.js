/// <reference path="../angular.js" />
var mcfapp = angular.module('mcfapp', ['ngResource', 'ui.grid', 'ui.grid.pagination', 'ui.grid.edit', 'ui.grid.rowEdit', 'ui.grid.cellNav', 'ui.grid.resizeColumns', 'ui.grid.selection', 'ui.grid.exporter', 'ui.bootstrap']);

mcfapp.filter('trusted', ['$sce', function ($sce) {
    return $sce.trustAsResourceUrl;
}]);