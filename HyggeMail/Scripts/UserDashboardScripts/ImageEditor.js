var app;
$(document).ready(function () {
    ImageEditor.InitilizeEditor();
    // Do some initializing stuff
    fabric.Object.prototype.set({
        transparentCorners: false,
        cornerColor: 'rgba(102,153,255,0.5)',
        cornerSize: 12,
        padding: 5
    });
    fabric.Object.prototype.setOriginToCenter = function () {
        this._originalOriginX = this.originX;
        this._originalOriginY = this.originY;

        var center = this.getCenterPoint();

        this.set({
            originX: 'center',
            originY: 'center',
            left: center.x,
            top: center.y
        });
    };
    fabric.Object.prototype.setCenterToOrigin = function () {
        var originPoint = this.translateToOriginPoint(
        this.getCenterPoint(),
        this._originalOriginX,
        this._originalOriginY);

        this.set({
            originX: this._originalOriginX,
            originY: this._originalOriginY,
            left: originPoint.x,
            top: originPoint.y
        });
    };
});


$(document).on('dblclick', '.img-p', function () {
    //fabric.Image.fromURL($(this).attr('src'), function (img) {
    //    img.scale(0.5).set({
    //        left: 150,
    //        top: 150,
    //        angle: 0
    //    });


    //    img.crossOrigin = 'anonymous';
    //    img.crossOrigin = 'anonymous';
    //    img.crossOrigin = 'anonymous';
    //    canvas.add(img);
    //    canvas.setActiveObject(img);
    //    canvas.setActiveObject(img);
    //    canvas.setActiveObject(img);

    //}, { crossOrigin: 'Anonymous' });

    var imagepath = $(this).data('org-img');
    fabric.Image.fromURL(imagepath, function (img) {
        img.scale(0.5).set({
            left: 150,
            top: 150,
            angle: 0
        });
        img.crossOrigin = 'anonymous';
        canvas.add(img);
        canvas.setActiveObject(img);
    }, { crossOrigin: 'Anonymous' });
});

$(document).on('touchstart', '.img-p', function () {
    console.log('start')
});

$(document).on('touchend', '.img-p', function () {
    fabric.Image.fromURL($(this).attr('src'), function (img) {
        img.scale(0.5).set({
            left: 150,
            top: 150,
            angle: 0
        });
        canvas.add(img).setActiveObject(img);
    });
});

$(document).on('click', '#delete-selected', function () {
    ImageEditor.deleteMultipleObjects();
});

$(document).on('click', '.img-delete-a', function () {
    ImageEditor.DeleteImage($(this));
});

$(document).on('click', '#rotate-right', function () {

    var getObject = canvas.getActiveObject();
    var getGroup = canvas.getActiveGroup();
    if (getObject != null || getGroup != null) {
        if (getObject != null)
            ImageEditor.rotateObject(90, false);
        if (getGroup != null)
            ImageEditor.rotateObject(90, true);
    }
    else
        $.ShowMessage($('div.messageAlert'), "We didn't recognize which section of the design you would like to rotate. Try selecting an area first.", MessageType.Error);
});

$(document).on('click', '#rotate-left', function () {
    var getObject = canvas.getActiveObject();
    var getGroup = canvas.getActiveGroup();
    if (getObject != null || getGroup != null) {
        if (getObject != null)
            ImageEditor.rotateObject(-90, false);
        if (getGroup != null)
            ImageEditor.rotateObject(-90, true);
    }
    else
        $.ShowMessage($('div.messageAlert'), "We didn't recognize which section of the design you would like to rotate. Try selecting an area first.", MessageType.Error);
});

$(document).on('click', '#crop-object', function () {

    ImageEditor.CropImage();
});

$(document).on('change', '.font-change', function () {
    app.setTextParam($(this).data('type'), $(this).find('option:selected').val());
});

$(document).on('click', '#addText', function () {

    app.addText();
});

$(document).on('keyup', '#text-cont', function () {
    app.setTextValue($(this).val());
});

//document.getElementById('local-uploader').onchange = function handleImage(e) {
//    var reader = new FileReader();
//    reader.onload = function (event) {
//        var imgObj = new Image();
//        imgObj.src = event.target.result;
//        imgObj.onload = function () {
//            var image = new fabric.Image(imgObj);
//            image.set({
//                left: 250,
//                top: 250,
//                angle: 20,
//                padding: 10,
//                cornersize: 10
//            });
//            canvas.add(image).setActiveObject(image);
//        }
//    }
//    reader.readAsDataURL(e.target.files[0]);
//}

var ImageEditor = {

    DeleteImage: function (sender) {
        $.ConfirmBox("", "Are you sure want to delete image?", null, true, "Yes", true, null, function () {
            $.ajaxExt({
                url: baseUrl + siteURL.DeleteImage,
                type: 'POST',
                validate: false,
                showErrorMessage: false,
                messageControl: $('div.messageAlert'),
                showThrobber: false,
                data: { id: $(sender).data('id') },
                success: function (results, message) {
                    //$.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                    $('.alert').show();
                    $(".statusMessage").html(results || message);
                    setTimeout(function () {
                        $(".alert").fadeOut();
                        $(".statusMessage").html('');
                        UserDashboard.GetImages('');
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

    InitilizeEditor: function () {
    },
    DragEvent: function (sender) {
        alert(sender);
    },
    rotateObject: function (angleOffset, IsGroup) {
        var obj = canvas.getActiveObject(),
            resetOrigin = false;
        if (obj.id != "barCodeRect" && obj.id != "returnAddressRect") {
            if (IsGroup)
                obj = canvas.getActiveGroup(),
                resetOrigin = false;

            if (!obj) return;

            var angle = obj.getAngle() + angleOffset;

            if ((obj.originX !== 'center' || obj.originY !== 'center') && obj.centeredRotation) {
                obj.setOriginToCenter && obj.setOriginToCenter();
                resetOrigin = true;
            }

            angle = angle > 360 ? 90 : angle < 0 ? 270 : angle;

            obj.setAngle(angle).setCoords();

            if (resetOrigin) {
                obj.setCenterToOrigin && obj.setCenterToOrigin();
            }

            canvas.renderAll();
            updateModifications(true);
        }
        else {
            return false;
        }
    },

    // select all objects
    deleteMultipleObjects: function () {
        var activeObject = canvas.getActiveObject(),
        activeGroup = canvas.getActiveGroup();
        if (activeObject) {
            canvas.remove(activeObject);
        }
        else if (activeGroup) {
            var objectsInGroup = activeGroup.getObjects();
            canvas.discardActiveGroup();
            objectsInGroup.forEach(function (object) {
                canvas.remove(object);
            });
        }
        else {
            $.ShowMessage($('div.messageAlert'), "We didn't recognize which section of the design you would like to remove. Try selecting an area first.", MessageType.Error);

        }
        updateModifications(true);
    }

};
