$(function() {

    var $status = $("[type=submit]"),
        showImages = function(files) {

            for (var i = 0; i < files.length; i++) {
                try {
                    $("#image" + (i + 1) + "+img").attr("src", files[i].Media.Uri);
                } catch(ex) {
                }
            }
        };

    $("form").upload({

        start: function() {
            $status.val("");
        },

        status: function(status) {
            
            $status.val(status.Percent + "%");

            showImages(status.Model.Files);
        },

        complete: function(response) {

        },

        success: function(response) {
            showImages(response.Files);

            $status.val("Sent");
        },
        error: function(errorResponse) {

            $status.val("Error");
        }
    });

});