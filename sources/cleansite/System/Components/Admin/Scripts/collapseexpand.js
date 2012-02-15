// jQeury to Load
// http: //www.thomasfrank.se/sessionvars.html

$(document).ready(function () {
    if (sessvars.vs == null) {
        sessvars.vs = { pada_: 'collapse', fida_: 'expand', foda_: 'expand', usda_: 'expand' };
    }

    var sessvar;
    for (sessvar in sessvars.vs) {
        if (document.getElementById(sessvar + 'body') != null && document.getElementById(sessvar + 'vs') != null) {
            if (sessvars.vs[sessvar] == 'expand') {
                $('#' + sessvar + 'body').show();
                $('#' + sessvar + 'vs').html('˄');
            }
            else {
                $('#' + sessvar + 'body').hide();
                $('#' + sessvar + 'vs').html('˅');
            }
        }
    }

    var cmsElementId = new (function () {
        var result;
        if ($.url.param('e') != null && $.url.param('e') != '') {
            result = $.url.param('e').split('_');
        }

        if (result != null && result.length == 3) {
            return { ContainerId: parseInt(result[1]), ElementId: parseInt(result[2]) };
        } else {
            return { ContainerId: '0', ElementId: '0' };
        }
    });

    $('.element_body').hide();

    if (cmsElementId.ContainerId != '0' && cmsElementId.ElementId != '0') {
        var tabId = cmsElementId.ContainerId - 1;
        if ($('#tabs').tabs().length < tabId) {
            return;
        }

        var baseElementName = '#element_' + cmsElementId.ContainerId + '_' + cmsElementId.ElementId;
        if ($(baseElementName).length == 0) {
            return;
        }

        if ($(baseElementName + '_body').is(':hidden')) {
            $(baseElementName + '_body').slideDown('fast');
            $(baseElementName).html('˄');
        } else {
            $(baseElementName + '_body').slideUp('fast');
            $(baseElementName).html('˅');
        }

        $('#tabs').tabs('select', tabId);
    }



    $('a.expand').click(function () {
        var id = this.id;
        if (id != '') {
            if ($('#' + id + '_body').is(':hidden')) {
                $('#' + id + '_body').slideDown('fast');
                $('#' + id).html('˄');
            } else {
                $('#' + id + '_body').slideUp('fast');
                $('#' + id).html('˅');
            }
        }
    });

    $('a.expand')
        .filter('#pada_vs').click(function () {
            if ($('#pada_body').is(':hidden')) {
                $('#pada_body').slideDown('fast');
                $('#pada_vs').html('˄');
                sessvars.vs.pada_ = 'expand';
            } else {
                $('#pada_body').slideUp('fast');
                $('#pada_vs').html('˅');
                sessvars.vs.pada_ = 'collapse';
            }
        })
        .end()
        .filter('#fida_vs').click(function () {
            if ($('#fida_body').is(':hidden')) {
                $('#fida_body').slideDown('fast');
                $('#fida_vs').html('˄');
                sessvars.vs.fida_ = 'expand';
            } else {
                $('#fida_body').slideUp('fast');
                $('#fida_vs').html('˅');
                sessvars.vs.fida_ = 'collapse';
            }
        })
        .end()
        .filter('#foda_vs').click(function () {
            if ($('#foda_body').is(':hidden')) {
                $('#foda_body').slideDown('fast');
                $('#foda_vs').html('˄');
                sessvars.vs.foda_ = 'expand';
            } else {
                $('#foda_body').slideUp('fast');
                $('#foda_vs').html('˅');
                sessvars.vs.foda_ = 'collapse';
            }
        })
        .end()
        .filter('#usda_vs').click(function () {
            if ($('#usda_body').is(':hidden')) {
                $('#usda_body').slideDown('fast');
                $('#usda_vs').html('˄');
                sessvars.vs.usda_ = 'expand';
            } else {
                $('#usda_body').slideUp('fast');
                $('#usda_vs').html('˅');
                sessvars.vs.usda_ = 'collapse';
            }
        })
        .end();
});