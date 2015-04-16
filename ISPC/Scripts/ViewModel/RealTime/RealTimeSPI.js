$(function () {    
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });  
    $("#Cpk").highcharts({
        chart:{
            borderColor: '#EBBA95',
            borderWidth: 2
        },        
        title:{
            text: "Selected Groups Cpk Trend Of Ten Panels"
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">Last {point.key} Panel</span><table>',
            pointFormat: '<tr><td style="padding:0"><b>{point.y:.3f}</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        }, 
        xAxis:{
            categories:[1,2,3,4,5,6,7,8,9,10]
        },
        legend:{
            enabled: false
        },
        credits: {
            enabled: false
        } 
    });     
    $("#XBar").highcharts({
        chart:{
            borderColor: '#EBBA95',
            borderWidth: 2, 
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">Last {point.key} Panel</span><table>',
            pointFormat: '<tr><td style="padding:0">{series.name}:</td>' + '<td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        }, 
        title:{
            text: "Selected Groups XBar Chart of Ten Panels"
        },
        xAxis:{
            categories:[1,2,3,4,5,6,7,8,9,10]
        },
        credits: {
            enabled: false
        } 
    });
    var Cpkchart = $("#Cpk").highcharts();
    var XBarchart = $("#XBar").highcharts();
    Cpkchart.showLoading("Please Select Monitored Station First !");
    XBarchart.showLoading("Please Select Monitored Station First !");
    ko.applyBindings(new SPI_RealTimeViewModel());
});

 
var SPI_RealTimeViewModel = function () {
    var self = this;    
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.availableComps = ko.observableArray([]);
    self.availableLines = ko.observableArray([]);
    self.availableSPIStations = ko.observableArray([]);
    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.selectedProjectTable = ko.observable();
    self.selectedModelTable = ko.observable();
    self.selectedLineTable = ko.observable();   
    self.selectedSPIStationTable = ko.observableArray();
    self.selectedCpkComp = ko.observableArray([]);
    self.selectedXBarComp = ko.observableArray([]);
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

    self.SPIRTData = {
        LastTenPanelsVCpk: ko.observableArray([]),
        LastTenPanelsHCpk: ko.observableArray([]),
        LastTenPanelsACpk: ko.observableArray([]),
        LastTenPanelsXOCpk: ko.observableArray([]),
        LastTenPanelsYOCpk: ko.observableArray([]),
        XBarACompDataList: ko.observableArray([]),
        XBarHCompDataList: ko.observableArray([]),
        XBarVCompDataList: ko.observableArray([]),
        XBarOXCompDataList: ko.observableArray([]),
        XBarOYCompDataList: ko.observableArray([]),
        LastTenPanelsScatterList: ko.observableArray([])
    };
    self.SPIRange = {
        VUSL: ko.observable(),
        VLSL: ko.observable(),
        VUCL: ko.observable(),
        VLCL: ko.observable(),
        
        AUSL: ko.observable(),
        ALSL: ko.observable(),
        AUCL: ko.observable(),
        ALCL: ko.observable(),
        
        HUSL: ko.observable(),
        HLSL: ko.observable(),
        HUCL: ko.observable(),
        HLCL: ko.observable(),     
        
        XOUSL: ko.observable(),
        XOLSL: ko.observable(),
        XOUCL: ko.observable(),
        XOLCL: ko.observable(),
        
        YOUSL: ko.observable(),
        YOLSL: ko.observable(),
        YOUCL: ko.observable(),
        YOLCL: ko.observable()            
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
        else{
            self.availableSPIStations([]);
        }
    };
    self.loadSPIRange = function(){
         $.ajax({
                url: "/RTSPI/GetSPIRange",
                type: "JSON",               
                dataType: "json",
                success: function (data) {                   
                    self.SPIRange.VUSL(data.VUSL);
                    self.SPIRange.VLSL(data.VLSL);
                    self.SPIRange.VUCL(data.VUCL);
                    self.SPIRange.VLCL(data.VLCL);

                    self.SPIRange.AUSL(data.AUSL);
                    self.SPIRange.ALSL(data.ALSL);
                    self.SPIRange.AUCL(data.AUCL);
                    self.SPIRange.ALCL(data.ALCL);
                    
                    self.SPIRange.HUSL(data.HUSL);
                    self.SPIRange.HLSL(data.HLSL);
                    self.SPIRange.HUCL(data.HUCL);
                    self.SPIRange.HLCL(data.HLCL);
                    
                    self.SPIRange.XOUSL(data.XOUSL);
                    self.SPIRange.XOLSL(data.XOLSL);
                    self.SPIRange.XOUCL(data.XOUCL);
                    self.SPIRange.XOLCL(data.XOLCL);
                    
                    self.SPIRange.YOUSL(data.YOUSL);
                    self.SPIRange.YOLSL(data.YOLSL);
                    self.SPIRange.YOUCL(data.YOUCL);
                    self.SPIRange.YOLCL(data.YOLCL);
                }
            });
    };
    self.updateSPIRange = function(){
        $.ajax({
                url: "/RTSPI/UpdateSPIRange",
                type: "POST",  
                data: ko.toJSON(self.SPIRange),             
                dataType: "text",
                success: function (data) {
                    self.plotScatter();
                    alert(data);        
                }
        }); 
    };

    self.loadComps = function(){   
        if(self.selectedSPIStationTable()) {
            $.ajax({
            url: "/RTSPI/GetCompList",
            type: "POST",
            data: { Station_Id: self.selectedSPIStationTable() },
            dataType: "json",
            success: function(data){                       
                self.selectedModelTable(data.ModelName);                              
                self.availableComps(data.CompList);
                 $("#Group").modal('show') //just open;
            }
        });    
        }             
    };

    self.plotCpk = function (type) {
        var chart = $("#Cpk").highcharts();
        chart.hideLoading();
        if (type == undefined || type == "Volume") {
            type="Volume";
            var series = chart.series;
            while (series.length > 0) {
                series[0].remove(false);
            }
            chart.redraw();
            chart.addSeries({
                data: self.SPIRTData.LastTenPanelsVCpk()
            }, false);           
        }
        else if (type == "Area") {
            var series = chart.series;
            while (series.length > 0) {
                series[0].remove(false);
            }
            chart.redraw();
            chart.addSeries({
                data: self.SPIRTData.LastTenPanelsACpk()
            }, false);
        }
        else if (type == "Height") {
            var series = chart.series;
            while (series.length > 0) {
                series[0].remove(false);
            }
            chart.redraw();
            chart.addSeries({
                data: self.SPIRTData.LastTenPanelsHCpk()
            }, false);
        }
        else if (type == "XOffset") {
            var series = chart.series;
            while (series.length > 0) {
                series[0].remove(false);
            }
            chart.redraw();
            chart.addSeries({
                data: self.SPIRTData.LastTenPanelsXOCpk()
            }, false);
        }
        else if (type == "YOffset") {
            var series = chart.series;
            while (series.length > 0) {
                series[0].remove(false);
            }
            chart.redraw();
            chart.addSeries({
                data: self.SPIRTData.LastTenPanelsYOCpk()
            }, false);
        }
        chart.setTitle(null, {text: type}, false);
        chart.redraw();
    };
    self.plotXBar = function (type) {
        var chart = $("#XBar").highcharts();
        chart.hideLoading();
         var series = chart.series;
            while(series.length>0){
                series[0].remove(false);
            }
        chart.redraw();
        if(type == undefined || type == "Volume") {
            type="Volume";
            $.each(self.SPIRTData.XBarVCompDataList(), function(key,data){
                var newFlagArray = new Array();
                $.each(data, function(key, arrayData){
                    newFlagArray.push(arrayData);
                });
                chart.addSeries({
                    name: key,
                    data: newFlagArray
                }, false);    
            });       
        }
        else if(type == "Area"){
             $.each(self.SPIRTData.XBarACompDataList(), function(key,data){
                var newFlagArray = new Array();
                $.each(data, function(key, arrayData){
                    newFlagArray.push(arrayData);
                });
                chart.addSeries({
                    name: key,
                    data: newFlagArray
                }, false);    
            });           
        }
         else if(type == "Height"){
             $.each(self.SPIRTData.XBarHCompDataList(), function(key,data){
                var newFlagArray = new Array();
                $.each(data, function(key, arrayData){
                    newFlagArray.push(arrayData);
                });
                chart.addSeries({
                    name: key,
                    data: newFlagArray
                }, false);    
            });           
        } 
        else if(type == "XOffset"){
            $.each(self.SPIRTData.XBarOXCompDataList(), function(key,data){
            var newFlagArray = new Array();
            $.each(data, function(key, arrayData){
                newFlagArray.push(arrayData);
            });
            chart.addSeries({
                name: key,
                data: newFlagArray
            }, false);    
            });           
         }
        else if(type == "YOffset"){
        $.each(self.SPIRTData.XBarOYCompDataList(), function(key,data){
        var newFlagArray = new Array();
        $.each(data, function(key, arrayData){
            newFlagArray.push(arrayData);
        });
        chart.addSeries({
            name: key,
            data: newFlagArray
        }, false);    
        });           
        }
        chart.setTitle(null, {text: type}, false);
        chart.redraw();  
    };
    self.plotScatter = function (type) {
        for(var i=1;i<=10;i++){
            var chart = $("#placeholder"+i).highcharts();
             if(chart != undefined)chart.destroy();
        }                       
        for (var i = 1; i <= self.SPIRTData.LastTenPanelsScatterList().length; i++) {                                                    
           if(type == undefined||type=="Volume"){
                areaOptions.xAxis.plotLines[0].value=self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.VolumeAvg;
                areaOptions.xAxis.plotLines[0].label.text=self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.VolumeAvg+"%"; 
                
                areaOptions.xAxis.plotLines[1].value=self.SPIRange.VLCL();
                areaOptions.xAxis.plotLines[1].label.text="LCL("+self.SPIRange.VLCL()+")%";

                areaOptions.xAxis.plotLines[2].value=self.SPIRange.VUCL();
                areaOptions.xAxis.plotLines[2].label.text="UCL("+self.SPIRange.VUCL()+")%";
                               
                //areaOptions.series[0].name= self.SPIRTData.LastTenPanelsScatterList()[i-1].Panel_Id;
                areaOptions.series[0].data= self.SPIRTData.LastTenPanelsScatterList()[i - 1].PadVolumeDistributeList;
                areaOptions.labels.items[0].html = "Time:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].TestTime+" Squeegee: "+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Squeegee+"<br/>"
                                                   +"Average:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.VolumeAvg+"% "
                                                   +"Max:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.VolumeMax+"% "
                                                   +"Min:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.VolumeMin+"%";
                $("#placeholder" + i).highcharts(areaOptions);
           }
           else if(type == "Area"){              
                //areaOptions.series[0].name= self.SPIRTData.LastTenPanelsScatterList()[i-1].Panel_Id;
                areaOptions.xAxis.plotLines[0].value=self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.AreaAvg;
                areaOptions.xAxis.plotLines[0].label.text=self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.AreaAvg+"%";

                areaOptions.xAxis.plotLines[1].value=self.SPIRange.ALCL();
                areaOptions.xAxis.plotLines[1].label.text="LCL("+self.SPIRange.ALCL()+")%";

                areaOptions.xAxis.plotLines[2].value=self.SPIRange.AUCL();
                areaOptions.xAxis.plotLines[2].label.text="UCL("+self.SPIRange.AUCL()+")%";

                areaOptions.series[0].data= self.SPIRTData.LastTenPanelsScatterList()[i - 1].PadAreaDistributeList;
                areaOptions.labels.items[0].html = "Time:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].TestTime+" Squeegee: "+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Squeegee+"<br/>"
                                                   +"Average:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.AreaAvg+"% "
                                                   +"Max:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.AreaMax+"% "
                                                   +"Min:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.AreaMin+"% ";
                $("#placeholder" + i).highcharts(areaOptions);     
           }
           else if(type =="Height"){                
                //areaOptions.series[0].name= self.SPIRTData.LastTenPanelsScatterList()[i-1].Panel_Id;
                areaHOptions.series[0].data= self.SPIRTData.LastTenPanelsScatterList()[i - 1].PadHeightDistributeList;
                areaHOptions.labels.items[0].html = "Time:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].TestTime+" Squeegee: "+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Squeegee+"<br/>"
                                                   +"Average:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.HeightAvg + self.SPIRTData.LastTenPanelsScatterList()[i - 1].Unit
                                                   +" Max:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.HeightMax+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Unit
                                                   +" Min:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.HeightMin+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Unit;
                areaHOptions.xAxis.labels.format = '{value}'+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Unit;
                $("#placeholder" + i).highcharts(areaHOptions);          
           } 
           else if(type == "Offset"){              
                //scatterOptions.series[0].name= self.SPIRTData.LastTenPanelsScatterList()[i-1].Panel_Id;
                scatterOptions.series[0].data= self.SPIRTData.LastTenPanelsScatterList()[i - 1].PadOffsetSplashList;
                scatterOptions.labels.items[0].html ="Time:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].TestTime+" Squeegee: "+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Squeegee+"<br/>"                                                  
                                                   +"MaxX:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.XOffsetMax+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Unit
                                                   +" MaxY:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.YOffsetMax+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Unit+"<br/>"
                                                   +"MinX:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.XOffsetMin+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Unit
                                                   +" MinY:"+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Range.YOffsetMin+self.SPIRTData.LastTenPanelsScatterList()[i - 1].Unit;            
                $("#placeholder" + i).highcharts(scatterOptions);                          
           }                              
       }
    };
    self.PlotAllElement = function () {
        self.plotCpk();
        self.plotXBar();
        self.plotScatter();
    };
       
    self.loadRTData = function () {       
        $.ajax({
            url: "/RTSPI/GetSPIRTData",
            type: "POST",
            data: { Station_Id: self.selectedSPIStationTable(),
                    selectedCpkComp: self.selectedCpkComp().length !=0 ? self.selectedCpkComp() : self.availableComps()[0],                       
                    selectedXBarComp: self.selectedXBarComp().length !=0 ? self.selectedXBarComp() : self.availableComps()[0]                    
                  },
            dataType: "JSON",
            beforeSend: function () {
                    var target = $("#firstDiv").get(0);
                    spinner.spin(target);                    
            },
            success: function (data) { 
                self.SPIRTData.LastTenPanelsVCpk(data.LastTenPanelsVCpk);
                self.SPIRTData.LastTenPanelsHCpk(data.LastTenPanelsHCpk);
                self.SPIRTData.LastTenPanelsACpk(data.LastTenPanelsACpk);
                self.SPIRTData.LastTenPanelsXOCpk(data.LastTenPanelsXOCpk);
                self.SPIRTData.LastTenPanelsYOCpk(data.LastTenPanelsYOCpk);
                self.SPIRTData.LastTenPanelsScatterList(data.LastTenPanelsScatterList);
                self.SPIRTData.XBarACompDataList(data.XBarACompDataList);
                self.SPIRTData.XBarVCompDataList(data.XBarVCompDataList);
                self.SPIRTData.XBarHCompDataList(data.XBarHCompDataList);
                self.SPIRTData.XBarOXCompDataList(data.XBarOXCompDataList);
                self.SPIRTData.XBarOYCompDataList(data.XBarOYCompDataList);
                self.PlotAllElement();     
                spinner.spin();           
            },
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                spinner.spin();
                alert("Error happened!");                                        
            }
        });
    };

    $("#SaveUSLDialog").click(function(){
        self.updateSPIRange();
        $("#USLRange").modal('hide') //just close;     
    });
    $("#SaveSelectGroupDialog").click(function(){
        self.loadRTData();
        $("#Group").modal('hide') //just close;     
    });
    loadBuildings();
    self.loadSPIRange();
};

var scatterOptions = {
    chart: {
        type: "scatter",
        borderColor: '#EBBA95',
        borderWidth: 2
    },
    labels:{ 
        items:[{ 
            html:'', 
            style:{
                top: "0px",
                left: "25px",                                
                color: 'black' 
            } 
        }],
        style:{             
            fontSize:"9px",            
            zIndex:10 
        }        
    },
    series:[{
        name: null,
        data: null
    }],   
    exporting:{
        enabled:false
    },
    title:{            
        text: null,             
    },
    legend: {       
        enabled: false           
    },
    yAxis: {
        min: -5,
        max: 5,
        tickInterval: 1,  
        showFirstLabel: false,
        showLastLabel: false,
        startOnTick: true,         
        labels: {  
            style:{
                    fontSize: "8px"
                },
            align: 'left',
            x: 0,
            y: 0                                  
        },                   
        title:{
            enabled: false
        },        
        lineWidth: 1,
        lineColor: 'black',
        plotLines: [{
            dashStyle: 'ShortDash',
            color: 'red',
            width: 1,
            value: 0,
            zIndex: 3
            }]
    },
    xAxis:{          
        min: -5,
        max: 5,   
        tickInterval: 1,  
        showFirstLabel: false,
        showLastLabel: false,
        startOnTick: true,                
        labels: {  
            style:{
                    fontSize: "8px"
                },
            align: 'left',
            x: -8,
            y: 0                                  
        },               
        lineWidth: 1,
        lineColor: 'black',
        plotLines: [{
            dashStyle: 'ShortDash',
            color: 'red',
            width: 1,
            value: 0,
            zIndex: 3
            }]
    },      
    plotOptions: {
        scatter: {                
            marker: { 
                enabled: true, 
                symbol: 'circle', 
                radius: 1.5, 
                states: { 
                    hover: { 
                        enabled: false 
                    } 
                } 
            } 
        },       
    },
    tooltip: {
        enabled: false
    },
    credits: {
        enabled: false
    }
};


var areaOptions = {
    chart: {
        type: "areaspline",
        borderColor: '#EBBA95',
        borderWidth: 2,
        inverted: true
    },
    labels:{ 
        items:[{ 
            html:'', 
            style:{
                top: "-5px",
                left: "0px",                                
                color: 'black' 
            } 
        }],
        style:{             
            fontSize:"9px",            
            zIndex:1000 
        }        
    },
    series:[{
        name: null,
        data: null
    }], 
    exporting:{
        enabled:false
    },
    title:{            
        text: null,             
    },
    legend: {
        align: 'center',
        margin: 0,   
        enabled: false         
    },
    yAxis: {
        showFirstLabel: false,
        startOnTick: true,
        labels:{
            enabled: false,
            style:{
                fontSize: "8px"
            },
            align: 'left',
                x: 0,
                y: 10           
        },                                 
        title:{
            enabled: false
        },       
        lineWidth: 1,
        lineColor: 'black'
    },
    xAxis:{  
        labels:{
            enabled: false
        },               
        min:0,
        max:200,
        plotLines: [
        {
            dashStyle: 'ShortDash',
            color: '#339900',
            width: 1,
            value: 0,
            label:{
                text: null,
                align: 'right'              
            },
            zIndex: 1000
        },
        {
            dashStyle: 'ShortDash',
            color: '#990000',
            width: 1,
            value: 0,
            label:{
                text: '',
                align: 'right'
            },
            zIndex: 1000
        },
        {
            dashStyle: 'ShortDash', 
            color: '#990000',
            width: 1,
            value: 0,
            label:{
                text: '',
                align: 'right'
            },
            zIndex: 1000
        }],       
        lineWidth: 1,
        lineColor: 'black'
    },
    plotOptions: {
            areaspline: {                
                marker: { 
                    enabled: false, 
                    symbol: 'circle', 
                    radius: 2, 
                    states: { 
                        hover: { 
                            enabled: false 
                        } 
                    } 
                } 
            }          
        },
    tooltip: {
        enabled: false
    },
    credits: {
        enabled: false
    }
};

var areaHOptions = {
    chart: {
        type: "areaspline",
        borderColor: '#EBBA95',
        borderWidth: 2,
        //inverted: true
    },
    labels:{ 
        items:[{ 
            html:'', 
            style:{
                top: "5px",
                left: "25px",                                
                color: 'black' 
            } 
        }],
        style:{             
            fontSize:"9px",            
            zIndex:1000 
        }              
    },
    series:[{
        name: null,
        data: null
    }], 
    exporting:{
        enabled:false
    },
    title:{            
        text: null            
    },
    legend: {
        align: 'center',
        margin: 0,   
        enabled: false         
    },
    yAxis: {
        labels:{
            enabled: false
        },
        showFirstLabel: false,
        showLastLabel: false,
        startOnTick: true,                                       
        title:{
             enabled: false
        },       
        lineWidth: 1,
        lineColor: 'black'
    },
    xAxis:{
        labels:{
            format: null,
            style:{
                fontSize: "8px"
            },
            align: 'left',
                x: 0,
                y: 10 
        },
        lineWidth: 1,
        lineColor: 'black'  
    }, 
    plotOptions: {
            areaspline: {                
                marker: { 
                    enabled: false, 
                    symbol: 'circle', 
                    radius: 2, 
                    states: { 
                        hover: { 
                            enabled: false 
                        } 
                    } 
                }, 
                lineWidth: 1     
                },            
             series: {
                fillColor: {
                    linearGradient: [0, 0, 0, 300],
                    stops: [
                        [0, Highcharts.getOptions().colors[0]],
                        [1, Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                    ]
                }
            }
        },
    tooltip: {
        enabled: false
    },
    credits: {
        enabled: false
    }
};