$(function() {
    $('textarea.mceeditor').tinymce({
        script_url : '/System/Components/Admin/Scripts/tiny_mce/tiny_mce.js',

        theme : 'advanced',
        plugins : 'sharpcmschooser,fullscreen,spellchecker,safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,imagemanager,filemanager',

        theme_advanced_buttons1 : 'fullscreen,formatselect,cleanup,forecolor,bold,italic,underline,strikethrough,justifyleft,justifycenter,justifyright,image,indent,outdent,bullist,numlist,undo,redo,link,unlink',
        theme_advanced_buttons2 : 'tablecontrols,|,sharpcmslinkchooser,sharpcmsfilechooser,sharpcmsimagechooser,|,code',
        theme_advanced_buttons3 : '',
        theme_advanced_buttons4 : '',
        theme_advanced_toolbar_location : 'top',
        theme_advanced_toolbar_align : 'left',
        theme_advanced_statusbar_location : 'bottom',
        theme_advanced_resizing : false,

        content_css: '/System/Components/Admin/Styles/tiny_mce/tinystyle.css'
    });

    // Tabs
    $('#tabs').tabs();
    $('#pada_body_tabs').tabs();
    
    // Select
    //$('select').selectmenu();
});