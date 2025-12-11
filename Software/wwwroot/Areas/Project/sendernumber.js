var startSubmition = false;

var senderNumber = {

    /* ============================
       LIST (DATATABLE)
    ============================ */
    list: {

        table: null,

        initial: function () {

            this.table = $('#datatables').DataTable({
                drawCallback: function () {
                    $('[data-toggle="tooltip"]').tooltip();
                },

                language: { url: "/assets/datatables/fa-lang.json" },
                pagingType: "full_numbers",
                responsive: true,

                ajax: {
                    url: "/Project/SenderNumber/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" }, // برای sort داخلی

                    { data: "fixedPrefix" },

                    {
                        data: null,
                        render: function (data, type, row) {
                            var text = "";
                            if (row.specialtyCode) text += row.specialtyCode;
                            if (row.specialtyTitle) {
                                text += (text ? " - " : "") + row.specialtyTitle;
                            }
                            return text;
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            var text = "";
                            if (row.organizationLevelCode) text += row.organizationLevelCode;
                            if (row.organizationLevelTitle) {
                                text += (text ? " - " : "") + row.organizationLevelTitle;
                            }
                            return text;
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            var text = "";
                            if (row.subAreaCode) text += row.subAreaCode;
                            if (row.subAreaTitle) {
                                text += (text ? " - " : "") + row.subAreaTitle;
                            }
                            return text;
                        }
                    },

                    { data: "fullNumber" },

                    {
                        data: "status",
                        className: "text-center",
                        render: function (data) {
                            if (!data) return "";

                            switch (data) {
                                case "Purchasing":
                                    return '<span class="badge bg-warning text-dark">در حال خرید</span>';
                                case "CompletingDocuments":
                                    return '<span class="badge bg-info text-dark">تکمیل مدارک</span>';
                                case "ReviewingDocuments":
                                    return '<span class="badge bg-secondary">بررسی مدارک</span>';
                                case "Active":
                                    return '<span class="badge bg-success">فعال</span>';
                                case "Inactive":
                                    return '<span class="badge bg-dark">غیرفعال</span>';
                                default:
                                    // هر وضعیت جدیدی که اضافه شود، حداقل متن خامش نمایش داده شود
                                    return '<span class="badge bg-light text-dark">' + data + '</span>';
                            }
                        }
                    },

                    { data: "description" },

                    {
                        data: null,
                        className: "text-center",
                        orderable: false,
                        render: function (data, type, row) {
                            return `
            <div class="d-flex justify-content-center gap-2">

                <button onclick="senderNumber.edit.loadForm('${row.id}')"
                        class="btn btn-light action-btn"
                        data-toggle="tooltip"
                        title="ویرایش">
                    <i class="bi bi-pencil text-primary"></i>
                </button>

                <button onclick="senderNumber.status.openStatusDialog('${row.id}', '${row.status || ""}')"
                        class="btn btn-light action-btn"
                        data-toggle="tooltip"
                        title="تغییر وضعیت">
                    <i class="bi bi-arrow-repeat text-warning"></i>
                </button>

                <button onclick="senderNumber.delete.confirm('${row.id}', '${row.fullNumber || ""}')"
                        class="btn btn-light action-btn"
                        data-toggle="tooltip"
                        title="حذف">
                    <i class="bi bi-trash text-danger"></i>
                </button>

            </div>`;
                        }
                    }
                ],

                serverSide: true,
                order: [[0, "desc"]]
            });
        },

        reload: function () {
            if (senderNumber.list.table) {
                senderNumber.list.table.ajax.reload(null, false);
            }
        }
    },


    /* ============================
       CREATE
    ============================ */

    create: {

        loadForm: function () {

            $.get("/Project/SenderNumber/LoadCreateForm", function (res) {

                $("#modal-form").html(res);

                const modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                var form = $(".create-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(form);
                }
            });
        },

        save: function (e) {

            e.preventDefault();
            startSubmition = true;

            var form = $(".create-form");

            if ($.validator && $.validator.unobtrusive) {
                form.validate();
                if (!form.valid()) {
                    startSubmition = false;
                    return;
                }
            }

            $.post(form.attr("action"), form.serialize())
                .done(function (res) {

                    startSubmition = false;

                    if (res.status) {

                        senderNumber.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: "سرشماره با موفقیت ایجاد شد."
                        });
                    }
                    else {
                        $(".create-form .error").html(res.message || "خطا در ذخیره اطلاعات.");
                    }
                })
                .fail(function () {
                    startSubmition = false;
                    $(".create-form .error").html("خطا در ذخیره اطلاعات.");
                });
        }
    },


    /* ============================
       EDIT (LOAD + SAVE)
    ============================ */

    edit: {

        loadForm: function (id) {

            $.get("/Project/SenderNumber/LoadEditForm", { id: id }, function (res) {

                $("#modal-form").html(res);

                const modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                var form = $(".edit-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(form);
                }

            });
        },


        save: function (e) {

            e.preventDefault();

            var form = $(".edit-form");

            if ($.validator && $.validator.unobtrusive) {
                form.validate();
                if (!form.valid()) return;
            }

            $.ajax({
                url: form.attr("action"),
                type: "PUT",
                data: form.serialize(),
                success: function (res) {

                    if (res.status) {

                        senderNumber.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: "ویرایش با موفقیت انجام شد."
                        });
                    }
                    else {
                        $(".edit-form .error").html(res.message || "خطا در ویرایش اطلاعات.");
                    }
                },
                error: function () {
                    $(".edit-form .error").html("خطا در ارتباط با سرور");
                }
            });
        }

    },


    /* ============================
       DELETE (CONFIRM + EXEC)
    ============================ */

    delete: {

        confirm: function (id, fullNumber) {

            Swal.fire({
                title: "حذف سرشماره",
                text: fullNumber
                    ? `آیا از حذف سرشماره «${fullNumber}» مطمئن هستید؟`
                    : "آیا از حذف این سرشماره مطمئن هستید؟",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "انصراف"
            }).then(function (result) {

                if (result.isConfirmed) {
                    senderNumber.delete.exec(id);
                }
            });
        },

        exec: function (id) {

            $.post("/Project/SenderNumber/Delete", { id: id })

                .done(function (res) {

                    if (res.status) {

                        senderNumber.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "حذف شد",
                            text: "سرشماره با موفقیت حذف شد."
                        });
                    }
                    else {
                        Swal.fire({
                            icon: "error",
                            title: "خطا",
                            text: res.message || "خطا در حذف سرشماره."
                        });
                    }
                })

                .fail(function () {
                    Swal.fire({
                        icon: "error",
                        title: "خطا",
                        text: "در ارتباط با سرور خطایی رخ داد."
                    });
                });
        }
    },

    status: {

        openStatusDialog: function (id, currentStatus) {

            Swal.fire({
                title: "تغییر وضعیت سرشماره",
                input: "select",
                inputOptions: {
                    "Purchasing": "در حال خرید",
                    "CompletingDocuments": "تکمیل مدارک",
                    "ReviewingDocuments": "بررسی مدارک",
                    "Active": "فعال",
                    "Inactive": "غیرفعال"
                },
                inputValue: currentStatus || "Purchasing",
                showCancelButton: true,
                confirmButtonText: "ثبت",
                cancelButtonText: "انصراف",
                inputValidator: function (value) {
                    if (!value) {
                        return "لطفاً یک وضعیت انتخاب کنید.";
                    }
                }
            }).then(function (result) {

                if (!result.isConfirmed) return;

                $.ajax({
                    url: "/Project/SenderNumber/ChangeStatus",
                    type: "PUT",
                    data: {
                        id: id,
                        status: result.value
                    },
                    success: function (res) {

                        if (res.status) {
                            senderNumber.list.reload();

                            Swal.fire({
                                icon: "success",
                                title: "موفق",
                                text: "وضعیت سرشماره با موفقیت تغییر یافت."
                            });
                        } else {
                            Swal.fire({
                                icon: "error",
                                title: "خطا",
                                text: res.message || "خطا در تغییر وضعیت سرشماره."
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: "error",
                            title: "خطا",
                            text: "در ارتباط با سرور خطایی رخ داد."
                        });
                    }
                });
            });
        }
    }

};


$(document).ready(function () {
    senderNumber.list.initial();
});
