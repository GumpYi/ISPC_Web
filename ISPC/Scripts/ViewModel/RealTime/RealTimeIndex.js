$(function () {
    $(".pinned").pin();
  Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
   var rtIndex = new RealTimeIndexViewModel();
    function refreshCategories() {
        var date = new Date();
        var categories = [];
        for (var v = 1; v <= 12; v++) {
            categories.push((((date.getHours() - 12 + v) >= 0) ? (date.getHours() - 12 + v) : (24 + (date.getHours() - 12 + v))) + ":00~" + (((date.getHours() - 12 + v + 1) >= 0) ? (date.getHours() - 12 + v + 1) : (24 + (date.getHours() - 12 + v + 1))) + ":00");
        }
        return categories;
    }

  
    $(".datetimepicker").datetimepicker({
        format: "YYYY-MM-DD HH:mm", 
        showClear: true,
        showClose: true,        
    });
    ko.bindingHandlers.refreshData = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var allBindings = allBindingsAccessor();
            var type = ko.utils.unwrapObservable(allBindings.type);
            if (type == "spifpy") {
                $(element).highcharts({
                    chart: {
                        type: 'spline',
                        borderColor: '#EBBA95',
                        borderWidth: 2, 
                    },
                    tooltip: {
                        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                        pointFormat: '<tr><td style="padding:0"><b>{point.y:.2f} %</b></td></tr>',
                        footerFormat: '</table>',
                        shared: true,
                        useHTML: true
                    }, 
                    title: {
                        text: 'Last 12 Hours FPY Of SPI Station',
                        x: -20 //center
                    },
                    subtitle: {
                        text: 'RealTime Monitor',
                        x: -20 //center
                    },
                    xAxis: {
                        categories: refreshCategories(),
                        title: {
                            text: 'Periods of time',
                            align: 'high'
                        }
                    },
                    yAxis: {
                        title: {
                            align: 'high'
                        },
                        labels: {
                            format: '{value}%'
                        },                        
                        max: 100,
                        min: 0,                       
                    },
                    loading: {
                        labelStyle: {
                            color: 'white'
                        },
                        style: {
                            backgroundColor: 'gray'
                        }
                    },
                    credits: {
                        enabled: false
                    }
                });
            }
            else if (type == "spidppm") {
                $(element).highcharts({
                    chart: {
                        type: 'spline',
                        borderColor: '#EBBA95',
                        borderWidth: 2, 
                    },
                    tooltip: {
                        pointFormat: '<tr><td style="padding:0"><b>{point.y:.0f}</b></td></tr>'                                         
                    },
                    title: {
                        text: 'Last 12 Hours DPPM Of SPI Station',
                        x: -20 //center
                    },
                    subtitle: {
                        text: 'RealTime Monitor',
                        x: -20 //center
                    },
                    xAxis: {
                        categories: refreshCategories(),
                        title: {
                            text: 'Periods of time',
                            align: 'high'
                        }
                    },
                    yAxis: {
                        title: {
                            align: 'high'
                        },
                        min: 0,  
                        lineWidth: 1,
                        minorGridLineWidth: 0,
                        minorTickInterval: 'auto',
                        minorTickPosition: 'inside',
                        minorTickWidth: 1                     
                    },
                    loading: {
                        labelStyle: {
                            color: 'white'
                        },
                        style: {
                            backgroundColor: 'gray'
                        }
                    },
                    credits: {
                        enabled: false
                    }
                });
            }
            else if (type == "aoifpy") {
                $(element).highcharts({
                    chart: {
                        type: 'spline',
                        borderColor: '#EBBA95',
                        borderWidth: 2, 
                    },
                    tooltip: {
                        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                        pointFormat: '<tr><td style="padding:0"><b>{point.y:.2f} %</b></td></tr>',
                        footerFormat: '</table>',
                        shared: true,
                        useHTML: true
                    }, 
                    title: {
                        text: 'Last 12 Hours FPY Of AOI Station',
                        x: -20 //center
                    },
                    subtitle: {
                        text: 'RealTime Monitor',
                        x: -20 //center
                    },
                    xAxis: {
                        categories: refreshCategories(),
                        title: {
                            text: 'Periods of time',
                            align: 'high'
                        }
                    },
                    yAxis: {
                        title: {
                            align: 'high'
                        },
                        labels: {
                            format: '{value}%'
                        },                        
                        max: 100,
                        min: 0,                       
                    },
                    loading: {
                        labelStyle: {
                            color: 'white'
                        },
                        style: {
                            backgroundColor: 'gray'
                        }
                    },
                    credits: {
                        enabled: false
                    }
                });
            }
            else if (type == "aoidppm") {
                $(element).highcharts({
                    chart: {
                        type: 'spline',
                        borderColor: '#EBBA95',
                        borderWidth: 2, 
                    },
                    tooltip: {
                        pointFormat: '<tr><td style="padding:0"><b>{point.y:.0f}</b></td></tr>'                                         
                    },
                    title: {
                        text: 'Last 12 Hours DPPM Of AOI Station',
                        x: -20 //center
                    },
                    subtitle: {
                        text: 'RealTime Monitor',
                        x: -20 //center
                    },
                    xAxis: {
                        categories: refreshCategories(),
                        title: {
                            text: 'Periods of time',
                            align: 'high'
                        }
                    },
                    yAxis: {
                        title: {
                            align: 'high'
                        },
                        min: 0,  
                        lineWidth: 1,
                        minorGridLineWidth: 0,
                        minorTickInterval: 'auto',
                        minorTickPosition: 'inside',
                        minorTickWidth: 1                     
                    },
                    loading: {
                        labelStyle: {
                            color: 'white'
                        },
                        style: {
                            backgroundColor: 'gray'
                        }
                    },
                    credits: {
                        enabled: false
                    }
                });
            }
            else if (type == "aoifppm") {
                $(element).highcharts({
                    chart: {
                        type: 'spline',
                        borderColor: '#EBBA95',
                        borderWidth: 2, 
                    },
                    tooltip: {
                        pointFormat: '<tr><td style="padding:0"><b>{point.y:.0f}</b></td></tr>'                                         
                    },
                    title: {
                        text: 'Last 12 Hours FPPM Of AOI Station',
                        x: -20 //center
                    },
                    subtitle: {
                        text: 'RealTime Monitor',
                        x: -20 //center
                    },
                    xAxis: {
                        categories: refreshCategories(),
                        title: {
                            text: 'Periods of time',
                            align: 'high'
                        }
                    },
                    yAxis: {
                        title: {
                            align: 'high'
                        },
                        min: 0,  
                        lineWidth: 1,
                        minorGridLineWidth: 0,
                        minorTickInterval: 'auto',
                        minorTickPosition: 'inside',
                        minorTickWidth: 1                     
                    },
                    loading: {
                        labelStyle: {
                            color: 'white'
                        },
                        style: {
                            backgroundColor: 'gray'
                        }
                    },
                    credits: {
                        enabled: false
                    }
                });
            }
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            var data = ko.utils.unwrapObservable(valueAccessor());
            var allBindings = allBindingsAccessor();
            var type = ko.utils.unwrapObservable(allBindings.type);
            var chart = $(element).highcharts();           
            var series = chart.series;
            while (series.length > 0) {
                series[0].remove(false);
            }
            chart.redraw();
            if (type == "spifpy") {
                $.each(data, function (key, value) {
                    chart.addSeries({
                        id: key,
                        type: value.type,
                        data: value.data,
                        name: value.name
                    }, false);                 
                });
                chart.yAxis[0].update({
                    plotLines: [{
                    color: 'red',
                    width: 2,
                    value: rtIndex.ULCLSetting.SPIFPYLCL(),
                    label: {
                        text: 'FPY LCL (' + rtIndex.ULCLSetting.SPIFPYLCL() + '%)',
                        align: 'left'
                    },
                    id: "spifpy-plot-line",
                    zIndex: 5
                }]
                }, false);
            }
            else if (type == "spidppm") {
                $.each(data, function (key, value) {
                    chart.addSeries({
                        id: key,
                        type: value.type,
                        data: value.data,
                        name: value.name
                    }, false);
                });
                chart.yAxis[0].update({
                    plotLines: [{
                        color: 'red',
                        width: 2,
                        value: rtIndex.ULCLSetting.SPIDPPMUCL(),
                        label: {
                            text: 'DPPM UCL (' + rtIndex.ULCLSetting.SPIDPPMUCL() + ')',
                            align: 'left'         
                        },
                        id: "spidppm-plot-line",
                        zIndex: 5
                }]
                }, false);
            }
            else if (type == "aoifpy") {
                $.each(data, function (key, value) {
                    chart.addSeries({
                        id: key,
                        type: value.type,
                        data: value.data,
                        name: value.name
                    }, false);                 
                });
                chart.yAxis[0].update({
                    plotLines: [{
                    color: 'red',
                    width: 2,
                    value: rtIndex.ULCLSetting.AOIFPYLCL(),
                    label: {
                        text: 'FPY LCL (' + rtIndex.ULCLSetting.AOIFPYLCL() + '%)',
                        align: 'left'
                    },
                    id: "aoifpy-plot-line",
                    zIndex: 5
                }]
                }, false);
            }
            else if (type == "aoidppm") {
                $.each(data, function (key, value) {
                    chart.addSeries({
                        id: key,
                        type: value.type,
                        data: value.data,
                        name: value.name
                    }, false);
                });
                chart.yAxis[0].update({
                    plotLines: [{
                        color: 'red',
                        width: 2,
                        value: rtIndex.ULCLSetting.AOIDPPMUCL(),
                        label: {
                            text: 'DPPM UCL (' + rtIndex.ULCLSetting.AOIDPPMUCL() + ')',
                            align: 'left'         
                        },
                        id: "aoidppm-plot-line",
                        zIndex: 5
                }]
                }, false);
            }
            else if (type == "aoifppm") {
                $.each(data, function (key, value) {
                    chart.addSeries({
                        id: key,
                        type: value.type,
                        data: value.data,
                        name: value.name
                    }, false);
                });
                chart.yAxis[0].update({
                    plotLines: [{
                        color: 'red',
                        width: 2,
                        value: rtIndex.ULCLSetting.AOIFPPMUCL(),
                        label: {
                            text: 'FPPM UCL (' + rtIndex.ULCLSetting.AOIFPPMUCL() + ')',
                            align: 'left'         
                        },
                        id: "aoifppm-plot-line",
                        zIndex: 5
                }]
                }, false);
            }
            chart.xAxis[0].setCategories(refreshCategories(), false);
            chart.redraw();           
        }
    };
    ko.applyBindings(rtIndex);
});

var RealTimeIndexViewModel = function () {
    var self = this;
    self.ShownLineList = ko.observableArray([]);
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.FPYList_SPI = ko.observableArray([]);
    self.DPPMList_SPI = ko.observableArray([]);
    self.FPYList_AOI = ko.observableArray([]);
    self.DPPMList_AOI = ko.observableArray([]);
    self.FPPMList_AOI = ko.observableArray([]);   
    self.ULCLSetting = {
        SPIFPYLCL: ko.observable(),
        SPIDPPMUCL: ko.observable(),

        AOIFPYLCL: ko.observable(),
        AOIFPPMUCL: ko.observable(),
        AOIDPPMUCL: ko.observable()
    };
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
            url: "/Common/getShownLineList",
            type: "POST",
            data: { Building_Id: (self.selectedBuildingTable() ? self.selectedBuildingTable() : 0), Segment_Id: (self.selectedSegmentTable() ? self.selectedSegmentTable() : 0) },
            dataType: "json",
            success: function (data) {
                self.ShownLineList(ko.toJS(data));
            }
        });
    };
    var loadULCLSetting = function () {
        $.ajax({
            url: "/RTIndex/GetULCLSetting",
            type: "JSON",
            dataType: "json",
            success: function (data) {
                self.ULCLSetting.SPIFPYLCL(data.SPIFPYLCL);
                self.ULCLSetting.SPIDPPMUCL(data.SPIDPPMUCL);

                self.ULCLSetting.AOIFPYLCL(data.AOIFPYLCL);
                self.ULCLSetting.AOIFPPMUCL(data.AOIFPPMUCL);
                self.ULCLSetting.AOIDPPMUCL(data.AOIDPPMUCL);
            }
        });
    };
    self.drawPlotBands = function () { 
        var spifpychart = $('#spifpy').highcharts();
        var spidppmchart = $('#spidppm').highcharts()
        spifpychart.yAxis[0].removePlotBand('spifpy-plot-line');
        spidppmchart.yAxis[0].removePlotBand('spidppm-plot-line');
        spifpychart.yAxis[0].addPlotLine({          
                color: 'red',
                width: 2,
                value: self.ULCLSetting.SPIFPYLCL(),
                label: {
                    text: 'FPY LCL (' + self.ULCLSetting.SPIFPYLCL() + '%)',
                    align: 'left'                                         
                },
                id: "spifpy-plot-line",
                zIndex: 5   
        });
        spidppmchart.yAxis[0].addPlotLine({    
                color: 'red',
                width: 2,
                value: self.ULCLSetting.SPIDPPMUCL(),
                label: {
                    text: 'DPPM UCL (' + self.ULCLSetting.SPIDPPMUCL() + ')',
                    align: 'left'         
                },
                id: "spidppm-plot-line",
                zIndex: 5
        });

        var aoifpychart = $('#aoifpy').highcharts();
        var aoidppmchart = $('#aoidppm').highcharts()
        var aoifppmchart = $('#aoifppm').highcharts()
        aoifpychart.yAxis[0].removePlotBand('aoifpy-plot-line');
        aoidppmchart.yAxis[0].removePlotBand('aoidppm-plot-line');
        aoidppmchart.yAxis[0].removePlotBand('aoifppm-plot-line');
        aoifpychart.yAxis[0].addPlotLine({          
                color: 'red',
                width: 2,
                value: self.ULCLSetting.AOIFPYLCL(),
                label: {
                    text: 'FPY LCL (' + self.ULCLSetting.AOIFPYLCL() + '%)',
                    align: 'left'                                         
                },
                id: "aoifpy-plot-line",
                zIndex: 5   
        });
        aoidppmchart.yAxis[0].addPlotLine({    
                color: 'red',
                width: 2,
                value: self.ULCLSetting.AOIDPPMUCL(),
                label: {
                    text: 'DPPM UCL (' + self.ULCLSetting.AOIDPPMUCL() + ')',
                    align: 'left'         
                },
                id: "aoidppm-plot-line",
                zIndex: 5
        });
        aoidppmchart.yAxis[0].addPlotLine({    
            color: 'red',
            width: 2,
            value: self.ULCLSetting.AOIDPPMUCL(),
            label: {
                text: 'FPPM UCL (' + self.ULCLSetting.AOIFPPMUCL() + ')',
                align: 'left'         
            },
            id: "aoifppm-plot-line",
            zIndex: 5
        });
    };
    self.updateULCLSetting = function () {
        $.ajax({
            url: "/RTIndex/UpdateULCLSetting",
            type: "POST",
            data: ko.toJSON(self.ULCLSetting),
            dataType: "text",
            success: function (data) {
                alert(data);
            }
        });
    };
    loadBuildings();
    self.loadLines();
    loadULCLSetting();

    $("#SaveLineDialog").click(function(){
        $.ajax({
            url: "/Common/UpdateSessionLine",
            type: "POST",
            data: ko.toJSON(self.ShownLineList),
            dataType: "text",
            success: function (data) {               
                getRealTimeData();
                $('#LineModel').modal('hide') //just close;                                                                                  
            },
            error: function () {
                alert("Error");
            }
        });
    });
    
    $("#SaveUSLDialog").click(function(){
        self.updateULCLSetting();
        self.drawPlotBands();
        $("#USLModel").modal('hide') //just close;     
    });
  
    var getRealTimeData = function () {
        var dataSet_FPY_SPI = [];
        var dataSet_DPPM_SPI = [];

        var dataSet_FPY_AOI = [];
        var dataSet_DPPM_AOI = [];
        var dataSet_FPPM_AOI = [];       
        $.ajax({
            url: "/RTIndex/RefreshIndexData",
            type: "JSON",            
            dataType: "json",
            beforeSend: function () {
                var target = $("#firstDiv").get(0);
                spinner.spin(target);                    
            },
            success: function (data) {               
                $.each(data, function (key, value) {
                    var Line_Name = value.Line_Name;
                    // spi station data type-in 
                    $.each(value.RealTimeData_SPIList, function (key, value) {
                        var data_FPY = { type: 'spline',
                           // name: Line_Name + "~" + value.Station_Name,
                            name: value.Station_Name,
                            data: value.FPY.reverse()
                        };                        
                        var data_DPPM = { type: 'spline',
                            //name: Line_Name + "~" + value.Station_Name,
                            name: value.Station_Name,
                            data: value.DPPM.reverse()
                        };
                        dataSet_FPY_SPI.push(data_FPY);
                        dataSet_DPPM_SPI.push(data_DPPM);
                    });
                     // aoi station data type-in 
                    $.each(value.RealTimeData_AOIList, function (key, value) {
                        var data_FPY = { type: 'spline',
                            //name: Line_Name + "~" + value.Station_Name,
                            name: value.Station_Name,
                            data: value.FPY.reverse()
                        };                        
                        var data_DPPM = { type: 'spline',
                            //name: Line_Name + "~" + value.Station_Name,
                            name: value.Station_Name,
                            data: value.DPPM.reverse()
                        };
                        var data_FPPM = { type: 'spline',
                            //name: Line_Name + "~" + value.Station_Name,
                            name: value.Station_Name,
                            data: value.FPPM.reverse()
                        };
                        dataSet_FPY_AOI.push(data_FPY);
                        dataSet_DPPM_AOI.push(data_DPPM);
                        dataSet_FPPM_AOI.push(data_FPPM);
                    });
                });
                self.FPYList_SPI(dataSet_FPY_SPI);
                self.DPPMList_SPI(dataSet_DPPM_SPI); 

                self.FPYList_AOI(dataSet_FPY_AOI);
                self.DPPMList_AOI(dataSet_DPPM_AOI); 
                self.FPPMList_AOI(dataSet_FPPM_AOI);     
                spinner.spin();             
            },
            error: function () {
                alert("Please Choose Shown Line or something wrong");
                self.FPYList_SPI([]);
                self.DPPMList_SPI([]);
                spinner.spin();
            }
        });
    };
    getRealTimeData();
    window.setInterval(getRealTimeData, 1000 * 60 * 10);
};
