function ThrowEvent(mainevent, mainvalue) {
    document.systemform.event_main.value = mainevent;
    document.systemform.event_mainvalue.value = mainvalue;
    document.systemform.submit();
}

function ThrowEventConfirm(mainevent, mainvalue, confirmtext) {
    if (confirm(confirmtext)) {
        ThrowEvent(mainevent, mainvalue);
    }
}

function ThrowEventNew(mainevent, mainvalue, text, suggest) {
    if (suggest == null) suggest = '';

    var input = prompt(text, suggest);
    if (input != null) {
        if (mainvalue != '')
            ThrowEvent(mainevent, mainvalue + '*' + input);
        else
            ThrowEvent(mainevent, input);
    }
}

function showAndHide(showLayerId, hideLayerId) {
    setDisplay(showLayerId, 'block');
    setDisplay(hideLayerId, 'none');
}

function setDisplay(whichElement, newValue) {
    var style;
    if (document.getElementById) {
        // this is the way the standards work
        style = document.getElementById(whichElement).style;
        style.display = newValue;
    }
    else if (document.all) {
        // this is the way old msie versions work
        style = document.all[whichElement].style;
        style.display = newValue;
    }
    else if (document.layers) {
        // this is the way nn4 works
        style = document.layers[whichElement].style;
        style.display = newValue;
    }
}

function open_window(url, name) {
    var editwinWin = window.open(url, name);
    editwinWin.focus();
    editwinWin.opener = self;
}

function open_editwin(url, name, winWidth, winHeight) {
    if (winWidth == null) winWidth = '500';
    if (winHeight == null) winHeight = '500';

    var editwin = '';
    editwin = editwin + 'width=' + winWidth;
    editwin = editwin + ',height=' + winHeight;
    editwin = editwin + ',resizable=yes';
    editwin = editwin + ',scrollbars=yes';
    editwin = editwin + ',menubar=no';
    editwin = editwin + ',toolbar=no';
    editwin = editwin + ',directories=no';
    editwin = editwin + ',location=no';
    editwin = editwin + ',status=no';
    editwin = editwin + ',top=20';
    editwin = editwin + ',left=250';

    var editwinWin = window.open(url, name, editwin);
    editwinWin.focus();
    editwinWin.opener = self;
}