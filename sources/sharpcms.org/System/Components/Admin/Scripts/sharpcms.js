$(document).ready(function () {
    // Variables
    var splitters = (';;');

    // Hyperlinks (Commonactions)
    var $hlSave = $('.hlSave');
    var $hlSaveAndShow = $('.hlSaveAndShow');
    var $hlShow = $('.hlShow');
    var $hlCloseForm = $('.hlCloseForm');

    // Hyperlinks (Pages)
    var $hlRemoveContrainer = $('.hlRemoveContrainer');
    var $hlRemoveElement = $('.hlRemoveElement');
    var $hlMoveElementTop = $('.hlMoveElementTop');
    var $hlMoveElementDown = $('.hlMoveElementDown');
    var $hlMoveElementUp = $('.hlMoveElementUp');
    var $hlCopyElement = $('.hlCopyElement');
    var $hlAddLanguage = $('.hlAddLanguage');
    var $hlThrowEvent = $('.hlThrowEvent');

    // Hyperlinks (Files)
    var $hlUploadFile = $('.hlUploadFile');
    var $hlRemoveFile = $('.hlRemoveFile');
    var $hlRenameFile = $('.hlRenameFile');
    var $hlMoveFile = $('.hlMoveFile');
    var $hlResizeFile = $('.hlResizeFile');
    var $hlAddFolder = $('.hlAddFolder');
    var $hlRemoveFolder = $('.hlRemoveFolder');
    var $hlRenameFolder = $('.hlRenameFolder');
    var $hlMoveFolder = $('.hlMoveFolder');
    var $hlMoreFiles = $('.hlMoreFiles');

    // Hyperlinks (Users/Groups)
    var $hlAddUser = $('.hlAddUser');
    var $hlAddGroup = $('.hlAddGroup');
    var $hlDeleteGroup = $('.hlDeleteGroup');
    var $hlSaveUser = $('.hlSaveUser');
    var $hlDeleteUser = $('.hlDeleteUser');

    // Hyperlinks (Choose)
    var $hlChoosePage = $('.hlChoosePage');
    var $hlChooseFile = $('.hlChooseFile');
    var $hlChooseFolder = $('.hlChooseFolder');

    $hlSave.live().click(function () {
        ThrowEvent('save', '');
    });

    $hlSaveAndShow.live().click(function () {
        ThrowEvent('save', 'openwindow');
    });

    $hlShow.live().live().click(function () {
        ThrowEvent('', 'openwindow');
    });

    $hlRemoveContrainer.live().click(function () {
        var containerId = $(this).attr('value'); // container

        ThrowEventConfirm('pageremovecontainer', containerId, 'This will remove the container.\n\nAre you sure?');
    });

    $hlRemoveElement.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // container / element
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        ThrowEventConfirm('remove', 'element-' + containerId + '-' + elementId, 'Are you sure you want to delete this element?');
    });

    $hlMoveElementTop.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // container / element
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        ThrowEvent('movetop', 'element-' + containerId + '-' + elementId);
    });

    $hlMoveElementDown.live().click(function () {
        var atrributeValue = $(this).attr('value').split(splitters); // container / element
        var containerId = atrributeValue[0];
        var elementId = atrributeValue[1];

        ThrowEvent('movedown', 'element-' + containerId + '-' + elementId);
    });

    $hlMoveElementUp.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // container / element
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        ThrowEvent('moveup', 'element-' + containerId + '-' + elementId);
    });

    $hlCopyElement.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // container / element
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        ThrowEvent('copy', 'element-' + containerId + '-' + elementId);
    });

    $hlAddLanguage.live().click(function () {
        var pageidentifier = $(this).attr('value'); // {attributes/pageidentifier}

        ThrowEventNew('addpage', pageidentifier, 'Type the name of the new page:');
    });

    $hlChoosePage.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        ModalDialogShow('admin/choose/page', 'ReturnMethodChoosePage("' + id + '", "' + attribute + '")');
    });

    $hlChooseFile.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];
        
        ModalDialogShow('admin/choose/file', 'ReturnMethodChooseFile("' + id + '", "' + attribute + '")');
    });

    $hlChooseFolder.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        ModalDialogShow('admin/choose/folder', 'ReturnMethodChooseFolder("' + id + '", "' + attribute + '")');
    });

    $hlThrowEvent.live().click(function () {
        var event = $(this).attr('action'); // @event

        ThrowEvent(event, '');
    });

    $hlUploadFile.live().click(function () {
        var path = $(this).attr('value'); // @path

        ThrowEvent('uploadfile', path, 'Type the name of the new folder:');
    });

    $hlRemoveFile.live().click(function () {
        var path = $(this).attr('value'); // @path

        ThrowEventConfirm('removefile', path, 'Do you want to delete the file?');
    });

    $hlRenameFile.live().click(function () {
        var path = $(this).attr('value'); // @path

        ThrowEventNew('renamefile', path, 'Write the new name');
    });

    $hlMoveFile.live().click(function () {
        var path = $(this).attr('value'); // @path    

        ModalDialogShow('admin/choose/folder', 'ReturnMethodMoveFile("' + path + '")');
    });

    $hlResizeFile.live().click(function () {
        ThrowEvent('doresize', '');
    });

    $hlAddFolder.live().click(function () {
        var path = $(this).attr('value'); // @path

        ThrowEventNew('addfolder', path, 'Type the name of the new folder');
    });

    $hlRemoveFolder.live().click(function () {
        var path = $(this).attr('value'); // @path    

        ThrowEventConfirm('removefolder', path, 'Do you want to delete the folder?');
    });

    $hlRenameFolder.live().click(function () {
        var path = $(this).attr('value'); // @path

        ThrowEventNew('renamefolder', path, 'Write the new name');
    });

    $hlMoveFolder.live().click(function () {
        var path = $(this).attr('value'); // @path

        ModalDialogShow('admin/choose/folder', 'ReturnMethodMoveFolder("' + path + '")');
    });

    $hlMoreFiles.live().click(function () {
        var currentlevel = $(this).attr('value'); // $currentlevel
        var level = parseInt(currentlevel) + 1;

        ShowAndHide('file_' + level, 'showlink_' + level);
    });

    $hlAddUser.live().click(function () {
        ThrowEventNew('adduser', '', 'Type the name of the new user');
    });

    $hlSaveUser.live().click(function () {
        var attributeValue = $(this).attr('value'); // login

        ThrowEvent('saveuser', attributeValue);
    });

    $hlDeleteUser.live().click(function () {
        var login = $(this).attr('value'); // login

        ThrowEventConfirm('deleteuser', login, 'Are you sure you wnat to delete the user?');
    });

    $hlAddGroup.live().click(function () {
        ThrowEventNew('addgroup', '', 'Type the name of the new group');
    });

    $hlDeleteGroup.live().click(function () {
        var name = $(this).attr('value'); // @name

        ThrowEventConfirm('deletegroup', name, 'Are you sure you wnat to delete the group?');
    });

    $('#adminmoreactions').change(function () {
        var attributeAction = $('#adminmoreactions :selected').attr('action');
        var attributeValue = $('#adminmoreactions :selected').attr('value').split(';;');

        if (attributeAction == 'removepage') {
            ThrowEventConfirm(attributeAction, attributeValue[0], 'Do you want to delete the page?');
        } else if (attributeAction == 'addpage') {
            ThrowEventNew(attributeAction, attributeValue[0], 'Type the name of the new page:');
        } else if (attributeAction == 'pagecreatcontainer') {
            ThrowEventNew(attributeAction, '', 'Type the name of the new container:');
        } else if (attributeAction == 'pagemoveup') {
            ThrowEvent(attributeAction, attributeValue[0]);
        } else if (attributeAction == 'pagemovedown') {
            ThrowEvent(attributeAction, attributeValue[0]);
        } else if (attributeAction == 'pagemovetop') {
            ThrowEvent(attributeAction, attributeValue[0]);
        } else if (attributeAction == 'pagemovebottom') {
            ThrowEvent(attributeAction, attributeValue[0]);
        } else if (attributeAction == 'movepage') {
            ModalDialogShow('admin/choose/page/', 'MovePage()');
        } else if (attributeAction == 'copypage') {
            ModalDialogShow('admin/choose/page/', 'CopyPage("' + attributeValue[0] + '","' + attributeValue[1] + '")');
        } else if (attributeAction == 'setstandardpage') {
            ThrowEvent('setstandardpage', '');
        }
    });

    $('.addelement').change(function () {
        var containerId = $(this).attr('name').replace('data_container_', '');

        ThrowEvent('addelement', 'text_' + containerId);
    });

    $hlCloseForm.live().click(function () {
        var attributeName = $(this).attr('value'); // $current-path + @name || $current-path

        CloseForm(attributeName);
    });
});

function MovePage() {
    if (ModalDialog.value != undefined && ModalDialog.value != '') {
        ThrowEvent('pagemove', ModalDialog.value);
    }

    ModalDialogRemoveWatch();
}

function CopyPage(pageidentifier, pagename) {
    if (ModalDialog.value != undefined && ModalDialog.value != '') {
        ThrowEvent('pagecopyto', pageidentifier + '¤' + ModalDialog.value + '¤' + prompt('Page name:', pagename));
    }

    ModalDialogRemoveWatch();
}

function ReturnMethodTinyMceChoosePage() {
    var html = '<a href="' + baseUrl + 'show/' + ModalDialog.value + '/">{$selection}</a>';
    tinyMCE.execCommand('mceReplaceContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}

function ReturnMethodTinyMceChooseFile() {
    var html = '<a href="' + baseUrl + 'download/' + ModalDialog.value + '?download=true">{$selection}</a>';
    tinyMCE.execCommand('mceReplaceContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}

function ReturnMethodTinyMceChoosePicture() {
    var html = '<img src="' + baseUrl + 'download/' + ModalDialog.value + '" />';
    tinyMCE.execCommand('mceInsertContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}

function ReturnMethodMoveFile(path) {
    var modalDialogValue = ModalDialog.value;
    
    ModalDialogRemoveWatch();
    ThrowEvent('movefile', path + '*' + modalDialogValue);
}

function ReturnMethodMoveFolder(path) {
    var modalDialogValue = ModalDialog.value;
    
    ModalDialogRemoveWatch();
    ThrowEvent('movefolder', path + '*' + modalDialogValue);
}

function ReturnMethodChoosePage(id, attribute) {
    $('[name="' + id + '_' + attribute + '"]').val(ModalDialog.value);

    ModalDialogRemoveWatch();
}

function ReturnMethodChooseFile(id, attribute) {
    $('[name="' + id + '_' + attribute + '"]').val(ModalDialog.value);

    ModalDialogRemoveWatch();
}

function ReturnMethodChooseFolder(id, attribute) {
    $('[name="' + id + '_' + attribute + '"]').val(ModalDialog.value);

    ModalDialogRemoveWatch();
}

function ThrowEvent(mainevent, mainvalue, redirect) {
    document.systemform.event_main.value = mainevent;
    document.systemform.event_mainvalue.value = mainvalue;
    document.systemform.event_redirect.value = redirect;
    document.systemform.submit();
}

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
        if (mainvalue != '') {
            ThrowEvent(mainevent, mainvalue + '*' + input);
        } else {
            ThrowEvent(mainevent, input);
        }
    }
}

function ShowAndHide(showLayerId, hideLayerId) {
    SetDisplay(showLayerId, 'block');
    SetDisplay(hideLayerId, 'none');
}

function SetDisplay(whichElement, newValue) {
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

var ModalDialogWindow;
var ModalDialogInterval;
var ModalDialog = new Object;

ModalDialog.value = '';
ModalDialog.eventhandler = '';

function CloseForm(response) {
    window.opener.ModalDialog.value = response;
    window.close();
}

function ModalDialogMaintainFocus() {
    try {
        if (ModalDialogWindow.closed) {
            window.clearInterval(ModalDialogInterval);
            eval(ModalDialog.eventhandler);
            return;
        }
        ModalDialogWindow.focus();
    }
    catch (everything) { }
}

function ModalDialogRemoveWatch() {
    ModalDialog.value = '';
    ModalDialog.eventhandler = '';
}

function ModalDialogShow(path, eventHandler) {
    ModalDialogRemoveWatch();
    ModalDialog.eventhandler = eventHandler;

    var args = 'width=450,height=525,left=125,top=100,toolbar=0,location=0,status=0,menubar=0,scrollbars=1,resizable=1';

    ModalDialogWindow = window.open(baseUrl + path, '', args);
    ModalDialogWindow.focus();
    ModalDialogInterval = window.setInterval('ModalDialogMaintainFocus()', 5);
}