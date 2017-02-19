//SearchList

function seachsubunit(obj, id, name) {
    //window.location.href = "SubUnits?id=" + id + "&exchangeName=" + name;
    //window.location = "SubUnits?id=" + id + "&exchangeName=" + name;
    obj.href = "SubUnits?id=" + id + "&exchangeName=" + name;
}


//SearchUnit
function keyupevent() {
    if (window.event.keyCode == 13)
        searchunit();
}

function searchunit() {
    var key = $("#myUnitFilter").val();
    $("#subsuitdetail").load("/Main/GetSubUnitListView", { name: key }, function () {
    });
}

function openpanel(obj, unitid) {
    obj.href = "#myPanel";
    $("#myPanel").load("/Main/InfoPanel", { id: unitid }, function () {
    });
}

