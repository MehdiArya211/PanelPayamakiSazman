/* ============================================================
   User Management – FINAL Version (Bootstrap 5 Dashboard)
   Developer: ChatGPT for مهدي آریا ❤️
============================================================ */

var startSubmition = false;

var user = {

    /* ============================================================
       لیست کاربران – DataTable
    ============================================================ */
    list: {

        table: null,

        initial: function () {

            this.table = $('#datatables').DataTable({
                language: { url: "/assets/datatables/fa-lang.json" },
                processing: true,
                serverSide: true,
                responsive: true,
                pagingType: "full_numbers",

                ajax: {
                    url: "/AuthSystem/Users/GetList",
                    type: "POST",
                    data: function (d) {
                        return $.extend({}, d, filter.collect());
                    }
                },

                columns: [
                    { data: "id" },
                    { data: "fullName" },
                    { data: "username" },
                    { data: "mobile" },
                    { data: "role" },

                    {
                        data: "isEnabled",
                        render: (data, type, row) => `
                            <div class="form-check form-switch text-center">
                                <input class="form-check-input"
                                       type="checkbox"
                                       onchange="user.toggleEnable(this, ${row.id})"
                                       ${data ? "checked" : ""}>
                            </div>`
                    },

                    {
                        data: null,
                        className: "text-center",
                        render: (data, type, row) => `
                            <button class="action-btn edit" onclick="user.edit.loadForm(${row.id})">
                                <i class="bi bi-pencil-square"></i>
                            </button>

                            <button class="action-btn delete" onclick="user.delete.loadForm(${row.id})">
                                <i class="bi bi-trash"></i>
                            </button>`
                    }
                ],

                columnDefs: [
                    { orderable: false, targets: [1, 2, 3, 4, 5, 6] }
                ],

                order: [[0, "desc"]]
            });
        },

        reload: function () {
            user.list.table.ajax.reload(null, false);
        }
    },


    /* ============================================================
       فرم‌ها (Create/Edit)
    ============================================================ */
    form: { initial: function () {} },


    /* ============================================================
       ایجاد کاربر
    ============================================================ */
    create: {

        loadForm: function () {

            $.get("/AuthSystem/users/LoadCreateForm", res => {

                $("#modal-form").html(res);
                const modal = new bootstrap.Modal(document.getElementById('base-modal'));
                modal.show();
            });
        },

        save: function (e) {
            e.preventDefault();
            if (startSubmition) return;

            startSubmition = true;

            var form = $(".create-form");

            form.validate();

            if (!form.valid()) {
                startSubmition = false;
                return;
            }

            $.post(form.attr("action"), form.serialize(), res => {

                startSubmition = false;

                if (res.status) {

                    // پیام موفقیت
                    Swal.fire({
                        icon: "success",
                        title: "عملیات موفق",
                        text: "کاربر با موفقیت ایجاد شد.",
                        showConfirmButton: false,
                        timer: 2000
                    });

                    user.list.reload();
                    bootstrap.Modal.getInstance(document.getElementById('base-modal')).hide();
                }
                else {
                    $(".create-form .error").html(res.message);
                }

            }).fail(() => {
                startSubmition = false;
                $(".create-form .error").html("خطا در ذخیره اطلاعات.");
            });
        }
    },



    /* ============================================================
       ویرایش کاربر
    ============================================================ */
    edit: {

        loadForm: function (id) {

            $.get("/AuthSystem/users/LoadEditForm/" + id, res => {

                $("#modal-form").html(res);
                new bootstrap.Modal(document.getElementById('base-modal')).show();
            });
        },

        save: function () {

            var form = $(".edit-form");
            form.validate();

            if (!form.valid()) return;

            $.post(form.attr("action"), form.serialize(), res => {

                if (res.status) {

                    // پیام موفقیت ویرایش
                    Swal.fire({
                        icon: "success",
                        title: "ویرایش موفق",
                        text: "اطلاعات کاربر با موفقیت ویرایش شد.",
                        showConfirmButton: false,
                        timer: 2000
                    });

                    user.list.reload();
                    bootstrap.Modal.getInstance(document.getElementById('base-modal')).hide();
                }
                else {
                    $(".edit-form .error").html(res.message);
                }
            });
        }
    },



    /* ============================================================
       فعال/غیرفعال کردن کاربر
    ============================================================ */
    toggleEnable: function (el, id) {

        $.post('/AuthSystem/users/ToggleEnable/' + id, res => {

            if (!res.status) {
                Swal.fire("خطا", "امکان تغییر وضعیت وجود ندارد.", "error");
                $(el).prop("checked", !$(el).prop("checked"));
            }
        });
    },



    /* ============================================================
       حذف کاربر
    ============================================================ */
    delete: {

        loadForm: function (id) {

            Swal.fire({
                title: "حذف کاربر؟",
                text: "این عملیات غیرقابل بازگشت است!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonText: "لغو",
                confirmButtonText: "بله، حذف شود"
            }).then(result => {
                if (result.isConfirmed) user.delete.confirm(id);
            });
        },

        confirm: function (id) {

            $.post("/AuthSystem/users/Delete/" + id, res => {

                if (res.status) {

                    user.list.reload();

                    // پیام موفقیت حذف
                    Swal.fire({
                        icon: "success",
                        title: "حذف شد",
                        text: "کاربر با موفقیت حذف گردید.",
                        showConfirmButton: false,
                        timer: 2000
                    });

                } else {
                    Swal.fire("خطا", res.message, "error");
                }

            }).fail(() => {
                Swal.fire("خطا", "حذف کاربر با خطا مواجه شد.", "error");
            });
        }
    }
};




/* ============================================================
   فیلترها
============================================================ */
var filter = {

    initial: function () {
        filter.startDate.initial();
        filter.endDate.initial();
    },

    collect: function () {
        return {
            StartDate: $("input[name=FilterStartDate]").val(),
            EndDate: $("input[name=FilterEndDate]").val(),
            Name: $("#FilterName").val(),
            Username: $("#FilterUsername").val(),
            Mobile: $("#FilterMobile").val(),
            RoleId: $("#FilterRoleId").val(),
            IsEnabled: $("#FilterIsEnabled").val()
        };
    },

    startDate: {
        initial: function () {
            if (!$("#VueStartDate").length) return;

            new Vue({
                el: '#VueStartDate',
                data: { date: '' },
                components: { DatePicker: VuePersianDatetimePicker }
            });
        }
    },

    endDate: {
        initial: function () {
            if (!$("#VueEndDate").length) return;

            new Vue({
                el: '#VueEndDate',
                data: { date: '' },
                components: { DatePicker: VuePersianDatetimePicker }
            });
        }
    }
};




/* ============================================================
   Document Ready
============================================================ */
$(document).ready(function () {
    user.list.initial();
    filter.initial();
});
