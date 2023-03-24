
function UserCheck() {
    $("#status").html("check ...");
    $.post("@Url.Action("CheckUsernameAvailability", "Admin")",
        {
            userdata: $("#Username").val()
        },
        function (data) {
            if (data == 0) {
                $("#status").html('<font color="Green">Available!</font>');
                $("#Username").css("border-color", "Green");
            }
            else {
                $("#status").html('<font color="Red">That name is taken. Try another !!</font>');
                $("#Username").css("border-color", "Red");
            }
        }
    )
}

function EmailCheck() {
    $("#mess").html("check ...");
    $.post("@Url.Action("CheckEmailAvailability", "Admin")",
        {
            usermail: $("#Email").val()
        },
        function (data) {
            if (data == 0) {
                $("#mess").html('<font color="Green">Available!</font>');
                $("#Email").css("border-color", "Green");
            }
            else {
                $("#mess").html('<font color="Red">That name is taken. Try another !!</font>');
                $("#Email").css("border-color", "Red");
            }
        }
    )
}
function SDTCheck() {
    $("#noti").html("check ...");
    $.post("@Url.Action("CheckSDTAvailability", "Admin")",
        {
            userSDT: $("#Sdt").val()
        },
        function (data) {
            if (data == 0) {
                $("#noti").html('<font color="Green">Available!</font>');
                $("#Sdt").css("border-color", "Green");
            }
            else {
                $("#noti").html('<font color="Red">That name is taken. Try another !!</font>');
                $("#Sdt").css("border-color", "Red");
            }
        }
    )
}
