$(document).ready(function () {
    $('.search').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return FAQ.SearchByCategory($(this).val(), $('.active-tab').data('catid'));
        }
    });
    $('.faq-cat').live("click", function () {

        $('ul.nav-tabs-cat li a').each(function () {
            $(this).removeClass("active-tab");
        })
        $(this).addClass("active-tab");
        return FAQ.SearchByCategory($('#search').val(), $(this).data('catid'));
    });
});

var FAQ = {

    SearchByCategory: function (search, category) {
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: { cat: category, search: search },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + '/Home/GetFAQByCategory',
            success: function (results, message) {
                $('.list-by-category').html(results[0]);
                FAQ.BindAccordion();
            }
        });
    },
    BindAccordion: function () {
        $(".accordion_example").smk_Accordion({
            closeAble: true, //boolean
        });
    }
};
