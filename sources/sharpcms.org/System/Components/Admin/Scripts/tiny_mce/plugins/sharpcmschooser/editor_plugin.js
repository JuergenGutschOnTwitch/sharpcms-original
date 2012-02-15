/**
* $Id: sharpcmschooser_src.js 201 2007-02-12 15:56:56Z spocke $
*
* @author Thomas Huber
* @copyright Copyright © 2009, sharpcms.org, All rights reserved.
*/

(function() {
  tinymce.PluginManager.requireLangPack('sharpcmschooser');

  tinymce.create('tinymce.plugins.SharpcmsChooserPlugin', {
    init: function(ed, url) {
      ed.addCommand('mceInsertLink', function() {
        ModalDialogShow(basePath + '/admin/choose/page', 'ReturnMethodChoosePage()');
      });

      ed.addCommand('mceInsertFileLink', function() {
        ModalDialogShow(basePath + '/admin/choose/file', 'ReturnMethodChooseFile()');
      });

      ed.addCommand('mceInsertPicture', function() {
        ModalDialogShow(basePath + '/admin/choose/file', 'ReturnMethodChoosePicture()');
      });

      ed.addButton('sharpcmslinkchooser', { title: 'insert Link', cmd: 'mceInsertLink', image: url + '/img/internal_link.png' });
      ed.addButton('sharpcmsfilechooser', { title: 'insert File', cmd: 'mceInsertFileLink', image: url + '/img/page_white_add.png' });
      ed.addButton('sharpcmsimagechooser', { title: 'insert Picture', cmd: 'mceInsertPicture', image: url + '/img/image_add.png' });

      ed.onNodeChange.add(function(ed, cm, n) { cm.setActive('sharpcmslinkchooser', n.nodeName == 'IMG'); });
      ed.onNodeChange.add(function(ed, cm, n) { cm.setActive('sharpcmsfilechooser', n.nodeName == 'IMG'); });
      ed.onNodeChange.add(function(ed, cm, n) { cm.setActive('sharpcmsimagechooser', n.nodeName == 'IMG'); });
    },

    createControl: function(n, cm) {
      return null;
    },

    getInfo: function() {
        return {
            longname: 'sharpcms Chooser',
            author: 'Thomas Huber',
            authorurl: 'http://www.klickflupp.ch',
            infourl: 'http://www.sharpcms.org',
            version: '1.0'
        };
    }
  });

  // Register plugin
  tinymce.PluginManager.add('sharpcmschooser', tinymce.plugins.SharpcmsChooserPlugin);
})();

function ReturnMethodChoosePage() {
    var html = "<a href=\"show/" + ModalDialog.value + "/\">{$selection}</a>";
    tinyMCE.execCommand('mceReplaceContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}

function ReturnMethodChooseFile() {
    var html = "<a href=\"download/" + ModalDialog.value + "/?download=true\">{$selection}</a>";
    tinyMCE.execCommand('mceReplaceContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}

function ReturnMethodChoosePicture() {
    var html = "<img src=\"" + basePath + "/download/" + ModalDialog.value + "/?\" />";
    tinyMCE.execCommand('mceInsertContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}