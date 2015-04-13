$(function () {
    $(".datetimepicker").datetimepicker({
        format: "YYYY-MM-DD HH:mm"
    });
    $("#MultiYieldModel").modal('show');
    $('#FPYContainer').highcharts({
        chart: {
            type: 'line',
            borderColor: '#EBBA95',
            borderWidth: 2
        },
        title: {
            text: 'Multi Line FPY View'
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
            valueSuffix: ' %',
            pointFormat: '<tr><td style="padding:0"><b>{point.y:.2f} %</b></td></tr>'
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
            text: 'Multi Line DPPM View'
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
        tooltip: {           
            pointFormat: '<tr><td style="padding:0"><b>{point.y:.0f}</b></td></tr>'
        },
        credits: {
            enabled: false
        }
    });
    ko.applyBindings(new MultiYieldViewModel());
    
});

var MultiYieldViewModel = function () {
    var self = this;
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.shownLineList = ko.observableArray([]);
    self.selectedLineList = ko.observableArray([]);
    self.sStartTime = ko.observable();
    self.sEndTime = ko.observable();
    self.timeBy = ko.observable("Hour");
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

    var loadBuildings = function () {
        $.ajax({
            url: "/Common/getBuildings",
            type: "JSON",
            dataType: "json",
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
        $.ajax({
            url: "/Common/GetHAMultiLineList",
            type: "POST",
            data: { Building_Id: (self.selectedBuildingTable() ? self.selectedBuildingTable() : 0), Segment_Id: (self.selectedSegmentTable() ? self.selectedSegmentTable() : 0) },
            dataType: "json",
            success: function (data) {
                self.shownLineList(data);
            }
        });
    };
    loadBuildings();
    self.loadLines();

    self.loadMultiYieldViewData = function () {
        self.sStartTime($("#StartTime").val());
        self.sEndTime($("#EndTime").val());       
        if (self.selectedLineList() != 0 && self.sStartTime() != "" && self.sEndTime() != "") {
            $.ajax({
                url: "/HASPIYieldView/GetMultiYieldView",
                type: "POST",
                data: {
                    StartTime: self.sStartTime(),
                    EndTime: self.sEndTime(),
                    LineIdList: self.selectedLineList(),
                    TimeBy: self.timeBy()
                },
                dataType: "json",
                beforeSend: function () {
                    var target = $("#firstDiv").get(0);
                    spinner.spin(target);
                },
                success: function (data) {
                    var chart = $("#FPYContainer").highcharts();
                    var dppmchart = $("#DPPMContainer").highcharts();
                    var series = chart.series;
                    while (series.length > 0) {
                        series[0].remove(false);
                    }
                    var dppmseries = dppmchart.series;
                    while (dppmseries.length > 0) {
                        dppmseries[0].remove(false);
                    }
                    chart.redraw();
                    dppmchart.redraw();
                    var categories = new Array();
                    var categoriesFlag = true;
                    $.each(data.DataList, function (stationName, list) {
                        var pointData = new Array();
                        var dppmPointData = new Array();
                        if (categories.length != 0) categoriesFlag = false;
                        $.each(list, function (date, data) {
                            if (categoriesFlag) {
                                categories.push(date);
                            }
                            pointData.push(data.FPY);
                            dppmPointData.push(data.DPPM);
                        });
                        chart.addSeries({
                            name: stationName,
                            data: pointData
                        }, false);
                        dppmchart.addSeries({
                            name: stationName,
                            data: dppmPointData
                        }, false);
                    });
                    chart.xAxis[0].setCategories(categories, false);
                    chart.redraw();
                    dppmchart.xAxis[0].setCategories(categories, false);
                    dppmchart.redraw();
                    spinner.spin();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    spinner.spin();
                    alert(textStatus);
                }
            });
        }
        else {
            alert("All fields is required!");
        }
    };
   
    $("#SaveMultiYieldDialog").click(function () {
        self.loadMultiYieldViewData();
        $("#MultiYieldModel").modal('hide'); //just close;     
    });
};