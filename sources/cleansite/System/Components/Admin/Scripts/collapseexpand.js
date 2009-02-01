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
});

function collapseexpand(element, button) {
    if (document.getElementById(element).style.display == 'none') {
        document.getElementById(element).style.display = '';
        document.getElementById(button).innerHTML = '˄';
        sessvars.vs[element.toString().replace('body', '')] = 'expand';
    }
    else {
        document.getElementById(element).style.display = 'none';
        document.getElementById(button).innerHTML = '˅';
        sessvars.vs[element.toString().replace('body', '')] = 'collapse';
    }
}
