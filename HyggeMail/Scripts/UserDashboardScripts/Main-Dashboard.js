var canvasFront = new fabric.Canvas('canvas-step-1', {
    preserveObjectStacking: true,
    backgroundColor: "#fff"
});
var canvasBack = new fabric.Canvas('canvas-step-2', {
    preserveObjectStacking: true,
    backgroundColor: "#fff"
});

var Isfontsizechanged = false;
var canvas = canvasFront;
var images;
var state = [];
var mods = 0;
var selectedImages = { List: [] };
var goodtop, goodleft, boundingObject;
var Direction = {
    LEFT: 0,
    UP: 1,
    RIGHT: 2,
    DOWN: 3
};
const STEP = 10;
var ar = new Array(33, 34, 35, 36, 37, 38, 39, 40, 46);
var recipientcanvasgroup = [];
var ctrlDown = false,
       ctrlKey = 17,
       cmdKey = 91,
       vKey = 86,
       cKey = 67;



$(document).keydown(function (e) {
    var key = e.which;
    //console.log(key);
    //if(key==35 || key == 36 || key == 37 || key == 39)
    if ($.inArray(key, ar) > -1) {
        e.preventDefault();
        return false;
    }
    return true;
});

$(document).on('click', 'button.btnflip-postcard', function () {
    $('.flip-postcard').toggleClass('show-card');
})

ManageReduUndoButton();


fabric.util.addListener(document.body, 'keydown', function (options) {
    if (options.repeat) {
        return;
    }
    if (canvas._activeObject != null) {
        if (canvas._activeObject.id > 0) {
            return;
        }
    }
    var key = options.which || options.keyCode; // key detection
    if (key === 37) { // handle Left key
        moveSelected(Direction.LEFT);
    } else if (key === 38) { // handle Up key
        moveSelected(Direction.UP);
    } else if (key === 39) { // handle Right key
        moveSelected(Direction.RIGHT);
    } else if (key === 40) { // handle Down key
        moveSelected(Direction.DOWN);
    }
});

function moveSelected(direction) {
    var activeObject = canvas.getActiveObject();
    var activeGroup = canvas.getActiveGroup();

    if (activeObject) {
        switch (direction) {
            case Direction.LEFT:
                activeObject.setLeft(activeObject.getLeft() - STEP);
                break;
            case Direction.UP:
                activeObject.setTop(activeObject.getTop() - STEP);
                break;
            case Direction.RIGHT:
                activeObject.setLeft(activeObject.getLeft() + STEP);
                break;
            case Direction.DOWN:
                activeObject.setTop(activeObject.getTop() + STEP);
                break;
        }
        activeObject.setCoords();
        canvas.renderAll();
        console.log('selected objects was moved');
    } else if (activeGroup) {
        switch (direction) {
            case Direction.LEFT:
                activeGroup.setLeft(activeGroup.getLeft() - STEP);
                break;
            case Direction.UP:
                activeGroup.setTop(activeGroup.getTop() - STEP);
                break;
            case Direction.RIGHT:
                activeGroup.setLeft(activeGroup.getLeft() + STEP);
                break;
            case Direction.DOWN:
                activeGroup.setTop(activeGroup.getTop() + STEP);
                break;
        }
        activeGroup.setCoords();
        canvas.renderAll();
        console.log('selected group was moved');
    } else {
        console.log('no object selected');
    }

}

function handleDragStart(e) {
    [].forEach.call(images, function (img) {
        img.classList.remove('img_dragging');
    });
    this.classList.add('img_dragging');
    UserDashboard.SetBlockSectionToTop();

}

function ReplicaContent() {

    var json = JSON.stringify(canvas.toDatalessJSON());
    console.log(json);
    var canvas1 = new fabric.Canvas('canvas-step-1-replica')
    canvas1.loadFromJSON(json, canvas1.renderAll.bind(canvas1), function (o, object) { fabric.log(o, object); });
}

function handleDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault(); // Necessary. Allows us to drop.
    }

    e.dataTransfer.dropEffect = 'copy'; // See the section on the DataTransfer object.
    // NOTE: comment above refers to the article (see top) -natchiketa
    UserDashboard.SetBlockSectionToTop();
    return false;
}

function handleDragEnter(e) {
    UserDashboard.SetBlockSectionToTop();
    // this / e.target is the current hover target.
    this.classList.add('over');
}

function handleDragLeave(e) {
    UserDashboard.SetBlockSectionToTop();
    this.classList.remove('over'); // this / e.target is previous target element.
}

function handleDrop(e) {
    // this / e.target is current target element.
    e.preventDefault();
    if (e.stopPropagation) {
        e.stopPropagation(); // stops the browser from redirecting.
    }
    var realImg = document.querySelector('.dimages img.img_dragging');
    //  console.log('event: ', e);
    var imagepath = $('.dimages img.img_dragging').data('org-img');
    var newImage = fabric.Image.fromURL(imagepath, function (img) {
        //var bckImage = img.set({ left: 100, top: 100, height: realImg.height, width: realImg.width, selectable: true });
        var bckImage = img.scale(0.5).set({
            left: 100,
            top: 100,
            angle: 0
        });
        canvas.add(bckImage);
        canvas.setActiveObject(bckImage);
        img.crossOrigin = 'anonymous';
    }, { crossOrigin: 'Anonymous' });

    //canvas.add(newImage);

    if (canvas._activeObject != null)
        canvas._activeObject.ontop = true;

    updateModifications(true);
    canvas.counter++;
    var curImage = {};
    //curImage.AdminImageID = $('.dimages img.img_dragging').attr('data-id');

    var id = $('.dimages img.img_dragging').attr('data-id');
    if ($.isNumeric(id)) {
        curImage.AdminImageID = id;
    }

    selectedImages.List.push(curImage);
    UserDashboard.SetBlockSectionToTop();
    return false;
}
function handleDragEnd(e) {
    // this/e.target is the source node.
    [].forEach.call(images, function (img) {
        img.classList.remove('img_dragging');
    });
    e.preventDefault();
    UserDashboard.SetBlockSectionToTop();
}


function Binding() {
    images = document.querySelectorAll('#images img');
    [].forEach.call(images, function (img) {
        img.addEventListener('dragstart', handleDragStart, false);
        img.addEventListener('dragend', handleDragEnd, false);
    });
    // Bind the event listeners for the canvas
    var canvasContainer = document.getElementById('canvas-container');
    canvasContainer.addEventListener('dragenter', handleDragEnter, false);
    canvasContainer.addEventListener('dragover', handleDragOver, false);
    canvasContainer.addEventListener('dragleave', handleDragLeave, false);
    canvasContainer.addEventListener('drop', handleDrop, false);

}
if (Modernizr.draganddrop) {
    // Browser supports HTML5 DnD.
    // Bind the event listeners for the image elements   
    Binding()
}
else {
    // Replace with a fallback to a library solution.
    alert("This browser doesn't support the HTML5 Drag and Drop API.");
}


//add background
document.getElementById('file2').addEventListener("change", function (e) {
    var file = e.target.files[0];
    var reader = new FileReader();
    reader.onload = function (f) {
        var data = f.target.result;
        fabric.Image.fromURL(data, function (img) {
            canvas.setBackgroundImage(img, canvas.renderAll.bind(canvas), {
                width: canvas.getWidth(),
                height: canvas.getHeight()
            });

            img.crossOrigin = 'anonymous';
        }, { crossOrigin: 'Anonymous' });
    };
    reader.readAsDataURL(file);
});


// Delete selected object
window.deleteObject = function () {
    var activeGroup = canvas.getActiveGroup();
    if (activeGroup) {
        var activeObjects = activeGroup.getObjects();
        if (activeObjects) {
            for (var i = 0; i < activeObjects.length; i++) {
                canvas.remove(activeObjects[i]);
                if (activeObjects[i].id > 0 && activeObjects[i] != null) {
                    Recipient.UncheckRecipientFromList(activeObjects[i].id)
                    cTop -= 40;
                    freeSpaceForRecipient.push(recipientcanvasgroup[i].top);
                }
                updateModifications(true);
                canvas.counter++;
            }
        }
        //for (let i in activeObjects) {
        //    canvas.remove(activeObjects[i]);
        //    if (activeObjects[i].id > 0 && activeObjects[i] != null) {
        //        Recipient.UncheckRecipientFromList(activeObjects[i].id)
        //        cTop -= 40;
        //        freeSpaceForRecipient.push(recipientcanvasgroup[i].top);
        //    }
        //    updateModifications(true);
        //    canvas.counter++;

        //}
        canvas.discardActiveGroup();
        canvas.renderAll();
    } else {
        if (canvas.getActiveObject().type == "i-text") {
            if (canvas.getActiveObject().id == "AddTextSection") {
                return false;
            }
        }

        if (canvas.getActiveObject().id > 0 && canvas.getActiveObject() != null) {
            Recipient.UncheckRecipientFromList(canvas.getActiveObject().id)
            cTop -= 40;
            freeSpaceForRecipient.push(canvas.getActiveObject().top);
            Recipient.HideRecipientDetailsOnHover(canvas.getActiveObject().id);
        }
        canvas.getActiveObject().remove();
        updateModifications(true);
        canvas.counter++;
    }

}
// Refresh page
function refresh() {
    setTimeout(function () {
        location.reload()
    }, 100);
}
// Add text
function Addtext() {
    var text = new fabric.IText("Start Typing", {
        left: 50,
        top: 100,
        width: 300,
        height: 200,
        fontFamily: fontStyle,

        fill: txtColor,
        fontSize: fontSize,

        fill: txtColor,
        stroke: strokeColor,
        strokeWidth: .1,
        fontSize: 60,
        id: "AddTextSection",
        fixedWidth: 300,
        textAlign: 'left',
    });
    text.setStrokeWidth(0.5);
    text.setStroke(txtColor);



    loadAndUse(fontStyle);
    canvas.add(text);
    //var text1 =  new fabric.IText('Tap and Type', {
    //    left: 50,
    //    top: 150,
    //    width: 350,
    //    height: 200,
    //    fontFamily: fontStyle,
    //    fill: txtColor,
    //    stroke: strokeColor,
    //    strokeWidth: .1,
    //    fontSize: fontSize
    //});
    //canvas.add(text1);

    canvas.renderAll();
    canvas.setActiveObject(text);
    text.enterEditing();
    text.selectAll();
    text.hiddenTextarea.focus();
    canvas.getActiveObject().ontop = true;
    UserDashboard.SetBlockSectionToTop();
    updateModifications(true);
    canvas.counter++;
}

canvas.on('text:changed', function (opt) {

    var t1 = opt.target;

    var getLastTextLine = t1._textLines.length;
    var firstTextLineCount = 0;

    if (t1.width > 700 && t1.text.length > 1 && !Isfontsizechanged) {

        var lastIndex = t1._textLines.length;

        if (getLastTextLine > 1) {

            firstTextLineCount = t1._textLines[0].length;

            if (t1._textLines[lastIndex - 1].length == firstTextLineCount) {
                t1.setText(t1.text + "\n");
                t1.setSelectionStart(t1.text.length);
                t1.setSelectionEnd(t1.text.length);
                t1.setSelectionEnd(t1.text.length);
            }
        }
        else {
            if (getLastTextLine == 1) {
                t1.setText(t1.text + "\n");
                t1.setSelectionStart(t1.text.length);
                t1.setSelectionEnd(t1.text.length);
                t1.setSelectionEnd(t1.text.length);
            }
        }

    }
    if (Isfontsizechanged) {
        if (t1.width > 700 && t1.text.length > 1) {

            var lastIndex = t1._textLines.length;

            if (getLastTextLine > 2) {

                firstTextLineCount = t1._textLines[0].length;

                if (t1._textLines[lastIndex - 1].length == t1._textLines[lastIndex - 2].length) {
                    t1.setText(t1.text + "\n");
                    t1.setSelectionStart(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                }
            }
            else {
                if (getLastTextLine == 1) {
                    t1.setText(t1.text + "\n");
                    t1.setSelectionStart(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                }
                if (getLastTextLine == 2 && t1._textLines[lastIndex - 1].length >= t1._textLines[lastIndex - 2].length) {
                    t1.setText(t1.text + "\n");
                    t1.setSelectionStart(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                }
            }

        }
    }
    //t1.fontSize *= t1.fixedWidth / (t1.width + 1);
    //t1.width = t1.fixedWidth;
});

canvasBack.on('text:changed', function (opt) {

    var t1 = opt.target;

    var getLastTextLine = t1._textLines.length;
    var firstTextLineCount = 0;

    if (t1.width > t1.fixedWidth && t1.text.length > 1 && !Isfontsizechanged) {

        var lastIndex = t1._textLines.length;

        if (getLastTextLine > 1) {

            firstTextLineCount = t1._textLines[0].length;

            if (t1._textLines[lastIndex - 1].length == firstTextLineCount) {
                t1.setText(t1.text + "\n");
                t1.setSelectionStart(t1.text.length);
                t1.setSelectionEnd(t1.text.length);
                t1.setSelectionEnd(t1.text.length);
            }
        }
        else {
            if (getLastTextLine == 1) {
                t1.setText(t1.text + "\n");
                t1.setSelectionStart(t1.text.length);
                t1.setSelectionEnd(t1.text.length);
                t1.setSelectionEnd(t1.text.length);
            }
        }

    }
    if (Isfontsizechanged) {
        if (t1.width > t1.fixedWidth && t1.text.length > 1) {

            var lastIndex = t1._textLines.length;

            if (getLastTextLine > 2) {

                firstTextLineCount = t1._textLines[0].length;

                if (t1._textLines[lastIndex - 1].length == t1._textLines[lastIndex - 2].length) {
                    t1.setText(t1.text + "\n");
                    t1.setSelectionStart(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                }
            }
            else {
                if (getLastTextLine == 1) {
                    t1.setText(t1.text + "\n");
                    t1.setSelectionStart(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                }
                if (getLastTextLine == 2 && t1._textLines[lastIndex - 1].length >= t1._textLines[lastIndex - 2].length) {
                    t1.setText(t1.text + "\n");
                    t1.setSelectionStart(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                    t1.setSelectionEnd(t1.text.length);
                }
            }

        }
    }
});

// Edit Text
//document.getElementById('text-color').onchange = function () {
//    canvas.getActiveObject().setFill(this.value);
//    canvas.renderAll();
//    updateModifications(true);
//    canvas.counter++;
//};
function ChangeTextColorFromColorPicker(value) {
    txtColor = "#" + value;
    if (canvas.getActiveObject() != null) {
        if (canvas.getActiveObject().id != "barCodeRect" && canvas.getActiveObject().id != "returnAddressRect") {
            canvas.getActiveObject().setFill(txtColor);
            canvas.renderAll();
            updateModifications(true);
            canvas.counter++;
        }
    }
}

//document.getElementById('drawing-shape-color').onchange = function () {
//    shapeColor = this.value;
//    canvas.getActiveObject().setFill(this.value);

//    canvas.renderAll();
//    updateModifications(true);
//    canvas.counter++;
//};
function ChangeColorFromColorPicker(value) {
    shapeColor = '#' + value
    if (canvas.getActiveObject() != null) {
        if (canvas.getActiveObject().id != "barCodeRect" && canvas.getActiveObject().id != "returnAddressRect") {
            canvas.getActiveObject().setFill(shapeColor);
            if (canvas.getActiveObject().stroke == null)
                canvas.getActiveObject();
            canvas.renderAll();
            updateModifications(true);
            canvas.counter++;
        }
    }
}
function ChangeLineColorFromColorPicker(value) {
    lineColor = '#' + value;
    canvas.freeDrawingBrush.color = lineColor;
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
}
function ChangeShadowColorFromColorPicker(value) {
    shadowColor = '#' + value;
    canvas.freeDrawingBrush.shadow.color = shadowColor;
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
}
//document.getElementById('text-bg-color').onchange = function () {
//    canvas.getActiveObject().setBackgroundColor(this.value);
//    canvas.renderAll();
//    updateModifications(true);
//    canvas.counter++;
//};

function ChangeBGColorFromColorPicker(value) {
    bgColor = "#" + value;
    canvas.getActiveObject().setBackgroundColor(bgColor);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
}

//document.getElementById('text-lines-bg-color').onchange = function () {
//    canvas.getActiveObject().setTextBackgroundColor(this.value);
//    canvas.renderAll();
//    updateModifications(true);
//    canvas.counter++;
//};
function ChangeTxtBGColorFromColorPicker(value) {
    bgTxtColor = "#" + value;
    canvas.getActiveObject().setTextBackgroundColor(bgTxtColor);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
}

$(document).on('click', '.removeBGColor', function () {
    if (canvas.getActiveObject() != null) {
        canvas.getActiveObject().setTextBackgroundColor('');
        canvas.renderAll();
        updateModifications(true);
        canvas.counter--;
    }
    else {
        $.ShowMessage($('div.messageAlert'), "We didn't recognize which section of the design you would like to remove BG Color. Try selecting an area first.", MessageType.Error);
    }
})

//document.getElementById('text-stroke-color').onchange = function () {
//    canvas.getActiveObject().setStroke(this.value);
//    canvas.renderAll();
//    updateModifications(true);
//    canvas.counter++;
//};

function ChangetxtStrokeColorFromColorPicker(value) {
    strokeColor = "#" + value;
    canvas.getActiveObject().setStroke(strokeColor);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
}

document.getElementById('text-stroke-width').onchange = function () {
    canvas.getActiveObject().setStrokeWidth(this.value);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};
document.getElementById('font-family').onchange = function () {
    loadAndUse(this.value);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};
function loadAndUse(font) {
    var myfont = new FontFaceObserver(font);
    myfont.load()
      .then(function () {
          // when font is loaded, use it.
          canvas.getActiveObject().set("fontFamily", font);
          canvas.renderAll();
      }).catch(function (e) {
          console.log(e)
          canvas.getActiveObject().set("fontFamily", "Ondise");
      });
}
document.getElementById('text-font-size').onchange = function () {
    canvas.getActiveObject().setFontSize(this.value);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
    Isfontsizechanged = true;
};
document.getElementById('text-line-height').onchange = function () {
    canvas.getActiveObject().setLineHeight(this.value);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};

$(document).on('click', '.text-align', function () {
    canvas.getActiveObject().setTextAlign(this.value);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
});

document.getElementById('text-font-weight').onchange = function () {
    if ($("option:selected", this).data('t') == "f") {
        canvas.getActiveObject().set("textDecoration", '');
        canvas.getActiveObject().set("fontWeight", this.value);
    }
    else {
        canvas.getActiveObject().set("textDecoration", this.value);
    }
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};


$(document).on('click', '.text-font-weight', function () {
    canvas.getActiveObject().set("textDecoration", this.value);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
})

// Send selected object to front or back
var selectedObject;
var recImage;
var background2Image;
var topLeftBlockedSection, bottomLeftBlockedSection;
canvas.on('object:selected', function (event) {
    selectedObject = event.target;
    if (event.target != null && event.target.id == null) {
        UserDashboard.SetBlockSectionToTop();
        canvasBack.bringToFront(topLeftBlockedSection);
        canvasBack.bringToFront(bottomLeftBlockedSection);
    }

    if (canvas._activeObject != null) {
        if (canvas._activeObject.fill != null) {
            document.getElementById('objColor').value = '';
            document.getElementById('objColor').jscolor.fromString(canvas.getActiveObject().fill)
            if (canvas.getActiveObject().stroke != null)
                document.getElementById('objColor').jscolor.fromString(canvas.getActiveObject().stroke)
        }
    }
    ManageFrontBackButtons();
    MainDashboard.SetRecipientToTop();
    updateModifications(true)
});

//canvasBack.on('object:selected', function (event) {
//    selectedObject = event.target;
//    if (event.target != null && event.target.id == null)
//        canvasBack.bringToFront(recImage);
//    if (canvas._activeObject != null) {
//        if (canvas._activeObject.fill != null) {
//            document.getElementById('objColor').value = '';
//            document.getElementById('objColor').jscolor.fromString(canvas.getActiveObject().fill)
//        }
//    }
//    ManageFrontBackButtons();
//    MainDashboard.SetRecipientToTop();
//    updateModifications(true)
//});



function ManageFrontBackButtons() {
    if (canvas.getActiveObject() != null) {
        var onTop = canvas.getActiveObject().ontop;
        if (onTop) {
            $('.obj-front').removeClass('cFront-icon-active');
            $('.obj-front').addClass('cFront-icon-inactive');
            $('.obj-back').addClass('cBack-icon-active');
            $('.obj-back').removeClass('cBack-icon-inactive');

        }
        else {
            $('.obj-front').addClass('cFront-icon-active');
            $('.obj-front').removeClass('cFront-icon-inactive');
            $('.obj-back').removeClass('cBack-icon-active');
            $('.obj-back').addClass('cBack-icon-inactive');
        }
    }
}

canvasFront.on("text:editing:entered", clearText);

canvasBack.on("text:editing:entered", clearText);

function clearText(e) {
    if (e.target.type === "textbox") {
        if (e.target.text === "Tap and Type") {
            e.target.text = "";
            canvas.renderAll();
        };
    }
    if (e.which == 8) {
        return false;
    }
}


canvas.on('object:moving', function (event) {
    //Set recipient to not movable
    if (event.target != null && event.target.id > 0) {
        event.target.lockMovementX = true;
        event.target.lockMovementY = true;
        canvasBack.bringToFront(event.target);
        MainDashboard.SetRecipientToTop();
    }
    else {
        UserDashboard.SetBlockSectionToTop();
        MainDashboard.SetRecipientToTop();
    }
    //   updateModifications(true)
});
canvasBack.on('object:moving', function (event) {
    UserDashboard.SetBlockSectionToTop();
    //Set recipient to not movable
    if (event.target != null && event.target.id > 0) {
        event.target.lockMovementX = true;
        event.target.lockMovementY = true;
        canvasBack.bringToFront(event.target);
        MainDashboard.SetRecipientToTop();
    }
    else {
        UserDashboard.SetBlockSectionToTop();
        MainDashboard.SetRecipientToTop();
    }
    //  updateModifications(true)
});
canvasBack.on('text:changed', function (e) {
    //this will only work using the dev build
    //   alert('text changed event in canvas back')
});
canvas.on('object:added', function () {
    updateModifications(true);
    updateCanvasState();
    canvasBack.renderAll();

    for (var i = 0; i < canvas._objects.length; i++) {
        canvas._objects[i].ontop = false;
    }
    //canvas.getActiveObject().ontop = true;
    UserDashboard.SetBlockSectionToTop();

});
canvas.on(
  'object:modified', function (event) {
      if (event.target != null && event.target.id > 0) {
          e.preventDefault();
          event.target.lockMovementX = true;
          event.target.lockMovementY = true;
          //    canvasBack.bringToFront(event.target);
      }
      else {
          //   canvasBack.bringToFront(recImage);
      }
      updateModifications(true);
      updateCanvasState();
  });

canvasBack.on(
  'mouse:over', function (event) {
      var thirdstep = $('.steps > li.active > a').attr('data-step');
      if (event.target.id == "barCodeRect" || event.target.id == "returnAddressRect") {
          event.target.ismoving = false;
          event.target.lockMovementX = true;
          event.target.lockMovementY = true;
          return;
      }
      else if (thirdstep == "3") {
          event.target.ismoving = false;
          event.target.lockMovementX = true;
          event.target.lockMovementY = true;
          return;
      }
      else {
          event.target.ismoving = true;
          event.target.lockMovementX = false;
          event.target.lockMovementY = false;
          return;
      }
  })



canvasBack.on(
  'object:modified', function (event) {

      if (event.target != null && event.target.id > 0) {
          event.target.lockMovementX = true;
          event.target.lockMovementY = true;
          //    canvasBack.bringToFront(event.target);
      }
      else {
          //   canvasBack.bringToFront(recImage);
      }

      updateModifications(true);
      updateCanvasState();
  }),

  canvasBack.on('object:added', function () {
      updateModifications(true);
      canvasBack.renderAll();

      for (var i = 0; i < canvas._objects.length; i++) {
          canvas._objects[i].ontop = false;
      }
      updateCanvasState();
      UserDashboard.SetBlockSectionToTop();
      MainDashboard.SetRecipientToTop();
  });

canvasBack.on('mouse:over', function (e) {
    if (e.target != null) {
        if (e.target.id > 0) {
            //alert(JSON.stringify(Recipient.ShowRecipientDetailsOnHover(e.target.id)));
            //    console.log('Mouse over event fired', e.target.id);
            Recipient.ShowRecipientDetailsOnHover(e.target.id);

        }
        else {

        }
    }
});
canvasBack.on('mouse:out', function (e) {
    if (e.target != null) {
        if (e.target.id > 0)
            Recipient.HideRecipientDetailsOnHover(e.target.id);
    }
});

canvasBack.on('mouse:down', function (e) {
    var thirdstep = $('.steps > li.active > a').attr('data-step');
    if (e.target != null) {
        if (e.target.id <= 0) {
            if (e.target.get('type') === "rect" && thirdstep == "3") {
                $('.add-recipient').trigger('click');
            }
        }

    }
});


var sendSelectedObjectBack = function () {
    if (canvas.getActiveObject() != null) {
        canvas.sendToBack(canvas.getActiveObject());
        for (var i = 0; i < canvas._objects.length; i++) {
            canvas._objects[i].ontop = true;
        }
        canvas.getActiveObject().ontop = false;
        canvas.renderAll();
        UserDashboard.SetBlockSectionToTop();
        updateModifications(true);
        canvas.counter++;
        ManageFrontBackButtons();

    }
    else
        $.ShowMessage($('div.messageAlert'), "We didn't recognize which section of the design you would like to send selected to back. Try selecting an area first.", MessageType.Error);
}
var sendSelectedObjectToFront = function () {
    if (canvas.getActiveObject() != null) {
        canvas.bringToFront(canvas.getActiveObject());

        for (var i = 0; i < canvas._objects.length; i++) {
            canvas._objects[i].ontop = false;
        }
        if (canvas._activeObject != null)
            canvas._activeObject.ontop = true;
        canvas.renderAll();
        UserDashboard.SetBlockSectionToTop();
        updateModifications(true);
        canvas.counter++;
        ManageFrontBackButtons();
    }
    else
        $.ShowMessage($('div.messageAlert'), "We didn't recognize which section of the design you would like to send selected to front. Try selecting an area first.", MessageType.Error);
}
// Download
//var imageSaver = document.getElementById('lnkDownload');
//imageSaver.addEventListener('click', saveImage, false);

function saveImage(e) {

    this.href = canvas.toDataURL({
        format: 'png',
        quality: 0.8
    });
    this.download = 'custom.png'
}
// Do some initializing stuff
fabric.Object.prototype.set({
    transparentCorners: true,
    cornerColor: '#22A7F0',
    borderColor: '#22A7F0',
    cornerSize: 12,
    padding: 5
});





//Paint Functionality

var circle, rectangle, triangle, Square, Line, shapeColor = "#005E7A", lineColor = "#005E7A", shadowColor = "#005E7A", txtColor = "#005E7A", strokeColor = "#005E7A", bgColor = "#fff", bgTxtColor = "#fff", fontStyle = "Ondise", fontSize = 24;

var drawingModeEl = document.getElementById('drawing-mode'),
    drawingOptionsEl = document.getElementById('drawing-mode-options'),
    drawingColorEl = document.getElementById('drawing-color'),
    drawingLineWidthEl = document.getElementById('drawing-line-width'),
    drawingCircle = document.getElementById('drawing-circle'),
    drawingRectangle = document.getElementById('drawing-Rectangle'),
    drawingSquare = document.getElementById('drawing-Square'),
    drawingTriangle = document.getElementById('drawing-Triangle'),
    drawingLine = document.getElementById('drawing-Line'),
    drawingArrow = document.getElementById('drawing-Arrow'),
    drawingHeart = document.getElementById('drawing-Heart'),
drawingShadowColorEl = document.getElementById('drawing-shadow-color'),
drawingShadowWidth = document.getElementById('drawing-shadow-width'),
drawingShadowOffset = document.getElementById('drawing-shadow-offset');


drawingModeEl.onclick = function () {



    canvas.isDrawingMode = !canvas.isDrawingMode;
    if (canvas.isDrawingMode) {
        drawingModeEl.innerHTML = 'Exit drawing mode';
        drawingOptionsEl.style.display = '';
        //canvas.freeDrawingCursor = "url('/Content/images/paint-brushDown.png'),auto";
        canvas.freeDrawingCursor = "url('/Content/images/paint-brush.png'),auto";
        freeDrawingCursor: 'none'
        canvas.freeDrawingBrush.width = 30;
        //canvas.freeDrawingBrush.onMouseUp();

    }
    else {
        drawingModeEl.innerHTML = 'Enter drawing mode';
        drawingOptionsEl.style.display = 'none';
    }
    updateModifications(true);
    canvas.counter++;


    if (canvas.isDrawingMode) {
        canvasBack.isDrawingMode = false;
    }

    canvasBack.isDrawingMode = !canvasBack.isDrawingMode;
    if (canvasBack.isDrawingMode) {
        drawingModeEl.innerHTML = 'Exit drawing mode';
        drawingOptionsEl.style.display = '';
        // canvasBack.freeDrawingCursor = "url('/Content/images/paint-brushDown.png'),auto";
        canvas.freeDrawingCursor = "url('/Content/images/paint-brush.png'),auto";
        freeDrawingCursor: 'none'
        canvasBack.freeDrawingBrush.width = 30;
        //canvasBack.freeDrawingBrush.onMouseUp();

    }
    else {
        drawingModeEl.innerHTML = 'Enter drawing mode';
        drawingOptionsEl.style.display = 'none';
    }
    updateModifications(true);
    canvasBack.counter++;
};



canvas.on("object:added", function (e) {
    if (canvas.isDrawingMode) {
        canvas.freeDrawingCursor = "url('/Content/images/paint-brush.png'),auto";
        canvas.renderAll();
    }
}, false);

canvas.on("mouse:down", function (e) {
    if (canvas.isDrawingMode) {
        canvas.freeDrawingCursor = "url('/Content/images/paint-brush.png'),auto";
        canvas.renderAll();
    }
}, false);


canvas.on("mouse:up", function (e) {
    if (canvas.isDrawingMode) {
        //canvas.freeDrawingCursor = "url('/Content/images/paint-brushDown.png'),auto";
        canvas.freeDrawingCursor = "url('/Content/images/paint-brush.png'),auto";
        canvas.renderAll();
    }
}, false);


canvasBack.on("object:added", function (e) {
    if (canvasBack.isDrawingMode) {
        canvasBack.freeDrawingCursor = "url('/Content/images/paint-brush.png'),auto";
        canvasBack.renderAll();
    }
}, false);

canvasBack.on("mouse:down", function (e) {
    if (canvasBack.isDrawingMode) {
        canvasBack.freeDrawingCursor = "url('/Content/images/paint-brush.png'),auto";
        canvasBack.renderAll();
    }
}, false);


canvasBack.on("mouse:up", function (e) {

    if (canvasBack.isDrawingMode) {
        canvas.freeDrawingCursor = "url('/Content/images/paint-brush.png'),auto";
        //canvasBack.freeDrawingCursor = "url('/Content/images/paint-brushDown.png'),auto";
        canvasBack.renderAll();
    }
}, false);

if (fabric.PatternBrush) {
    var vLinePatternBrush = new fabric.PatternBrush(canvas);
    vLinePatternBrush.getPatternSrc = function () {

        var patternCanvas = fabric.document.createElement('canvas');
        patternCanvas.width = patternCanvas.height = 10;
        var ctx = patternCanvas.getContext('2d');

        ctx.strokeStyle = this.color;
        ctx.lineWidth = 5;
        ctx.beginPath();
        ctx.moveTo(0, 5);
        ctx.lineTo(10, 5);
        ctx.closePath();
        ctx.stroke();

        return patternCanvas;
    };

    var hLinePatternBrush = new fabric.PatternBrush(canvas);
    hLinePatternBrush.getPatternSrc = function () {

        var patternCanvas = fabric.document.createElement('canvas');
        patternCanvas.width = patternCanvas.height = 10;
        var ctx = patternCanvas.getContext('2d');

        ctx.strokeStyle = this.color;
        ctx.lineWidth = 5;
        ctx.beginPath();
        ctx.moveTo(5, 0);
        ctx.lineTo(5, 10);
        ctx.closePath();
        ctx.stroke();

        return patternCanvas;
    };

    var squarePatternBrush = new fabric.PatternBrush(canvas);
    squarePatternBrush.getPatternSrc = function () {

        var squareWidth = 10, squareDistance = 2;

        var patternCanvas = fabric.document.createElement('canvas');
        patternCanvas.width = patternCanvas.height = squareWidth + squareDistance;
        var ctx = patternCanvas.getContext('2d');

        ctx.fillStyle = this.color;
        ctx.fillRect(0, 0, squareWidth, squareWidth);

        return patternCanvas;
    };

    var diamondPatternBrush = new fabric.PatternBrush(canvas);
    diamondPatternBrush.getPatternSrc = function () {

        var squareWidth = 10, squareDistance = 5;
        var patternCanvas = fabric.document.createElement('canvas');
        var rect = new fabric.Rect({
            width: squareWidth,
            height: squareWidth,
            angle: 45,
            fill: this.color
        });

        var canvasWidth = rect.getBoundingRectWidth();

        patternCanvas.width = patternCanvas.height = canvasWidth + squareDistance;
        rect.set({ left: canvasWidth / 2, top: canvasWidth / 2 });

        var ctx = patternCanvas.getContext('2d');
        rect.render(ctx);

        return patternCanvas;
    };

    var img = new Image();
    img.src = '../../content/images/honey_im_subtle.png';

    var texturePatternBrush = new fabric.PatternBrush(canvas);
    texturePatternBrush.source = img;
}

document.getElementById('drawing-mode-selector').addEventListener('change', function () {

    if (this.value === 'hline') {
        canvas.freeDrawingBrush = vLinePatternBrush;
    }
    else if (this.value === 'vline') {
        canvas.freeDrawingBrush = hLinePatternBrush;
    }
    else if (this.value === 'square') {
        canvas.freeDrawingBrush = squarePatternBrush;
    }
    else if (this.value === 'diamond') {
        canvas.freeDrawingBrush = diamondPatternBrush;
    }
    else if (this.value === 'texture') {
        canvas.freeDrawingBrush = texturePatternBrush;
    }
    else {
        canvas.freeDrawingBrush = new fabric[this.value + 'Brush'](canvas);
    }

    if (canvas.freeDrawingBrush) {
        canvas.freeDrawingBrush.color = lineColor;
        canvas.freeDrawingBrush.width = parseInt(drawingLineWidthEl.value, 10) || 10;
        canvas.freeDrawingBrush.shadowBlur = parseInt(drawingShadowWidth.value, 10) || 0;
        canvas.freeDrawingBrush.shadow = new fabric.Shadow({
            blur: parseInt(drawingShadowWidth.value, 10) || 0,
            offsetX: 0,
            offsetY: 0,
            affectStroke: true,
            color: shadowColor,
        });
    }
    updateModifications(true);
    canvas.counter++;
});

//drawingColorEl.onchange = function () {
//    canvas.freeDrawingBrush.color = drawingColorEl.value;
//    updateModifications(true);
//    canvas.counter++;
//};
drawingLineWidthEl.onchange = function () {
    canvas.freeDrawingBrush.width = parseInt(drawingLineWidthEl.value, 10) || 10;
    updateModifications(true);
    canvas.counter++;
};
drawingShadowWidth.onchange = function () {

    canvas.freeDrawingBrush.width = parseInt(drawingLineWidthEl.value, 10) || 10;
    canvas.freeDrawingBrush.shadowBlur = parseInt(drawingShadowWidth.value, 10) || 0;
    updateModifications(true);
    canvas.counter++;
};
//drawingShadowColorEl.onchange = function () {
//    canvas.freeDrawingBrush.shadow.color = this.value;
//};


drawingCircle.onclick = function () {
    circle = new fabric.Circle({
        radius: 50, left: 275, top: 75, fill: shapeColor
    });
    canvas.add(circle);
    canvas.setActiveObject(circle);

    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};
drawingRectangle.onclick = function () {
    rectangle = new fabric.Rect({
        width: 200, height: 100, left: 0, top: 50, angle: 0,
        fill: shapeColor
    });
    canvas.add(rectangle);
    canvas.setActiveObject(rectangle);

    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};
drawingTriangle.onclick = function () {
    triangle = new fabric.Triangle({
        width: 100, height: 100, left: 50, top: 300, fill: shapeColor
    });
    canvas.add(triangle);
    canvas.setActiveObject(triangle);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};

drawingSquare.onclick = function () {
    Square = new fabric.Rect({
        width: 100, height: 100, left: 150, top: 250, angle: 0, fill: shapeColor
    });
    canvas.add(Square); canvas.setActiveObject(Square);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};

drawingLine.onclick = function () {
    Line = new fabric.Line([100, 400, 400, 400], {
        left: 250, top: 150, angle: 0, stroke: shapeColor, strokeWidth: 5,
    });
    canvas.add(Line);
    canvas.setActiveObject(Line);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};



drawingHeart.onclick = function () {
    var path = new fabric.Path('M 272.70141,238.71731 \
    C 206.46141,238.71731 152.70146,292.4773 152.70146,358.71731  \
    C 152.70146,493.47282 288.63461,528.80461 381.26391,662.02535 \
    C 468.83815,529.62199 609.82641,489.17075 609.82641,358.71731 \
    C 609.82641,292.47731 556.06651,238.7173 489.82641,238.71731  \
    C 441.77851,238.71731 400.42481,267.08774 381.26391,307.90481 \
    C 362.10311,267.08773 320.74941,238.7173 272.70141,238.71731  \
    z ');
    var scale = 100 / path.width;
    path.set({ left: 50, top: 200, scaleX: scale, scaleY: scale, fill: shapeColor, });
    canvas.add(path);
    canvas.setActiveObject(path);
    canvas.renderAll();
    updateModifications(true);
    canvas.counter++;
};

//drawingArrow.onclick = function () {
//    addArrowToCanvas();
//};

if (canvas.freeDrawingBrush) {
    canvas.freeDrawingBrush.color = lineColor;
    canvas.freeDrawingBrush.width = parseInt(drawingLineWidthEl.value, 10) || 1;
    canvas.freeDrawingBrush.shadowBlur = 0;
}

//document.getElementById('canvas-background-picker').addEventListener('change', function () {
//    canvas.backgroundColor = this.value;
//    canvas.renderAll();
//});
//$(document).ready(function () {
//    shapeColor = $('#drawing-shape-color').val();
//})

drawingShadowWidth.onchange = function () {

    canvas.freeDrawingBrush.shadow.blur = parseInt(this.value, 10) || 0;
    this.previousSibling.innerHTML = this.value;
};
drawingShadowOffset.onchange = function () {

    canvas.freeDrawingBrush.shadow.offsetX = parseInt(this.value, 10) || 0;
    canvas.freeDrawingBrush.shadow.offsetY = parseInt(this.value, 10) || 0;
    this.previousSibling.innerHTML = this.value;
};

if (canvas.freeDrawingBrush) {
    canvas.freeDrawingBrush.color = lineColor;
    canvas.freeDrawingBrush.width = parseInt(drawingLineWidthEl.value, 10) || 1;
    canvas.freeDrawingBrush.shadow = new fabric.Shadow({
        blur: parseInt(drawingShadowWidth.value, 10) || 0,
        offsetX: 0,
        offsetY: 0,
        affectStroke: true,
        color: shadowColor,
    });
}

function addArrowToCanvas() {
    var line,
    arrow,
    circle;

    line = new fabric.Line([50, 50, 100, 100], {
        stroke: '#000',
        selectable: true,
        strokeWidth: '2',
        padding: 5,
        hasBorders: false,
        hasControls: false,
        originX: 'center',
        originY: 'center',
        lockScalingX: true,
        lockScalingY: true
    });

    var centerX = (line.x1 + line.x2) / 2,
        centerY = (line.y1 + line.y2) / 2;
    deltaX = line.left - centerX,
    deltaY = line.top - centerY;

    arrow = new fabric.Triangle({
        left: line.get('x1') + deltaX,
        top: line.get('y1') + deltaY,
        originX: 'center',
        originY: 'center',
        hasBorders: false,
        hasControls: false,
        lockScalingX: true,
        lockScalingY: true,
        lockRotation: true,
        pointType: 'arrow_start',
        angle: -45,
        width: 20,
        height: 20,
        fill: '#000'
    });
    arrow.line = line;

    circle = new fabric.Circle({
        left: line.get('x2') + deltaX,
        top: line.get('y2') + deltaY,
        radius: 3,
        stroke: '#000',
        strokeWidth: 3,
        originX: 'center',
        originY: 'center',
        hasBorders: false,
        hasControls: false,
        lockScalingX: true,
        lockScalingY: true,
        lockRotation: true,
        pointType: 'arrow_end',
        fill: '#000'
    });
    circle.line = line;

    line.customType = arrow.customType = circle.customType = 'arrow';
    line.circle = arrow.circle = circle;
    line.arrow = circle.arrow = arrow;

    canvas.add(line, arrow, circle);
    updateModifications(true);
    canvas.counter++;
    function moveEnd(obj) {
        var p = obj,
            x1, y1, x2, y2;

        if (obj.pointType === 'arrow_end') {
            obj.line.set('x2', obj.get('left'));
            obj.line.set('y2', obj.get('top'));
        } else {
            obj.line.set('x1', obj.get('left'));
            obj.line.set('y1', obj.get('top'));
        }

        obj.line._setWidthHeight();

        x1 = obj.line.get('x1');
        y1 = obj.line.get('y1');
        x2 = obj.line.get('x2');
        y2 = obj.line.get('y2');

        angle = calcArrowAngle(x1, y1, x2, y2);

        if (obj.pointType === 'arrow_end') {
            obj.arrow.set('angle', angle - 90);
        } else {
            obj.set('angle', angle - 90);
        }

        obj.line.setCoords();
        canvas.renderAll();
    }

    function moveLine() {
        var oldCenterX = (line.x1 + line.x2) / 2,
            oldCenterY = (line.y1 + line.y2) / 2,
            deltaX = line.left - oldCenterX,
            deltaY = line.top - oldCenterY;

        line.arrow.set({
            'left': line.x1 + deltaX,
            'top': line.y1 + deltaY
        }).setCoords();

        line.circle.set({
            'left': line.x2 + deltaX,
            'top': line.y2 + deltaY
        }).setCoords();

        line.set({
            'x1': line.x1 + deltaX,
            'y1': line.y1 + deltaY,
            'x2': line.x2 + deltaX,
            'y2': line.y2 + deltaY
        });

        line.set({
            'left': (line.x1 + line.x2) / 2,
            'top': (line.y1 + line.y2) / 2
        });
    }

    arrow.on('moving', function () {
        moveEnd(arrow);
    });

    circle.on('moving', function () {
        moveEnd(circle);
    });

    line.on('moving', function () {
        moveLine();
    });
}

function calcArrowAngle(x1, y1, x2, y2) {
    var angle = 0,
        x, y;

    x = (x2 - x1);
    y = (y2 - y1);

    if (x === 0) {
        angle = (y === 0) ? 0 : (y > 0) ? Math.PI / 2 : Math.PI * 3 / 2;
    } else if (y === 0) {
        angle = (x > 0) ? 0 : Math.PI;
    } else {
        angle = (x < 0) ? Math.atan(y / x) + Math.PI : (y < 0) ? Math.atan(y / x) + (2 * Math.PI) : Math.atan(y / x);
    }

    return (angle * 180 / Math.PI);
}



function onMoving() {
    UserDashboard.SetBlockSectionToTop();
    updateModifications(true);
}

var MainDashboard = {
    SetRecipientToTop: function () {
        if (recipientcanvasgroup != null && recipientcanvasgroup.length > 0) {
            for (var i = 0; i < recipientcanvasgroup.length; i++) {
                canvasBack.bringToFront(recipientcanvasgroup[i]);
            }
        }
    },
    lockRecipients: function () {
        if (recipientcanvasgroup != null && recipientcanvasgroup.length > 0) {
            for (var i = 0; i < recipientcanvasgroup.length; i++) {
                recipientcanvasgroup[i].lockRotation = true;
            }
        }
    }
}





//// Edit and delete button on controll

//var HideControls = {
//    'tl': true,
//    'tr': true,
//    'bl': false,
//    'br': false,
//    'ml': false,
//    'mt': false,
//    'mr': false,
//    'mb': false,
//    'mtr': false
//};

//var ctrlImages = new Array()

//function preload() {
//    for (i = 0; i < preload.arguments.length; i++) {
//        ctrlImages[i] = new Image();
//        ctrlImages[i].src = preload.arguments[i];
//    }
//}

//preload(
//    "../../Content/images/cross.png",
//       "../../Content/images/edit-icon.png"
//)


////override _drawControl function to change the corner images    
//fabric.Object.prototype._drawControl = function (control, ctx, methodName, left, top, flipiX, flipiY) {

//    var sizeX = this.cornerSize / this.scaleX,
//        sizeY = this.cornerSize / this.scaleY;

//    if (this.isControlVisible(control)) {
//        /* isVML ||*/ this.transparentCorners || ctx.clearRect(left, top, sizeX, sizeY);


//        var SelectedIconImage = new Image();
//        var lx = '';
//        var ly = '';
//        var n = '';

//        switch (control) {
//            case 'tl':
//                if (flipiY) { ly = 'b'; } else { ly = 't'; }
//                if (flipiX) { lx = 'r'; } else { lx = 'l'; }
//                break;
//            case 'tr':
//                if (flipiY) { ly = 'b'; } else { ly = 't'; }
//                if (flipiX) { lx = 'l'; } else { lx = 'r'; }
//                break;
//            case 'bl':
//                if (flipiY) { ly = 't'; } else { ly = 'b'; }
//                if (flipiX) { lx = 'r'; } else { lx = 'l'; }
//                break;
//            case 'br':
//                if (flipiY) { ly = 't'; } else { ly = 'b'; }
//                if (flipiX) { lx = 'l'; } else { lx = 'r'; }
//                break;
//            default:
//                ly = control.substr(0, 1);
//                lx = control.substr(1, 1);
//                break;
//        }

//        control = ly + lx;

//        switch (control) {
//            case 'tl':
//                SelectedIconImage.src = ctrlImages[1].src;
//                break;
//            case 'tr':
//                if (flipiX && !flipiY) { n = '2'; }
//                if (!flipiX && flipiY) { n = '3'; }
//                if (flipiX && flipiY) { n = '4'; }
//                SelectedIconImage.src = ctrlImages[0].src;
//                break;
//            case 'mt':

//                break;
//            case 'bl':
//                if (flipiY) { n = '2'; }
//                SelectedIconImage.src = ctrlImages[3].src;
//                break;
//            case 'br':
//                if (flipiX || flipiY) { n = '2'; }
//                if (flipiX && flipiY) { n = ''; }
//                SelectedIconImage.src = ctrlImages[2].src;
//                break;
//            case 'mb':

//                break;
//            case 'ml':

//                break;
//            case 'mr':

//                break;
//            default:
//                ctx[methodName](left, top, sizeX, sizeY);
//                break;
//        }

//        if (control == 'tl' || control == 'tr' || control == 'bl' || control == 'br'
//        || control == 'mt' || control == 'mb' || control == 'ml' || control == 'mr') {
//            sizeX = 15;
//            sizeY = 15;
//            ctx.drawImage(SelectedIconImage, left, top, sizeX, sizeY);
//        }


//        try {
//            ctx.drawImage(SelectedIconImage, left, top, sizeX, sizeY);

//        } catch (e) {
//            if (e.name != "NS_ERROR_NOT_AVAILABLE") {
//                throw e;
//            }
//        }


//    }
//};//end


////object corners
//var cursorOffset = {
//    mt: 0, // n
//    tr: 1, // ne
//    mr: 2, // e
//    br: 3, // se
//    mb: 4, // s
//    bl: 5, // sw
//    ml: 6, // w
//    tl: 7 // nw
//};

////override prorotype _setCornerCursor to change the corner cusrors
////when mouse is over corner (tl,tr,bl,br),we change the mouse cursor
//fabric.Canvas.prototype._setCornerCursor = function (corner, target) {
//    //for top left corner
//    if (corner == "tl") {
//        this.setCursor('pointer'); return false;
//        //for top right corner
//    } else if (corner == "tr") {
//        this.setCursor('pointer'); return false;
//        //for bottom left corner
//    }
//    //else if (corner == "bl") {
//    //    this.setCursor('help'); return false;
//    //    //for bottom right corner
//    //} else if (corner == "br") {
//    //    this.setCursor('copy'); return false;
//    //}
//};


////we can write different functionality for each object corner, currently just an alert message
//canvasBack.on('mouse:down', function (e) {
//    if (canvasBack.getActiveObject()) {
//        var target = this.findTarget();

//        if (target.__corner == 'tr') {
//            $.ConfirmBox("", "Are you sure to delete this record?", null, true, "Yes", true, target.id, function () {
//                Recipient.DeleteRecipientByID(target.id);
//            });
////            $.ShowMessage($('div.messageAlert'), "Delete pressed" + target.id, 1);
//            //alert('Delete pressed');
//        } else if (target.__corner == 'tl') {
//            //alert('Edit pressed');
//            Recipient.BindRecipientModel(target.id);
//            //$.ShowMessage($('div.messageAlert'), "Edit pressed" + target.id, 1);
//        }
//        //else if (target.__corner == 'bl') {
//        //    alert('save pressed');
//        //} else if (target.__corner == 'br') {
//        //    alert('copy pressed');
//        //}
//    }

//});

function updateModifications(savehistory) {
    if (savehistory === true) {
        myjson = JSON.stringify(canvas);
        state.push(myjson);
        ManageReduUndoButton();
    }
}
function undo() {
    if (mods < state.length) {
        canvas.clear().renderAll();
        canvas.loadFromJSON(state[state.length - 1 - mods - 1]);
        canvas.renderAll();
        canvas.getActiveObject()
        canvas.setActiveObject(canvas._objects[canvas._objects.length - 1])
        //console.log("geladen " + (state.length-1-mods-1));
        //console.log("state " + state.length);
        mods += 1;
        //console.log("mods " + mods);
    }
    ManageReduUndoButton();

}
function redo() {
    if (mods > 0) {
        canvas.clear().renderAll();
        canvas.loadFromJSON(state[state.length - 1 - mods + 1]);
        canvas.renderAll();
        canvas.setActiveObject(canvas._objects[canvas._objects.length - 1])
        //console.log("geladen " + (state.length-1-mods+1));
        mods -= 1;
        //console.log("state " + state.length);
        //console.log("mods " + mods);
    }
    ManageReduUndoButton();
}
$(document).ready(function () {
    ManageReduUndoButton()
})
function ManageReduUndoButton() {
    if (JSON.stringify(canvas) == state[state.length - 1]) {
        $('.redu').removeClass('redu-record')
        $('.redu').addClass('redu-norecord')
    }
    else {
        $('.redu').removeClass('redu-norecord')
        $('.redu').addClass('redu-record')

    }
    if (JSON.stringify(canvas) != state[0]) {
        $('.undo').removeClass('undo-norecord')
        $('.undo').addClass('undo-record')
    }
    else {
        $('.undo').removeClass('undo-record')
        $('.undo').addClass('undo-norecord')
    }


}

var _configFront = {
    canvasState: [],
    currentStateIndex: -1,
    undoStatus: false,
    redoStatus: false,
    undoFinishedStatus: 1,
    redoFinishedStatus: 1,
    undoButton: document.getElementById('undo1'),
    redoButton: document.getElementById('redo1'),
};

var _configBack = {
    canvasState: [],
    currentStateIndex: -1,
    undoStatus: false,
    redoStatus: false,
    undoFinishedStatus: 1,
    redoFinishedStatus: 1,
    undoButton: document.getElementById('undo1'),
    redoButton: document.getElementById('redo1'),
};

function undo1() {
    var step = $('.steps > li.active > a').attr('data-step')
    if (step == "1") {
        if (_configFront.undoFinishedStatus) {
            if (_configFront.currentStateIndex == -1) {
                _configFront.undoStatus = false;
            }
            else {
                if (_configFront.canvasState.length >= 1) {
                    _configFront.undoFinishedStatus = 0;
                    if (_configFront.currentStateIndex != 0) {
                        _configFront.undoStatus = true;
                        canvas.loadFromJSON(_configFront.canvasState[_configFront.currentStateIndex - 1], function () {
                            var jsonData = JSON.parse(_configFront.canvasState[_configFront.currentStateIndex - 1]);
                            canvas.renderAll();
                            _configFront.undoStatus = false;
                            _configFront.currentStateIndex -= 1;
                            _configFront.undoButton.removeAttribute("disabled");
                            if (_configFront.currentStateIndex !== _configFront.canvasState.length - 1) {
                                _configFront.redoButton.removeAttribute('disabled');
                            }
                            _configFront.undoFinishedStatus = 1;
                        });
                    }
                    else if (_configFront.currentStateIndex == 0) {
                        canvas.clear();
                        _configFront.undoFinishedStatus = 1;
                        _configFront.undoButton.disabled = "disabled";
                        _configFront.redoButton.removeAttribute('disabled');
                        _configFront.currentStateIndex -= 1;
                    }
                }
            }
        }
    }
    else if (step == "2") {
        if (_configBack.undoFinishedStatus) {
            if (_configBack.currentStateIndex == -1) {
                _configBack.undoStatus = false;
            }
            else {
                if (_configBack.canvasState.length >= 1) {
                    _configBack.undoFinishedStatus = 0;
                    if (_configBack.currentStateIndex != 0) {
                        _configBack.undoStatus = true;
                        canvasBack.loadFromJSON(_configBack.canvasState[_configBack.currentStateIndex - 1], function () {
                            var jsonData = JSON.parse(_configBack.canvasState[_configBack.currentStateIndex - 1]);
                            canvas.renderAll();
                            _configBack.undoStatus = false;
                            _configBack.currentStateIndex -= 1;
                            _configBack.undoButton.removeAttribute("disabled");
                            if (_configBack.currentStateIndex !== _configBack.canvasState.length - 1) {
                                _configBack.redoButton.removeAttribute('disabled');
                            }
                            _configBack.undoFinishedStatus = 1;
                        });
                    }
                    else if (_configBack.currentStateIndex == 0) {
                        canvas.clear();
                        _configBack.undoFinishedStatus = 1;
                        _configBack.undoButton.disabled = "disabled";
                        _configBack.redoButton.removeAttribute('disabled');
                        _configBack.currentStateIndex -= 1;
                    }
                }
            }
        }
    }
}

function redo1() {
    debugger
    var step = $('.steps > li.active > a').attr('data-step')
    if (step == "1") {
        if (_configFront.redoFinishedStatus) {
            if ((_configFront.currentStateIndex == _configFront.canvasState.length - 1) && _configFront.currentStateIndex != -1) {
                _configFront.redoButton.disabled = "disabled";
            } else {
                if (_configFront.canvasState.length > _configFront.currentStateIndex && _configFront.canvasState.length != 0) {
                    _configFront.redoFinishedStatus = 0;
                    _configFront.redoStatus = true;
                    canvas.loadFromJSON(_configFront.canvasState[_configFront.currentStateIndex + 1], function () {
                        var jsonData = JSON.parse(_configFront.canvasState[_configFront.currentStateIndex + 1]);
                        canvas.renderAll();
                        _configFront.redoStatus = false;
                        _configFront.currentStateIndex += 1;
                        if (_configFront.currentStateIndex != -1) {
                            _configFront.undoButton.removeAttribute('disabled');
                        }
                        _configFront.redoFinishedStatus = 1;
                        if ((_configFront.currentStateIndex == _configFront.canvasState.length - 1) && _configFront.currentStateIndex != -1) {
                            _configFront.redoButton.disabled = "disabled";
                        }
                    });
                }
            }
        }
    }
    else if (step == "2") {
        if (_configBack.redoFinishedStatus) {
            if ((_configBack.currentStateIndex == _configBack.canvasState.length - 1) && _configBack.currentStateIndex != -1) {
                _configBack.redoButton.disabled = "disabled";
            } else {
                if (_configBack.canvasState.length > _configBack.currentStateIndex && _configBack.canvasState.length != 0) {
                    _configBack.redoFinishedStatus = 0;
                    _configBack.redoStatus = true;
                    canvasBack.loadFromJSON(_configBack.canvasState[_configBack.currentStateIndex + 1], function () {
                        var jsonData = JSON.parse(_configBack.canvasState[_configBack.currentStateIndex + 1]);
                        canvas.renderAll();
                        _configBack.redoStatus = false;
                        _configBack.currentStateIndex += 1;
                        if (_configBack.currentStateIndex != -1) {
                            _configBack.undoButton.removeAttribute('disabled');
                        }
                        _configBack.redoFinishedStatus = 1;
                        if ((_configBack.currentStateIndex == _configBack.canvasState.length - 1) && _configBack.currentStateIndex != -1) {
                            _configBack.redoButton.disabled = "disabled";
                        }
                    });
                }
            }
        }
    }
}

function updateCanvasState() {
    var step = $('.steps > li.active > a').attr('data-step')
    if (step == "1") {
        if ((_configFront.undoStatus == false && _configFront.redoStatus == false)) {
            var jsonData = canvas.toJSON();
            var canvasAsJson = JSON.stringify(jsonData);
            if (_configFront.currentStateIndex < _configFront.canvasState.length - 1) {
                var indexToBeInserted = _configFront.currentStateIndex + 1;
                _configFront.canvasState[indexToBeInserted] = canvasAsJson;
                var numberOfElementsToRetain = indexToBeInserted + 1;
                _configFront.canvasState = _configFront.canvasState.splice(0, numberOfElementsToRetain);
            } else {
                _configFront.canvasState.push(canvasAsJson);
            }
            _configFront.currentStateIndex = _configFront.canvasState.length - 1;
            if ((_configFront.currentStateIndex == _configFront.canvasState.length - 1) && _configFront.currentStateIndex != -1) {
                _configFront.redoButton.disabled = "disabled";
            }
        }
    }
    else if (step == "2") {
        if ((_configBack.undoStatus == false && _configBack.redoStatus == false)) {
            var jsonData = canvasBack.toJSON();
            var canvasAsJson = JSON.stringify(jsonData);
            if (_configBack.currentStateIndex < _configBack.canvasState.length - 1) {
                var indexToBeInserted = _configBack.currentStateIndex + 1;
                _configBack.canvasState[indexToBeInserted] = canvasAsJson;
                var numberOfElementsToRetain = indexToBeInserted + 1;
                _configBack.canvasState = _configBack.canvasState.splice(0, numberOfElementsToRetain);
            } else {
                _configBack.canvasState.push(canvasAsJson);
            }
            _configBack.currentStateIndex = _configBack.canvasState.length - 1;
            if ((_configBack.currentStateIndex == _configBack.canvasState.length - 1) && _configBack.currentStateIndex != -1) {
                _configBack.redoButton.disabled = "disabled";
            }
        }
    }
}

$(document).on('click', '#undo1', function () {
    var step = $('.steps > li.active > a').attr('data-step')
    if (step == "1") {
        if (_configFront.currentStateIndex <= 9) {
            $.ShowMessage($('div.messageAlert'), "You must add an image or text to the card first. For additional information, Please watch our tutorial.", MessageType.Error);
        }
        else {
            undo1();
        }
    }
    else if (step == "2") {
        if (_configBack.currentStateIndex == -1) {
            $.ShowMessage($('div.messageAlert'), "You must add an image or text to the card first. For additional information, Please watch our tutorial.", MessageType.Error);
        }
        else {
            undo1();
        }
    }
});

$(document).on('click', '#redo1', function () {
    redo1();
});



