$(document).ready(function () {
    var $hlCloseForm = $('.hlCloseForm');

    $hlCloseForm.live().click(function() {
        var attributeName = $(this).attr('value'); // $current-path + @name || $current-path
        
        CloseForm(attributeName);
    });
});

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

  ModalDialogWindow = window.open(path, '', args);
  ModalDialogWindow.focus();
  ModalDialogInterval = window.setInterval('ModalDialogMaintainFocus()', 5);
}