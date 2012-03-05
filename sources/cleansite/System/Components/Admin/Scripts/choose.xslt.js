$(function () {
    var $pages = $("#pages");
    var $hlCloseForm = $('.hlCloseForm');

    // TreeView (Pages)
    $pages.treeview({
        persist: 'location',
        collapsed: true,
        unique: true
    });

    // Hyperlink (close Form)
    $hlCloseForm.live().click(function () {
        var path = $(this).attr('value'); // $current-path + @name || $current-path

        closeModalDialog(path);
    });
});