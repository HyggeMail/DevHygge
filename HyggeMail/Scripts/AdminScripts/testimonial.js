$(document).ready(function () {
    $("input[type=button]#addTestimonialBtn").live("click", function () {

        return testimonials.AddEdittestimonial($(this));
    });
    $("input[type=button]#edittestimonialBtn").live("click", function () {
        return testimonials.Updatetestimonial($(this));
    });
    $("a.deletetestimonial").live("click", function () {
        return testimonials.Deletetestimonial($(this));
    });

    $("input[type=button]#btnFilterVersion").live("click", function () {
        return testimonials.Managetestimonials($(this));
    });
    $("select#showRecords").on("change", function () {
        return testimonials.ShowRecords($(this));
    });
    $('.sorting').live("click", function () {
        return testimonials.Sorttestimonials($(this));
    });
    $('#Search').keyup(function () {
        return testimonials.Searchtestimonials($(this));
    });

    //upload user image for  testimonial
    $('#image').on('change', function () {

        var allowedExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'rif', 'tiff'];
        var value = $(this).val();
        var file = value.toLowerCase();
        var extension = file.substring(file.lastIndexOf('.') + 1);
        if ($.inArray(extension, allowedExtensions) == -1) {
            $(this).val('');
            $('#UserImage').next('span').removeClass('field-validation-valid').addClass('field-validation-error').text('Only support jpg,jpeg,png,gif,bmp,rif,tiff formats.');
        }
        else {
            var files = this.files[0];
            var reader = new FileReader();
            reader.onload = (
                function (e) {

                    $('#UserImage').attr('src', e.target.result);
                    $('#UserImage').next('span').removeClass('field-validation-error').addClass('field-validation-valid').text('');
                });
            reader.readAsDataURL(files);
        }
    });
    $('#Search').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return testimonials.Searchtestimonials($(this));
        }
    });
});

var testimonials = {

    Sorttestimonials: function (sender) {
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

    AddEdittestimonial: function (sender) {
        //$("#Description").val(CKEDITOR.instances["Description"].getData());

        $.ajaxExt({
            url: baseUrl + siteURL.Testimonials,
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
                    window.location.href = baseUrl + siteURL.Managetestimonials;
                }, 1500);

            }
        });

    },

    Updatetestimonial: function (sender) {


        //var value = $("#Description").val(CKEDITOR.instances["Description"].getData());
        //$("#Description").val(CKEDITOR.instances["Description"].getData());

        $.ajaxExt({
            url: baseUrl + siteURL.UpdatetestimonialDetails,
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
                    window.location.href = baseUrl + siteURL.Managetestimonials;
                }, 1500);
            }
        });

    },

    Deletetestimonial: function (sender) {
        if (confirm("Are you sure to delete this record?")) {
            $.ajaxExt({
                url: baseUrl + siteURL.Deletetestimonial,
                type: 'POST',
                validate: false,
                showErrorMessage: true,
                messageControl: $('div.messageAlert'),
                showThrobber: true,
                button: $(sender),
                throbberPosition: { my: "left center", at: "right center", of: $(sender) },
                data: { testimonialId: $(sender).attr("data-testimonialid") },
                success: function (results, message) {
                    $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                    Paging();
                }
            });
        }
    },

    Managetestimonials: function (totalCount) {
        var totalRecords = 0;
        totalRecords = parseInt(totalCount);
        //alert(totalRecords);
        PageNumbering(totalRecords);
    },

    Searchtestimonials: function (sender) {
        paging.startIndex = 1;
        Paging(sender);

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
        url: baseUrl + '/Admin/Testimonial/GettestimonialsPagingList',
        success: function (results, message) {
            $('#divResult table:first tbody').html(results[0]);
            PageNumbering(results[1]);

        }
    });
}