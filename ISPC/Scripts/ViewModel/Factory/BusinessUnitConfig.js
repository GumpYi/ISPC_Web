$(document).ready(function () {
    $("#BUCheck").bootstrapSwitch({
        state: true,
        onColor: "success",
        offColor: "danger",
        onText: "Active",
        offText: "InActive",
        labelText: "Select"
    });
    ko.applyBindings(new BUViewModel(), document.getElementById("BU_M"));
});

var BUViewModel = function () {
    var self = this;
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.BUList = ko.observableArray([]);
    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.sBU_Id = ko.observable();
    self.sBU_Name = ko.observable();
    self.sCreator_Name = ko.observable();
    self.sCreation_Time = ko.observable();
    self.sEffect_Time = ko.observable();
    var BU = function (BU_Id, BU_Name, Segment_Id, Building_Id, Is_Active, Creator_Name, Creation_Time, Effect_Time) {
        this.BU_Id = BU_Id;
        this.BU_Name = BU_Name;
        this.Segment_Id = Segment_Id;
        this.Building_Id = Building_Id
        this.Is_Active = Is_Active;
        this.Creator_Name = Creator_Name;
        this.Creation_Time = Creation_Time;
        this.Effect_Time = Effect_Time;
    }
    var loadBuilding = function () {
        $.ajax({
            url: "/Common/GetBuildings",
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
                url: "/Common/GetSegments",
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
    self.loadBUs = function () {
        $.ajax({
            url: "/JsonBU/GetBUs",
            type: "POST",
            data: { Building_Id: (self.selectedBuildingTable() ? self.selectedBuildingTable() : 0),
                    Segment_Id: (self.selectedSegmentTable() ? self.selectedSegmentTable() : 0) },
            dataType: "json",
            success: function (data) {
                self.BUList(ko.toJS(data));
            }
        });
    };
    self.editBU = function (bu) {
        self.sBU_Id(bu.BU_Id);
        self.sBU_Name(bu.BU_Name);
        self.selectedBuildingTable(bu.Building_Id);
        self.loadSegments(bu.Segment_Id);
        self.selectedSegmentTable(bu.Segment_Id);
        self.sCreator_Name(bu.Creator_Name);
        self.sCreation_Time(bu.Creation_Time);
        self.sEffect_Time(bu.Effect_Time);
        $("#BUCheck").bootstrapSwitch("state", bu.Is_Active);
    };
    self.clearBU = function () {
        self.sBU_Id(0);
        self.sBU_Name("");
        self.sCreator_Name("N/A");
        self.sCreation_Time("2014-01-01 00:00");
        self.sEffect_Time("");
        $("#BUCheck").bootstrapSwitch("state", true);
    };
    self.deleteBU = function () {
        if (self.sBU_Id() != 0) {
            if (confirm("Are you confirm to delete this BU ?")) {
                $.ajax({
                    url: "/JsonBU/DeleteBU",
                    type: "POST",
                    data: { BU_Id: self.sBU_Id() },
                    dataType: "text",
                    success: function (data) {
                        alert(data);
                        self.loadBUs();
                    }
                });
            }
        }
        else {
            alert("Please select one Segment!");
        }
    };
    self.storeBU = function () {       
        var sBU = new BU(self.sBU_Id(), self.sBU_Name(), self.selectedSegmentTable(), self.selectedBuildingTable(), $("#BUCheck").bootstrapSwitch("state"), self.sCreator_Name(), self.sCreation_Time(), $("#buEffect_Time").val());        
        if (sBU.BU_Id == 0) {
            $.ajax({
                url: "/JsonBU/AddBU",
                type: "POST",
                data: ko.toJSON(sBU),
                dataType: "text",
                success: function (data) {
                    alert(data);
                    self.loadBUs();
                }
            });
        }
        else {
            $.ajax({
                url: "/JsonBU/UpdateBU",
                type: "POST",
                data: ko.toJSON(sBU),
                dataType: "text",
                success: function (data) {
                    alert(data);
                    self.loadBUs();
                }
            });
        }
    };
    loadBuilding();
    self.loadBUs();
    self.clearBU();
};