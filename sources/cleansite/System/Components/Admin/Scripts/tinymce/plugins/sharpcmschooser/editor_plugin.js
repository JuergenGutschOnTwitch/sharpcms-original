/**
 * $RCSfile: editor_plugin_src.js,v $
 * $Revision: 1.23 $
 * $Date: 2006/02/10 16:29:38 $
 *
 * @author Moxiecode
 * @copyright Copyright © 2004-2006, Moxiecode Systems AB, All rights reserved.
 */
 
tinyMCE.importPluginLanguagePack('sharpcmschooser', 'en');
// Plucin static class
var TinyMCE_SharpcmschooserPlugin = {
	getInfo : function() {
		return {
			longname : 'Sharpcmschooser',
			author : 'Sharpcms.net',
			authorurl : 'http://Sharpcms.net',
			infourl : 'http://Sharpcms.net',
			version : tinyMCE.majorVersion + "." + tinyMCE.minorVersion
		};
	},

	/**
	 * Returns the HTML contents of the emotions control.
	 */
	getControlHTML : function(cn) {
	
		switch (cn) {
			
			case "sharpcmschooser":
				return tinyMCE.getButtonHTML(cn, 'lang_sharpcmschooser_insertlink', '{$pluginurl}/images/internal_link.gif', 'mceInsertLink') 
				+ tinyMCE.getButtonHTML(cn, 'lang_sharpcmschooser_insertfilelink', '{$pluginurl}/images/page_white_add.gif', 'mceInsertFileLink')
				+ tinyMCE.getButtonHTML(cn, 'lang_sharpcmschooser_insertpicture', '{$pluginurl}/images/image_add.gif', 'mceInsertPicture');
				
		}

		return "";
	},

	/**
	 * Executes the mceEmotion command.
	 */
	execCommand : function(editor_id, element, command, user_interface, value) {
		// Handle commands
		switch (command) {
			case "mceInsertLink":
                ModalDialogShow(basePath + '/default.aspx?process=admin/choose/page','ReturnMethodChoosePage()');
				
				return true;
			case "mceInsertFileLink":
                ModalDialogShow(basePath + '/default.aspx?process=admin/choose/file','ReturnMethodChooseFile()');
				
			    return true;
			 case "mceInsertPicture":
                ModalDialogShow(basePath + '/default.aspx?process=admin/choose/file','ReturnMethodChoosePicture()');
				
			    return true;
		}

		// Pass to next handler in chain
		return false;
	}
};

function ReturnMethodChoosePage()
{
    var html = "<a href=\"show/" + ModalDialog.value + ".aspx\">{$selection}</a>";
    tinyMCE.execCommand('mceReplaceContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}
function ReturnMethodChooseFile()
{
    var html = "<a href=\"download/" + ModalDialog.value + ".aspx?download=true\">{$selection}</a>";
    tinyMCE.execCommand('mceReplaceContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}
function ReturnMethodChoosePicture()
{
    var html = "<img src=\"" + basePath + "/download/" + ModalDialog.value + ".aspx?\"/>";
    tinyMCE.execCommand('mceInsertContent', false, html);
    tinyMCEPopup.close();
    ModalDialogRemoveWatch();
}

// Register plugin
tinyMCE.addPlugin('sharpcmschooser', TinyMCE_SharpcmschooserPlugin);
