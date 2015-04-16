$(function () {
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    $('#DefectContainer').highcharts({
        chart: {
            type: 'column',
            borderColor: '#EBBA95',
            borderWidth: 2
        },
        title: {
            text: 'Multi Line Defect Count View'
        },
        legend: {
            align: 'center',
            margin: 0,
            enabled: false
        },
        xAxis: {

            title: {
                text: 'Line Name',
                align: 'high'
            }
        },
        yAxis: {
            title: {
                text: 'Count',
                align: 'high'
            },
            labels: {
                format: '{value}'
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
             formatter: function () {
                return 'The value for <b>' + this.x +
                    '</b> is <b>' + this.y + '</b>';
            },
            valueSuffix: ' ' 
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
    $(".datetimepicker").datetimepicker({
        format: "YYYY-MM-DD HH:mm"
    });
    $("#MultiDefectModel").modal('show');
    ko.applyBindings(new MultiSPIDefectViewModel());
});

var MultiSPIDefectViewModel = function () {
    var self = this;
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.sStartTime = ko.observable();
    self.sEndTime = ko.observable();

    var opts = {
        lines: 13, // 
        length: 20, // the length of leaf
        width: 10, // the width of leaf
        radius: 30, // the radius 
        corners: 1, // the corners of leaf (0-1)
        rotate: 0, // the rotate of leaf
        direction: 1, // the direction of leadf 1: 顺时针, -1: 逆时针
        color: '#000', // the color of leaf
        speed: 1, // the speed of leaf
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
    loadBuildings();

    self.loadMultiLineDefectCountData = function () {
        self.sStartTime($("#StartTime").val());
        self.sEndTime($("#EndTime").val());
        if (self.sStartTime() != "" && self.sEndTime() != "") {
            $.ajax({
                url: "/HASPIDefectView/GetMultiDefectViewDataCollection",
                type: "POST",
                data: {
                    BuildingId: (self.selectedBuildingTable() ? self.selectedBuildingTable() : 0),
                    SegmentId: (self.selectedSegmentTable() ? self.selectedSegmentTable() : 0),
                    StartTime: self.sStartTime(),
                    EndTime: self.sEndTime()
                },
                dataType: "json",
                beforeSend: function () {
                    var target = $("#firstDiv").get(0);
                    spinner.spin(target);
                },
                success: function (data) {
                    var categories = new Array();
                    var pointData = new Array();
                    $.each(data, function (k, v) {
                        categories.push(k);
                        pointData.push(v);
                    });
                    var chart = $("#DefectContainer").highcharts();
                    var series = chart.series;
                    while (series.length > 0) {
                        series[0].remove(false);
                    }
                    chart.redraw();
                    chart.addSeries({
                        data: pointData
                    }, false);
                    chart.xAxis[0].setCategories(categories, false);
                    chart.redraw();
                    spinner.spin();
                },
                error: function () {
                    spinner.spin();
                    alert("Data loading error");
                }
            });
        }
        else {
            alert("All fields is required!");
        }
    };

    $("#SaveMultiDefectDialog").click(function () {
        self.loadMultiLineDefectCountData();
        $("#MultiDefectModel").modal('hide'); //just close;     
    });
};