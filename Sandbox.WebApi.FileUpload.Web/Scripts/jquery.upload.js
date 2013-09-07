var $status = $("[type=submit]").width(102),
    statusTimeout,
    getStatus = function(statusId) {

        statusTimeout = window.setTimeout(
            function() {
                $.get("/api/upload/" + statusId, function (status) {
                    statusChange(status);
                    getStatus(statusId);
                });
            }, 1000);
    },
    start = function() {
        $status.val("");
    },
    statusChange = function(status) {
        $status
            .css({
                borderRightWidth: (101 - status) + "px"
            });
    },
    complete = function(response) {
        window.clearTimeout(statusTimeout);
        statusTimeout = null;
        $status.css({ paddingRight: "", borderRightWidth: "" });
    },
    success = function (response) {

        $("#image1+img").attr("src", response.Files[0].Media.Uri);
        $("#image2+img").attr("src", response.Files[1].Media.Uri);

        $status.val("Sent");
    },
    error = function (response) {
        $status.val("Error");
    };

$("form").submit(function () {

    if (statusTimeout) return false;

    var statusId = Math.floor(Math.random() * 9999999999999999);

    $(this).ajaxSubmit({
        url: "/api/upload/",
        method: "POST",
        beforeSend: function(xhr) {
            xhr.setRequestHeader("x-upload-status-id", statusId);
            start();
        },
        success: function (data) {
            console.log(data);
            complete();
            success(data);
        },
        error:function(data) {
            console.log(data);
            complete();
            error(data);
        }
    });

    getStatus(statusId);
    return false;
});
