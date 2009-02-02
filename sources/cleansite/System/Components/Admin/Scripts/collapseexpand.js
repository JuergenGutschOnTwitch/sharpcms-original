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

  $("p")
    .filter("#pada_vs")
      .click(function() {
        if ($("#pada_body").is(":hidden")) {
          $("#pada_body").slideDown("slow");
          $("#pada_vs").html('˄');
          sessvars.vs.pada_ = 'expand';
        } else {
          $("#pada_body").slideUp("slow");
          $("#pada_vs").html('˅');
          sessvars.vs.pada_ = 'collapse';
        }
      })
    .end()
    .filter("#fida_vs")
      .click(function() {
        if ($("#fida_body").is(":hidden")) {
          $("#fida_body").slideDown("slow");
          $("#fida_vs").html('˄');
          sessvars.vs.fida_ = 'expand';
        } else {
          $("#fida_body").slideUp("slow");
          $("#fida_vs").html('˅');
          sessvars.vs.fida_ = 'collapse';
        }
      })
    .end()
    .filter("#foda_vs")
      .click(function() {
        if ($("#foda_body").is(":hidden")) {
          $("#foda_body").slideDown("slow");
          $("#foda_vs").html('˄');
          sessvars.vs.foda_ = 'expand';
        } else {
          $("#foda_body").slideUp("slow");
          $("#foda_vs").html('˅');
          sessvars.vs.foda_ = 'collapse';
        }
      })
    .end()
    .filter("#usda_vs")
      .click(function() {
        if ($("#usda_body").is(":hidden")) {
          $("#usda_body").slideDown("slow");
          $("#usda_vs").html('˄');
          sessvars.vs.usda_ = 'expand';
        } else {
          $("#usda_body").slideUp("slow");
          $("#usda_vs").html('˅');
          sessvars.vs.usda_ = 'collapse';
        }
      })
    .end();
});
