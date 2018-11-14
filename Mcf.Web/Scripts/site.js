jQuery('#sidebar .sub-menu > a').click(function () {
    var last = jQuery('.sub-menu.open', jQuery('#sidebar'));
    jQuery('.menu-arrow').removeClass('arrow_carrot-right');
    jQuery('.sub', last).slideUp(200);
    var sub = jQuery(this).next();
    jQuery('#sidebar .sub-menu').find('ul').slideUp();
    if (sub.is(":visible")) {
        jQuery('.menu-arrow').addClass('arrow_carrot-right');
        sub.slideUp(200);
    } else {
        jQuery('.menu-arrow').addClass('arrow_carrot-down');
        sub.slideDown(200);
    }
    var o = (jQuery(this).offset());
    diff = 200 - o.top;
    if (diff > 0) {
        jQuery("#sidebar").scrollTo("-=" + Math.abs(diff), 500);
    }
    else {
        jQuery("#sidebar").scrollTo("+=" + Math.abs(diff), 500);
    }
});

// sidebar menu toggle
jQuery(function () {
    function responsiveView() {
        // var wSize = jQuery(window).width();
        // if (wSize <= 768) {
        //     jQuery('#container').addClass('sidebar-close');
        //     jQuery('#sidebar > ul').hide();
        // }

        // if (wSize > 768) {
        //     jQuery('#container').removeClass('sidebar-close');
        //     jQuery('#sidebar > ul').show();
        // }
    }
    jQuery(window).on('load', responsiveView);
    jQuery(window).on('resize', responsiveView);
});

jQuery('.toggle-nav').click(function () {
    jQuery("body").toggleClass("IsSidebar");
    // if (jQuery('#sidebar > ul').is(":visible") === true) {
    //     // jQuery('#main-content').css({
    //     //     'margin-left': '0px'
    //     // });
    //     // jQuery('#sidebar').css({
    //     //     'margin-left': '-180px'
    //     // });
    //     // jQuery('#sidebar > ul').hide();
    //     jQuery("body").addClass("sidebar-closed");
    // } else {
    //     // jQuery('#main-content').css({
    //     //     'margin-left': '180px'
    //     // });
    //     // jQuery('#sidebar > ul').show();
    //     // jQuery('#sidebar').css({
    //     //     'margin-left': '0'
    //     // });
    //     jQuery("body").removeClass("sidebar-closed");
    // }
});
jQuery('.sidebarOff').click(function () {
    jQuery("body").removeClass("IsSidebar");    
});

jQuery(document).ready(function () {
    jQuery('.datepicker').datepicker();
    var item = location.pathname.split("/")[location.pathname.split("/").length - 1];
    if (item != '' && item != undefined) {
        jQuery('#sidebar ul li').removeClass('active');
        var href = "#sidebar ul a." + item;
        jQuery(href).parent('li').addClass('active');
        var sub = jQuery(href).parents().hasClass('sub');
        // debugger;
        if(sub == true) {
            var parent = jQuery(href).parent('li').parent('ul.sub');
            jQuery(parent).show();
            // jQuery(parent).parent('li').addClass('active');
            jQuery(parent).parent('li').find('.menu-arrow').removeClass('arrow_carrot-right');
            jQuery(parent).parent('li').find('.menu-arrow').addClass('arrow_carrot-down');
        }
    }
    var date = new Date();
    var enddate = new Date();
    date = date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();
    enddate = enddate.getMonth() + 1 + '/' + enddate.getDate() + '/' + (enddate.getFullYear() + 1);
    $('#calendarStartDate').val(date);
    $('#calendarEndDate').val(enddate);
});

function blockUI() {
    $.blockUI({ message: '<h3 style="color:gray"><img src="img/loading.gif" style="margin-top:-12px"/> Just a moment...</h3>' });
}
function unblockUI() {
    $.unblockUI();
}