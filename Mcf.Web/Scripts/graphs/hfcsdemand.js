var json = [{"value":15.5,"year":1994},{"value":16.5,"year":1995},{"value":15.6,"year":1996},{"value":16.2,"year":1997},{"value":17.3,"year":1998},{"value":18,"year":1999},{"value":17.7,"year":2000},{"value":18.1,"year":2001},{"value":17.8,"year":2002},{"value":17.5,"year":2003},{"value":17.6,"year":2004},{"value": 17.5,"year": 2005},{"value": 16.5,"year": 2006},{"value": 16,"year": 2007},{"value": 15.6,"year": 2008},{"value": 15.4,"year":2009},{"value": 15,"year": 2010},{"value": 15,"year": 2011},{"value": 14.4,"year": 2012},{"value": 14,"year": 2013},{"value": 14.2,"year": 2014},{"value": 13.9,"year": 2015},{"value": 13.7,"year": 2016},{"value": 13.5,"year":"2017*"},{"value": 13.4,"year":"2018*"}];
		var _chart, categories = [], data = [];
		$(document).ready(function () {

			$.each(json, function (inde, item) {
				categories.push(item.year);
				data.push(item.value);
			});

			console.log(data, categories)
			function updateChart() {
				_chart = Highcharts.chart('mapcontainer', {
					chart: {
						type: 'spline',
					},
					title: {
						text: 'U.S. HFCS demand',
						style: {
							color: '#8B3910',
							fontSize: '36px',
							fontWeight: 'normal',
							fontFamily: 'Cordia New'
						}
					},
					xAxis: {
						categories: categories,
						labels: {
							overflow: 'justify'
						}
					},
					yAxis: {
						title: {
							text: 'Billion pounds, dry',
							style: {
								color: '#8B3910',
								fontSize: '24px',
								fontFamily: 'Cordia New'
							}
						},min:6
					},
					tooltip: {
						formatter: function () {
							var comment = (this.point.comment) ? '<b>' + this.point.comment + '</b>' : '';
							return '<b>' + this.x + '</b><br/>' +
								this.series.name + ': ' + this.y + '<br/>' + comment;
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
						spline: {
							lineWidth: 4,
							states: {
								hover: {
									lineWidth: 5
								}
							},
							marker: {
								enabled: false
							}
						}
					},
					series: [{
						id: '_1',
						name: 'U.S. HFCS demand',
						data: data,
						color: '#FFC000',
						marker: {
							enabled: false
						}
					}]
				});
			}

			updateChart();
		});	