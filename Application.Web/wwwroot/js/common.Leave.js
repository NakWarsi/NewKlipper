﻿
//cancel leave
function CancelLeave(leaveId, isRealisedLeave, isCancelledLeave) {
    var controllerName = "Leave";
    var actionName = "CancelLeave";
    var newURL = window.location.protocol + "//" + window.location.host + "/" + controllerName + "/" + actionName;

    if (isRealisedLeave == "true") {
        alert('Passed leaves can not be cancelled !');
        return;
    }
    if (isCancelledLeave == "true") {
        alert('cancelled leaves can not be updated !');
            return;
    }
   

    if (confirm('Do you want to cancel this leave ?')) {
        $.ajax({
            type: "POST",
            url: newURL,
            data: {
                LeaveId: leaveId
            },
            dataType: "html",
            success: function (response) {
                window.location.reload();
            }
        });
    }
}


