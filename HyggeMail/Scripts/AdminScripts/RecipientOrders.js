
var cTop = 145;
var currentRecipient = {};
var img1 = "../../Content/images/frame.jpg";
var canvasBack;
var status = 1;
var action = "";
var dataID = 0;
var currentIsApprved;
var currentIsCompleted;
var showFront = true;
$(document).ready(function () {
    $("a.shownByStatus").live("click", function () {
        $('#hfMainStatus').val($(this).data('status'));
        if ($('#hfMainStatus').val() == "Approved")
            $('.daystablist').show();
        else
            $('.daystablist').hide();

        return PostCards.SearchPostCards($(this));
    });
    $("a.viewcard").live("click", function () {
        PostCards.ShowButtonWithChecks($(this).data('isapproved'), $(this).data('iscompleted'), $(this).data('isrejected'));
        currentRecipient.rname = $(this).data('rname');
        currentRecipient.raddress = $(this).data('raddress');
        currentRecipient.rcity = $(this).data('rcity');
        currentRecipient.rstate = $(this).data('rstate');
        currentRecipient.rcountry = $(this).data('rcountry');
        currentRecipient.rzip = $(this).data('rzip');
        dataID = $(this).data('recipientid');
        $("a.approveRecipientPostCard").data('recipientid', dataID)
        $("a.errorPostCard").data('recipientid', dataID)
        $("a.RejectPostCard").data('recipientid', dataID)
        $("a.saveRequest").data('recipientid', dataID)
        $("a.CompleteRecipientPostCard").data('recipientid', dataID)
        PostCards.ViewSelectedCard($(this).data('cardfrontpath'), $(this).data('cardbackpath'));
    })

    $("a.shownsBydays").live("click", function () {
        $('#hfDays').val($(this).data('shownby'));
        return PostCards.SearchPostCards($(this));
    });

    $("a.RejectPostCard").live("click", function () {
        $('#RecipientCardID').val($(this).data('postcardid'));
    });
    $("a#formReject").live("click", function () {
        $('#RecipientCardID').val(dataID);
        PostCards.RejectWithReason($(this));
    });
    $("a.approveRecipientPostCard").live("click", function () {
        dataID = $(this).data('recipient');
        PostCards.ApproveRecipientPostCard($(this));
    });
    $("a.errorPostCard").live("click", function () {
        if ($(this).data('recipient') != null)
            dataID = $(this).data('recipient');
        PostCards.SentToError($(this));
    });


    $("a.CompletePostCard").live("click", function () {
        PostCards.CompletePostCard($(this));
    });
    $("a.CompleteRecipientPostCard").live("click", function () {
        if ($(this).data('recipient') != null)
            dataID = $(this).data('recipient');
        PostCards.CompleteRecipientPostCard($(this));
    });

    $("a.btnBindRecipientDetails").live("click", function () {
        $('.modalAjaxLoader').show();
        canvasBack.loadFromJSON($('#hfSelectedPostCardBack').val(), canvasBack.renderAll.bind(canvasBack), function (o, object) { fabric.log(o, object); });

        canvasBack.renderAll();
        setTimeout(function () {
            PostCards.FillRecipientDetails();

        }, 2000);
        $("a.btnBindRecipientDetails").hide();

    });
    $("a.RejectPostCard").live("click", function () {
        if ($(this).data('recipient') != null)
            dataID = $(this).data('recipient');
        $("#view-card").modal("hide");
        //return PostCards.RejectPostCard($(this));
    });

    $(document).on('click', '.printAsPDF', function () {
        var EditorObject = new Object();
        EditorObject.cardFront = $('#cardFrontForEachRecipient').attr('src');
        EditorObject.cardBack = $('.cardBackForEachRecipient').attr('src');
        EditorObject.UserID = $('#hfSelectedUserID').val();
        PostCards.PrintPostCard(EditorObject);


        setTimeout(function () {
            $(".previewModel").modal("hide");
            $(".card-popup").modal("hide");
            Paging();
        }, 2000);

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
        var curStatus = $(this).val();
        var orderID = $(this).find(':selected').attr('data-id');
        PostCards.UpdateStatus(curStatus, orderID);
    });

    $(document).on('click', '.approveReciptent', function () {
        PostCards.ShowButtonWithChecks($(this).data('isapproved'), $(this).data('iscompleted'), $(this).data('isrejected'));
        $('#cardFrontForEachRecipient').attr('src', $(this).attr('data-cardfrontpath'));
        $('.cardBackForEachRecipient').attr('src', $(this).attr('data-cardbackpath'));
        $('#hfSelectedUserID').val($(this).data('userid'));
        $('#hfSelectedSenderAddress').val($(this).data('sender'));
        $('#hfSelectedSenderAddressNewLine').val($(this).data('newline'));
        $('#hfSelectedSenderName').val($(this).data('sendername'));
        $('#hfSelectedSenderInclude').val($(this).data('includeaddress'));
        $(".previewModel").modal("show");
        action = "approve";
        dataID = $(this).attr('data-id');
        PostCards.GetPostCardBackSideJsonResult(dataID);
        PostCards.BindCanvasToAdmin($(this));
        $("a.btnBindRecipientDetails").show();
    });
    $(document).on('click', '.disapproveReciptent', function () {
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
            PostCards.ApproveRecipientPostCard($(this), dataID);
        }
    });
    $(document).on('click', '.disapproveRequest', function () {
        $(".previewModel").modal("hide");
        PostCards.DispproveReciptent($(this), dataID);
    });
    $(document).on('click', '.flipCard', function () {
        if (showFront == true) {
            $('#cardFront').hide();
            $('#cardBack').show();
            showFront = false;
        }
        else {
            $('#cardFront').show();
            $('#cardBack').hide();
            showFront = true;
        }
    });
});
var PostCards = {
    GetPostCardBackSideJsonResult: function (sender) {
        $.ajaxExt({
            url: baseUrl + siteURL.GetPostCardBackSideJsonResult,
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            data: { ReceiptentID: sender },
            success: function (results, message) {
                $('#hfSelectedPostCardBack').val(results[0]);
            }
        });
    },
    ShowButtonWithChecks: function (isapproved, iscompleted, isRejected) {
        currentIsApprved = isapproved;
        currentIsCompleted = iscompleted;
        currentIsRejected = isRejected;

        if (currentIsRejected == 'False' || currentIsRejected == "") {
            $('.RejectPostCard').show();
        }
        else
        {
            $('.RejectPostCard').hide();
        }

        if (currentIsCompleted != 'False') {
            $('.saveRequest').show();
            $('.CompleteRecipientPostCard').hide();
            $('.RejectPostCard').hide();
        }
        if (currentIsApprved != 'False') {
            $('.saveRequest').hide();

            if (currentIsCompleted == 'False') {
                $('.CompleteRecipientPostCard').show();
            }
            //$('.disapproveRequest').show();
        }
        else {
            $('.disapproveRequest').hide();
            $('.saveRequest').show();
            $('.CompleteRecipientPostCard').hide();

        }

    },
    BindCanvasToAdmin: function (sender) {
        canvasBack = new fabric.Canvas('cBack', {
            preserveObjectStacking: true,
        });
        canvasBack.loadFromJSON($('#hfSelectedPostCardBack').val(), canvasBack.renderAll.bind(canvasBack), function (o, object) { fabric.log(o, object); });
        canvasBack.renderAll();
        currentRecipient.rname = $(sender).data('rname');
        currentRecipient.raddress = $(sender).data('raddress');
        currentRecipient.rcity = $(sender).data('rcity');
        currentRecipient.rstate = $(sender).data('rstate');
        currentRecipient.rcountry = $(sender).data('rcountry');
        currentRecipient.rzip = $(sender).data('rzip');
        $('.cardBackForEachRecipient').attr('src', canvasBack.toDataURL({ format: 'png', quality: 0.8 }));

    },

    ViewSelectedCard: function (front, back) {
        $('.address').html(currentRecipient.rname + "<br/><br/>" + currentRecipient.raddress + "<br/>" + currentRecipient.rcity + "<br/>"
            + currentRecipient.rstate + "<br/>" + currentRecipient.rcountry + "<br/>" + currentRecipient.rzip)
        $('#cardFront').attr('src', front);
        $('#cardBack').attr('src', back);
        $('#view-card').modal('show');
    },

    FillRecipientDetails: function () {
        var addressSplit = currentRecipient.raddress.split(" ");

        var firstSplitAddress = "";
        var secondSplitAddress = "";
        var temp = "";
        $.each(addressSplit, function (index, value) {

            temp += " " + value;
            if (temp.length <= 33) {
                firstSplitAddress += " " + value;
            }
            else {
                secondSplitAddress += " " + value;
            }
        });
        firstSplitAddress = firstSplitAddress.replace(/(\r\n\t|\n|\r\t)/gm, "");
        secondSplitAddress = secondSplitAddress.replace(/(\r\n\t|\n|\r\t)/gm, "");
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rname, cTop, 24, 450, true);
        PostCards.AddTextOnCanvasOnParticularLocation(firstSplitAddress, cTop, 24, 445, true);
        if (secondSplitAddress != "")
            PostCards.AddTextOnCanvasOnParticularLocation(secondSplitAddress, cTop, 24, 450, true);
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rcity, cTop, 24, 450, true);
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rstate + ", " + currentRecipient.rzip, cTop, 24, 450, true);
        PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rcountry, cTop, 24, 450, true);
        //   PostCards.AddTextOnCanvasOnParticularLocation(currentRecipient.rzip, cTop, 24, 440, true);
        if ($('#hfSelectedSenderInclude').val() == "True") {
            PostCards.AddTextOnCanvasOnParticularLocation($('#hfSelectedSenderName').val(), 5, 15, 5);
            PostCards.AddTextOnCanvasOnParticularLocation($('#hfSelectedSenderAddress').val(), 30, 11, 5);
            PostCards.AddTextOnCanvasOnParticularLocation($('#hfSelectedSenderAddressNewLine').val(), 50, 11, 5);
        }
        cTop = 145;
        $('.cardBackForEachRecipient').attr('crossorigin', "");
        $('.cardBackForEachRecipient').attr('src', canvasBack.toDataURL({ format: 'png', quality: 0.8 }));
        canvasBack.renderAll();
        $('#cBack').hide();

        $('.modalAjaxLoader').hide();
    },
    AddTextOnCanvasOnParticularLocation: function (text, top, fontSize, left, address) {
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
        if (address)
            cTop += 43;
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
            data: { PostCardId: dataID },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                Paging();
            }
        });
    },
    ApproveRecipientPostCard: function (sender) {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {

        //});
        $.ajaxExt({
            url: baseUrl + siteURL.ApproveRecipientPostCard,
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: { ReceiptentID: dataID },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                $(".previewModel").modal("hide");
                $(".card-popup").modal("hide");
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
                $(".previewModel").modal("hide");
                $(".card-popup").modal("hide");

                Paging();
            }
        });
    },
    CompleteRecipientPostCard: function (sender) {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {

        //});
        $.ajaxExt({
            url: baseUrl + siteURL.CompleteRecipientPostCard,
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: { ReceiptentID: dataID },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                $(".previewModel").modal("hide");
                $(".card-popup").modal("hide");
                Paging();
            }
        });
    },
    RejectWithReason: function (sender) {
        $.ajaxExt({
            url: baseUrl + siteURL.RejectWithReason,
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
                    Paging();
                    $(".card-popup").modal("hide");
                    $("#myModal-RejectOrder").modal("hide");
                }, 1500);
            }
        });
    },

    SentToError: function (sender) {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {

        //});
        $.ajaxExt({
            url: baseUrl + siteURL.SentToError,
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: { ReceiptentID: dataID },
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
    $('.partialPortian').html('<tr class="odd gradeX"><td colspan="6"><img class="cloading" src="../../Content/images/bx_loader.gif" /></td></tr>')
    var obj = new Object();
    obj.Search = $('#Search').val();
    obj.Status = status;
    obj.PageNo = paging.startIndex;
    obj.RecordsPerPage = paging.pageSize;
    obj.SortBy = $('#SortBy').val();
    obj.shownByStatus = $('#hfMainStatus').val();
    obj.shownsBydays = $('#hfDays').val();
    obj.SortOrder = $('#SortOrder').val();
    $.ajaxExt({
        type: "POST",
        validate: false,
        parentControl: $(sender).parents("form:first"),
        data: $.postifyData(obj),
        messageControl: null,
        showThrobber: false,
        throbberPosition: { my: "left center", at: "right center", of: sender, offset: "5 0" },
        url: baseUrl + '/Admin/PostCard/GetPostCardRecipientPagingList',
        success: function (results, message) {
            $('#divResult table:first tbody').html(results[0]);
            PageNumbering(results[1]);

        }
    });
}