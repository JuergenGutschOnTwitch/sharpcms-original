$(function () {
    $('#choosePageDialog').dialog({
        modal: true
    });
});

$(function () {
    // Hyperlinks (Commonactions)
    var $hlSave = $('.hlSave');
    var $hlSaveAndShow = $('.hlSaveAndShow');
    var $hlShow = $('.hlShow');

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
        Sharpcms.Actions.ThrowEvent('save', '', '');
    });

    $hlSaveAndShow.live().click(function () {
        Sharpcms.Actions.ThrowEvent('save', 'openwindow', '');
    });

    $hlShow.live().live().click(function () {
        Sharpcms.Actions.ThrowEvent('', 'openwindow', '');
    });

    $hlRemoveContrainer.live().click(function () {
        var containerId = $(this).attr('value'); // container

        Sharpcms.Actions.ThrowEventConfirm('pageremovecontainer', containerId, 'This will remove the container.\n\nAre you sure?');
    });

    $hlRemoveElement.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // container / element
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        Sharpcms.Actions.ThrowEventConfirm('remove', 'element-' + containerId + '-' + elementId, 'Are you sure you want to delete this element?');
    });

    $hlMoveElementTop.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // container / element
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        Sharpcms.Actions.ThrowEvent('movetop', 'element-' + containerId + '-' + elementId, '');
    });

    $hlMoveElementDown.live().click(function () {
        var atrributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // container / element
        var containerId = atrributeValue[0];
        var elementId = atrributeValue[1];

        Sharpcms.Actions.ThrowEvent('movedown', 'element-' + containerId + '-' + elementId, '');
    });

    $hlMoveElementUp.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // container / element
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        Sharpcms.Actions.ThrowEvent('moveup', 'element-' + containerId + '-' + elementId, '');
    });

    $hlCopyElement.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // container / element
        var containerId = attributeValue[0];
        var elementId = attributeValue[1];

        Sharpcms.Actions.ThrowEvent('copy', 'element-' + containerId + '-' + elementId, '');
    });

    $hlAddLanguage.live().click(function () {
        var pageidentifier = $(this).attr('value'); // {attributes/pageidentifier}

        Sharpcms.Actions.ThrowEventNew('addpage', pageidentifier, 'Type the name of the new page:');
    });

    $hlChoosePage.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        //Sharpcms.ModalDialog.Show('admin/choose/page', 'ReturnMethodChoosePage("' + id + '", "' + attribute + '")');
    });

    $hlChooseFile.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        //Sharpcms.ModalDialog.Show('admin/choose/file', 'ReturnMethodChooseFile("' + id + '", "' + attribute + '")');
    });

    $hlChooseFolder.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        //Sharpcms.ModalDialog.Show('admin/choose/folder', 'ReturnMethodChooseFolder("' + id + '", "' + attribute + '")');
    });

    $hlThrowEvent.live().click(function () {
        var event = $(this).attr('action'); // @event

        Sharpcms.Actions.ThrowEvent(event, '', '');
    });

    $hlUploadFile.live().click(function () {
        var path = $(this).attr('value'); // @path

        Sharpcms.Actions.ThrowEvent('uploadfile', path, 'Type the name of the new folder:', '');
    });

    $hlRemoveFile.live().click(function () {
        var path = $(this).attr('value'); // @path

        Sharpcms.Actions.ThrowEventConfirm('removefile', path, 'Do you want to delete the file?');
    });

    $hlRenameFile.live().click(function () {
        var path = $(this).attr('value'); // @path

        Sharpcms.Actions.ThrowEventNew('renamefile', path, 'Write the new name');
    });

    $hlMoveFile.live().click(function () {
        var path = $(this).attr('value'); // @path    

        //Sharpcms.ModalDialog.Show('admin/choose/folder', 'ReturnMethodMoveFile("' + path + '")');
    });

    $hlResizeFile.live().click(function () {
        Sharpcms.Actions.ThrowEvent('doresize', '', '');
    });

    $hlAddFolder.live().click(function () {
        var path = $(this).attr('value'); // @path

        Sharpcms.Actions.ThrowEventNew('addfolder', path, 'Type the name of the new folder');
    });

    $hlRemoveFolder.live().click(function () {
        var path = $(this).attr('value'); // @path    

        Sharpcms.Actions.ThrowEventConfirm('removefolder', path, 'Do you want to delete the folder?');
    });

    $hlRenameFolder.live().click(function () {
        var path = $(this).attr('value'); // @path

        Sharpcms.Actions.ThrowEventNew('renamefolder', path, 'Write the new name');
    });

    $hlMoveFolder.live().click(function () {
        var path = $(this).attr('value'); // @path

        //Sharpcms.ModalDialog.Show('admin/choose/folder', 'ReturnMethodMoveFolder("' + path + '")');
    });

    $hlMoreFiles.live().click(function () {
        var currentlevel = $(this).attr('value'); // $currentlevel
        var level = parseInt(currentlevel) + 1;

        $('#file_' + level).show();
        $('#showlink_' + level).hide();
    });

    $hlAddUser.live().click(function () {
        Sharpcms.Actions.ThrowEventNew('adduser', '', 'Type the name of the new user');
    });

    $hlSaveUser.live().click(function () {
        var attributeValue = $(this).attr('value'); // login

        Sharpcms.Actions.ThrowEvent('saveuser', attributeValue, '');
    });

    $hlDeleteUser.live().click(function () {
        var login = $(this).attr('value'); // login

        Sharpcms.Actions.ThrowEventConfirm('deleteuser', login, 'Are you sure you wnat to delete the user?');
    });

    $hlAddGroup.live().click(function () {
        Sharpcms.Actions.ThrowEventNew('addgroup', '', 'Type the name of the new group');
    });

    $hlDeleteGroup.live().click(function () {
        var name = $(this).attr('value'); // @name

        Sharpcms.Actions.ThrowEventConfirm('deletegroup', name, 'Are you sure you wnat to delete the group?');
    });

    $('#adminmoreactions').change(function () {
        var attributeAction = $('#adminmoreactions :selected').attr('action');
        var attributeValue = $('#adminmoreactions :selected').attr('value').split(Sharpcms.Common.Splitters);

        if (attributeAction == 'removepage') {
            Sharpcms.Actions.ThrowEventConfirm(attributeAction, attributeValue[0], 'Do you want to delete the page?');
        } else if (attributeAction == 'addpage') {
            Sharpcms.Actions.ThrowEventNew(attributeAction, attributeValue[0], 'Type the name of the new page:');
        } else if (attributeAction == 'pagecreatcontainer') {
            Sharpcms.Actions.ThrowEventNew(attributeAction, '', 'Type the name of the new container:');
        } else if (attributeAction == 'pagemoveup') {
            Sharpcms.Actions.ThrowEvent(attributeAction, attributeValue[0], '');
        } else if (attributeAction == 'pagemovedown') {
            Sharpcms.Actions.ThrowEvent(attributeAction, attributeValue[0], '');
        } else if (attributeAction == 'pagemovetop') {
            Sharpcms.Actions.ThrowEvent(attributeAction, attributeValue[0], '');
        } else if (attributeAction == 'pagemovebottom') {
            Sharpcms.Actions.ThrowEvent(attributeAction, attributeValue[0], '');
        } else if (attributeAction == 'movepage') {
            Sharpcms.Actions.MovePage();
        } else if (attributeAction == 'copypage') {
            //Sharpcms.ModalDialog.Show('admin/choose/page/', 'Sharpcms.Actions.CopyPage("' + attributeValue[0] + '","' + attributeValue[1] + '")');
        } else if (attributeAction == 'setstandardpage') {
            Sharpcms.Actions.ThrowEvent('setstandardpage', '', '');
        }
    });

    $('.addelement').change(function () {
        var containerId = $(this).attr('name').replace('data_container_', '');

        Sharpcms.Actions.ThrowEvent('addelement', 'text_' + containerId, '');
    });

    // TinyMCE
    $('textarea.mceeditor').tinymce({
        script_url: '/System/Components/Admin/Scripts/tiny_mce/tiny_mce.js',

        theme: 'advanced',
        plugins: 'sharpcmschooser,fullscreen,spellchecker,safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,imagemanager,filemanager',
        skin: 'sharpcms',
        theme_advanced_buttons1: 'fullscreen,formatselect,cleanup,forecolor,bold,italic,underline,strikethrough,justifyleft,justifycenter,justifyright,image,indent,outdent,bullist,numlist,undo,redo,link,unlink',
        theme_advanced_buttons2: 'tablecontrols,|,sharpcmslinkchooser,sharpcmsfilechooser,sharpcmsimagechooser,|,code',
        theme_advanced_buttons3: '',
        theme_advanced_buttons4: '',
        theme_advanced_toolbar_location: 'top',
        theme_advanced_toolbar_align: 'left',
        theme_advanced_statusbar_location: 'bottom',
        theme_advanced_resizing: false,

        relative_urls: false,
        content_css: '/System/Components/Admin/Styles/tiny_mce/tinystyle.css'
    });

    // Tabs
    $('.tabs').tabs();

    // Select
    $('.pagedata_menu select').selectmenu({ width: 441 });
    $('.container_menu select').selectmenu({ width: 624 });

    // TreeView
    $('.filetree').treeview({
        persist: 'location',
        collapsed: true,
        unique: false
    });
});