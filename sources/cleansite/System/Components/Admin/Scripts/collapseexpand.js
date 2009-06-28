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

  if ($.url.param("e") != null) {
    if ($.url.param("e") != "") {
      var s = $.url.param("e");
      var h = 0;
      var j = parseInt(s.substring(s.length - 1, s.length));
      var i = 0;
      for (i = 1; i < j; i++)
        h += $("#" + s.substring(0, s.length - 1) + i + "_body").height() + 7;

      var destination = $("#" + $.url.param("e") + "_anchor").offset().top - h + 8;

      if ($("#" + $.url.param("e") + "_body").is(":hidden")) {
        $("#" + $.url.param("e") + "_body").slideDown("fast");
        $("#" + $.url.param("e")).html('˄');
      } else {
        $("#" + $.url.param("e") + "_body").slideUp("fast");
        $("#" + $.url.param("e")).html('˅');
      }

      $("html:not(:animated),body:not(:animated)").animate({ scrollTop: destination }, 800, function() { });
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