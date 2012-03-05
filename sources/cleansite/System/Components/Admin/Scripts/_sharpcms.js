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

function ReturnMethodChoosePage(id, attribute) {
    $('[name="' + id + '_' + attribute + '"]').val(Sharpcms.ModalDialog.DialogBox.value);

    Sharpcms.ModalDialog.Reset();
}

function ReturnMethodChooseFile(id, attribute) {
    $('[name="' + id + '_' + attribute + '"]').val(Sharpcms.ModalDialog.DialogBox.value);

    Sharpcms.ModalDialog.Reset();
}

function ReturnMethodChooseFolder(id, attribute) {
    $('[name="' + id + '_' + attribute + '"]').val(Sharpcms.ModalDialog.DialogBox.value);

    Sharpcms.ModalDialog.Reset();
}