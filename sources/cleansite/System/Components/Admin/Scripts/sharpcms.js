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
            $('#choosePageDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function() {
                $('.choose').dialog('close');
                
                var path = $(this).attr('path');
                throwEvent(Sharpcms.ActionType.MovePage, path, '');
            });
        }

        function moveFolder(fromPath) {
            $('#chooseFileDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function() {
                $('.choose').dialog('close');
                
                var toPath = $(this).attr('path');
                throwEvent(Sharpcms.ActionType.MoveFolder, fromPath + '*' + toPath, '');
            });
        }

        function moveFile(fromPath) {
            $('#chooseFileDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function() {
                $('.choose').dialog('close');
                
                var toPath = $(this).attr('path');
                throwEvent(Sharpcms.ActionType.MoveFile, fromPath + '*' + toPath, '');
            });
        }

        var copyPagePromptMessage = 'Page name:';
        function copyPage(pageId, pageName) {
            if (dialogBox.value != undefined && dialogBox.value != '') {
                throwEvent(Sharpcms.ActionType.CopyPage, pageId + '¤' + dialogBox.value + '¤' + prompt(copyPagePromptMessage, pageName, ''));
            }

            resetModalDialog();
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

        function addUser() {
            throwEventNew(Sharpcms.ActionType.AddUser, '', 'Type the name of the new user');
        }

        function saveUser(unserName) {
            throwEvent(Sharpcms.ActionType.SaveUser, unserName, '');
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
            throwEvent(Sharpcms.ActionType.ResizeImage, '', '');
        }

        function uploadFile(path) {
            throwEvent(Sharpcms.ActionType.UploadFile, path, 'Type the name of the new folder:', '');
        }

        function removeFile(path) {
            throwEventConfirm(Sharpcms.ActionType.RemoveFile, path, 'Do you want to delete the file?');
        }

        function renameFile(path) {
            throwEventNew(Sharpcms.ActionType.RenameFile, path, 'Write the new name');
        }
        
        function choosePage(id, attribute) {
            $('#choosePageDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function() {
                $('.choose').dialog('close');
                
                var path = $(this).attr('path');
                $('[name="' + id + '_' + attribute + '"]').val(path);
            });
        }
        
        function chooseFile(id, attribute) {
            $('#chooseFileDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function() {
                $('.choose').dialog('close');
                
                var path = $(this).attr('path');
                $('[name="' + id + '_' + attribute + '"]').val(path);
            });
        }
        
        function chooseFolder(id, attribute) {
            $('#chooseFolderDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function() {
                $('.choose').dialog('close');
                
                var path = $(this).attr('path');
                $('[name="' + id + '_' + attribute + '"]').val(path);
            });
        }
        
        function chooseImage(id, attribute) {
            $('#chooseImageDialog').dialog({ modal: true });
            $('.hlCloseDialog').live().click(function() {
                $('.choose').dialog('close');
                
                var path = $(this).attr('path');
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

        init();

        return {
            
            // Pages
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