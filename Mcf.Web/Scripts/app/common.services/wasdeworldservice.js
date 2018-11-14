/// <reference path="../app.js" />
mcfapp.factory('wasdeworldservice', ['$resource', wasdeworldservice]);

function wasdeworldservice($resource) {
	return $resource(location.protocol + '//' + location.host + "/Api/Common", { Index: '@index', From: '@from', To: '@to' }, {
		getWASDEWorldFormattedData: {
			url: location.protocol + '//' + location.host + "/Api/Common/GetWASDEWorldFormattedData"
		},
		saveData: {
		    url: location.protocol + '//' + location.host + '/Api/Common/SaveData',
		    method: 'POST'
		}
	});
}