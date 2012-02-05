// jQeury to Load
// http: //www.thomasfrank.se/sessionvars.html

$(document).ready(function () {
    if (sessvars.vs == null) {
        sessvars.vs = { pada_: 'collapse', fida_: 'expand', foda_: 'expand', usda_: 'expand' };
    }

    var element;
    for (element in sessvars.vs) {
        if (document.getElementById(element + 'body') != null && document.getElementById(element + 'vs') != null) {
            if (sessvars.vs[element] == 'expand') {
                document.getElementById(element + 'body').style.display = '';
                document.getElementById(element + 'vs').innerHTML = '˄';
            }
            else {
                document.getElementById(element + 'body').style.display = 'none';
                document.getElementById(element + 'vs').innerHTML = '˅';
            }
        }
    }

    if ($.url.param("e") != null) {
        if ($.url.param("e") != "") {
            element = $.url.param("e");
            var h = 0;
            var j = parseInt(element.substring(element.length - 1, element.length));
            var i;
            for (i = 1; i < j; i++)
                h += $("#" + element.substring(0, element.length - 1) + i + "_body").height() + 7;

            var destination = $("#" + $.url.param("e") + "_anchor").offset().top - h + 8;

            if ($("#" + $.url.param("e") + "_body").is(":hidden")) {
                $("#" + $.url.param("e") + "_body").slideDown("fast");
                $("#" + $.url.param("e")).html('˄');
            } else {
                $("#" + $.url.param("e") + "_body").slideUp("fast");
                $("#" + $.url.param("e")).html('˅');
            }

            $("html:not(:animated),body:not(:animated)").animate({ scrollTop: destination }, 800, function () { });
        }
    }

    $(".element_body").toggle(0);

    $("a.button").click(function () {
        var elementId = this.id;
        if (elementId != "") {
            if ($("#" + elementId + "_body").is(":hidden")) {
                $("#" + elementId + "_body").slideDown("fast");
                $("#" + elementId).html('˄');
            } else {
                $("#" + elementId + "_body").slideUp("fast");
                $("#" + elementId).html('˅');
            }
        }
    });

    $("a.button")
        .filter("#pada_vs").click(function () {
            if ($("#pada_body").is(":hidden")) {
                $("#pada_body").slideDown("fast");
                $("#pada_vs").html('˄');
                sessvars.vs.pada_ = 'expand';
            } else {
                $("#pada_body").slideUp("fast");
                $("#pada_vs").html('˅');
                sessvars.vs.pada_ = 'collapse';
            }
        })
        .end()
        .filter("#fida_vs").click(function () {
            if ($("#fida_body").is(":hidden")) {
                $("#fida_body").slideDown("fast");
                $("#fida_vs").html('˄');
                sessvars.vs.fida_ = 'expand';
            } else {
                $("#fida_body").slideUp("fast");
                $("#fida_vs").html('˅');
                sessvars.vs.fida_ = 'collapse';
            }
        })
        .end()
        .filter("#foda_vs").click(function () {
            if ($("#foda_body").is(":hidden")) {
                $("#foda_body").slideDown("fast");
                $("#foda_vs").html('˄');
                sessvars.vs.foda_ = 'expand';
            } else {
                $("#foda_body").slideUp("fast");
                $("#foda_vs").html('˅');
                sessvars.vs.foda_ = 'collapse';
            }
        })
        .end()
        .filter("#usda_vs").click(function () {
            if ($("#usda_body").is(":hidden")) {
                $("#usda_body").slideDown("fast");
                $("#usda_vs").html('˄');
                sessvars.vs.usda_ = 'expand';
            } else {
                $("#usda_body").slideUp("fast");
                $("#usda_vs").html('˅');
                sessvars.vs.usda_ = 'collapse';
            }
        })
        .end();
});