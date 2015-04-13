$(document).ready(function () {
    $(".datetimepicker").datetimepicker({
        format: "YYYY-MM-DD HH:mm"
    });
    ko.applyBindings(new SupplierViewModel());
});

var SupplierViewModel = function () {
    var self = this;
    self.SupplierList = ko.observableArray([]);
    self.selectedSupplierTable = ko.observable();
    self.sCreator_Name = ko.observable();
    self.sCreation_Time = ko.observable();
    self.sEffect_Time = ko.observable();

    self.loadSupplierList = function () { 
            
    };
};
