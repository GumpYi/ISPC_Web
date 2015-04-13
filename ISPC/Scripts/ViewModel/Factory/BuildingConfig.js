$(document).ready(function () {
    $(".datetimepicker").datetimepicker({
        format: "YYYY-MM-DD HH:mm"
    });
    $("#BuildingCheck").bootstrapSwitch({
        state: true,
        onColor: "success",
        offColor: "danger",
        onText: "Active",
        offText: "InActive",
        labelText: "Select"
    });
    ko.applyBindings(new BuildingViewModel(), document.getElementById("B_M"));
});
var BuildingViewModel = function () {
    var self = this;
    self.BuildingList = ko.observableArray([]);
    self.sBuilding_Id = ko.observable();
    self.sBuilding_Name = ko.observable();
    self.sIs_Active = ko.observable();
    self.sCreator_Name = ko.observable();
    self.sCreation_Time = ko.observable();
    self.sEffect_Time = ko.observable();
    var Building = function (Building_Id, Building_Name, Is_Active, Creator_Name, Creation_Time, Effect_Time) {
        this.Building_Id = Building_Id;
        this.Building_Name = Building_Name;
        this.Is_Active = Is_Active;
        this.Creator_Name = Creator_Name;
        this.Creation_Time = Creation_Time;
        this.Effect_Time = Effect_Time;
    };
    self.loadData = function () {
        $.ajax({
            url: "/JsonBuilding/GetBuildings",
            type: "JSON",
            dataType: "json",
            success: function (data) {
                self.BuildingList(ko.toJS(data));
            }
        });
    };

    self.editBuilding = function (building) {
        self.sBuilding_Id(building.Building_Id);
        self.sBuilding_Name(building.Building_Name);
        self.sCreator_Name(building.Creator_Name);
        self.sCreation_Time(building.Creation_Time);
        self.sEffect_Time(building.Effect_Time);
        $("#BuildingCheck").bootstrapSwitch("state", building.Is_Active);
    };

    self.clearsBuilding = function () {
        self.sBuilding_Id(0);
        self.sBuilding_Name("");
        self.sIs_Active(true);
        self.sCreator_Name("N/A");
        self.sCreation_Time("2014-01-01 00:00");
        self.sEffect_Time("");
        $("#BuildingCheck").bootstrapSwitch("state", true);
    };
    self.deleteBuilding = function () {
        if (self.sBuilding_Id() != 0) {
            if (confirm("Are you confirm to delete this Building ?")) {
                $.ajax({
                    url: "/JsonBuilding/DeleteBuilding",
                    type: "POST",
                    data: { Building_Id: self.sBuilding_Id() },
                    dataType: "text",
                    success: function (data) {
                        alert(data);
                        self.loadData();
                    }
                });
            }
        }
        else {
            alert("Please select one building!");
        }
    };
    self.storeBuilding = function () {
        var sBuilding = new Building(self.sBuilding_Id(), self.sBuilding_Name(), $("#BuildingCheck").bootstrapSwitch("state"), self.sCreator_Name(), self.sCreation_Time(), $("#bEffect_Time").val());    
        //judge whether update exist building or add new building according the building.Building_Id equals 0;
        if (sBuilding.Building_Id == 0) {
            $.ajax({
                url: "/JsonBuilding/AddBuilding",
                type: "POST",
                data: ko.toJSON(sBuilding),
                dataType: "text",
                success: function (data) {
                    alert(data);
                    self.loadData();
                }
            });
        }
        else {

            $.ajax({
                url: "/JsonBuilding/UpdateBuilding",
                type: "POST",
                data: ko.toJSON(sBuilding),
                dataType: "text",
                success: function (data) {
                    alert(data);
                    self.loadData();
                }
            });
        }
    };
    self.loadData();
    self.clearsBuilding();
};


