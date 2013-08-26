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
        function throwEvent(mainevent, mainvalue, redirect, querystring) {
            if (mainevent != undefined) {
                document.systemform.event_main.value = mainevent;
            } else {
                document.systemform.event_main.value = '';
            }

            if (mainvalue != undefined) {
                document.systemform.event_mainvalue.value = mainvalue;
            } else {
                document.systemform.event_mainvalue.value = '';
            }

            if (redirect != undefined) {
                document.systemform.event_redirect.value = redirect;
            }

            if (querystring != undefined) {
                document.systemform.action = document.systemform.action + querystring;
            }

            document.systemform.submit();
        }

        function throwEventConfirm(mainevent, mainvalue, confirmtext) {
            if (confirm(confirmtext)) {
                throwEvent(mainevent, mainvalue);
            }
        }

        function throwEventNew(mainevent, mainvalue, text, suggest) {
            if (suggest == null) {
                suggest = '';
            }

            var input = prompt(text, suggest);
            if (input != null) {
                if (mainvalue != '') {
                    throwEvent(mainevent, mainvalue + '*' + input);
                } else {
                    throwEvent(mainevent, input);
                }
            }
        }

        function setAsStartPage() {
            throwEvent(Sharpcms.ActionType.SetAsStartPage);
        }

        function movePage() {
            $('#dialog').load('/admin/choose/page', function () {
                $("#pages").treeview({
                    persist: 'location',
                    collapsed: true,
                    unique: false
                });

                $(this).dialog({
                    model: true,
                    title: 'Move Page' 
                });

                $('.hlCloseDialog').live().click(function () {
                    $('#dialog').dialog('close');

                    var destinaltionPath = $(this).data('path');
                    throwEvent(Sharpcms.ActionType.MovePage, destinaltionPath);
                });
            });
        }

        function moveFolder(sourcePath) {
            $('#dialog').load('/admin/choose/folder', function () {
                $("#pages").treeview({
                    persist: 'location',
                    collapsed: true,
                    unique: false
                });

                $(this).dialog({
                    model: true,
                    title: 'Move Folder: ' + sourcePath
                });
                
                $('.hlCloseDialog').live().click(function () {
                    $('#dialog').dialog('close');

                    var destinaltionPath = $(this).data('path');
                    throwEvent(Sharpcms.ActionType.MoveFolder, sourcePath + '*' + destinaltionPath);
                });
            });
        }

        function moveFile(sourcePath) {
            $('#dialog').load('/admin/choose/file', function () {
                $("#pages").treeview({
                    persist: 'location',
                    collapsed: true,
                    unique: false
                });

                $(this).dialog({
                    model: true,
                    title: 'Move File: ' + sourcePath
                });
                
                $('.hlCloseDialog').live().click(function () {
                    $('#dialog').dialog('close');

                    var destinaltionPath = $(this).data('path');
                    throwEvent(Sharpcms.ActionType.MoveFile, sourcePath + '*' + destinaltionPath);
                });
            });
        }

        function copyPage(pageId, pageName) {
            $('#dialog').load('/admin/choose/page', function () {
                $("#pages").treeview({
                    persist: 'location',
                    collapsed: true,
                    unique: false
                });

                $(this).dialog({
                    model: true,
                    title: 'Copy Page: ' + pageName
                });
                
                $('.hlCloseDialog').click(function () {
                    $('#dialog').dialog('close');

                    var destinaltionPath = $(this).data('path');
                    throwEvent(Sharpcms.ActionType.CopyPage, pageId + '¤' + destinaltionPath + '¤' + pageName);
                });
            });
        }

        function movePageUp(pageId) {
            throwEvent(Sharpcms.ActionType.MovePageUp, pageId);
        }

        function movePageDown(pageId) {
            throwEvent(Sharpcms.ActionType.MovePageDown, pageId);
        }

        function movePageTop(pageId) {
            throwEvent(Sharpcms.ActionType.MovePageTop, pageId);
        }

        function movePageBottom(pageId) {
            throwEvent(Sharpcms.ActionType.MovePageBottom, pageId);
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

        function removePage(pageId) {
            throwEventConfirm(Sharpcms.ActionType.RemovePage, pageId, 'Do you want to delete the page?');
        }

        function savePage() {
            throwEvent(Sharpcms.ActionType.SavePage);
        }

        function previewPage() {
            throwEvent(Sharpcms.ActionType.PreviewPage, '', '', '?preview=true');
        }

        function saveAndPreviewPage() {
            throwEvent(Sharpcms.ActionType.SaveAndPreviewPage, '', '', '?preview=true');
        }

        function addElement(containerId, elementType) {
            throwEvent(Sharpcms.ActionType.AddElement, elementType + '_' + containerId);
        }

        function removeElement(containerId, elementId) {
            throwEventConfirm(Sharpcms.ActionType.RemoveElement, 'element-' + containerId + '-' + elementId, 'Are you sure you want to delete this element?');
        }

        function moveElementUp(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.MoveElementUp, 'element-' + containerId + '-' + elementId);
        }

        function moveElementDown(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.MoveElementDown, 'element-' + containerId + '-' + elementId);
        }

        function moveElementTop(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.MoveElementTop, 'element-' + containerId + '-' + elementId);
        }

        function moveElementBottom(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.MoveElementBottom, 'element-' + containerId + '-' + elementId);
        }

        function copyElement(containerId, elementId) {
            throwEvent(Sharpcms.ActionType.CopyElement, 'element-' + containerId + '-' + elementId);
        }

        function addUser() {
            throwEventNew(Sharpcms.ActionType.AddUser, '', 'Type the name of the new user');
        }

        function saveUser(userName) {
            throwEvent(Sharpcms.ActionType.SaveUser, userName);
        }

        function deleteUser(userName) {
            throwEventConfirm(Sharpcms.ActionType.DeleteUser, userName, 'Are you sure you wnat to delete the user?');
        }

        function addGroup() {
            throwEventNew(Sharpcms.ActionType.AddGroup, '', 'Type the name of the new group');
        }

        function deleteGroup(groupName) {
            throwEventConfirm(Sharpcms.ActionType.DeleteGroup, groupName, 'Are you sure you wnat to delete the group?');
        }

        function addFolder(path) {
            throwEventNew(Sharpcms.ActionType.AddFolder, path, 'Type the name of the new folder');
        }

        function removeFolder(path) {
            throwEventConfirm(Sharpcms.ActionType.RemoveFolder, path, 'Do you want to delete the folder?');
        }

        function renameFolder(path) {
            throwEventNew(Sharpcms.ActionType.RenameFolder, path, 'Write the new name');
        }

        function resizeImage() {
            throwEvent(Sharpcms.ActionType.ResizeImage);
        }

        function uploadFile(path) {
            throwEvent(Sharpcms.ActionType.UploadFile, path, 'Type the name of the new folder:');
        }

        function removeFile(path) {
            throwEventConfirm(Sharpcms.ActionType.RemoveFile, path, 'Do you want to delete the file?');
        }

        function renameFile(path) {
            throwEventNew(Sharpcms.ActionType.RenameFile, path, 'Write the new name');
        }

        function choosePage(id, attribute) {
            $('#choosePageDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function () {
                $('.choose').dialog('close');

                var path = $(this).data('path');
                $('[name="' + id + '_' + attribute + '"]').val(path);
            });
        }

        function chooseFile(id, attribute) {
            $('#chooseFileDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function () {
                $('.choose').dialog('close');

                var path = $(this).data('path');
                $('[name="' + id + '_' + attribute + '"]').val(path);
            });
        }

        function chooseFolder(id, attribute) {
            $('#chooseFolderDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function () {
                $('.choose').dialog('close');

                var path = $(this).data('path');
                $('[name="' + id + '_' + attribute + '"]').val(path);
            });
        }

        function chooseImage(id, attribute) {
            $('#chooseImageDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function () {
                $('.choose').dialog('close');

                var path = $(this).data('path');
                $('[name="' + id + '_' + attribute + '"]').val(path);
            });
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

        return {

            // Pages
            AddPage: addPage,
            RemovePage: removePage,
            SavePage: savePage,
            PreviewPage: previewPage,
            SaveAndPreviewPage: saveAndPreviewPage,
            SetAsStartPage: setAsStartPage,
            MovePage: movePage,
            MovePageUp: movePageUp,
            MovePageDown: movePageDown,
            MovePageTop: movePageTop,
            MovePageBottom: movePageBottom,
            CopyPage: copyPage,
            ChoosePage: choosePage,


            // Container
            CreatePageContainer: createPageContainer,
            RemovePageContainer: removePageContainer,


            // Elements
            AddElement: addElement,
            RemoveElement: removeElement,
            MoveElementUp: moveElementUp,
            MoveElementDown: moveElementDown,
            MoveElementTop: moveElementTop,
            MoveElementBottom: moveElementBottom,
            CopyElement: copyElement,


            // User
            AddUser: addUser,
            SaveUser: saveUser,
            DeleteUser: deleteUser,


            // Groups
            AddGroup: addGroup,
            DeleteGroup: deleteGroup,


            // Files
            UploadFile: uploadFile,
            RemoveFile: removeFile,
            RenameFile: renameFile,
            MoveFile: moveFile,
            ChooseFile: chooseFile,


            // Folders
            AddFolder: addFolder,
            RenameFolder: renameFolder,
            RemoveFolder: removeFolder,
            MoveFolder: moveFolder,
            ChooseFolder: chooseFolder,


            // Images
            ResizeImage: resizeImage,
            ChooseImage: chooseImage,


            // Other
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

                // Pages
                AddPage: 'addpage',
                RemovePage: 'removepage',
                SavePage: 'save',
                PreviewPage: '',
                SaveAndPreviewPage: 'save',
                SetAsStartPage: 'setstandardpage',
                MovePage: 'pagemove',
                MovePageUp: 'pagemoveup',
                MovePageDown: 'pagemovedown',
                MovePageTop: 'pagemovetop',
                MovePageBottom: 'pagemovebottom',
                CopyPage: 'pagecopyto',
                ChoosePage: '',


                // Container
                CreatePageContainer: 'pagecreatcontainer',
                RemovePageContainer: 'pageremovecontainer',


                // Elements
                AddElement: 'addelement',
                RemoveElement: 'remove',
                MoveElementUp: 'moveup',
                MoveElementDown: 'movedown',
                MoveElementTop: 'movetop',
                MoveElementBottom: 'movebottom',
                CopyElement: 'copy',


                // User
                AddUser: 'adduser',
                SaveUser: 'saveuser',
                DeleteUser: 'deleteuser',


                // Groups
                AddGroup: 'addgroup',
                DeleteGroup: 'deletegroup',


                // Files
                UploadFile: 'uploadfile',
                RemoveFile: 'removefile',
                RenameFile: 'renamefile',
                MoveFile: 'movefile',
                ChooseFile: '',


                // Folders
                AddFolder: 'addfolder',
                RenameFolder: 'renamefolder',
                RemoveFolder: 'removefolder',
                MoveFolder: 'movefolder',
                ChooseFolder: '',

                // Images
                ResizeImage: 'doresize',
                ChooseImage: ''
            }
        }
    });
})(jQuery);