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



function ReturnMethodTinyMceChoosePage() {
    var html = '<a href="' + Sharpcms.Common.BaseUrl + 'show/' + Sharpcms.ModalDialog.DialogBox.value + '/">{$selection}</a>';
    
    tinyMCE.execCommand('mceReplaceContent', false, html);
    tinyMCEPopup.close();
    Sharpcms.ModalDialog.Reset();
}

function ReturnMethodTinyMceChooseFile() {
    var html = '<a href="' + Sharpcms.Common.BaseUrl + 'download/' + Sharpcms.ModalDialog.DialogBox.value + '?download=true">{$selection}</a>';
    
    tinyMCE.execCommand('mceReplaceContent', false, html);
    tinyMCEPopup.close();
    Sharpcms.ModalDialog.Reset();
}

function ReturnMethodTinyMceChoosePicture() {
    var html = '<img src="' + Sharpcms.Common.BaseUrl + 'download/' + Sharpcms.ModalDialog.DialogBox.value + '" />';
    
    tinyMCE.execCommand('mceInsertContent', false, html);
    tinyMCEPopup.close();
    Sharpcms.ModalDialog.Reset();
}

function ReturnMethodMoveFile(path) {
    var modalDialogValue = Sharpcms.ModalDialog.DialogBox.value;

    Sharpcms.ModalDialog.Reset();
    Sharpcms.Actions.ThrowEvent('movefile', path + '*' + modalDialogValue, '');
}

function ReturnMethodMoveFolder(path) {
    var modalDialogValue = ModalDialog.value;

    Sharpcms.ModalDialog.Reset();
    Sharpcms.Actions.ThrowEvent('movefolder', path + '*' + modalDialogValue, '');
}