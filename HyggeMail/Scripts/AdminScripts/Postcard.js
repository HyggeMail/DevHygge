
var canvasBack;
var status = 1;
$(document).ready(function () {
    $("input[type=button]#addPostCardBtn").live("click", function () {
        return PostCards.AddPostCard($(this));
    });
    $("input[type=button]#editPostCardBtn").live("click", function () {
        return PostCards.UpdatePostCard($(this));
    });

    $(document).on('click', '.showByStatus', function () {
        status = $(this).data('status');
        return PostCards.SearchPostCards($(this));
    });
    $("a.deletePostCard").live("click", function () {
        return PostCards.DeletePostCard($(this));
    });
    $("a.ApprovePostCard").live("click", function () {
        $("a.btnBindRecipientDetails").show();
        PostCards.ApprovePostCard($(this));
    });
    $("a.CompletePostCard").live("click", function () {
        PostCards.CompletePostCard($(this));
    });


    $("a.btnBindRecipientDetails").live("click", function () {
        PostCards.FillRecipientDetails();
        $("a.btnBindRecipientDetails").hide();
    });
    $("a.RejectPostCard").live("click", function () {
        return PostCards.RejectPostCard($(this));
    });

    $(document).on('click', '.printAsPDF', function () {
        var EditorObject = new Object();
        EditorObject.cardFront = $('#cardFrontForEachRecipient').attr('src');
        EditorObject.cardBack = $('#cardBackForEachRecipient').attr('src');
        EditorObject.UserID = $('#hfSelectedUserID').val();
        PostCards.PrintPostCard(EditorObject);

    });

    $("input[type=button]#btnFilterVersion").live("click", function () {
        return PostCards.ManagePostCards($(this));
    });
    $("select#showRecords").on("change", function () {
        return PostCards.ShowRecords($(this));
    });
    $("select#ShowOrderBy").on("change", function () {
        $('#hfShowOrderBy').SortBy(parseInt($(this).find('option:selected').val()));
    });
    $('.sorting').live("click", function () {
        return PostCards.SortPostCards($(this));
    });
    $('#Search').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return PostCards.SearchPostCards($(this));
        }
    });

    $(document).on('change', '.order-status', function () {
        debugger
        var curStatus = $(this).val();
        var orderID = $(this).find(':selected').attr('data-id');
        PostCards.UpdateStatus(curStatus, orderID);
    });

    var action = "";
    var dataID = 0;
    $(document).on('click', '.approveReciptent', function () {
        debugger
        $(".previewModel").modal("show");
        action = "approve";
        dataID = $(this).attr('data-id');
        $('#hfSelectedUserID').val($(this).data('userID'));
        PostCards.BindCanvasToAdmin($(this));
        $("a.btnBindRecipientDetails").show();
    });
    $(document).on('click', '.disapproveReciptent', function () {
        debugger
        $(".previewModel").modal("show");
        action = "disapprove";
        dataID = $(this).attr('data-id');
        PostCards.BindCanvasToAdmin($(this));
        $("a.btnBindRecipientDetails").show();
    });

    $(document).on('click', '.saveRequest', function () {
        $(".previewModel").modal("hide");
        if (action == "disapprove" && dataID > 0) {
            PostCards.DispproveReciptent($(this), dataID);
        } else {
            PostCards.ApproveReciptent($(this), dataID);
        }
    });
});
var cTop = 200;
var currentRecipient = {};
var PostCards = {
    BindCanvasToAdmin: function (sender) {
        canvasBack = new fabric.Canvas('cBack', {
            preserveObjectStacking: true,
        });
        canvasBack.loadFromJSON($('#hfCardBackJson').val(), canvasBack.renderAll.bind(canvasBack), function (o, object) { fabric.log(o, object); });
        canvasBack.renderAll();
        $('#cardBackForEachRecipient').attr('src', canvasBack.toDataURL({ format: 'png', quality: 0.8 }));

        currentRecipient.rname = $(sender).data('rname');
        currentRecipient.raddress = $(sender).data('raddress');
        currentRecipient.rcity = $(sender).data('rcity');
        currentRecipient.rstate = $(sender).data('rstate');
        currentRecipient.rcountry = $(sender).data('rcountry');
        currentRecipient.rzip = $(sender).data('rzip');

    },
    FillRecipientDetails: function () {

        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rname, cTop, 24, 500);
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.raddress, cTop, 24, 500);
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rcity, cTop, 24, 500);
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rstate, cTop, 24, 500);
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rcountry, cTop, 24, 500);
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rzip, cTop, 24, 500);
        if ($('#Object_IncludeAddress').val() == "True") {
            PostCards.AddTextOnCanvasOnParticularLocation($('#hfUserName').val(), 20, 15, 660);
            PostCards.AddTextOnCanvasOnParticularLocation($('#hfUserAddress').val(), 40, 11, 660);
        }
        cTop = 200;
        $('#cardBackForEachRecipient').attr('src', canvasBack.toDataURL({ format: 'png', quality: 0.8 }));
        $('#cBack').hide();
    },
    AddTextOnCanvasOnParticularLocation: function (text, top, fontSize, left) {
        var result = new fabric.IText(String(text), {
            left: left,
            top: top,
            fontFamily: 'helvetica neue',
            selectable: true,
            lockRotation: true,
            fill: '#000',
            stroke: '#fff',
            strokeWidth: .1,
            fontSize: fontSize
        });
        cTop += 40;
        canvasBack.add(result);
        canvasBack.bringToFront(result);
        canvasBack.renderAll();
    },
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
    AddPostCard: function (sender) {
        $.ajaxExt({
            url: baseUrl + '/Admin/PostCard/AddPostCardDetails',
            type: 'POST',
            validate: true,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            formToPost: $(sender).parents("form:first"),
            isAjaxForm: true,
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                setTimeout(function () {
                    window.location.href = baseUrl + '/Admin/PostCard/ManagePostCards';
                }, 1500);

            }
        });

    },
    UpdatePostCard: function (sender) {
        $.ajaxExt({
            url: baseUrl + '/Admin/PostCard/UpdatePostCardDetails',
            type: 'POST',
            validate: true,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            formToPost: $(sender).parents("form:first"),
            isAjaxForm: true,
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                setTimeout(function () {
                    window.location.href = baseUrl + '/Admin/PostCard/ManagePostCards';
                }, 1500);
            }
        });

    },
    UpdateStatus: function (status, orderID) {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {
            
        //});
        $.ajaxExt({
            url: baseUrl + siteURL.UpdateOrderStatus,
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            data: { orderID: orderID, status: status },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                Paging();
            }
        });
    },
    DeletePostCard: function (sender) {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {
           
        //});
        $.ajaxExt({
            url: baseUrl + '/Admin/PostCard/DeletePostCard',
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: { PostCardId: $(sender).attr("data-PostCardid") },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                Paging();
            }
        });
    },
    ApprovePostCard: function (sender) {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {
            
        //});
        $.ajaxExt({
            url: baseUrl + '/Admin/PostCard/ApprovePostCard',
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: { PostCardId: $(sender).attr("data-PostCardid") },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                Paging();
            }
        });
    },
    CompletePostCard: function (sender) {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {
            
        //});
        $.ajaxExt({
            url: baseUrl + '/Admin/PostCard/CompletePostCard',
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: { PostCardId: $(sender).attr("data-PostCardid") },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                Paging();
            }
        });
    },

    RejectPostCard: function (sender) {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {
           
        //});
        $.ajaxExt({
            url: baseUrl + '/Admin/PostCard/RejectPostCard',
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: { PostCardId: $(sender).attr("data-PostCardid") },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                Paging();
            }
        });
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
        paging.pageSize = parseInt($(sender).find('option:selected').val());
        Paging(sender);

    },

    ApproveReciptent: function (sender, value) {
        $.ConfirmBox("", "Are you sure to approve this recipient?", null, true, "Yes", true, null, function () {
            $.ajaxExt({
                url: baseUrl + '/Admin/PostCard/ApproveReciptent',
                type: 'POST',
                validate: false,
                showErrorMessage: true,
                messageControl: $('div.messageAlert'),
                showThrobber: true,
                button: $(sender),
                throbberPosition: { my: "left center", at: "right center", of: $(sender) },
                data: { ReceiptentID: value },
                success: function (results, message, status) {
                    $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                    if (status == ActionStatus.Successfull) {
                    }
                }
            });
        });
    },
    DispproveReciptent: function (sender, value) {
        $.ConfirmBox("", "Are you sure to disapprove this recipient?", null, true, "Yes", true, null, function () {
            $.ajaxExt({
                url: baseUrl + '/Admin/PostCard/DisapproveReciptent',
                type: 'POST',
                validate: false,
                showErrorMessage: true,
                messageControl: $('div.messageAlert'),
                showThrobber: true,
                button: $(sender),
                throbberPosition: { my: "left center", at: "right center", of: $(sender) },
                data: { ReceiptentID: value },
                success: function (results, message) {
                    $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                    window.location.reload();
                }
            });
        });
    },
    PrintPostCard: function (obj) {
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: $.postifyData(obj),
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.PrintPostCard,
            success: function (results, message, status, id, list, object) {
                window.open(results[0]);
            }
        });
    }

};

function Paging(sender) {
    var obj = new Object();
    obj.Search = $('#Search').val();
    obj.Status = status;
    obj.PageNo = paging.startIndex;
    obj.RecordsPerPage = paging.pageSize;
    obj.SortBy = $('#SortBy').val();
    obj.SortByOrder = $('#hfSortByOrder').val();
    obj.ShowOrderBy = $('#hfShowOrderBy').val();
    obj.SortOrder = $('#SortOrder').val();
    $.ajaxExt({
        type: "POST",
        validate: false,
        parentControl: $(sender).parents("form:first"),
        data: $.postifyData(obj),
        messageControl: null,
        showThrobber: false,
        throbberPosition: { my: "left center", at: "right center", of: sender, offset: "5 0" },
        url: baseUrl + '/Admin/PostCard/GetPostCardPagingList',
        success: function (results, message) {
            $('#divResult table:first tbody').html(results[0]);
            PageNumbering(results[1]);

        }
    });
}