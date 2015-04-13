$(function () {
    $(".datetimepicker").datetimepicker({
        format: "YYYY-MM-DD HH:mm"
    });
    $("#YieldModel").modal('show');
    $('#FPYContainer').highcharts({
        chart: {
            type: 'column',
            borderColor: '#EBBA95',
            borderWidth: 2
        },
        title: {
            text: 'FPY View'
        },
        xAxis: {

            title: {
                text: 'Periods of time',
                align: 'high'
            }
        },
        yAxis: {

            title: {
                text: '',
                align: 'high'
            },
            labels: {
                format: '{value}%'
            },
            max: 100,
            min: 0,
            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="padding:0"><b>{point.y:.2f} %</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        credits: {
            enabled: false
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        }
    });

    $("#DPPMContainer").highcharts({
        chart: {
            type: 'line',
            borderColor: '#EBBA95',
            borderWidth: 2
        },
        title: {
            text: 'DPPM View'
        },
        xAxis: {
            title: {
                text: 'Periods of time',
                align: 'high'
            }
        },
        yAxis: {
            title: {
                text: '',
                align: 'high'
            },
            min: 0,
            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            enabled: false
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="padding:0"><b>{point.y:.0f} </b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        credits: {
            enabled: false
        }
    });
    ko.applyBindings(new YieldViewModel());
});

var YieldViewModel = function () {
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
    self.timeBy = ko.observable("Hour");
    self.categoryBy = ko.observable("Time");
    var opts = {
        lines: 13, // 花瓣数目
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

        if (self.selectedProjectTable()) {
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

    self.loadYieldViewData = function () {
        self.sStartTime($("#StartTime").val());
        self.sEndTime($("#EndTime").val());
        $.ajax({
            url: "/HASPIYieldView/GetYieldViewData",
            type: "POST",
            data: { StationId: self.selectedSPIStationTable().Station_Id,
                ModelId: self.selectedModelTable() ? self.selectedModelTable().Model_Id : 0,
                StartTime: self.sStartTime(),
                EndTime: self.sEndTime(),
                TimeBy: self.timeBy(),
                CategroyBy: self.categoryBy()
            },
            dataType: "json",
            beforeSend: function () {
                var target = $("#firstDiv").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                var chart = $("#FPYContainer").highcharts();
                var series = chart.series;
                while (series.length > 0) {
                    series[0].remove(false);
                }
                chart.redraw();
                var categories = new Array();
                var categoriesFlag = true;
                $.each(data.FPYBySelectedSection, function (model, list) {
                    var columnData = new Array();
                    if (categories.length != 0) categoriesFlag = false;
                    $.each(list, function (date, data) {
                        if (categoriesFlag) {
                            categories.push(date);
                        }
                        var pointData = new Array();
                        pointData.push(date);
                        pointData.push(data);
                        columnData.push(pointData);
                    });
                    chart.addSeries({
                        name: model,
                        data: columnData
                    }, false);
                });
                chart.xAxis[0].setCategories(categories, false);
                chart.redraw();
                // art the dppm chart

                var chartdppm = $("#DPPMContainer").highcharts();
                var series = chartdppm.series;
                while (series.length > 0) {
                    series[0].remove(false);
                }
                chartdppm.redraw();
                var dppmcategories = new Array();
                var dppmdata = new Array();
                $.each(data.DPPMBySelectedSection, function (key, value) {
                    dppmcategories.push(key);
                    dppmdata.push(value);
                });
                chartdppm.addSeries({
                    data: dppmdata
                }, false);
                chartdppm.xAxis[0].setCategories(dppmcategories, false);
                chartdppm.redraw();
                spinner.spin();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                spinner.spin();
                alert("Error happened!");
            }
        });
    };    
    $("#SaveYieldDialog").click(function () {
        self.loadYieldViewData();
        $("#YieldModel").modal('hide'); //just close;     
    });

    loadBuildings();
};