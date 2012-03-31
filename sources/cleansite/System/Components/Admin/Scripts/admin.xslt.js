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
    var $hlAddPage = $('.hlAddPage');
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
        Sharpcms.Actions.SavePage();
    });

    $hlSaveAndShow.live().click(function () {
        Sharpcms.Actions.ThrowEvent('save', 'openwindow', '');
    });

    $hlShow.live().live().click(function () {
        Sharpcms.Actions.ThrowEvent('', 'openwindow', '');
    });

    $hlRemoveContrainer.live().click(function () {
        var containerId = $(this).attr('containerId');

        Sharpcms.Actions.RemovePageContainer(containerId);
    });

    $hlRemoveElement.live().click(function () {
        var containerId = $(this).attr('containerId');
        var elementId = $(this).attr('elementId');

        Sharpcms.Actions.RemoveElement(containerId, elementId);
    });

    $hlMoveElementTop.live().click(function () {
        var containerId = $(this).attr('containerId');
        var elementId = $(this).attr('elementId');

        Sharpcms.Actions.MoveElementTop(containerId, elementId);
    });

    $hlMoveElementDown.live().click(function () {
        var containerId = $(this).attr('containerId');
        var elementId = $(this).attr('elementId');

        Sharpcms.Actions.MoveElementDown(containerId, elementId);
    });

    $hlMoveElementUp.live().click(function () {
        var containerId = $(this).attr('containerId');
        var elementId = $(this).attr('elementId');

        Sharpcms.Actions.MoveElementUp(containerId, elementId);
    });

    $hlCopyElement.live().click(function () {
        var containerId = $(this).attr('containerId');
        var elementId = $(this).attr('elementId');

        Sharpcms.Actions.CopyElement(containerId, elementId);
    });

    $hlAddPage.live().click(function () {
        var pageId = $(this).attr('pageId');

        Sharpcms.Actions.AddPage(pageId);
    });

    $hlChoosePage.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        Sharpcms.Actions.ChoosePage(id, attribute);
    });

    $hlChooseFile.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        Sharpcms.Actions.ChooseFile(id, attribute);
    });

    $hlChooseFolder.live().click(function () {
        var attributeValue = $(this).attr('value').split(Sharpcms.Common.Splitters); // $id / @attribute
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        Sharpcms.Actions.ChooseFolder(id, attribute);
    });

    $hlThrowEvent.live().click(function () {
        var action = $(this).attr('action');

        Sharpcms.Actions.ThrowEvent(action, '', '');
    });

    $hlUploadFile.live().click(function () {
        var path = $(this).attr('path');

        Sharpcms.Actions.UploadFile(path);
    });

    $hlRemoveFile.live().click(function () {
        var path = $(this).attr('path');

        Sharpcms.Actions.RemoveFile(path);
    });

    $hlRenameFile.live().click(function () {
        var path = $(this).attr('path');

        Sharpcms.Actions.RenameFile(path);
    });

    $hlMoveFile.live().click(function () {
        var path = $(this).attr('path');

        Sharpcms.Actions.MoveFile(path);
    });

    $hlResizeFile.live().click(function () {
        Sharpcms.Actions.ResizeImage();
    });

    $hlAddFolder.live().click(function () {
        var path = $(this).attr('path');

        Sharpcms.Actions.AddFolder(path);
    });

    $hlRemoveFolder.live().click(function () {
        var path = $(this).attr('path');

        Sharpcms.Actions.RemoveFolder(path);
    });

    $hlRenameFolder.live().click(function () {
        var path = $(this).attr('path');

        Sharpcms.Actions.RenameFolder(path);
    });

    $hlMoveFolder.live().click(function () {
        var path = $(this).attr('path');

        Sharpcms.Actions.MoveFolder(path);
    });

    $hlMoreFiles.live().click(function () {
        var currentlevel = $(this).attr('currentlevel');
        var level = parseInt(currentlevel) + 1;

        $('#file_' + level).show();
        $('#showlink_' + level).hide();
    });

    $hlAddUser.live().click(function () {
        Sharpcms.Actions.AddUser();
    });

    $hlSaveUser.live().click(function () {
        var userName = $(this).attr('userName');

        Sharpcms.Actions.SaveUser(userName);
    });

    $hlDeleteUser.live().click(function () {
        var userName = $(this).attr('userName');

        Sharpcms.Actions.DeleteUser(userName);
    });

    $hlAddGroup.live().click(function () {
        Sharpcms.Actions.AddGroup();
    });

    $hlDeleteGroup.live().click(function () {
        var groupName = $(this).attr('groupName');

        Sharpcms.Actions.DeleteGroup(groupName);
    });

    $('#adminmoreactions').change(function () {
        var attributeAction = $('#adminmoreactions :selected').attr('action');
        var attributeValue = $('#adminmoreactions :selected').attr('value').split(Sharpcms.Common.Splitters);

        if (attributeAction == Sharpcms.ActionType.RemovePage) {
            Sharpcms.Actions.RemovePage(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.AddPage) {
            Sharpcms.Actions.AddPage(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.CreatePageContainer) {
            Sharpcms.Actions.CreatePageContainer();
        } else if (attributeAction == Sharpcms.ActionType.MovePageUp) {
            Sharpcms.Actions.MovePageUp(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.MovePageDown) {
            Sharpcms.Actions.MovePageDown(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.MovePageTop) {
            Sharpcms.Actions.MovePageTop(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.MovePageBottom) {
            Sharpcms.Actions.MovePageBottom(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.MovePage) {
            Sharpcms.Actions.MovePage();
        } else if (attributeAction == Sharpcms.ActionType.CopyPage) {
            Sharpcms.Actions.CopyPage(attributeValue[0], attributeValue[1]);
            //Sharpcms.ModalDialog.Show('admin/choose/page/', 'Sharpcms.Actions.CopyPage("' + attributeValue[0] + '","' + attributeValue[1] + '")');
        } else if (attributeAction == Sharpcms.ActionType.SetAsStartPage) {
            Sharpcms.Actions.SetAsStartPage();
        }
    });

    $('.addelement').change(function () {
        var containerId = $(this).attr('name').replace('data_container_', '');

        Sharpcms.Actions.AddElement(containerId);
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

    // Dialogs
    $('.choose').hide();

    // Tabs
    $('.tabs').tabs();

    // Select
    $('.pagedata_menu select').selectmenu({ width: 441 });
    $('.container_menu select').selectmenu({ width: 624 });

    // TreeView
    $('.filetree').treeview({ persist: 'location', collapsed: true, unique: false });

    // Dirty Hack :-)
    $(window).resize(function () {
        setContentWidth();
    });

    $('.treeview li').live().click(function () {
        setContentWidth();
    });

    function setContentWidth() {
        var leftWidth = $('div.content div.left').width();
        var rightWidth = $('div.content div.right').width();

        $('div.header').css('min-width', parseInt(leftWidth) + parseInt(rightWidth) + 20);
        $('div.mainnavi').css('min-width', parseInt(leftWidth) + parseInt(rightWidth) + 20);
        $('div.content').css('min-width', parseInt(leftWidth) + parseInt(rightWidth) + 20);

        if ($(document).width() <= parseInt(leftWidth) + parseInt(rightWidth) + 20) {
            $('.messages').css('width', parseInt(leftWidth) + parseInt(rightWidth) + 20);
        } else {
            $('.messages').css('width', '100%');
        }
    }

    setContentWidth();
});