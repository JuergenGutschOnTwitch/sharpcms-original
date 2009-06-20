// <reference path="jQuery/jQuery-1.3.min-vsdoc.js" />

// jQeury to Load
// http: //www.thomasfrank.se/sessionvars.html

$(document).ready(function() {
    if (sessvars.vs == null)
        sessvars.vs = { pada_: 'collapse', fida_: 'expand', foda_: 'expand', usda_: 'expand' };

    for (var s in sessvars.vs) {
        if (document.getElementById(s + 'body') != null && document.getElementById(s + 'vs') != null) {
            if (sessvars.vs[s] == 'expand') {
                document.getElementById(s + 'body').style.display = '';
                document.getElementById(s + 'vs').innerHTML = '˄';
            }
            else {
                document.getElementById(s + 'body').style.display = 'none';
                document.getElementById(s + 'vs').innerHTML = '˅';
            }
        }
    }

    $(".element_body").toggle(0);

    $("a.button").click(function() {
        elementID = this.id;
        if (elementID != "") {
            if ($("#" + elementID + "_body").is(":hidden")) {
                $("#" + elementID + "_body").slideDown("fast");
                $("#" + elementID).html('˄');
            } else {
                $("#" + elementID + "_body").slideUp("fast");
                $("#" + elementID).html('˅');
            }
        }
    });

    $("p")
    .filter("#pada_vs")
      .click(function() {
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
    .filter("#fida_vs")
      .click(function() {
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
    .filter("#foda_vs")
      .click(function() {
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
    .filter("#usda_vs")
      .click(function() {
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