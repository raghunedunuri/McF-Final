$(document).ready(function () {
			var categories = ['2005/06','2006/07','2007/08','2008/09','2009/10','2010/11','2011/12','2012/13', '2013/14', '2014/15', '2015/16', '2016/17*']; 
var data = { 
            '_1': [4, 3, 4, 7, 2, 4, 3, 4, 7, 2, 4, 2], 
			'_2': [5, 4, 4, 2, 5, 5, 4, 4, 2, 5, 2, 3], 
			'_3': [1, 5, 6, 2, 1, 1, 5, 6, 2, 1, 2, 4] 
			};

			function updateChart() {
				Highcharts.chart('mapcontainer', {
					chart: {
						type: 'column'
					},
					title: {
						text: 'Domestic beet & cane sugar deliveries',
						style: {
							color: '#8B3910',
							fontSize: '36px',
							fontWeight: 'normal',
							fontFamily: 'Cordia New'
						}
					},
					xAxis: {
						categories: categories
					},
					yAxis: {
						allowDecimals: false,
						min: 0,
						title: {
							text: 'Million short tons, raw value',
							style: {
								color: '#8B3910',
								fontSize: '24px',
								fontFamily: 'Cordia New'
							}
						},
						stackLabels: {
							enabled: true,
							style: {
								fontWeight: 'bold',
								color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
							},
						}
					},
					tooltip: {
						formatter: function () {
							return '<b>' + this.x + '</b><br/>' +
								this.series.name + ': ' + this.y + '<br/>' +
								'Total: ' + this.point.stackTotal;
						}
					},
					legend: {
						symbolWidth: 20,
						itemStyle: {
							color: '#000000',
							fontWeight: 'normal',
							fontSize: '13px'
						}
					},
					plotOptions: {
						column: {
							stacking: 'normal',
							dataLabels: {
								enabled: false,
								color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white'
							}
						}
					},
					series: [{
						id: '_1',
						name: 'Beet Sugar Domestic Consumption',
						data: data['_1'],
						stack: 'total',
						color: '#E48823'
					}, {
						id: '_2',
						name: 'Cane Sugar Domestic Consumption',
						data: data['_2'],
						stack: 'total',
						color: '#2A5F21'
					}, {
						id: '_3',
						name: 'Presumed Non-reporter Deliveries',
						data: data['_3'],
						stack: 'total',
						color: '#A41F0D'
					}]
				});
			}

			updateChart();
		});	