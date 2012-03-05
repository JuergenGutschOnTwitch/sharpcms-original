/* ModalDialog */
var dialogBoxWindow;
var dialogBoxInterval;
var dialogBox = new Object;

function maintainModalDialogFocus() {
    try {
        if (dialogBoxWindow.closed) {
            window.clearInterval(dialogBoxInterval);
            eval(dialogBox.eventhandler);
            return;
        }

        dialogBoxWindow.focus();
    }
    catch (everything) { }
}

function resetModalDialog() {
    dialogBox.value = '';
    dialogBox.eventhandler = '';
}

function showModalDialog(path, eventHandler) {
    resetModalDialog();
    dialogBox.eventhandler = eventHandler;

    var args = 'width=450,height=525,left=125,top=100,toolbar=0,location=0,status=0,menubar=0,scrollbars=1,resizable=1';

    dialogBoxWindow = window.open(Sharpcms.Common.BaseUrl + path, '', args);
    dialogBoxWindow.focus();
    dialogBoxInterval = window.setInterval('maintainModalDialogFocus()', 5);
}

function closeModalDialog(response) {
    window.opener.dialogBox.value = response;
    window.close();
}
/* ModalDialog */

(function ($) {
    function common() {
        var baseUrl;
        var splitters;

        function init() {
            baseUrl = $('head base').attr('href');
            splitters = (';;');
        }

        init();

        return {
            BaseUrl: baseUrl,
            Splitters: splitters
        };
    }

    function actions() {
        function init() {

        }

        function throwEvent(mainevent, mainvalue, redirect) {
            document.systemform.event_main.value = mainevent;
            document.systemform.event_mainvalue.value = mainvalue;

            if (redirect != '') {
                document.systemform.event_redirect.value = redirect;
            }

            document.systemform.submit();
        }

        function throwEventConfirm(mainevent, mainvalue, confirmtext) {
            if (confirm(confirmtext)) {
                throwEvent(mainevent, mainvalue, '');
            }
        }

        function throwEventNew(mainevent, mainvalue, text, suggest) {
            if (suggest == null) suggest = '';

            var input = prompt(text, suggest);
            if (input != null) {
                if (mainvalue != '') {
                    throwEvent(mainevent, mainvalue + '*' + input, '');
                } else {
                    throwEvent(mainevent, input, '');
                }
            }
        }

        function movePage() {
            showModalDialog('admin/choose/page/', throwMovePageEvent);
        }

        function throwMovePageEvent() {
            if (dialogBox.value != undefined && dialogBox.value != '') {
                throwEvent(Sharpcms.ActionType.MovePage, dialogBox.value, '');
            }

            resetModalDialog();
        }

        var copyPagePromptMessage = 'Page name:';
        function copyPage(pageidentifier, pagename) {
            if (dialogBox.value != undefined && dialogBox.value != '') {
                throwEvent(Sharpcms.ActionType.CopyPage, pageidentifier + '¤' + dialogBox.value + '¤' + prompt(copyPagePromptMessage, pagename, ''));
            }

            resetModalDialog();
        }

        function openWindow(url, name) {
            var editwin = '';
            editwin += 'width=500';
            editwin += ',height=500';
            editwin += ',resizable=yes';
            editwin += ',scrollbars=yes';
            editwin += ',menubar=no';
            editwin += ',toolbar=no';
            editwin += ',directories=no';
            editwin += ',location=no';
            editwin += ',status=no';
            editwin += ',top=20';
            editwin += ',left=250';

            var editwinWin = window.open(url, name, editwin);
            editwinWin.focus();
            editwinWin.opener = self;
        }

        init();

        return {
            MovePage: movePage,
            CopyPage: copyPage,
            ThrowEvent: throwEvent,
            ThrowEventNew: throwEventNew,
            ThrowEventConfirm: throwEventConfirm,
            OpenWindow: openWindow
        };
    }

    $.extend(true, window, {
        Sharpcms: {
            Common: common(),
            Actions: actions(),
            ActionType: {
                MovePage: 'pagemove',
                CopyPage: 'pagecopyto'
            }
        }
    });
})(jQuery);