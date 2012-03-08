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

        function setAsStartPage() {
            throwEvent(Sharpcms.ActionType.SetAsStartPage, '', '');
        }

        function movePage() {
            $('#choosePageDialog').dialog(function () { $(this).modal = true; });
            $('.hlCloseDialog').live().click(function() {
                $('.choose').dialog('close');
                
                var path = $(this).attr('value'); // @path
                throwEvent(Sharpcms.ActionType.MovePage, path, '');
            });
        }

        function movePageUp(pageId) {
            throwEvent(Sharpcms.ActionType.MovePageUp, pageId, '');
        }
        
        function movePageDown(pageId) {
            throwEvent(Sharpcms.ActionType.MovePageDown, pageId, '');
        }

        function movePageTop(pageId) {
            throwEvent(Sharpcms.ActionType.MovePageTop, pageId, '');
        }

        function movePageBottom(pageId) {
            throwEvent(Sharpcms.ActionType.MovePageBottom, pageId, '');
        }

        function createPageContainer() {
            throwEventNew(Sharpcms.ActionType.CreatePageContainer, '', 'Type the name of the new container:');
        }

        function removePageContainer(containerId) {
            throwEventConfirm(Sharpcms.ActionType.RemovePageContainer, containerId, 'This will remove the container.\n\nAre you sure?');
        }

        function addPage(pageId) {
            throwEventNew(Sharpcms.ActionType.AddPage, pageId, 'Type the name of the new page:');
        }

        var copyPagePromptMessage = 'Page name:';
        function copyPage(pageidentifier, pagename) {
            if (dialogBox.value != undefined && dialogBox.value != '') {
                throwEvent(Sharpcms.ActionType.CopyPage, pageidentifier + '¤' + dialogBox.value + '¤' + prompt(copyPagePromptMessage, pagename, ''));
            }

            resetModalDialog();
        }
        
        function removePage(pageId) {
            throwEventConfirm(Sharpcms.ActionType.RemovePage, pageId, 'Do you want to delete the page?');
        }

        function savePage() {
            throwEvent(Sharpcms.ActionType.SavePage, '', '');
        }

        function addElement(containerId) {
            throwEvent(Sharpcms.ActionType.AddElement, 'text_' + containerId, '');
        }

        function removeElement(containerId, elementId) {
            throwEventConfirm(Sharpcms.ActionType.RemoveElement, 'element-' + containerId + '-' + elementId, 'Are you sure you want to delete this element?');
        }

        function moveElementUp(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.MoveElementUp, 'element-' + containerId + '-' + elementId, '');
        }

        function moveElementDown(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.MoveElementDown, 'element-' + containerId + '-' + elementId, '');
        }

        function moveElementTop(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.MoveElementTop, 'element-' + containerId + '-' + elementId, '');
        }
        
        function moveElementBottom(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.MoveElementBottom, 'element-' + containerId + '-' + elementId, '');
        }

        function copyElement(containerId, elementId) {
            throwEvent(Sharpcms.ActionTypeCopy, 'element-' + containerId + '-' + elementId, '');
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
            AddPage: addPage,
            RemovePage: removePage,
            SavePage: savePage,
            SetAsStartPage: setAsStartPage,
            
            MovePage: movePage,
            MovePageUp: movePageUp,
            MovePageDown: movePageDown,
            MovePageTop: movePageTop,
            MovePageBottom: movePageBottom,
            
            CopyPage: copyPage,
            
            CreatePageContainer: createPageContainer,
            RemovePageContainer: removePageContainer,
            
            AddElement: addElement,
            RemoveElement: removeElement,

            MoveElementUp: moveElementUp,
            MoveElementDown: moveElementDown,
            MoveElementTop: moveElementTop,
            MoveElementBottom: moveElementBottom,

            CopyElement: copyElement,

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
                AddPage: 'addpage',
                RemovePage: 'removepage',
                SavePage: 'save',
                SetAsStartPage: 'setstandardpage',
                
                MovePage: 'pagemove',
                MovePageUp: 'pagemoveup',
                MovePageDown: 'pagemovedown',
                MovePageTop: 'pagemovetop',
                MovePageBottom: 'pagemovebottom',
                
                CopyPage: 'pagecopyto',
                
                CreatePageContainer: 'pagecreatcontainer',
                RemovePageContainer: 'pageremovecontainer',
                
                AddElement: 'addelement',
                RemoveElement: 'remove',
                
                MoveElementUp: 'moveup',
                MoveElementDown: 'movedown',
                MoveElementTop: 'movetop',
                MoveElementBottom: 'movebottom',
                
                CopyElement: 'copy'
            }
        }
    });
})(jQuery);