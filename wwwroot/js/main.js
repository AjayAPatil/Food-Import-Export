AOS.init({
    duration: 800,
    easing: 'slide'
});

(function ($) {

    "use strict";

    var isMobile = {
        Android: function () {
            return navigator.userAgent.match(/Android/i);
        },
        BlackBerry: function () {
            return navigator.userAgent.match(/BlackBerry/i);
        },
        iOS: function () {
            return navigator.userAgent.match(/iPhone|iPad|iPod/i);
        },
        Opera: function () {
            return navigator.userAgent.match(/Opera Mini/i);
        },
        Windows: function () {
            return navigator.userAgent.match(/IEMobile/i);
        },
        any: function () {
            return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
        }
    };


    $(window).stellar({
        responsive: true,
        parallaxBackgrounds: true,
        parallaxElements: true,
        horizontalScrolling: false,
        hideDistantElements: false,
        scrollProperty: 'scroll'
    });


    var fullHeight = function () {

        $('.js-fullheight').css('height', $(window).height());
        $(window).resize(function () {
            $('.js-fullheight').css('height', $(window).height());
        });

    };
    fullHeight();

    // loader
    var loader = function () {
        setTimeout(function () {
            if ($('#ftco-loader').length > 0) {
                $('#ftco-loader').removeClass('show');
            }
        }, 1);
    };
    loader();

    // Scrollax
    $.Scrollax();

    var carousel = function () {
        $('.home-slider').owlCarousel({
            loop: true,
            autoplay: true,
            margin: 0,
            animateOut: 'fadeOut',
            animateIn: 'fadeIn',
            nav: false,
            autoplayHoverPause: false,
            items: 1,
            navText: ["<span class='ion-md-arrow-back'></span>", "<span class='ion-chevron-right'></span>"],
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 1
                },
                1000: {
                    items: 1
                }
            }
        });

        $('.carousel-testimony').owlCarousel({
            center: true,
            loop: true,
            items: 1,
            margin: 30,
            stagePadding: 0,
            nav: false,
            navText: ['<span class="ion-ios-arrow-back">', '<span class="ion-ios-arrow-forward">'],
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 3
                },
                1000: {
                    items: 3
                }
            }
        });

    };
    carousel();

    $('nav .dropdown').hover(function () {
        var $this = $(this);
        // 	 timer;
        // clearTimeout(timer);
        $this.addClass('show');
        $this.find('> a').attr('aria-expanded', true);
        // $this.find('.dropdown-menu').addClass('animated-fast fadeInUp show');
        $this.find('.dropdown-menu').addClass('show');
    }, function () {
        var $this = $(this);
        // timer;
        // timer = setTimeout(function(){
        $this.removeClass('show');
        $this.find('> a').attr('aria-expanded', false);
        // $this.find('.dropdown-menu').removeClass('animated-fast fadeInUp show');
        $this.find('.dropdown-menu').removeClass('show');
        // }, 100);
    });


    $('#dropdown04').on('show.bs.dropdown', function () {
        console.log('show');
    });

    // scroll
    var scrollWindow = function () {
        $(window).scroll(function () {
            var $w = $(this),
                st = $w.scrollTop(),
                navbar = $('.ftco_navbar'),
                sd = $('.js-scroll-wrap');

            if (st > 150) {
                if (!navbar.hasClass('scrolled')) {
                    navbar.addClass('scrolled');
                }
            }
            if (st < 150) {
                if (navbar.hasClass('scrolled')) {
                    navbar.removeClass('scrolled sleep');
                }
            }
            if (st > 350) {
                if (!navbar.hasClass('awake')) {
                    navbar.addClass('awake');
                }

                if (sd.length > 0) {
                    sd.addClass('sleep');
                }
            }
            if (st < 350) {
                if (navbar.hasClass('awake')) {
                    navbar.removeClass('awake');
                    navbar.addClass('sleep');
                }
                if (sd.length > 0) {
                    sd.removeClass('sleep');
                }
            }
        });
    };
    scrollWindow();


    var counter = function () {

        $('#section-counter').waypoint(function (direction) {

            if (direction === 'down' && !$(this.element).hasClass('ftco-animated')) {

                var comma_separator_number_step = $.animateNumber.numberStepFactories.separator(',')
                $('.number').each(function () {
                    var $this = $(this),
                        num = $this.data('number');
                    console.log(num);
                    $this.animateNumber(
                        {
                            number: num,
                            numberStep: comma_separator_number_step
                        }, 7000
                    );
                });

            }

        }, { offset: '95%' });

    }
    counter();

    var contentWayPoint = function () {
        var i = 0;
        $('.ftco-animate').waypoint(function (direction) {

            if (direction === 'down' && !$(this.element).hasClass('ftco-animated')) {

                i++;

                $(this.element).addClass('item-animate');
                setTimeout(function () {

                    $('body .ftco-animate.item-animate').each(function (k) {
                        var el = $(this);
                        setTimeout(function () {
                            var effect = el.data('animate-effect');
                            if (effect === 'fadeIn') {
                                el.addClass('fadeIn ftco-animated');
                            } else if (effect === 'fadeInLeft') {
                                el.addClass('fadeInLeft ftco-animated');
                            } else if (effect === 'fadeInRight') {
                                el.addClass('fadeInRight ftco-animated');
                            } else {
                                el.addClass('fadeInUp ftco-animated');
                            }
                            el.removeClass('item-animate');
                        }, k * 50, 'easeInOutExpo');
                    });

                }, 100);

            }

        }, { offset: '95%' });
    };
    contentWayPoint();


    // navigation
    var OnePageNav = function () {
        $(".smoothscroll[href^='#'], #ftco-nav ul li a[href^='#']").on('click', function (e) {
            e.preventDefault();

            var hash = this.hash,
                navToggler = $('.navbar-toggler');
            $('html, body').animate({
                scrollTop: $(hash).offset().top
            }, 700, 'easeInOutExpo', function () {
                window.location.hash = hash;
            });


            if (navToggler.is(':visible')) {
                navToggler.click();
            }
        });
        $('body').on('activate.bs.scrollspy', function () {
            console.log('nice');
        })
    };
    OnePageNav();


    // magnific popup
    $('.image-popup').magnificPopup({
        type: 'image',
        closeOnContentClick: true,
        closeBtnInside: false,
        fixedContentPos: true,
        mainClass: 'mfp-no-margins mfp-with-zoom', // class to remove default margin from left and right side
        gallery: {
            enabled: true,
            navigateByImgClick: true,
            preload: [0, 1] // Will preload 0 - before current, and 1 after the current image
        },
        image: {
            verticalFit: true
        },
        zoom: {
            enabled: true,
            duration: 300 // don't foget to change the duration also in CSS
        }
    });

    $('.popup-youtube, .popup-vimeo, .popup-gmaps').magnificPopup({
        disableOn: 700,
        type: 'iframe',
        mainClass: 'mfp-fade',
        removalDelay: 160,
        preloader: false,

        fixedContentPos: false
    });



    var goHere = function () {

        $('.mouse-icon').on('click', function (event) {

            event.preventDefault();

            $('html,body').animate({
                scrollTop: $('.goto-here').offset().top
            }, 500, 'easeInOutExpo');

            return false;
        });
    };
    goHere();


    function makeTimer() {

        var endTime = new Date("21 December 2019 9:56:00 GMT+01:00");
        endTime = (Date.parse(endTime) / 1000);

        var now = new Date();
        now = (Date.parse(now) / 1000);

        var timeLeft = endTime - now;

        var days = Math.floor(timeLeft / 86400);
        var hours = Math.floor((timeLeft - (days * 86400)) / 3600);
        var minutes = Math.floor((timeLeft - (days * 86400) - (hours * 3600)) / 60);
        var seconds = Math.floor((timeLeft - (days * 86400) - (hours * 3600) - (minutes * 60)));

        if (hours < "10") { hours = "0" + hours; }
        if (minutes < "10") { minutes = "0" + minutes; }
        if (seconds < "10") { seconds = "0" + seconds; }

        $("#days").html(days + "<span>Days</span>");
        $("#hours").html(hours + "<span>Hours</span>");
        $("#minutes").html(minutes + "<span>Minutes</span>");
        $("#seconds").html(seconds + "<span>Seconds</span>");

    }

    setInterval(function () { makeTimer(); }, 1000);



})(jQuery);


window.onload = onloadPage

function onloadPage() {
    var lin = document.getElementById('user-login-btn');
    var lout = document.getElementById('user-logout-btn');
    var admn = document.getElementById('admin-btn');
    var home = document.getElementById('home-btn');
    var admnhome = document.getElementById('admin-home-btn');
    var shop = document.getElementById('shop-btn');
    var usrName = document.getElementById('logged-username');
    var usrNameLst = document.getElementsByName('logged-username');

    lin.style.display = 'block';
    lout.style.display = 'none';
    admn.style.display = 'none';
    home.style.display = 'block';
    admnhome.style.display = 'none';
    shop.style.display = 'block';
    var jsonuserdata = localStorage.getItem("UserInfo");
    if (jsonuserdata) {
        var user = JSON.parse(jsonuserdata);

        if (user) {
            usrName.innerHTML = user.userName;
            usrNameLst.forEach(u => u.innerHTML = user.userName);
            lin.style.display = 'none'

            if (user.role == 'Admin') {
                admn.style.display = 'block';
                admnhome.style.display = 'block';
                home.style.display = 'none';
                shop.style.display = 'none';
            } else {
                lout.style.display = 'block'
                shop.style.display = 'block';
            }
        }
    }
}

function logout() {
    localStorage.clear();
    onloadPage();
    location.reload();
}

function showSuccessMsg(message) {
    showMsg('success', message);
}
function showErrorMsg(message) {
    showMsg('error', message);
}
function showInfoMsg(message) {
    showMsg('info', message);
}
function showWarningMsg(message) {
    showMsg('warning', message);
}
function showMsg(messageType, message) {
    var msgDiv = document.getElementById("alertbox");
    msgDiv.style.display = 'block';
    msgDiv.style.animation = 'food-alert-animate-right 0.4s forwards';

    if (messageType == 'success') {
        msgDiv.style.backgroundColor = '#04AA6D';
        msgDiv.style.color = '#fff';
    } else if (messageType == 'error') {
        msgDiv.style.backgroundColor = '#f44336';
        msgDiv.style.color = '#fff';
    } else if (messageType == 'info') {
        msgDiv.style.backgroundColor = '#2196F3';
        msgDiv.style.color = '#fff';
    }else if (messageType == 'warning') {
        msgDiv.style.backgroundColor = '#ff9800';
        msgDiv.style.color = '#fff';
    }
    else {
        msgDiv.style.backgroundColor = '#555555';
        msgDiv.style.color = '#fff';
    }
    var msgDiv = document.getElementById("alertbox-msg");
    msgDiv.innerHTML = message;

    setTimeout(hideMsg, 5000)
}

function hideMsg() {
    var msgDiv = document.getElementById("alertbox");
    msgDiv.style.animation = 'food-alert-animate-left 0.4s forwards';
}

function showImageInModal(imageId) {
    if (typeof (imageId) == typeof ('string')) {
        var img = document.getElementById(imageId);
        if (img && img.src) {
            // Get the modal
            var modal = document.getElementById("ImgModal");

            // Get the image and insert it inside the modal - use its "alt" text as a caption
            var modalImg = document.getElementById("ImgModalImg");
            var captionText = document.getElementById("ImgModalcaption");
            //img.onclick = function () {
            modal.style.display = "block";
            modalImg.src = img.src;
            captionText.innerHTML = img.alt;
            //}

            // Get the <span> element that closes the modal
            var span = document.getElementsByClassName("img-modal-close")[0];

            // When the user clicks on <span> (x), close the modal
            span.onclick = function () {
                modal.style.display = "none";
            }
        }
    }
}

const toBase64 = file => new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = reject;
});

function base64ToFile(base64Data) {
    //Create a Blob object
    let blob = (dataURItoBlob(base64Data));
    //Use the Blob to create a File Object
    let file = new File([blob], "product.jpg", { type: "image/jpg", lastModified: new Date().getTime() });
    return new FileListItems([file])
}

//Function that converts a data64 png image to a Blob object
function dataURItoBlob(dataURI) {
    var binary = atob(dataURI.split(',')[1]);
    var array = [];
    for (i = 0; i < binary.length; i++) {
        array.push(binary.charCodeAt(i));
    }
    return new Blob([new Uint8Array(array)], { type: 'image/png' });
}

//Function that inserts an array of File objects inside a input type file, because HTMLInputElement.files cannot be setted directly
function FileListItems(file_objects) {
    new_input = new ClipboardEvent("").clipboardData || new DataTransfer()
    for (i = 0, size = file_objects.length; i < size; ++i) {
        new_input.items.add(file_objects[i]);
    }
    return new_input.files;
}



var validation = {
    isEmailAddress: function (str) {
        var pattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
        return pattern.test(str);  // returns a boolean
    },
    isNotEmpty: function (str) {
        var pattern = /\S+/;
        return str == undefined ? false : pattern.test(str);  // returns a boolean
    },
    isNumber: function (str) {
        var pattern = /^\d+\.?\d*$/;
        return pattern.test(str);  // returns a boolean
    },
    isSame: function (str1, str2) {
        return str1 === str2;
    },
    isPassword: function (str) {
        var pattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$@!%&*?.])[A-Za-z\d#$@!%&*?.]{8,30}$/;
        return pattern.test(str)
    },
    isNullEmptyUndefined: function (str) {
        return str == null || str == undefined || str == '';
    }
};

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [day, month, year].join('-');
}

function setFoodModalContent(htmlContent, showSaveButton = false, design = '') {
    document.getElementById('modal-save-btn').hidden = showSaveButton;
    if (design == '') {
        document.getElementById('modal-content').innerHTML = htmlContent;
    } else {
        document.getElementById('modal-body').innerHTML = htmlContent;
    }
    return document.getElementById('modal-save-btn');
}
function openFoodModal() {
    document.getElementById('launch-modal-btn').click();
}