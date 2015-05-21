$(function () {
    $(".datetimepicker").datetimepicker({
        format: "YYYY-MM-DD HH:mm"
    });
    $("#DefectModel").modal({
        show: true,
        backdrop: "static"
    });   
    var compChart = $("#ComponentContainer").highcharts(compOption);
    var partNumberChart = $("#PartNumberContainer").highcharts(compOption);
    var algorithmChart = $("#AlgorithmContainer").highcharts(compOption);
    var feederChart = $("#FeederContainer").highcharts(compOption);
    
    var defectPieChart = $("#DefectPieChart").highcharts({
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
   
    ko.applyBindings(new DefectViewModel());
});

var DefectViewModel = function () {
    var self = this;
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.availableProjects = ko.observableArray([]);
    self.availableModels = ko.observableArray([]);
    self.availableLines = ko.observableArray([]);
    self.availableAOIStations = ko.observableArray([]);

    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.selectedProjectTable = ko.observable();
    self.selectedModelTable = ko.observable();
    self.selectedLineTable = ko.observable();
    self.selectedAOIStationTable = ko.observable();
    self.sStartTime = ko.observable();
    self.sEndTime = ko.observable();
    
    self.DefectViewDataCollection = {
        DefectTypeCountPercent: ko.observable(),
        TotalPanelCounts: ko.observable(),
        GoodPanelCounts: ko.observable(),
        NGPanelCounts: ko.observable(),
        FalseCallPanelCounts: ko.observable(),
        TotalDefectCompCounts: ko.observable(),
        CompGroupDrilldownDataList: ko.observableArray([]),
        PartNumberGroupDrilldownDataList: ko.observableArray([]),
        AlgorithmDrilldownDataList: ko.observableArray([]),
        FeederGroupDrilldownDataList: ko.observableArray([])
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
    self.loadAOIStations = function () {
        if (self.selectedLineTable()) {
            $.ajax({
                url: "/Common/GetAOIStations",
                type: "POST",
                data: { LineId: self.selectedLineTable() },
                dataType: "json",
                success: function (data) {
                    self.availableAOIStations(ko.toJS(data));
                }
            });
        }
        else {
            self.availableAOIStations([]);
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
                url: "/Common/GetAOIModels",
                type: "POST",
                data: { ProjectId: self.selectedProjectTable().Project_Id },
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
    
    self.clickSaveChange = function(){
        self.loadDefectViewData();
        $("#DefectModel").modal('hide'); //just close;     
    };

    self.loadDefectViewData = function(){
        self.sStartTime($("#StartTime").val());
        self.sEndTime($("#EndTime").val());       
        $.ajax({
            url: "/HAAOIDefectView/GetDefectViewDataCollection",
            type: "POST",
            data: 
            { 
                stationId: self.selectedAOIStationTable().Station_Id, 
                startTime: self.sStartTime(), 
                endTime: self.sEndTime(), 
                modelId: self.selectedModelTable().Model_Id 
            },
            dataType: "JSON",
            beforeSend: function () {
                var target = $("#firstDiv").get(0);
                spinner.spin(target);                    
            },
            success: function (data) {                 
                self.DefectViewDataCollection.DefectTypeCountPercent(data.DefectTypeCountPercent);
                self.DefectViewDataCollection.TotalPanelCounts(data.TotalPanelCounts);
                self.DefectViewDataCollection.GoodPanelCounts(data.GoodPanelCounts);
                self.DefectViewDataCollection.NGPanelCounts(data.NGPanelCounts);
                self.DefectViewDataCollection.FalseCallPanelCounts(data.FalseCallPanelCounts);
                self.DefectViewDataCollection.TotalDefectCompCounts(data.TotalDefectCompCounts);
                self.DefectViewDataCollection.CompGroupDrilldownDataList(data.CompGroupDrilldownDataList);
                self.DefectViewDataCollection.PartNumberGroupDrilldownDataList(data.PartNumberGroupDrilldownDataList);
                self.DefectViewDataCollection.AlgorithmDrilldownDataList(data.AlgorithmDrilldownDataList);
                self.DefectViewDataCollection.FeederGroupDrilldownDataList(data.FeederGroupDrilldownDataList);
                self.plotDefectPercentPieChart();
                self.plotDefectByTypeChart("Component", self.DefectViewDataCollection.CompGroupDrilldownDataList());
                self.plotDefectByTypeChart("PartNumber", self.DefectViewDataCollection.PartNumberGroupDrilldownDataList());
                self.plotDefectByTypeChart("Algorithm", self.DefectViewDataCollection.AlgorithmDrilldownDataList());
                //self.plotDefectByTypeChart("Feeder", self.DefectViewDataCollection.FeederGroupDrilldownDataList());
                spinner.spin();
            },
             error: function () {
                spinner.spin();
                alert("Data loading error");
            }
        });
    };

    self.plotDefectByTypeChart = function(type, dataCollection){
        var tempOption = JSON.parse(JSON.stringify(compOption));    
        tempOption.title.text = type + " Drilldown";         
        var dataArray = new Array();
        var drilldownBindingData = new Array();
        
        $.each(dataCollection, function(k, v){           
            var drilldownData = new Array();
            $.each(v.SonData, function(key,value){
                var drilldownRawData = new Array();
                drilldownRawData.push(key);
                drilldownRawData.push(value);
                drilldownData.push(drilldownRawData);                            
            });
            dataArray.push({           
                name: v.X,
                y: v.Y ,
                drilldown: v.X               
            });                   
            drilldownBindingData.push({id: v.X, name:"Counts", data: drilldownData});                    
        });
        tempOption.series[0].data = dataArray;        
        tempOption.drilldown.series = drilldownBindingData;      
        var chart = $("#"+ type +"Container").highcharts();
        chart.destroy();
        $("#"+ type +"Container").highcharts(tempOption);        
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

    loadBuildings();       
};

var compOption = {
    chart: {
            type: 'column'
        },
    title: {
        text: 'Type drilldown'
    },
    xAxis: {
        type: 'category'
    },
    series:[{
        name: "Counts",
        colorByPoint: true,
        data:[]
    }],
    legend: {
        enabled: false
    },     
    drilldown: {
            activeAxisLabelStyle: {
                textDecoration: 'none',
                fontStyle: 'bold'
            },
            activeDataLabelStyle: {
                textDecoration: 'none',
                fontStyle: 'bold'
            },
            series: []
        },      
    credits: {
        enabled: false
    }
};
