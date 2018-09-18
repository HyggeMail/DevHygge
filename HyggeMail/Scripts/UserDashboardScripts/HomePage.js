$(document).ready(function () {

    homePage.GetDemoPostCardListing(this);

});

$('.homePostCardClick').live("click", function () {
    var cardfrontpath = $(this).attr("data-cardfrontpath");
    var cardbackpath = $(this).attr("data-cardbackpath");

    $('#srcCardfrontpath').attr("src", "");
    $('#srcCardfrontpath').attr("src", cardfrontpath);
    $('#srcCardbackpath').attr("src", "");
    $('#srcCardbackpath').attr("src", cardbackpath);
    var href  =$(this).attr("data-href");
    if (href != undefined)
    {
        $('#setPostCardEditHREF').attr("href", href);
    }
    var returnurl = $(this).attr("data-returnurl");
    if (returnurl != undefined) {
        localStorage.setItem("EditCardReturnUrl", returnurl);
    }
});

var homePage = {

    GetDemoPostCardListing: function (sender) {

        $.ajaxExt({
            type: "POST",
            validate: false,
            parentControl: $(sender).parents("form:first"),
            data: {},
            messageControl: null,
            showThrobber: false,
            throbberPosition: { my: "left center", at: "right center", of: sender, offset: "5 0" },
            url: siteURL.GetDemoPostCardListing,
            success: function (results, message) {
                $('#div_DemoPostCards').html(results[0]);
            }
        });

    },

};
