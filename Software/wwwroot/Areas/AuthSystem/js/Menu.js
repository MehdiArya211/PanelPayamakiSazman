// ===============================
//  Breadcrumb (در صورت نیاز)
// ===============================
var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/AuthSystem/Dashboard" });
breadcrumb.push({ title: "منو‌ ها", link: "#" });


// ===============================
//  عملیات مربوط به مدیریت منوها
// ===============================
var menu = {

    // منوی انتخاب شده در درخت
    selectedItem: null,


    // رفرش کردن لیست منو ها
    refreshList: function () {
        $.get("/AuthSystem/" + controller + "/GetAll", function (data) {
            menu.treeView.initial(data);
        });
    },


    // ===============================
    //  TreeView
    // ===============================
    treeView: {

        // راه اندازی تری ویو
        initial: function (data) {

            var model = menu.treeView.reshapeData(data, null);

            if (!model || model.length === 0) {
                $("#treeview").html("<div class='text-center py-3'>موردی برای نمایش یافت نشد.</div>");
                return;
            }

            $("#treeview").treeview({
                data: model,
                levels: 0,
                onNodeSelected: function (event, node) {
                    menu.selectedItem = node;
                    $(".edit-btn,.delete-btn").removeClass("hide");
                    $(".create-btn").html("افزودن زیر منو");
                },
                onNodeUnselected: function (event, node) {
                    menu.selectedItem = null;
                    $(".edit-btn,.delete-btn").addClass("hide");
                    $(".create-btn").html("افزودن منو جدید");
                }
            });

            // انتخاب و باز کردن ساختار درختی آخرین نودی که انتخاب شده بود
            if (menu.selectedItem) {
                var unSelectedNodes = $("#treeview").treeview('getUnselected');
                for (var j = 0; j < unSelectedNodes.length; j++) {
                    var node = unSelectedNodes[j];
                    if (node.id === menu.selectedItem.id) {
                        $("#treeview").treeview('selectNode', [node, { silent: false }]);
                        if (node.nodes && node.nodes.length > 0)
                            $("#treeview").treeview('expandNode', node);

                        while (node.parentId != null) {
                            var parent = $("#treeview").treeview('getParent', node);
                            $("#treeview").treeview('expandNode', parent);
                            node = parent;
                        }
                    }
                }
            }
        },


        // تبدیل داده‌ها به مدل مورد نیاز treeview
        reshapeData: function (data, parentId) {
            if (!data || data.length === 0)
                return null;

            var model = data.filter(function (obj) {
                return obj.parentId === parentId;
            });

            if (!model || model.length === 0)
                return null;

            $.each(model, function () {

                // اضافه کردن بچه‌ها
                this.nodes = menu.treeView.reshapeData(data, this.id);

                // رنگ پس‌زمینه برای غیرفعال‌ها
                if (!this.isEnabled)
                    this.backColor = "rgba(0, 0, 0, 0.06)";

                // آیکن
                var iconHtml = this.icon ? `<i class="${this.icon}"></i> ` : "";

                // *** نکته مهم ***
                // text باید برابر title باشد
                this.text = iconHtml + `<span class="node-title">${this.title}</span>`;
            });

            return model;
        }



    },



    // ===============================
    //  ایجاد منو
    // ===============================
    create: {

        // لود کردن فرم افزودن منو جدید
        loadForm: function () {

            var data = {};

            if (menu.selectedItem) {
                data.ParentId = menu.selectedItem.id;
            }

            $.get("/AuthSystem/" + controller + "/LoadCreateForm", data, function (res) {

                $("#modal-form").html(res);

                const modalEl = document.getElementById('base-modal');
                const modal = new bootstrap.Modal(modalEl);
                modal.show();

                var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                tooltipTriggerList.map(function (tooltipTriggerEl) {
                    return new bootstrap.Tooltip(tooltipTriggerEl);
                });

                var form = $(".create-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse(form);

            });
        }



        ,


        // ذخیره کردن فرم افزودن منو جدید
        save: function (e) {
            e.preventDefault();

            var $form = $(".create-form");
            var targetUrl = $form.attr("action");

            // ولیدیت jQuery Validate
            $form.validate();
            if (!$form.valid()) return false;

            // اعتبارسنجی اختصاصی لینک
            if (!menu.validate(".create-form")) return false;

            // رندر کردن پارامترها
            menu.parameters.render();

            var data = $form.serialize();

            $.post(targetUrl, data, function (res) {

                if (res.status) {
                    // بستن مدال
                    const modalEl = document.getElementById('base-modal');
                    const modalInstance = bootstrap.Modal.getInstance(modalEl);
                    modalInstance && modalInstance.hide();

                    // رفرش تری‌ویو
                    menu.refreshList();

                    // ریست دکمه‌ها
                    $(".edit-btn,.delete-btn").addClass("hide");
                    $(".create-btn").html("افزودن منو جدید");

                    Swal.fire({
                        icon: "success",
                        title: "ثبت شد",
                        text: "منو با موفقیت ایجاد شد.",
                        timer: 2000,
                        showConfirmButton: false
                    });
                }
                else {
                    $("#modal-form .create-error").html(res.message || "خطا در ثبت اطلاعات.");
                    $("#base-modal").scrollTop(0);
                }

            }).fail(function () {
                $("#modal-form .create-error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                $("#base-modal").scrollTop(0);
            });

            return false;
        }
    },



    // ===============================
    //  ویرایش منو
    // ===============================
    edit: {

        // لود کردن فرم ویرایش منو
        loadForm: function () {
            if (menu.selectedItem == null) {
                Swal.fire({
                    icon: "warning",
                    title: "توجه",
                    text: "منوی مورد نظر را انتخاب کنید!"
                });
                return;
            }

            var id = menu.selectedItem.id;

            $.get("/AuthSystem/" + controller + "/LoadEditForm/" + id, function (res) {
                $("#modal-form").html(res);

                const modalEl = document.getElementById('base-modal');
                const modal = new bootstrap.Modal(modalEl);
                modal.show();

                // گسترش پارامترها اگر وجود دارند
                menu.parameters.expand();

                // تولتیپ‌ها
                var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                tooltipTriggerList.map(function (tooltipTriggerEl) {
                    return new bootstrap.Tooltip(tooltipTriggerEl);
                });

                // اعمال ولیدیشن unobtrusive روی فرم ویرایش
                var form = $(".edit-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse(form);
            });
        },


        // ذخیره کردن فرم ویرایش
        save: function (e) {
            e.preventDefault();

            var $form = $(".edit-form");
            var targetUrl = $form.attr("action");

            // ولیدیت
            $form.validate();
            if (!$form.valid()) return false;

            // اعتبارسنجی اختصاصی لینک
            if (!menu.validate(".edit-form")) return false;

            // رندر پارامترها
            menu.parameters.render();

            var data = $form.serialize();

            $.post(targetUrl, data, function (res) {

                if (res.status) {
                    const modalEl = document.getElementById('base-modal');
                    const modalInstance = bootstrap.Modal.getInstance(modalEl);
                    modalInstance && modalInstance.hide();

                    menu.refreshList();
                    $(".edit-btn,.delete-btn").addClass("hide");
                    $(".create-btn").html("افزودن منو جدید");

                    Swal.fire({
                        icon: "success",
                        title: "ویرایش شد",
                        text: "منو با موفقیت ویرایش شد.",
                        timer: 2000,
                        showConfirmButton: false
                    });
                }
                else {
                    $("#modal-form .edit-error").html(res.message || "خطا در ویرایش اطلاعات.");
                    $("#base-modal").scrollTop(0);
                }

            }).fail(function () {
                $("#modal-form .edit-error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                $("#base-modal").scrollTop(0);
            });

            return false;
        }
    },



    // ===============================
    //  حذف منو
    // ===============================
    delete: {

        loadForm: function () {
            if (!menu.selectedItem) {
                Swal.fire({
                    icon: "warning",
                    title: "توجه",
                    text: "لطفاً ابتدا منو مورد نظر را انتخاب نمایید!"
                });
                return;
            }

            if (menu.selectedItem.nodes != null && menu.selectedItem.nodes.length > 0) {
                Swal.fire({
                    icon: "warning",
                    title: "توجه",
                    text: "لطفاً ابتدا زیرمنوهای این منو را حذف کنید."
                });
                return;
            }

            Swal.fire({
                title: 'آیا مطمئن هستید؟',
                text: "بعد از حذف، اطلاعات این منو قابل بازگشت نیست.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'بله، حذف شود',
                cancelButtonText: 'لغو',
                confirmButtonColor: '#d33',
                cancelButtonColor: '#6c757d'
            }).then(function (result) {
                if (result.isConfirmed) {
                    menu.delete.confirm();
                }
            });
        },

        confirm: function () {
            $.post("/AuthSystem/" + controller + "/Delete/" + menu.selectedItem.id, function (res) {

                if (res.status) {
                    menu.refreshList();
                    $(".edit-btn,.delete-btn").addClass("hide");
                    $(".create-btn").html("افزودن منو جدید");

                    Swal.fire({
                        icon: 'success',
                        title: 'حذف شد',
                        text: 'منو با موفقیت حذف شد.',
                        confirmButtonText: 'باشه'
                    });
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'حذف نشد',
                        text: res.message || 'خطایی رخ داده است.',
                        confirmButtonText: 'باشه'
                    });
                }

            }).fail(function () {
                Swal.fire({
                    icon: 'error',
                    title: 'حذف نشد',
                    text: 'لطفاً ابتدا زیرمجموعه‌های این منو را حذف کنید.',
                    confirmButtonText: 'باشه'
                });
            });
        }
    },



    // ===============================
    //  اعتبارسنجی لینک
    // ===============================
    validate: function (form) {
        var $form = $(form);

        if ($form.find('.HasLink').is(':checked')) {

            if (!$form.find('#Controller').val()) {
                $("#modal-form .error").html('کنترلر را وارد کنید!');
                $("#base-modal").scrollTop(0);
                return false;
            }
            if (!$form.find('#Action').val()) {
                $("#modal-form .error").html('اکشن را وارد کنید!');
                $("#base-modal").scrollTop(0);
                return false;
            }
        }
        return true;
    },



    // ===============================
    //  مدیریت پارامترهای آدرس
    // ===============================
    parameters: {

        // تولید HTML یک آیتم پارامتر
        item: function (index, key, value) {
            var keyStr = key ? ' value="' + key + '"' : "";
            var valStr = value ? ' value="' + value + '"' : "";

            var tmp =
                '<div class="col-md-12 parameter-item mb-2" rel="' + index + '">' +
                '  <div class="row g-2">' +

                '    <div class="col-md-4">' +
                '      <label class="form-label">کلید</label>' +
                '      <input name="key' + index + '" id="key' + index + '" class="form-control form-control-sm" ' + keyStr + ' />' +
                '    </div>' +

                '    <div class="col-md-4">' +
                '      <label class="form-label">مقدار</label>' +
                '      <input name="value' + index + '" id="value' + index + '" class="form-control form-control-sm" ' + valStr + ' />' +
                '    </div>' +

                '    <div class="col-md-4 d-flex align-items-end">' +
                '      <button type="button" class="btn btn-outline-danger btn-sm ms-2 delete" ' +
                '              onclick="menu.parameters.delete(this)" data-bs-toggle="tooltip" title="حذف پارامتر">' +
                '          <i class="bi bi-trash"></i>' +
                '      </button>' +
                '    </div>' +

                '  </div>' +
                '</div>';

            return tmp;
        },


        // افزودن پارامتر جدید
        add: function () {
            var index = 1;
            var $last = $('.parameters-section .parameter-item:last-of-type');

            if ($last.length > 0) {
                var rel = $last.attr('rel');
                index = parseInt(rel, 10) + 1;
            }

            $('.parameters-section .card-body').append(menu.parameters.item(index));

            var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        },


        // حذف یک پارامتر
        delete: function (el) {
            $(el).closest('.parameter-item').remove();
        },


        // جمع‌آوری پارامترها و تبدیل به JSON رشته‌ای
        render: function () {
            var parameters = {};
            var parameterItems = $('.parameters-section .parameter-item');

            $.each(parameterItems, function () {
                var index = $(this).attr('rel');
                var key = $(this).find('#key' + index).val().trim();
                var val = $(this).find('#value' + index).val().trim();

                if (key && val)
                    parameters[key] = val;
            });

            if (!$.isEmptyObject(parameters))
                $('#Parameters').val(JSON.stringify(parameters));
            else
                $('#Parameters').val('');
        },


        // تبدیل رشته JSON به آیتم‌های فرم هنگام ویرایش
        expand: function () {
            var parameters = $("#Parameters").val();

            if (!parameters) {
                menu.parameters.add();
                return;
            }

            var obj = {};
            try {
                obj = JSON.parse(parameters);
            } catch (e) {
                menu.parameters.add();
                return;
            }

            var index = 1;
            Object.keys(obj).forEach(function (key) {
                var value = obj[key];
                $('.parameters-section .card-body').append(menu.parameters.item(index, key, value));
                index++;
            });

            var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        }
    }
};


// اجرای اولیه
menu.refreshList();


// باز/بستن بخش لینک (همان قبلی)
var toggleSlide = function (el, target) {
    if (!$(el).is(":checked")) {
        $(target).slideUp();
    }
    else {
        $(target).slideDown();
    }
};
