$(function () {
    var $pages = $("#pages");

    // TreeView (Pages)
    $pages.treeview({
        persist: 'location',
        collapsed: true,
        unique: false
    });
});