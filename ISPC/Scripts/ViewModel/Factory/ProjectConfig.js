$(document).ready(function () {
    $("#ProjectCheck").bootstrapSwitch({
        state: true,
        onColor: "success",
        offColor: "danger",
        onText: "Active",
        offText: "InActive",
        labelText: "Select"
    });
    ko.applyBindings(new ProjectViewModel(), document.getElementById("P_M"));
});
var ProjectViewModel = function () {
    var self = this;
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.availableBUs = ko.observableArray([]);
    self.ProjectList = ko.observableArray([]);
    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.selectedBUTable = ko.observable();
    self.sProject_Id = ko.observable();
    self.sProject_Name = ko.observable();
    self.sCreator_Name = ko.observable();
    self.sCreation_Time = ko.observable();
    self.sEffect_Time = ko.observable();
    var Project = function (Project_Id, Project_Name, Is_Active, Building_Id, Segment_Id, BU_Id, Creator_Name, Creation_Time, Effect_Time) {
        this.Project_Id = Project_Id;
        this.Project_Name = Project_Name;
        this.Is_Active = Is_Active;
        this.Building_Id = Building_Id;
        this.Segment_Id = Segment_Id;
        this.BU_Id = BU_Id;
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
    self.loadBUs = function (BU_Id) {
        if (self.selectedSegmentTable()) {
            $.ajax({
                url: "/Common/GetBUs",
                type: "POST",
                data: { Segment_Id: self.selectedSegmentTable() },
                dataType: "json",
                success: function (data) {
                    self.availableBUs(ko.toJS(data));
                    self.selectedBUTable(BU_Id);
                }
            });
        }
        else {
            self.availableBUs([]);
        }
    }
    self.loadProjects = function () {
        $.ajax({
            url: "/JsonProject/GetProjects",
            type: "POST",
            data: { Building_Id: (self.selectedBuildingTable() ? self.selectedBuildingTable() : 0),
                Segment_Id: (self.selectedSegmentTable() ? self.selectedSegmentTable() : 0),
                BU_Id: (self.selectedBUTable() ? self.selectedBUTable() : 0)
            },
            dataType: "json",
            success: function (data) {
                self.ProjectList(ko.toJS(data));
            }
        });
    };
    self.editProject = function (project) {
        self.selectedBuildingTable(project.Building_Id);
        self.loadSegments(project.Segment_Id);
        self.loadBUs(project.BU_Id);
        self.sProject_Id(project.Project_Id);
        self.sProject_Name(project.Project_Name);
        self.sCreation_Time(project.Creation_Time);
        self.sEffect_Time(project.Effect_Time);
        $("#ProjectCheck").bootstrapSwitch("state", project.Is_Active);
    };
    self.clearProject = function () {
        self.sProject_Id(0);
        self.sProject_Name("");
        self.sCreator_Name("N/A");
        self.sCreation_Time("2014-01-01 00:00");
        self.sEffect_Time("");
        $("#ProjectCheck").bootstrapSwitch("state", true);
    };
    self.deleteProject = function () {
        if (self.sProject_Id() != 0) {
            if (confirm("Are you confirm to delete this Project ?")) {
                $.ajax({
                    url: "/JsonProject/DeleteProject",
                    type: "POST",
                    data: { Project_Id: self.sProject_Id() },
                    dataType: "text",
                    success: function (data) {
                        alert(data);
                        self.loadProjects();
                    }
                });
            }
        }
        else {
            alert("Please select one Segment!");
        }
    };
    self.storeProject = function () {        
        var sProject = new Project(self.sProject_Id(), self.sProject_Name(), $("#ProjectCheck").bootstrapSwitch("state"), self.selectedBuildingTable(), self.selectedSegmentTable(), self.selectedBUTable(), self.sCreator_Name(), self.sCreation_Time(), $("#pEffect_Time").val());
        if (sProject.Project_Id == 0) {
            $.ajax({
                url: "/JsonProject/AddProject",
                type: "POST",
                data: ko.toJSON(sProject),
                dataType: "text",
                success: function (data) {
                    alert(data);
                    self.loadProjects();
                }
            });
        }
        else {
            $.ajax({
                url: "/JsonProject/UpdateProject",
                type: "POST",
                data: ko.toJSON(sProject),
                dataType: "text",
                success: function (data) {
                    alert(data);
                    self.loadProjects();
                }
            });
        }
    };
    loadBuilding();
    self.loadProjects();

};
