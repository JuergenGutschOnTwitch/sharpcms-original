/**
* $Id: sharpcmschooser_src.js 201 2007-02-12 15:56:56Z spocke $
*
* @author Thomas Huber
* @copyright Copyright � 2013, sharpcms.org, All rights reserved.
*/

(function () {
    tinymce.PluginManager.requireLangPack('sharpcmschooser');

    tinymce.create('tinymce.plugins.SharpcmsChooserPlugin', {
        init: function (ed, url) {
            ed.addCommand('mceInsertLink', function () {
                showModalDialog('admin/choose/page', 'ReturnMethodTinyMceChoosePage()');
            });

            ed.addCommand('mceInsertFileLink', function () {
                showModalDialog('admin/choose/file', 'ReturnMethodTinyMceChooseFile()');
            });

            ed.addCommand('mceInsertPicture', function () {
                showModalDialog('admin/choose/file', 'ReturnMethodTinyMceChoosePicture()');
            });

            ed.addButton('sharpcmslinkchooser', {
                title: 'insert Link',
                cmd: 'mceInsertLink',
                image: url + '/img/internal_link.png'
            });
            
            ed.addButton('sharpcmsfilechooser', {
                title: 'insert File',
                cmd: 'mceInsertFileLink',
                image: url + '/img/page_white_add.png'
            });
            
            ed.addButton('sharpcmsimagechooser', {
                 title: 'insert Picture', cmd: 'mceInsertPicture', image: url + '/img/image_add.png'
            });
        },

        createControl: function (n, cm) {
            return null;
        },

        getInfo: function () {
            return {
                longname: 'sharpcms Chooser',
                author: 'Thomas Huber',
                authorurl: 'http://www.klickflupp.ch',
                infourl: 'http://www.sharpcms.org',
                version: '1.1'
            };
        }
    });

    // Register plugin
    tinymce.PluginManager.add('sharpcmschooser', tinymce.plugins.SharpcmsChooserPlugin);
})();