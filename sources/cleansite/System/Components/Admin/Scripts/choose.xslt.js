var baseUrl;

$(function () {
    // Common Variables
    baseUrl = $('head base').attr('href');
    
    // TreeView (Pages)
    $("#pages").treeview({
        persist: 'location',
        collapsed: true,
        unique: true
    });
});