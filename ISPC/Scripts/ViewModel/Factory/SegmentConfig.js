$(document).ready(function () {
    $("#SegmentCheck").bootstrapSwitch({
        state: true,
        onColor: "success",
        offColor: "danger",
        onText: "Active",
        offText: "InActive",
        labelText: "Select"
    });
    ko.applyBindings(new SegmentViewModel(), document.getElementById("S_M"));
});

var SegmentViewModel = function () {
    var self = this;
    self.availableBuildings = ko.observableArray([]);
    self.SegmentList = ko.observableArray([]);
    self.selectedBuildingTable = ko.observable();
    self.sSegment_Id = ko.observable();
    self.sSegment_Name = ko.observable();
    self.sSegment_Code = ko.observable();
    self.sCreator_Name = ko.observable();
    self.sCreation_Time = ko.observable();
    self.sEffect_Time = ko.observable();    
    var Segment = function (Segment_Id, Segment_Name, Segment_Code, Building_Id, Is_Active, Creator_Name, Creation_Time, Effect_Time) {
        this.Segment_Id = Segment_Id;
        this.Segment_Name = Segment_Name;
        this.Segment_Code = Segment_Code;
        this.Building_Id = Building_Id;
        this.Is_Active = Is_Active;
        this.Creator_Name = Creator_Name;
        this.Creation_Time = Creation_Time;
        this.Effect_Time = Effect_Time;
    };
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
    self.loadSegments = function () {
        $.ajax({
            url: "/JsonSegment/GetSegments",
            type: "POST",
            data: { Building_Id: (self.selectedBuildingTable() ? self.selectedBuildingTable() : 0) },
            dataType: "json",
            success: function (data) {
                self.SegmentList(ko.toJS(data));
            }
        });
    };
    self.editSegment = function (segment) {
        self.sSegment_Id(segment.Segment_Id);
        self.selectedBuildingTable(segment.Building_Id);
        self.sSegment_Name(segment.Segment_Name);
        self.sSegment_Code(segment.Segment_Code);
        self.sCreator_Name(segment.Creator_Name);
        self.sCreation_Time(segment.Creation_Time);
        self.sEffect_Time(segment.Effect_Time);
        $("#SegmentCheck").bootstrapSwitch("state", segment.Is_Active);
    };
    self.clearSegment = function () {
        self.sSegment_Id(0);
        self.sSegment_Name("");
        self.sSegment_Code("");
        self.sCreator_Name("N/A");
        self.sCreation_Time("2014-01-01 00:00:00");
        self.sEffect_Time("");
        $("#SegmentCheck").bootstrapSwitch("state", true);
    };
    self.storeSegment = function () {
        var sSegment = new Segment(self.sSegment_Id(), self.sSegment_Name(), self.sSegment_Code(), self.selectedBuildingTable(), $("#SegmentCheck").bootstrapSwitch("state"), self.sCreator_Name(), self.sCreation_Time(), $("#sEffect_Time").val());
        if (sSegment.Segment_Id == 0) {
            $.ajax({
                url: "/JsonSegment/AddSegment",
                type: "POST",
                data: ko.toJSON(sSegment),
                dataType: "text",
                success: function (data) {
                    alert(data);
                    self.loadSegments();
                }
            });
        }
        else {
            $.ajax({
                url: "/JsonSegment/UpdateSegment",
                type: "POST",
                data: ko.toJSON(sSegment),
                dataType: "text",
                success: function (data) {
                    alert(data);
                    self.loadSegments();
                }
            });
        }
    };
    self.deleteSegment = function () {
        if (self.sSegment_Id() != 0) {
            if (confirm("Are you confirm to delete this Segments ?")) {
                $.ajax({
                    url: "/JsonSegment/DeleteSegment",
                    type: "POST",
                    data: { Segment_Id: self.sSegment_Id() },
                    dataType: "text",
                    success: function (data) {
                        alert(data);
                        self.loadSegments();
                    }
                });
            }
        }
        else {
            alert("Please select one Segment!");
        }
    };
    loadBuilding();
    self.loadSegments();
    self.clearSegment();
};