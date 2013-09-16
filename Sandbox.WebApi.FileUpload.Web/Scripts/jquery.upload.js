;
(function($) {
    "use strict";

    $.fn.upload = function(options) {

        $.each(this, function() {

            var $form = $(this);

            if ($form.data("uploadControl")) return;

            $form.data(new control($form, options));

        });
    };

    var control = function($form, options) {

        var action = $form.attr("action"),
            target = $form.attr("target");
        options = $.extend({}, $.upload.defaults, options);

        $form.submit(function() {
            window.console.log("submit");

            if (statusTimeout) return false;

            var statusId = "s" + Math.floor(Math.random() * 9999999999999999),
                statusAction = $.upload.addToUrl(action, { "x-upload-status-id": statusId }),
                isHttps = /^https/i.test(window.location.href || '');

            var $iframe = $("<iframe name='" + statusId + "' />")
                .attr({ "src": isHttps ? "javascript:false" : "about:blank" })
                .css({ position: 'absolute', top: '-800px', left: '-400px' });

            $form
                .attr({
                    "action": statusAction,
                    "target": statusId,
                    "method": "POST",
                    "enctype": "multipart/form-data"
                })
                .before($iframe);

            getStatus(statusId, $iframe);

            return true;
        });

        var statusTimeout,
            getStatus = function(statusId, $iframe, delay) {

                statusTimeout = window.setTimeout(
                    function() {

                        var $response = $iframe.contents().find("body");
                        if ($response.text().length) {
                            console.log($response.text());

                            var data = $.parseJSON($response.text());

                            $iframe.remove();
                            complete();
                            options.complete(data);
                            options.success(data);

                            return;
                        }

                        $.ajax({
                            url: options.urlStatus + statusId,
                            dataType: "json"
                        }).done(function(status) {

                            if (!status || !status.Model) {
                                if (delay < 50000) delay *= 2;
                            } else {
                                options.status(status);
                                delay = null;
                            }
                            getStatus(statusId, $iframe, delay);

                        });

                    }, delay || (delay = 1000));
            },
            complete = function() {
                window.console.log("complete " + statusTimeout);
                if (statusTimeout) {
                    window.clearTimeout(statusTimeout);
                    statusTimeout = null;
                }

                $form
                    .attr("target", target)
                    .attr("action", action);
            };

        window.console = window.console || {
            log: function() {
            }
        };
    };

    $.upload = {
        defaults: {
            urlStatus: "/api/upload/",
            start: function() {
            },
            status: function(status) {
            },
            complete: function(response) {
            },
            success: function(response) {
            },
            error: function(errorResponse) {
            }
        },
        addToUrl: function(url, o) {
            var s = url.split("?");
            for (var n in o) s.push(n + "=" + o[n]);
            return s.length === 1
                ? s[0]
                : s[0] + "?" + s.slice(1).join("&");
        }
    };

})((typeof(jQuery) != 'undefined') ? jQuery : window.Zepto);