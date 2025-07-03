/*--------------------- Copyright (c) 2020 -----------------------
[Master Javascript]
Project: Admin- Responsive HTML Template 
Version: 1.0.0
Assigned to: Theme Forest
-------------------------------------------------------------------*/

(function ($) {
    "use strict";

    var AdminApp = {
        config: {
            collapseScreenSize: 991,
            sideBarSize: 1199,
            animationSpeed: 300
        },
        initialized: false,
        version: "1.0.0",

        init: function () {
            if (this.initialized) return;
            this.initialized = true;

            // Initialize all components
            this.initLoader();
            this.initUserToggle();
            this.initSidebar();
            this.initSearch();
            this.initRightSlide();
            this.initTooltips();
            this.initCounters();
            this.initProductFeatures();
            this.initColorPicker();
        },

        // Loader functions
        initLoader: function () {
            $(window).on('load', function () {
                $(".loader").fadeOut();
                $(".spinner").delay(500).fadeOut("slow");
            });
        },

        // User toggle functions
        initUserToggle: function () {
            var count = 0;
            var self = this;

            $('.user-info').on("click", function (e) {
                e.stopPropagation();
                if ($(window).width() <= self.config.collapseScreenSize) {
                    $('.user-info-box').toggleClass('show');
                    count = $('.user-info-box').hasClass('show') ? 1 : 0;
                }
            });

            $('body').on("click", function () {
                if ($(window).width() <= self.config.collapseScreenSize && count === 1) {
                    $('.user-info-box').removeClass('show');
                    count = 0;
                }
            });
        },

        // Sidebar functions
        initSidebar: function () {
            var self = this;

            // Toggle sidebar
            $(".toggle-btn").on('click', function (e) {
                e.stopPropagation();
                $("body").toggleClass('mini-sidebar');
                $(this).toggleClass('checked');
            });

            // Sidebar hover effect
            if ($(window).width() >= this.config.sideBarSize) {
                $(".main-menu").hover(function () {
                    $('body').addClass('sidebar-hover');
                }, function () {
                    $('body').removeClass('sidebar-hover');
                });
            }

            // Initialize sidebar menu
            this.initSidebarMenu();
        },

        initSidebarMenu: function () {
            $('.side-menu-wrap ul li').has('.sub-menu').addClass('has-sub-menu');

            $('.main-menu').on('click', 'li a', function (e) {
                var $this = $(this);
                var checkElement = $this.next();

                if (checkElement.is('.sub-menu') && checkElement.is(':visible')) {
                    checkElement.slideUp(AdminApp.config.animationSpeed, function () {
                        checkElement.removeClass('menu-show');
                    });
                    checkElement.parent("li").removeClass("active");
                }
                else if ((checkElement.is('.sub-menu')) && (!checkElement.is(':visible'))) {
                    var parent = $this.parents('ul').first();
                    var ul = parent.find('ul:visible').slideUp(AdminApp.config.animationSpeed);
                    ul.removeClass('menu-show');
                    var parent_li = $this.parent("li");

                    checkElement.slideDown(AdminApp.config.animationSpeed, function () {
                        checkElement.addClass('menu-show');
                        parent.find('li.active').removeClass('active');
                        parent_li.addClass('active');
                    });
                }

                if (checkElement.is('.sub-menu')) {
                    e.preventDefault();
                }
            });

            // Set active menu item based on current URL
            this.setActiveMenuItem();
        },

        setActiveMenuItem: function () {
            var currentUrl = window.location.href;
            $(".main-menu a").filter(function () {
                return this.href == currentUrl;
            }).addClass("active").parent().addClass("active")
                .parents('li').addClass("in").addClass("active");
        },

        // Search functions
        initSearch: function () {
            $('.search-toggle').on("click", function (e) {
                e.stopPropagation();
                $('.serch-wrapper').addClass('show-search');
            });

            $('.search-close, .main-content').on("click", function () {
                $('.serch-wrapper').removeClass('show-search');
            });
        },

        // Right slide functions
        initRightSlide: function () {
            $(".setting-info").on('click', function (e) {
                e.stopPropagation();
                $("body").toggleClass('open-setting');
            });

            $('body, .close-btn').on('click', function () {
                $('body').removeClass('open-setting');
            });

            $('.slide-setting-box').on('click', function (e) {
                e.stopPropagation();
            });
        },

        // Tooltip functions
        initTooltips: function () {
            if ($('.toltiped').length > 0) {
                $(".toltiped").tooltip();
            }

            if ($('.toltiped-right').length > 0) {
                $(".toltiped-right").tooltip({
                    'placement': 'right',
                });
            }
        },

        // Counter functions
        initCounters: function () {
            // Animated counters
            if ($('.counter-holder').length > 0) {
                var a = 0;
                $(window).scroll(function () {
                    var topScroll = $('.counter-holder').offset().top - window.innerHeight;
                    if (a == 0 && $(window).scrollTop() > topScroll) {
                        $('.count-no').each(function () {
                            var $this = $(this),
                                countTo = $this.attr('data-count');
                            $({
                                countNum: $this.text()
                            }).animate({
                                countNum: countTo
                            }, {
                                duration: 5000,
                                easing: 'swing',
                                step: function () {
                                    $this.text(Math.floor(this.countNum));
                                },
                                complete: function () {
                                    $this.text(this.countNum);
                                }
                            });
                        });
                        a = 1;
                    }
                });
            }

            // Counter text appear
            if ($('.counter-text').length > 0) {
                $('.counter-text').appear(function () {
                    $('.count-no1').countTo();
                });
            }

            // Number spinners
            if ($('.number-spin').length > 0) {
                $('.number-increase').on('click', function () {
                    if ($(this).prev().val() < 50000) {
                        $(this).prev().val(+$(this).prev().val() + 1);
                    }
                });

                $('.number-decrease').on('click', function () {
                    if ($(this).next().val() > 1) {
                        $(this).next().val(+$(this).next().val() - 1);
                    }
                });
            }
        },

        // Product-related functions
        initProductFeatures: function () {
            // Product removal
            $(".remove-product").on('click', function () {
                $(this).closest('.product-thumb-wrap').parent().remove();
            });

            // Quantity controls
            $('.quantity-plus').on('click', function (e) {
                e.preventDefault();
                var quantity = parseInt($(this).siblings('.quantity').val());
                $(this).siblings('.quantity').val(quantity + 1);
            });

            $('.quantity-minus').on('click', function (e) {
                e.preventDefault();
                var quantity = parseInt($(this).siblings('.quantity').val());
                if (quantity > 0) {
                    $(this).siblings('.quantity').val(quantity - 1);
                }
            });

            // Price range slider
            if ($('.range-slider').length > 0) {
                $("#slider-range").slider({
                    range: true,
                    min: 12,
                    max: 2000,
                    values: [541, 1402],
                    slide: function (event, ui) {
                        $("#amount").text("$" + ui.values[0] + " - $" + ui.values[1]);
                    }
                });
                $("#amount").text("$" + $("#slider-range").slider("values", 0) + " - $" + $("#slider-range").slider("values", 1));
            }

            // Product thumbnail slider
            if ($('.int-thumb-slider').length > 0) {
                var galleryThumbs = new Swiper('.gallery-thumbs', {
                    spaceBetween: 10,
                    slidesPerView: 4,
                    freeMode: true,
                    watchSlidesVisibility: true,
                    watchSlidesProgress: true,
                });

                var galleryTop = new Swiper('.gallery-top', {
                    spaceBetween: 10,
                    thumbs: {
                        swiper: galleryThumbs
                    }
                });
            }

            // Single slide swiper
            if ($('.swiper-container.s1').length > 0) {
                new Swiper('.swiper-container.s1', {
                    autoHeight: false,
                    autoplay: false,
                    loop: true,
                    spaceBetween: 0,
                    centeredSlides: false,
                    speed: 1500,
                    autoplay: {
                        delay: 1000,
                    },
                    navigation: {
                        nextEl: '.swiperButtonNext',
                        prevEl: '.swiperButtonPrev',
                    },
                });
            }
        },

        // Color picker functions
        initColorPicker: function () {
            // Load saved color theme from cookie
            var colorcode = document.cookie;
            if (colorcode != "") {
                var cname = "colorcssfile";
                var name = cname + "=";
                var cssname = '';
                var ca = document.cookie.split(';');

                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i].trim();
                    if (c.indexOf(name) == 0) {
                        cssname = c.substring(name.length, c.length);
                    }
                }

                if (cssname != '') {
                    var new_style = '/Content/css/' + cssname + '.css';
                    $('#theme-change').attr('href', new_style);
                }
            }

            // Color change handler
            $('.colorchange').on("click", function () {
                var name = $(this).attr('id');
                var new_style = '/Content/css/' + name + '.css';
                $('#theme-change').attr('href', new_style);

                // Set cookie for color preference
                document.cookie = "colorcssfile=" + name + ";path=/";
            });

            // Style switcher toggle
            $("#style-switcher .bottom a.settings").on("click", function (e) {
                e.preventDefault();
                var div = $("#style-switcher");
                if (div.css("right") === "-133px") {
                    div.animate({ right: "0px" });
                } else {
                    div.animate({ right: "-133px" });
                }
            });
        }
    };

    // Initialize the application when DOM is ready
    $(document).ready(function () {
        AdminApp.init();
    });

})(jQuery);