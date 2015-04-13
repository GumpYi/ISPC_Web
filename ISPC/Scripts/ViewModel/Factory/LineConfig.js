$(document).ready(function () {
    ko.applybindings(new LineConfigViewModel());
});
var LineConfigViewModel = new function () {
    var self = this;
    self.Line_Id = ko.observable($("#Line_Id").val());
    self.Line = ko.observable();
};