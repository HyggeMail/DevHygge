$(document).ready(function () {

    $("input[type=button]#btnFilterVersion").live("click", function () {
        return Blog.ManageBlog($(this));
    });

    Blog.GetFeaturedArticleBlog(this);

});

var Blog = {

    GetFeaturedArticleBlog: function (sender)
    {
        $.ajaxExt({
            type: "POST",
            validate: false,
            parentControl: $(sender).parents("form:first"),
            messageControl: null,
            showThrobber: false,
            throbberPosition: { my: "left center", at: "right center", of: sender, offset: "5 0" },
            url: baseUrl + siteURL.GetFeaturedArticleBlog,
            success: function (results, message) {
                $('#featuredArticleDivResult').html(results[0]);
            }
        });
    },

    ManageBlog: function (totalCount) {
        var totalRecords = 0;
        totalRecords = parseInt(totalCount);
        PageNumbering(totalRecords);
    },

    ShowRecords: function (sender) {

        paging.startIndex = 1;
        paging.pageSize = parseInt($(sender).find('option:selected').val());
        Paging(sender);

    },

};

function Paging(sender) {
    var obj = new Object();
    obj.Search = $('#Search').val();
    obj.PageNo = paging.startIndex;
    obj.RecordsPerPage = paging.pageSize;
    obj.SortBy = $('#SortBy').val();
    obj.SortOrder = $('#SortOrder').val();
    $.ajaxExt({
        type: "POST",
        validate: false,
        parentControl: $(sender).parents("form:first"),
        data: $.postifyData(obj),
        messageControl: null,
        showThrobber: false,
        throbberPosition: { my: "left center", at: "right center", of: sender, offset: "5 0" },
        url: baseUrl + siteURL.GetBlogPagingList,
        success: function (results, message) {
            $('#divResult').html(results[0]);
            PageNumbering(results[1]);

        }
    });
}