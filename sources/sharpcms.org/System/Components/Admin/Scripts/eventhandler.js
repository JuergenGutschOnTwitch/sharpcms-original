$(document).ready(function () {
    // Variables
    var splitters = (';;');

    // Buttons
    var $btnSave = $('.btnSave');
    var $btnSaveAndShow = $('.btnSaveAndShow');
    var $btnShow = $('.btnShow');

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

    $btnSave.live().click(function () {
        ThrowEvent('save', '');
    });

    $btnSaveAndShow.live().click(function () {
        ThrowEvent('save', 'openwindow');
    });

    $btnShow.live().live().click(function () {
        ThrowEvent('', 'openwindow');
    });

    $hlRemoveContrainer.live().click(function () {
        var containerId = $(this).attr('value');

        ThrowEventConfirm('pageremovecontainer', containerId, 'This will remove the container.\n\nAre you sure?');
    });

    $hlRemoveElement.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters);
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        ThrowEventConfirm('remove', 'element-' + containerId + '-' + elementId, 'Are you sure you want to delete this element?');
    });

    $hlMoveElementTop.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters);
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        ThrowEvent('movetop', 'element-' + containerId + '-' + elementId);
    });

    $hlMoveElementDown.live().click(function () {
        var atrributeValue = $(this).attr('value').split(splitters);
        var containerId = atrributeValue[0];
        var elementId = atrributeValue[1];

        ThrowEvent('movedown', 'element-' + containerId + '-' + elementId);
    });

    $hlMoveElementUp.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters);
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        ThrowEvent('moveup', 'element-' + containerId + '-' + elementId);
    });

    $hlCopyElement.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters);
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        ThrowEvent('copy', 'element-' + containerId + '-' + elementId);
    });

    $hlAddLanguage.live().click(function () {
        var attributeValue = $(this).attr('value');

        ThrowEventNew('addpage', attributeValue, 'Type the name of the new page:');
    });

    $hlChoosePage.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // {/data/basepath} / $id / @attribute

        ModalDialogShow(attributeValue[0] + '/admin/choose/page', 'ReturnMethodChoosePage("' + attributeValue[1] + '", "' + attributeValue[2] + '")');
    });

    $hlChooseFile.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // {/data/basepath} / $id / @attribute

        ModalDialogShow(attributeValue[0] + '/admin/choose/file', 'ReturnMethodChooseFile("' + attributeValue[1] + '", "' + attributeValue[2] + '")');
    });

    $hlChooseFolder.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // {/data/basepath} / $id / @attribute

        ModalDialogShow(attributeValue[0] + '/admin/choose/folder', 'ReturnMethodChooseFolder("' + attributeValue[1] + '", "' + attributeValue[2] + '")');
    });

    $hlThrowEvent.live().click(function () {
        var attributeAction = $(this).attr('action'); // @event

        ThrowEvent(attributeAction, '');
    });

    $hlUploadFile.live().click(function () {
        var attributeValue = $(this).attr('value'); // @path

        ThrowEvent('uploadfile', attributeValue, 'Type the name of the new folder:');
    });

    $hlRemoveFile.live().click(function () {
        var attributeValue = $(this).attr('value'); // @path

        ThrowEventConfirm('removefile', attributeValue, 'Do you want to delete the file?');
    });

    $hlRenameFile.live().click(function () {
        var attributeValue = $(this).attr('value'); // @path

        ThrowEventNew('renamefile', attributeValue, 'Write the new name');
    });

    $hlMoveFile.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // {/data/basepath} / @path    

        ModalDialogShow(attributeValue[0] + '/admin/choose/folder', 'ReturnMethodMoveFile("' + attributeValue[1] + '")');
    });

    $hlResizeFile.live().click(function () {
        ThrowEvent('doresize', '');
    });

    $hlAddFolder.live().click(function () {
        var attributeValue = $(this).attr('value'); // @path

        ThrowEventNew('addfolder', attributeValue, 'Type the name of the new folder');
    });

    $hlRemoveFolder.live().click(function () {
        var attributeValue = $(this).attr('value'); // @path    

        ThrowEventConfirm('removefolder', attributeValue, 'Do you want to delete the folder?');
    });

    $hlRenameFolder.live().click(function () {
        var attributeValue = $(this).attr('value'); // @path

        ThrowEventNew('renamefolder', attributeValue, 'Write the new name');
    });

    $hlMoveFolder.live().click(function () {
        var attributeValue = $(this).attr('value').split(splitters); // {/data/basepath} / @path

        ModalDialogShow(attributeValue[0] + '/admin/choose/folder', 'ReturnMethodMoveFolder("' + attributeValue[1] + '")');
    });

    $hlMoreFiles.live().click(function () {
        var attributeValue = $(this).attr('value'); // $currentlevel
        var level = parseInt(attributeValue) + 1;

        ShowAndHide('file_' + level, 'showlink_' + level);
    });

    $hlAddUser.live().click(function () {
        ThrowEventNew('adduser', '', 'Type the name of the new user');
    });

    $hlSaveUser.live().click(function () {
        var attributeValue = $(this).attr('value'); // login
        
        ThrowEvent('saveuser', attributeValue);
    });

    $hlDeleteUser.live().click(function() {
        var attributeValue = $(this).attr('value'); // login
        
        ThrowEventConfirm('deleteuser', attributeValue, 'Are you sure you wnat to delete the user?');
    });

    $hlAddGroup.live().click(function () {
        ThrowEventNew('addgroup', '', 'Type the name of the new group');
    });

    $hlDeleteGroup.live().click(function () {
        var attributeValue = $(this).attr('value'); // @name

        ThrowEventConfirm('deletegroup', attributeValue, 'Are you sure you wnat to delete the group?');
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
            ModalDialogShow(attributeValue[0] + '/admin/choose/page/', 'MovePage()');
        } else if (attributeAction == 'copypage') {
            ModalDialogShow(attributeValue[0] + '/admin/choose/page/', 'CopyPage("' + attributeValue[1] + '","' + attributeValue[2] + '")');
        } else if (attributeAction == 'setstandardpage') {
            ThrowEvent('setstandardpage', '');
        }
    });

    $('.addelement').change(function () {
        var containerId = $(this).attr('name').replace('data_container_', '');

        ThrowEvent('addelement', 'text_' + containerId);
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