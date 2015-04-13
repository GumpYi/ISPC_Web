$(document).ready(function () {
    $("#dialog").dialog({
        autoOpen: false,
        modal: true, //if set to true, the dialog will have modal behavior. other items on the page will be disabled -- cannot be interacted with.
        //appendto: "#paper",
        title: "Line Configuration",
        //closeText: "",
        //dialogClass: "alert",

        buttons: [{
            text: "Save", click: function () {
                //$(this).dialog("destroy");// remover the dialog functionality completely. This will return the element back to its pre-init state;
                //$(this).dialog("instance");
                $(this).dialog("moveToTop");
            }
        },
        {
            text: "Cancel", click: function () {
                $(this).dialog("close"); //just close;
            }
        }],
        closeOnEscape: true
    });
    $("#refresh").click(function () {
        $("#dialog").dialog("open");
        return false;
    });
    ko.applyBindings(new LineViewModel());
});

var LineViewModel = function () {
    var self = this;
    self.availableBuildings = ko.observableArray([]);
    self.availableSegments = ko.observableArray([]);
    self.selectedBuildingTable = ko.observable();
    self.selectedSegmentTable = ko.observable();
    self.LineList = ko.observableArray([]);
    self.pageIndex = ko.observable(1);
    var loadBuildings = function () {
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
                    self.loadLines();
                }
            });
        }
        else {
            self.availableSegments([]);
        }
    };
    self.loadLines = function () {
        $.ajax({
            url: "/JsonLine/GetLines",
            type: "POST",
            data: { Building_Id: (self.selectedBuildingTable() ? self.selectedBuildingTable() : 0), Segment_Id: (self.selectedSegmentTable() ? self.selectedSegmentTable() : 0), pageSize: 4, pageIndex: self.pageIndex() },
            dataType: "json",
            success: function (data) {
                self.LineList(ko.toJS(data));
            }
        });
    };
    self.previousPage = function () {

        if (self.pageIndex() == 1) {
            alert("Already the first page");
        }
        else {
            self.pageIndex(self.pageIndex() - 1);
            self.loadLines();
        }
    };
    self.nextPage = function () {
        self.pageIndex(self.pageIndex() + 1);
        self.loadLines();
    };
    loadBuildings();
    self.loadLines();
};