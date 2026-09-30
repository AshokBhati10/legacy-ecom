/* Legacy eCommerce application behaviour (jQuery 3.4.1).
 *
 * - AJAX add-to-cart refreshes the header mini-cart via .load('/Cart/MiniCart')
 * - Product filtering GETs /Product/Filter and replaces the #product-list HTML
 * - Fancybox 3 product gallery, checkout wizard helpers
 */
var legacyEcom = legacyEcom || {};

(function ($) {
    'use strict';

    // Refresh the header mini-cart fragment.
    function refreshMiniCart() {
        $('#mini-cart').load('/Cart/MiniCart');
    }

    // Called by Ajax.BeginForm OnSuccess on every add-to-cart form.
    legacyEcom.cartAdded = function () {
        refreshMiniCart();
        var alert = $('<div class="alert alert-success alert-dismissible" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span></button>' +
            'Product added to your cart.</div>');
        $('.body-content').first().prepend(alert);
        window.setTimeout(function () {
            alert.fadeOut(function () { alert.remove(); });
        }, 2500);
    };

    // Called by Ajax.BeginForm OnFailure on every add-to-cart form.
    legacyEcom.cartFailed = function (xhr) {
        var msg = (xhr && xhr.responseText) ? xhr.responseText : 'Could not add the product to the cart.';
        window.alert(msg);
    };

    function loadProducts(url) {
        var $list = $('#product-list');
        $list.fadeTo(200, 0.4);
        $.get(url, function (html) {
            $list.html(html).fadeTo(200, 1);
        }).fail(function () {
            $list.fadeTo(200, 1);
            window.alert('Could not load products. Please try again.');
        });
    }

    $(function () {
        // Product filtering (GET /Product/Filter -> _ProductList partial).
        var $form = $('#product-filter-form');
        if ($form.length) {
            $form.on('submit', function (e) {
                e.preventDefault();
                loadProducts($form.attr('action') + '?' + $form.serialize());
            });
            $('#filter-category', $form).on('change', function () {
                $form.trigger('submit');
            });
        }

        // AJAX paging inside the product list fragment.
        $('#product-list').on('click', '#product-pager a', function (e) {
            e.preventDefault();
            loadProducts($(this).attr('href'));
        });

        // Fancybox 3 product gallery.
        if ($.fn.fancybox) {
            $('[data-fancybox]').fancybox();
        }

        // Checkout: show card fields only for card payment.
        var $cardFields = $('#card-fields');
        if ($cardFields.length) {
            $('input[name="PaymentMethod"]').on('change', function () {
                $cardFields.toggle($('#pm-card').is(':checked'));
            }).trigger('change');
        }

        // Order history: DataTables (also initialised inline on the view).
        if ($.fn.DataTable && $('#orders-table').length && !$.fn.DataTable.isDataTable('#orders-table')) {
            $('#orders-table').DataTable({ order: [[1, 'desc']], pageLength: 10 });
        }
    });
})(jQuery);
