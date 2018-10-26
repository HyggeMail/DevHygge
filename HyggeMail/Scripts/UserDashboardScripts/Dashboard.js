var StepGuidanceStatus = true;
var flipTop = true;
var flipLeft = true;
$(document).ready(function () {
    if (canvasBack) {
        canvasBack.on('mouse:over', function (e) {
            if (e.target != null) {
                if (e.target.id > 0) {
                    //alert(JSON.stringify(Recipient.ShowRecipientDetailsOnHover(e.target.id)));
                    //    console.log('Mouse over event fired', e.target.id);
                    Recipient.ShowRecipientDetailsOnHover(e.target.id);

                }
                else {
                    UserDashboard.SetBlockSectionToTop();
                }
            }
        });
    }

    UserDashboard.AddTransparentBackGround();
    $('.footer').hide();
    StepGuidanceStatus = $('#StepGuidanceStatus').val();
    if (!StepGuidanceStatus)
        $('.popup-steps').hide();
    else
        $('.popup-steps').show();

    // Disable Prev Button by default
    UserDashboard.BindPrevNextButon();
    $("input[type=button]#addImageBtn").live("click", function () {
        return UserDashboard.AddEditImage($(this));
    });
    $(document).on('change', '.checkbox-dontshow', function () {
        if ($(this).is(":checked"))
            return UserDashboard.DontShowAgainTooltip($(this));
    });
    $(document).on("click", '#save-postcard', function () {
        var id = $(this).attr('id');
        var EditorObject = UserDashboard.BindSubmitModel();
        return UserDashboard.SavePostCard(EditorObject);
    });

    $(document).on("click", '#thank-you-img-front', function () {
        $(this).addClass('flip-css');
        $('#thank-you-img-back').removeClass('flip-css');

    });
    $(document).on("click", '#thank-you-img-back', function () {
        $(this).addClass('flip-css');
        $('#thank-you-img-front').removeClass('flip-css');
    });
    $(document).on("click", 'a#flip-left', function () {
        if (canvas.getActiveObject() != null)
            canvas.getActiveObject().set('flipX', flipLeft);
        else
            $.ShowMessage($('div.messageAlert'), "We didn't recognize which section of the design you would like to flip. Try selecting an area first.", MessageType.Error);
        canvas.renderAll()
        updateModifications(true);
        flipLeft = flipLeft == true ? false : true;

    });

    $(document).on("click", 'a#flip-up', function () {
        if (canvas.getActiveObject() != null)
            canvas.getActiveObject().set('flipY', flipTop);
        else
            $.ShowMessage($('div.messageAlert'), "We didn't recognize which section of the design you would like to flip. Try selecting an area first.", MessageType.Error);
        canvas.renderAll()
        updateModifications(true);
        flipTop = flipTop == true ? false : true;
    });

    //$(document).on("click", 'a#flip-right', function () {
    //    if (canvas.getActiveObject() != null)
    //        canvas.getActiveObject().set('flipX', false);
    //    else
    //        $.ShowMessage($('div.messageAlert'), "Please select any object.", MessageType.Error);
    //    canvas.renderAll()
    //    updateModifications(true);

    //});

    //$(document).on("click", 'a#flip-down', function () {
    //    if (canvas.getActiveObject() != null)
    //        canvas.getActiveObject().set('flipY', false);
    //    else
    //        $.ShowMessage($('div.messageAlert'), "Please select any object.", MessageType.Error);
    //    canvas.renderAll()
    //    updateModifications(true);

    //});

    $(document).on('keyup', '#searchImage', function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return UserDashboard.GetUnSplashImages($(this).val(), 1);
            //return UserDashboard.GetImages($(this).val());
        }
    });
    $(document).on('click', '#pagingLeftKey', function (e) {

        var pageNumber = (parseInt($(this).attr("data-pageNumber")) - parseInt(1))
        return UserDashboard.GetUnSplashImages($('#searchImage').val(), pageNumber);

    });
    $(document).on('click', '#pagingRightKey', function (e) {

        var pageNumber = (parseInt($(this).attr("data-pageNumber")) + parseInt(1))
        return UserDashboard.GetUnSplashImages($('#searchImage').val(), pageNumber);

    });

    $("#Image").on("change", function (e) {

        var ext = $('#Image').val().split('.').pop().toLowerCase();
        if ($.inArray(ext, ['png', 'jpg', 'jpeg']) == -1) {
            //$.ShowMessage($('div.messageAlert'), "Invalid extension! File should be of .png, .jpg, .jpeg type", MessageType.Error);
            window.scrollTo(0, 0);
            $('.alert').show();
            $('.alert').addClass('alert-danger');
            $(".statusMessage").html('Invalid extension! File should be of .png, .jpg, .jpeg type');
            $('#Image').val('');
            setTimeout(function () {
                $(".alert").fadeOut();
                $('.alert').removeClass('alert-danger');
                $(".statusMessage").html('');
            }, 2000);
            return false;
        }
    })
    $(document).on('click', '.current a', function () {
        if ($(this).data('type') != "Paint") {
            UserDashboard.CancelDrawingMode()
        }
    })

    var curStep = 1;
    $(document).on('click', '.step-num', function () {
        // $('#myTextArea').val('');
        //if (getObject != null) {
        //    var objectText = getObject.text;
        //    if (objectText != '' && objectText != null && objectText != undefined) {
        //        $('#myTextArea').val(objectText);
        //        o = $Spelling.SpellCheckInWindow('myTextArea');
        //        o.onDialogComplete = function () {
        //            getObject.text = $('#myTextArea').val();
        //            canvas.setActiveObject(getObject);
        //        }
        //    }
        //}
        curStep = $(this).data('step');
        if (curStep == 3) {
            $("#step-2").addClass('step-3-canvas');
        }
        else {
            $("#step-2").removeClass('step-3-canvas');
        }

        $('ul.steps li').each(function () {
            $(this).removeClass('active');
        })
        $(this).parent().addClass('active');
        UserDashboard.ShowHideBlock(curStep);
        UserDashboard.BindPrevNextButon();

        changeDrawingMode(curStep);
    });

    function changeDrawingMode(curStep) {
        var drawingModeEl = document.getElementById('drawing-mode');
        drawingModeEl.innerHTML = 'Enter drawing mode';
        drawingOptionsEl.style.display = 'none';

        if (curStep == 3) {
            canvas.isDrawingMode = false;
            canvasBack.isDrawingMode = false;
        }

        if (curStep == 1) {
            canvas.isDrawingMode = false;
        }
        if (curStep == 2) {
            canvasBack.isDrawingMode = false;
        }
    }

    $(document).on('click', '.step-prev', function () {
        curStep = $('ul.steps li.active a').data('step');
        // if (curStep == 1 || curStep == 2) {
        //    $('#myTextArea').val('');
        //    var getObject = canvas.getActiveObject();
        //    if (getObject != null) {
        //        var objectText = getObject.text;
        //        if (objectText != '' && objectText != null && objectText != undefined) {
        //            $('#myTextArea').val(objectText);
        //            o = $Spelling.SpellCheckInWindow('myTextArea');
        //            o.onDialogComplete = function () {
        //                getObject.text = $('#myTextArea').val();
        //                canvas.setActiveObject(getObject);
        //            }
        //        }
        //    }
        // }

        if (curStep > 1) {
            var prevStep = parseInt(curStep) - 1;
            $(this).parent().hasClass('active');
            $('ul.steps li a').each(function () {
                if ($(this).data('step') == prevStep)
                    $(this).parent().addClass('active');
                else
                    $(this).parent().removeClass('active');
            });
            UserDashboard.ShowHideBlock(prevStep);
        }
        UserDashboard.BindPrevNextButon();
    });
    $(document).on('click', '.download-t-card', function () {
        var EditorObject = UserDashboard.BindSubmitModel();
        return UserDashboard.PrintPostCard(EditorObject);
        //UserDashboard.DownloadCanvas(canvasFront.toDataURL({ format: 'png', quality: 0.8 }), "Card-front.png");
        //UserDashboard.DownloadCanvas(canvasBack.toDataURL({ format: 'png', quality: 0.8 }), "Card-back.png");
    });

    $(document).on('click', '.step-next', function () {
        var curStep = $('ul.steps li.active a').data('step');
        //  if (curStep == 1 || curStep == 2) {
        //    $('#myTextArea').val('');
        //    var getObject = canvas.getActiveObject();
        //    if (getObject != null) {
        //        var objectText = getObject.text;
        //        if (objectText != '' && objectText != null && objectText != undefined) {
        //            $('#myTextArea').val(objectText);
        //            o = $Spelling.SpellCheckInWindow('myTextArea');
        //            o.onDialogComplete = function () {
        //                getObject.text = $('#myTextArea').val();
        //                canvas.setActiveObject(getObject);
        //            }
        //        }
        //    }
        //  }

        if (curStep < 4) {
            var nextStep = parseInt(curStep) + 1;
            $(this).parent().hasClass('active');
            $('ul.steps li a').each(function () {
                if ($(this).data('step') == nextStep)
                    $(this).parent().addClass('active');
                else
                    $(this).parent().removeClass('active');
            });
            UserDashboard.ShowHideBlock(nextStep);
        }
        UserDashboard.BindPrevNextButon();
    });
    var rightBackgroundImage = "../../Content/images/frame.png";


    $(document).on('click', '#place-order', function () {
        var id = $(this).attr('id');
        if (isNaN(Date.parse($('#shippingDate').val()))) {
            window.scrollTo(0, 0);
            $('.alert').show();
            $('.alert').addClass('alert-danger');
            $(".statusMessage").html('Please enter a proper shipping date by mm/dd/yy type');
            $('#Image').val('');
            setTimeout(function () {
                $(".alert").fadeOut();
                $('.alert').removeClass('alert-danger');
                $(".statusMessage").html('');
            }, 3000);
            return false;
        }

        var date = new Date();
        var currentDate = ((date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear());

        if (Date.parse($('#shippingDate').val()) < Date.parse(currentDate)) {
            window.scrollTo(0, 0);
            $('.alert').show();
            $('.alert').addClass('alert-danger');
            $(".statusMessage").html('You have entered wrong shipping date.');
            $('#Image').val('');
            setTimeout(function () {
                $(".alert").fadeOut();
                $('.alert').removeClass('alert-danger');
                $(".statusMessage").html('');
            }, 3000);
            return false;
        }

        if ($('#shippingDate').val() == "") {
            //$.ShowMessage($('div.messageAlert'), "Please select shipping date.", MessageType.Error);
            window.scrollTo(0, 0);
            $('.alert').show();
            $('.alert').addClass('alert-danger');
            $(".statusMessage").html('Please select shipping date.');
            $('#Image').val('');
            setTimeout(function () {
                $(".alert").fadeOut();
                $('.alert').removeClass('alert-danger');
                $(".statusMessage").html('');
            }, 3000);
            return false;
        }
        if (RecipientList.List.length == 0) {
            //$.ShowMessage($('div.messageAlert'), "Please add atleat one recipient.", MessageType.Error);
            window.scrollTo(0, 0);
            $('.alert').show();
            $('.alert').addClass('alert-danger');
            $(".statusMessage").html('Please, add at least one recipient');
            $('#Image').val('');
            setTimeout(function () {
                $(".alert").fadeOut();
                $('.alert').removeClass('alert-danger');
                $(".statusMessage").html('');
            }, 3000);
            return false;
        }
        // Recipient.removeRecipientsFromCanvas();
        var EditorObject = UserDashboard.BindSubmitModel();
        EditorObject.IsOrderPlaced = true;
        UserDashboard.PostCardSubmit(EditorObject);
    });
    $(document).on("click", '#cancel-order', function () {
        UserDashboard.CancelOrder();
    });
    $('html').keyup(function (e) {
        if (e.keyCode == 46) {
            window.deleteObject();

        }
    });
    function aa() {
        var Square = new fabric.Rect({
            width: 480, height: 480, left: 400, top: 1, angle: 0, fill: 'rgba(0,0,0,0)', stroke: 'black',
            strokeWidth: 3
        });
        canvasBack.add(Square);
        canvasBack.setActiveObject(Square);
        canvasBack.renderAll();
        updateModifications(true);
        canvasBack.counter++;
    };
    UserDashboard.BindAddedPostCard();
    UserDashboard.BlockLeftTopBottomSpace();
});


var UserDashboard = {
    BlockLeftTopBottomSpace: function () {

        fabric.Image.fromURL("../../Content/images/frame.jpg", function (img) {
            var image = img.set({
                left: 400, top: 0, height: 528, width: 440, selectable: false,
                lockMovementY: true,
                lockMovementX: true,
                hasBorders: false,
                hasControls: false,
                hoverCursor: 'pointer'
            });
            recImage = image;
            canvasBack.add(image).renderAll();
            img.crossOrigin = 'anonymous';
        }, { crossOrigin: 'Anonymous' });
        var leftTop = new fabric.Rect({
            width: 540, height: 80, left: 300, top: 455, angle: 0, fill: 'rgba(255,255,255,1)', strokeWidth: 1, strokeDashArray: [5, 5],
            stroke: 'rgba(240,240,240)', id: "returnAddressRect", selectable: true,
            lockMovementY: true,
            lockMovementX: true,
            hasBorders: false,
            hasControls: false,
            hoverCursor: 'pointer'
        });
        topLeftBlockedSection = leftTop;
        var rightTop = new fabric.Rect({
            width: 330, height: 70, left: -1, top: -1, angle: 0, fill: 'rgba(255,255,255,1)', strokeWidth: 1, strokeDashArray: [5, 5],
            stroke: 'rgba(240,240,240)', id: "barCodeRect", selectable: true,
            lockMovementY: true,
            lockMovementX: true,
            hasBorders: false,
            hasControls: false,
            hoverCursor: 'pointer'
        });
        bottomLeftBlockedSection = rightTop;
        canvasBack.add(leftTop);
        canvasBack.add(rightTop);
        UserDashboard.SetBlockSectionToTop();
        MainDashboard.SetRecipientToTop();
    },
    AddTransparentBackGround: function () {
        fabric.Image.fromURL("../../Content/images/background-postcard.png", function (img) {
            var bckImage = img.set({ left: 0, top: 0, height: 528, width: 440, selectable: false, });
            background2Image = bckImage;
            canvasBack.add(bckImage)
            canvasBack.renderAll();
            img.crossOrigin = 'anonymous';
        }, { crossOrigin: 'Anonymous' });
        canvasBack.sendToBack(background2Image);
        UserDashboard.SetBlockSectionToTop();

    },
    RemoveTransparentBackGround: function () {
        canvasBack.remove(background2Image);
    },
    SetBlockSectionToTop: function () {
        canvasBack.bringToFront(recImage);
        canvasBack.sendToBack(background2Image);
        canvasBack.bringToFront(topLeftBlockedSection);
        canvasBack.bringToFront(bottomLeftBlockedSection);

        MainDashboard.SetRecipientToTop();
        //canvasBack.forEachObject(function (o) {
        //    o.selectable = false;
        //});
    },
    BindSubmitModel: function () {
        Recipient.removeRecipientsFromCanvas();
        var EditorObject = new Object();
        EditorObject.Recipients = RecipientList.List;

        // if (id == "save-postcard") {

        //}
        //else {
        //UserDashboard.SetBlockSectionToTop();
        EditorObject.CardBackWithFrame = canvasBack.toDataURL({ format: 'png', quality: 0.8 });
        EditorObject.CardBackJsonWithIFrame = JSON.stringify(canvasBack.toDatalessJSON());
        //}
        canvasBack.remove(recImage);
        EditorObject.CardBack = canvasBack.toDataURL({ format: 'png', quality: 0.8 });
        EditorObject.CardBackJson = JSON.stringify(canvasBack.toDatalessJSON());

        EditorObject.CardFront = canvasFront.toDataURL({ format: 'png', quality: 0.8 });
        EditorObject.CardFrontJson = JSON.stringify(canvasFront.toDatalessJSON());
        EditorObject.ShipmentDate = $('#shippingDate').val();
        EditorObject.SelectedImages = selectedImages.List;
        EditorObject.IncludeAddress = $('#includeAddress').prop('checked');
        EditorObject.ID = $("#ID").val();
        EditorObject.IsCopyCard = $("#IsCopyCard").val();
        return EditorObject;

    },

    CancelDrawingMode: function () {
        canvas.isDrawingMode = false;
        drawingModeEl.innerHTML = 'Enter drawing mode';
        drawingOptionsEl.style.display = 'none';
    },
    BindAddedPostCard: function () {
        if ($('#copyID').val() != "" && $('#copyID').val() != "0") {
            canvasFront.loadFromJSON($('#CardFrontJson').val(), canvasFront.renderAll.bind(canvasFront), function (o, object) { fabric.log(o, object); });
            canvasBack.loadFromJSON($('#CardBackJson').val(), canvasBack.renderAll.bind(canvasBack), function (o, object) { fabric.log(o, object); });
            $('#includeAddress').attr("checked", $('#IncludeAddress').val())
            $('#shippingDate').val($('#ShippingDate').val())

        }
    },
    Timer: function startTimer(duration, display) {
        var timer = duration, minutes, seconds;
        setInterval(function () {
            minutes = parseInt(timer / 60, 10)
            seconds = parseInt(timer % 60, 10);

            minutes = minutes < 10 ? "0" + minutes : minutes;
            seconds = seconds < 10 ? "0" + seconds : seconds;

            display.textContent = minutes + ":" + seconds;

            if (--timer < 0) {
                timer = duration;
            }
            if (timer == duration || timer >= duration) {
                $('.cancel-block').hide();
                $('.timerHide').hide();
            }
        }, 1000);
    },
    DownloadCanvas: function (canvas, name) {
        var link = document.createElement('a');
        link.setAttribute('download', name);
        link.style.display = 'none';
        link.setAttribute('href', canvas);
        link.click();
    },
    AddEditImage: function (sender) {
        var form = $("#ImageForm");
        $.ajaxExt({
            url: baseUrl + siteURL.AddEditImage,
            type: 'POST',
            validate: true,
            formToValidate: form,
            formToPost: form,
            isAjaxForm: true,
            showThrobber: false,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            success: function (results, message) {
                $('.close').click()
                //$.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                $('.alert').show();
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                    $(".alert").fadeOut();
                    $(".statusMessage").html('');
                    UserDashboard.GetImages('');
                    $('#ImageForm').reset()
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
        return false;
    },
    ShowHideBlock: function (step) {

        $('.canvas-div').each(function () {
            if ($(this).data('step') == step)
                $(this).show();
            else
                $(this).hide();
        })
        $('.top-section').show();
        var recDiv = $('.recipient-div');
        var editor = $('.tab-mod');
        var topsection = $('.top-section');
        var step4left = $('.step-4-left');
        var recentlyUsed = $('#recentlyUsed');
        var backgroundOption = $('.myFile2');
        if (step == 1) {
            canvas = canvasFront;
            topsection.show();
            recDiv.hide();
            step4left.hide();
            editor.show();
            recentlyUsed.show();
            backgroundOption.show();
            UserDashboard.RemoveTransparentBackGround();
        }
        if (step == 2) {
            canvas = canvasBack;
            topsection.show();
            step4left.hide();
            editor.show();
            recentlyUsed.show();
            recDiv.hide();
            backgroundOption.hide();

            UserDashboard.SetBlockSectionToTop();
            canvas.bringToFront(recImage);
            UserDashboard.SetBlockSectionToTop();
        }
        if (step == 3) {
            $('#step-2').show();
            $('.top-section').hide();
            $("#addressdiv").show();
            canvas = canvasBack;
            recDiv.show();
            topsection.hide();
            editor.hide();
            step4left.hide();
            recentlyUsed.hide();
            backgroundOption.hide();
            UserDashboard.RemoveTransparentBackGround();
        }
        if (step == 4) {
            recDiv.hide();
            editor.hide();
            step4left.show();
            topsection.hide();
            recentlyUsed.hide();
            $('.img-front').attr('src', canvasFront.toDataURL({ format: 'png', quality: 0.8 }));
            $('.img-back').attr('src', canvasBack.toDataURL({ format: 'png', quality: 0.8 }));
            backgroundOption.hide();
            UserDashboard.RemoveTransparentBackGround();
        }
        MainDashboard.SetRecipientToTop();
        //else {
        //    editor.show();
        //    recDiv.hide();
        //}
    },
    BindPrevNextButon: function () {

        var step = $('ul.steps li.active a').data('step');
        var prev = $('.step-prev');
        var next = $('.step-next');
        prev.removeClass('blur-button')
        next.removeClass('blur-button')
        if (step == '1')
            prev.addClass('blur-button')
        if (step == '4')
            next.addClass('blur-button')

    },

    GetImages: function (sender) {

        $.ajaxExt({
            type: "POST",
            validate: false,
            data: { keyword: sender },
            messageControl: null,
            showErrorMessage: false,
            showThrobber: false,
            url: baseUrl + siteURL.ImagesByCategoryModel,
            success: function (results, message) {
                $('#images').html(results[0]);
                $(".emoji-div").html(results[1]);
                //  $('#recentlyUsed').html(results[0]);
                UserDashboard.AccordionInitilize();
                Binding();
                UserDashboard.BindScrollEvent();
            },
            error: function (results, message) {
                // alert(results || message);
                window.location.reload();

            }
        });
    },

    GetUnSplashImages: function (sender, pageNumber) {

        $.ajaxExt({
            type: "POST",
            validate: false,
            data: { keyword: sender, pageNumber: pageNumber },
            messageControl: null,
            showErrorMessage: false,
            showThrobber: false,
            url: baseUrl + siteURL.GetUnSplashImages,
            success: function (results, message) {
                if (results.length > 0) {
                    $('#unSplashimages').html(results[0]);
                    $(".accordion_scrolling").smk_Accordion({
                        closeAble: true,
                        activeIndex: 1,
                        closeOther: true,
                        slideSpeed: 200
                    });
                    Binding();
                    UserDashboard.BindScrollEvent();
                }
                else {
                    $.ShowMessage($('div.messageAlert'), "UnSplash Server Error.", MessageType.Error);
                }
            },
            error: function (results, message) {
                alert(results || message);
                //window.location.reload();
            }
        });
    },

    BindScrollEvent: function () {
        $('.scrollbar-inner').scrollbar();
        //$("#content-6").mCustomScrollbar({
        //    axis: "x",
        //    theme: "light-3",
        //    advanced: { autoExpandHorizontalScroll: true }
        //});
        //$(".faltu-inner").mCustomScrollbar({
        //    axis: "y",
        //    theme: "minimal-dark",
        //    advanced: { autoExpandHorizontalScroll: true }
        //});
        //$(".content11").mCustomScrollbar({
        //    axis: "y",
        //    theme: "minimal-dark",
        //    advanced: { autoExpandHorizontalScroll: true }
        //});
    },
    AccordionInitilize: function () {
        $(".accordion_example").smk_Accordion({
            closeAble: true, //boolean
        });
    },
    ShowThankYourSection: function () {
        var rightSection = $('#thank-you-right');
        var leftSection = $('#Thank-you-left');
        var rightfull = $('#canvas-container');
        var tabscontainer = $('#tabs-container');
        var recipient = $('.recipient-div');
        var step4left = $('.step-4-left');
        var rightheader = $('.right-header');
        rightSection.show();
        leftSection.show();
        rightfull.hide();
        tabscontainer.hide();
        recipient.hide();
        step4left.hide();
        rightheader.hide();
    },
    CancelOrder: function () {
        $.ConfirmBox("", "Are you sure want to cancel order?", null, true, "Yes", true, null, function () {
            $.ajaxExt({
                url: baseUrl + siteURL.OrderCancel,
                type: 'POST',
                validate: false,
                showErrorMessage: false,
                messageControl: $('div.messageAlert'),
                showThrobber: false,
                data: { postCardOrder: $('#orderID').val() },
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
    },
    DontShowAgainTooltip: function () {
        //$.ConfirmBox("", "Are you sure?", null, true, "Yes", true, null, function () {

        //});
        $.ajaxExt({
            url: baseUrl + siteURL.DontShowAgain,
            type: 'POST',
            validate: false,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            showThrobber: false,
            success: function (results, message) {
                // $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                $('.alert').show();
                $('.alert').addClass('alert-danger');
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                    $(".alert").fadeOut();
                    $(".statusMessage").html('');
                    $('#StepGuidanceStatus').val(false);
                    StepGuidanceStatus = false;
                    $('.popup-steps').hide();
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
    },
    PostCardSubmit: function (obj) {
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: $.postifyData(obj),
            messageControl: null,
            showErrorMessage: false,
            showThrobber: false,
            url: baseUrl + siteURL.PostCardSubmit,
            success: function (results, message, status, id, list, object) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                $('.alert').show();
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                    $(".alert").fadeOut();
                    $(".statusMessage").html('');
                }, 8000);
                if (status == ActionStatus.Successfull) {
                    $('#thank-you-img-front').attr('src', object.CardFront);
                    $('#thank-you-img-back').attr('src', object.CardBackWithFrame);
                    $("#orderID").val(object.ID);
                    UserDashboard.ShowThankYourSection();
                    var fiveMinutes = 60 * 15,
           display = document.querySelector('#time-remaining');
                    UserDashboard.Timer(fiveMinutes, display);
                }
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
    },
    convertStringToJson: function (canvas, objString) {
        canvas.loadFromJSON(objString, canvas.renderAll.bind(canvas), function (o, object) { fabric.log(o, object); });

    },
    SavePostCard: function (obj) {
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: $.postifyData(obj),
            messageControl: null,
            showThrobber: false,
            showErrorMessage: false,
            url: baseUrl + siteURL.PostCardSubmit,
            success: function (results, message, status, id, list, object) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                canvasBack.add(recImage);
                UserDashboard.SetBlockSectionToTop();
                $('.alert').show();
                $(".statusMessage").html(results || message);
                $('#ID').val(object.ID);
                setTimeout(function () {
                    $(".alert").fadeOut();
                    $(".statusMessage").html('');
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