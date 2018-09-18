$(document).ready(function () {
    $(document).on('click', '.view-card', function () {
        PostCard.ViewSelectedCard($(this).data('id'));
    })
    $(document).on('click', '.delete-card', function () {
        PostCard.DeletePostCard($(this).data('id'));
    })

    $('.sorting').live("click", function () {
        return PostCard.SortPostCards($(this));
    });
    $('#Search').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return PostCard.SearchPostCards($(this));
        }
    });
    $("select#ShowOrderBy").on("change", function () {
        //$('#hfShowOrderBy').val($(this).val())
        return PostCard.ShowRecords($(this));
    });

    $("select#showRecords").on("change", function () {
        //$('#hfSortByOrder').val($(this).val());
        return PostCard.ShowRecords($(this));
    });
    $("select#SortBy").on("change", function () {
        //$('#hfSortByOrder').val($(this).val())
        return PostCard.ShowRecords($(this));
    });
    $('.search-func').hide();

    $(document).on("click", '.cancel-order', function () {
        PostCard.CancelOrder($(this).attr("data-order"));
    });

});
var PostCard = {
    SortPostCards: function (sender) {
        if ($(sender).hasClass("sorting_asc")) {
            $('.sorting').removeClass("sorting_asc");
            $('.sorting').removeClass("sorting_desc")
            $(sender).addClass("sorting_desc");
            $('#SortBy').val($(sender).attr('data-sortby'));
            $('#SortOrder').val('Desc');
            paging.startIndex = 1;
            paging.currentPage = 0;
            Paging();
        } else {
            $('.sorting').removeClass("sorting_asc");
            $('.sorting').removeClass("sorting_desc")
            $(sender).addClass("sorting_asc");
            $('#SortBy').val($(sender).attr('data-sortby'));
            $('#SortOrder').val('Asc');
            paging.startIndex = 1;
            paging.currentPage = 0;
            Paging();
        }
    },
    ManagePostCards: function (totalCount) {
        var totalRecords = 0;
        totalRecords = parseInt(totalCount);
        //alert(totalRecords);
        PageNumbering(totalRecords);
    },

    SearchPostCards: function (sender) {
        paging.startIndex = 1;
        Paging(sender);

    },

    ShowRecords: function (sender) {

        paging.startIndex = 1;
        paging.pageSize = 2;
        Paging(sender);

    },

    ViewSelectedCard: function (current) {
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: { postCardID: current },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.GetPostCardForPopup,
            success: function (results, message, status, id, list) {
                if (status == ActionStatus.Successfull) {
                    $('#view-card').html(results[0]);
                    $('#view-card').modal('show');
                }
            }
        });
    },
    DeletePostCard: function (current) {
        $.ConfirmBox("", "Are you sure want to delete postcard?", null, true, "Yes", true, null, function () {

            $.ajaxExt({
                type: "POST",
                validate: false,
                data: { postCardID: current },
                messageControl: null,
                showThrobber: false,
                url: baseUrl + siteURL.DeletePostCard,
                success: function (results, message, status, id, list) {
                    $.ShowMessage($('div.messageAlert'), message, status);
                    if (status == ActionStatus.Successfull) {
                        $('#view-card').modal('hide');
                        Paging();
                    }
                }
            });
        });
    },
    ShowRecords: function (sender) {

        paging.startIndex = 1;
        paging.pageSize = parseInt($(sender).find('option:selected').val());
        Paging(sender);

    },

    CancelOrder: function (orderId) {
       
        $.ConfirmBox("", "Are you sure want to cancel order?", null, true, "Yes", true, null, function () {
            $.ajaxExt({
                url: baseUrl + siteURL.OrderCancel,
                type: 'POST',
                validate: false,
                showErrorMessage: false,
                messageControl: $('div.messageAlert'),
                showThrobber: true, 
                data: { postCardOrder: orderId },
                success: function (results, message) {
                    //$.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                    $('.alert').show();
                    $(".statusMessage").html(results || message);
                    setTimeout(function () {
                        $(".alert").fadeOut();
                        $(".statusMessage").html('');
                        window.location.reload();
                    }, 2000);
                }, error: function (results, message) {
                    $('.alert').show();
                    $('.alert').addClass('alert-danger');
                    $(".statusMessage").html(results || message);
                    setTimeout(function () {
                        $(".alert").fadeOut();
                        $('.alert').removeClass('alert-danger');
                        $(".statusMessage").html('');
                    }, 2000);
                }
            });
        });
    }
}
function Paging(sender) {
    debugger
    var obj = new Object();
    obj.Search = $('#Search').val();
    obj.PageNo = paging.startIndex;
    obj.RecordsPerPage = paging.pageSize;
    obj.SortBy = $('#hfShowOrderBy').val();
    obj.SortOrder = $('#hfSortByOrder').val();
    obj.ShowOrderBy = $('#hfShowOrderBy').val();
    obj.SortByOrder = $('#hfSortByOrder').val();

    $.ajaxExt({
        type: "POST",
        validate: false,
        parentControl: $(sender).parents("form:first"),
        data: $.postifyData(obj),
        messageControl: null,
        showThrobber: false,
        throbberPosition: { my: "left center", at: "right center", of: sender, offset: "5 0" },
        url: baseUrl + siteURL.GetMyOrderList,
        success: function (results, message) {
            debugger
            $('#postcardList').html(results[0]);
            PageNumbering(results[1]);

        }
    });
}