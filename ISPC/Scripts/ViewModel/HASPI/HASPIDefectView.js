$(function () {
    $(".datetimepicker").datetimepicker({
        format: "YYYY-MM-DD HH:mm"
    });
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });

    $("#DefectModel").modal({
        show: true,
        backdrop: "static"
    }); 
    $('#LocationContainer').highcharts({
        chart: {
            type: 'column',
            borderColor: '#EBBA95',
            borderWidth: 2
        },
        title: {
            text: 'Counts By Top Ten Component With Detail'
        },
        xAxis: {
           
            title: {
                text: 'Component',
                align: 'high'
            }
        },
        yAxis: {
            allowDecimals: false,
            min: 0,
            title: {
                text: 'Counts',
                align: 'high'
            },
            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.x + '</b><br/>' +
                    this.series.name + ': ' + this.y + '<br/>' +
                    'Total: ' + this.point.stackTotal;
            }
        },
        credits: {
            enabled: false
        },
        plotOptions: {
            column: {
                stacking: 'normal'
            }
        }
    });

    $("#DefectPieChart").highcharts({
        chart: {
            type: 'pie',
            option3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            },
            borderColor: '#EBBA95',
            borderWidth: 2,
            
        },
        title: {
            text: 'Defect Pad Count Pie Chart'
        },
        tooltip: {
            pointFormat: '{series.name}:<strong>{point.percentage: .2f}%</strong>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                depth: 35,
                dataLabels: {
                    enabled: true,
                    format: '{point.name}'
                }
            }
        },
        credits: {
            enabled: false
        }
    });

    ko.bindingHandlers.replot = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var bindings = allBindings();
            var type = ko.utils.unwrapObservable(bindings.type);
            var options = {
                chart: {
                    type: 'column'
                },
                labels: {
                    enabled: false,
                    style: {
                        fontSize: "5px",
                        margin: 0
                    }
                },
                yAxis: {                          
                    title:{
                        enabled: false
                    },                   
                    lineColor: 'black'
                },
                legend: {
                    align: 'center',
                    margin: 0,
                    enabled: false
                },
                exporting: {
                    enabled: false
                },
                title: {
                    text: 'Top 5 Excessive',
                    style: {
                        fontSize: '15px'
                    }
                },
                credits: {
                    enabled: false
                }    
            };
            switch (type) {
                case "ExcessiveChart":
                    options.title.text = "Top 5 Excessive";
                    break;
                case "InsufficientChart":
                    options.title.text = "Top 5 Insufficient";
                    break;
                case "LowAreaChart":
                    options.title.text = "Top 5 LowArea";
                    break;
                case "HighAreaChart":
                    options.title.text = "Top 5 HighArea";
                    break;
                case "LowerHeightChart":
                    options.title.text = "Top 5 LowerHeight";
                    break;
                case "UpperHeightChart":
                    options.title.text = "Top 5 UpperHeight";
                    break;
                case "ShapeChart":
                    options.title.text = "Top 5 UpperHeight";
                    break;
                case "PositionChart":
                    options.title.text = "Top 5 Position";
                    break;
                case "BridgingChart":
                    options.title.text = "Top 5 Bridging";
                    break;
                case "SmearChart":
                    options.title.text = "Top 5 Smear";
                    break;
            }
            $(element).highcharts(options);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var data = ko.utils.unwrapObservable(valueAccessor());
            var bindings = allBindings();
            var CompNameList = new Array();
            var RelatedCounts = new Array();
            var type = ko.utils.unwrapObservable(bindings.type);
            $.each(data, function (k, v) {
                CompNameList.push(k);
                RelatedCounts.push(v);
            });
            var bindings = allBindings();
            var type = ko.utils.unwrapObservable(bindings.type);
            var chart = $(element).highcharts();
            var series = chart.series;
            while (series.length > 0) {
                series[0].remove(false);
            }
            chart.xAxis[0].setCategories(CompNameList, false);
            chart.addSeries({
                name: type,
                data: RelatedCounts
            }, false);
            chart.redraw();
        }

    };
    ko.applyBindings(new DefectViewModel());
});

var DefectViewModel = function () {
    var self = this;
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.availableProjects = ko.observableArray([]);
    self.availableModels = ko.observableArray([]);
    self.availableLines = ko.observableArray([]);
    self.availableSPIStations = ko.observableArray([]);

    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.selectedProjectTable = ko.observable();
    self.selectedModelTable = ko.observable();
    self.selectedLineTable = ko.observable();
    self.selectedSPIStationTable = ko.observable();
    self.sStartTime = ko.observable();
    self.sEndTime = ko.observable();
    self.CountByTopTenLocationCategories = ko.observableArray([]);
    var opts = {            
        lines: 13, // 
        length: 20, // 花瓣长度
        width: 10, // 花瓣宽度
        radius: 30, // 花瓣距中心半径
        corners: 1, // 花瓣圆滑度 (0-1)
        rotate: 0, // 花瓣旋转角度
        direction: 1, // 花瓣旋转方向 1: 顺时针, -1: 逆时针
        color: '#000', // 花瓣颜色
        speed: 1, // 花瓣旋转速度
        trail: 60, // 花瓣旋转时的拖影(百分比)
        shadow: true, // 花瓣是否显示阴影
        hwaccel: false, //spinner 是否启用硬件加速及高速旋转            
        className: 'spinner', // spinner css 样式名称
        zIndex: 2e9, // spinner的z轴 (默认是2000000000)
        top: '50%', // spinner 相对父容器Top定位 单位 px
        left: '50%'// spinner 相对父容器Left定位 单位 px
    };
    var spinner = new Spinner(opts);

    //DefectView data collection except count by ten location series
    self.DefectViewDataCollection = {
        DefectTypeCountPercent: ko.observable(),
        TotalPanelCounts: ko.observable(),
        GoodPanelCounts: ko.observable(),
        NGPanelCounts: ko.observable(),
        PassPanelCounts: ko.observable(),
        WarningPanelCounts: ko.observable(),
        TotalDefectPadCounts: ko.observable(),
        TopFiveExcessiveComp: ko.observableArray([]),
        TopFiveInsufficientComp: ko.observableArray([]),
        TopFiveHighAreaComp: ko.observableArray([]),
        TopFiveLowAreaComp: ko.observableArray([]),
        TopFiveLowerHeightComp: ko.observableArray([]),
        TopFiveUpperHeightComp: ko.observableArray([]),
        TopFivePositionComp: ko.observableArray([]),
        TopFiveBridgingComp: ko.observableArray([]),
        TopFiveSmearComp: ko.observableArray([]),
        TopFiveShapeComp: ko.observableArray([])

    };
    self.CountByTopTenLocationSeries = {
        ExcessiveCountsArray: ko.observableArray([]),
        InsufficientCountsArray: ko.observableArray([]),
        LowAreaCountsArray: ko.observableArray([]),
        HighAreaCountsArray: ko.observableArray([]),
        LowerHeightCountsArray: ko.observableArray([]),
        UpperHeightCountsArray: ko.observableArray([]),
        ShapeCountsArray: ko.observableArray([]),
        PositionCountsArray: ko.observableArray([]),
        BridgingCountsArray: ko.observableArray([]),
        SmearCountsArray: ko.observableArray([])
    };
    var loadBuildings = function () {
        $.ajax({
            url: "/Common/getBuildings",
            type: "JSON",
            dataType: "json",
            async: true,
            success: function (data) {
                self.availableBuildings(ko.toJS(data));
            }
        });
    };
    self.loadSegments = function (Segment_Id) {
        if (self.selectedBuildingTable()) {
            $.ajax({
                url: "/Common/getSegments",
                type: "POST",
                data: { Building_Id: self.selectedBuildingTable() },
                dataType: "json",
                success: function (data) {
                    self.availableSegments(ko.toJS(data));
                    self.selectedSegmentTable(Segment_Id);
                }
            });
        }
        else {
            self.availableSegments([]);
        }
    };
    self.loadLines = function () {
        self.loadProjects();
        if (self.selectedSegmentTable()) {
            $.ajax({
                url: "/Common/GetLines",
                type: "POST",
                data: { Segment_Id: self.selectedSegmentTable() },
                dataType: "json",
                success: function (data) {
                    self.availableLines(ko.toJS(data));
                }
            });
        }
        else {
            self.availableLines([]);
        }
    };
    self.loadSPIStations = function () {
        if (self.selectedLineTable()) {
            $.ajax({
                url: "/Common/GetSPIStations",
                type: "POST",
                data: { Line_Id: self.selectedLineTable() },
                dataType: "json",
                success: function (data) {
                    self.availableSPIStations(ko.toJS(data));
                }
            });
        }
        else {
            self.availableSPIStations([]);
        }
    };

    self.loadProjects = function () {
        if (self.selectedSegmentTable()) {
            $.ajax({
                url: "/Common/GetProjects",
                type: "POST",
                data: { Segment_Id: self.selectedSegmentTable() },
                dataType: "json",
                success: function (data) {
                    self.availableProjects(data);
                }
            });
        }
        else {
            self.availableProjects([]);
        }
    };
    self.loadModels = function () {
        
        if (self.selectedProjectTable().Project_Id) {
            $.ajax({
                url: "/Common/GetSPIModels",
                type: "POST",
                data: { Project_Id: self.selectedProjectTable().Project_Id },
                dataType: "json",
                success: function (data) {
                    self.availableModels(data);
                }
            });
        }
        else {
            self.availableModels([]);
        }
    };
   
    $("#SaveDefectDialog").click(function () {
        self.loadDefectViewDataCollection();
        $("#DefectModel").modal('hide'); //just close;     
    });
    self.loadDefectViewDataCollection = function () {
        self.sStartTime($("#StartTime").val());
        self.sEndTime($("#EndTime").val());
        $.ajax({
            url: "/HASPIDefectView/GetDefectViewDataCollection",
            type: "POST",
            data: { Station_Id: self.selectedSPIStationTable().Station_Id, StartTime: self.sStartTime(), EndTime: self.sEndTime(), Model_Id: self.selectedModelTable().Model_Id },
            dataType: "json",
            beforeSend: function () {
                var target = $("#firstDiv").get(0);
                spinner.spin(target);                    
            },
            success: function (data) {
                self.destoryTopTenLocationSeries();
                $.each(data.ComponentDetailList, function (k, v) {
                    self.CountByTopTenLocationCategories.push(v.ComponentName);
                    self.CountByTopTenLocationSeries.ExcessiveCountsArray.push(v.ExcessiveCounts);
                    self.CountByTopTenLocationSeries.InsufficientCountsArray.push(v.InsufficientCounts);
                    self.CountByTopTenLocationSeries.LowAreaCountsArray.push(v.LowAreaCounts);
                    self.CountByTopTenLocationSeries.HighAreaCountsArray.push(v.HighAreaCounts);
                    self.CountByTopTenLocationSeries.LowerHeightCountsArray.push(v.LowerHeightCounts);
                    self.CountByTopTenLocationSeries.UpperHeightCountsArray.push(v.UpperHeightCounts);
                    self.CountByTopTenLocationSeries.ShapeCountsArray.push(v.ShapeCounts);
                    self.CountByTopTenLocationSeries.PositionCountsArray.push(v.PositionCounts);
                    self.CountByTopTenLocationSeries.BridgingCountsArray.push(v.BridgingCounts);
                    self.CountByTopTenLocationSeries.SmearCountsArray.push(v.SmearCounts);
                });
                self.plotLocationChart();

                self.DefectViewDataCollection.DefectTypeCountPercent(data.DefectTypeCountPercent);
                self.DefectViewDataCollection.TotalPanelCounts(data.TotalPanelCounts);
                self.DefectViewDataCollection.GoodPanelCounts(data.GoodPanelCounts);
                self.DefectViewDataCollection.NGPanelCounts(data.NGPanelCounts);
                self.DefectViewDataCollection.PassPanelCounts(data.PassPanelCounts);
                self.DefectViewDataCollection.WarningPanelCounts(data.WarningPanelCounts);
                self.DefectViewDataCollection.TotalDefectPadCounts(data.TotalDefectPadCounts);
                self.DefectViewDataCollection.TopFiveExcessiveComp(data.TopFiveExcessiveComp);
                self.DefectViewDataCollection.TopFiveInsufficientComp(data.TopFiveInsufficientComp);
                self.DefectViewDataCollection.TopFiveHighAreaComp(data.TopFiveHighAreaComp);
                self.DefectViewDataCollection.TopFiveLowAreaComp(data.TopFiveLowAreaComp);
                self.DefectViewDataCollection.TopFiveLowerHeightComp(data.TopFiveLowerHeightComp);
                self.DefectViewDataCollection.TopFiveUpperHeightComp(data.TopFiveUpperHeightComp);
                self.DefectViewDataCollection.TopFivePositionComp(data.TopFivePositionComp);
                self.DefectViewDataCollection.TopFiveBridgingComp(data.TopFiveBridgingComp);
                self.DefectViewDataCollection.TopFiveShapeComp(data.TopFiveShapeComp);
                self.DefectViewDataCollection.TopFiveSmearComp(data.TopFiveSmearComp);
                //Plot the defect type percent pie chart
                self.plotDefectPercentPieChart();
                spinner.spin();
            },           
            error: function () {
                spinner.spin();
                alert("Data loading error");
            }
        });
    };

    self.destoryTopTenLocationSeries = function () {
        self.CountByTopTenLocationCategories.removeAll();
        self.CountByTopTenLocationSeries.ExcessiveCountsArray.removeAll();
        self.CountByTopTenLocationSeries.InsufficientCountsArray.removeAll();
        self.CountByTopTenLocationSeries.LowAreaCountsArray.removeAll();
        self.CountByTopTenLocationSeries.HighAreaCountsArray.removeAll();
        self.CountByTopTenLocationSeries.LowerHeightCountsArray.removeAll();
        self.CountByTopTenLocationSeries.UpperHeightCountsArray.removeAll();
        self.CountByTopTenLocationSeries.ShapeCountsArray.removeAll();
        self.CountByTopTenLocationSeries.PositionCountsArray.removeAll();
        self.CountByTopTenLocationSeries.BridgingCountsArray.removeAll();
        self.CountByTopTenLocationSeries.SmearCountsArray.removeAll();

    };

    self.plotDefectPercentPieChart = function () {
        var data = self.DefectViewDataCollection.DefectTypeCountPercent();
        var defectPieArrayList = new Array();
        $.each(data, function (k, v) {
            var defectPieArray = new Array();
            defectPieArray.push(k);
            defectPieArray.push(v);
            defectPieArrayList.push(defectPieArray);
        });
        var chart = $("#DefectPieChart").highcharts();
        var series = chart.series;
        while (series.length > 0) {
            series[0].remove(false);
        }
        chart.redraw();
        chart.addSeries({
            type: 'pie',
            name: 'Percentage',
            data: defectPieArrayList
        }, false);
        chart.redraw();
    };

    self.plotLocationChart = function () {
        var chart = $("#LocationContainer").highcharts();
        var series = chart.series;
        while (series.length > 0) {
            series[0].remove(false);
        }
        chart.redraw();
        chart.xAxis[0].setCategories(self.CountByTopTenLocationCategories(), false);
        chart.addSeries({
            name: "Excessive",
            data: self.CountByTopTenLocationSeries.ExcessiveCountsArray()
        }, false);
        chart.addSeries({
            name: "Insufficient",
            data: self.CountByTopTenLocationSeries.InsufficientCountsArray()
        }, false);
        chart.addSeries({
            name: "LowArea",
            data: self.CountByTopTenLocationSeries.LowAreaCountsArray()
        }, false);
        chart.addSeries({
            name: "HighArea",
            data: self.CountByTopTenLocationSeries.HighAreaCountsArray()
        }, false);
        chart.addSeries({
            name: "LowerHeight",
            data: self.CountByTopTenLocationSeries.LowerHeightCountsArray()
        }, false);
        chart.addSeries({
            name: "UpperHeight",
            data: self.CountByTopTenLocationSeries.UpperHeightCountsArray()
        }, false);
        chart.addSeries({
            name: "Shape",
            data: self.CountByTopTenLocationSeries.ShapeCountsArray()
        }, false);
        chart.addSeries({
            name: "Position",
            data: self.CountByTopTenLocationSeries.PositionCountsArray()
        }, false);
        chart.addSeries({
            name: "Bridging",
            data: self.CountByTopTenLocationSeries.BridgingCountsArray()
        }, false);
        chart.addSeries({
            name: "Smear",
            data: self.CountByTopTenLocationSeries.SmearCountsArray()
        }, false);

        chart.redraw();
    };

    loadBuildings();
};