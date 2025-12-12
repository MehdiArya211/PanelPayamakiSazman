var senderAssignment = {

    /* ============================
       LIST (DATATABLE)
    ============================ */
    list: {

        table: null,

        initial: function () {

            this.table = $('#assignments-table').DataTable({
                drawCallback: function () {
                    $('[data-toggle="tooltip"]').tooltip();
                },

                language: { url: "/assets/datatables/fa-lang.json" },
                pagingType: "full_numbers",
                responsive: true,

                ajax: {
                    url: "/Project/SenderNumberAssignment/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" },

                    {
                        data: null,
                        render: function (data, type, row) {
                            if (row.fullNumber) return row.fullNumber;
                            return row.senderNumberId || "";
                        }
                    },

                    {
                        data: null,
                        render: function (data, type, row) {
                            if (row.userFullName) return row.userFullName;
                            if (row.userName) return row.userName;
                            return row.userId || "";
                        }
                    },

                    {
                        data: "senderNumberStatus",
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
                                    return '<span class="badge bg-light text-dark">' + data + '</span>';
                            }
                        }
                    },

                    {
                        data: "status",
                        className: "text-center",
                        render: function (data) {
                            if (!data) return "";
                            switch (data) {
                                case "Active":
                                    return '<span class="badge bg-success">فعال</span>';
                                case "Revoked":
                                    return '<span class="badge bg-danger">لغو شده</span>';
                                default:
                                    return '<span class="badge bg-light text-dark">' + data + '</span>';
                            }
                        }
                    },

                    {
                        data: "assignedAt",
                        render: function (data) {
                            if (!data) return "";
                            return data;
                        }
                    },

                    {
                        data: "revokedAt",
                        render: function (data) {
                            if (!data) return "";
                            return data;
                        }
                    },

                    { data: "revocationReason" },

                    { data: "description" },

                    {
                        data: null,
                        className: "text-center",
                        orderable: false,
                        render: function (data, type, row) {

                            var canRevoke = row.status === "Active";

                            var editBtn = `
                                <button onclick="senderAssignment.edit.loadForm('${row.id}')"
                                        class="btn btn-light action-btn"
                                        data-toggle="tooltip"
                                        title="ویرایش توضیحات">
                                    <i class="bi bi-pencil text-primary"></i>
                                </button>`;

                            var revokeBtn = `
                                <button onclick="senderAssignment.revoke.open('${row.id}')"
                                        class="btn btn-light action-btn"
                                        data-toggle="tooltip"
                                        title="لغو تخصیص"
                                        ${canRevoke ? "" : "disabled"}>
                                    <i class="bi bi-x-circle text-danger"></i>
                                </button>`;

                            return `
                                <div class="d-flex justify-content-center gap-2">
                                    ${editBtn}
                                    ${revokeBtn}
                                </div>`;
                        }
                    }
                ],

                serverSide: true,
                order: [[0, "desc"]]
            });
        },

        reload: function () {
            if (senderAssignment.list.table) {
                senderAssignment.list.table.ajax.reload(null, false);
            }
        }
    },

    /* ============================
       CREATE
    ============================ */

    create: {

        loadForm: function () {

            $.get("/Project/SenderNumberAssignment/LoadCreateForm", function (res) {

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

            var form = $(".create-form");

            if ($.validator && $.validator.unobtrusive) {
                form.validate();
                if (!form.valid()) {
                    return false;
                }
            }

            $.post(form.attr("action"), form.serialize())
                .done(function (res) {

                    if (res.status) {

                        senderAssignment.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: res.message || "تخصیص با موفقیت ثبت شد."
                        });
                    }
                    else {
                        $(".create-form .error").html(res.message || "خطا در ذخیره اطلاعات.");
                    }
                })
                .fail(function () {
                    $(".create-form .error").html("خطا در ذخیره اطلاعات.");
                });

            return false;
        }
    },

    /* ============================
       EDIT
    ============================ */

    edit: {

        loadForm: function (id) {

            $.get("/Project/SenderNumberAssignment/LoadEditForm", { id: id }, function (res) {

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
                if (!form.valid()) return false;
            }

            $.ajax({
                url: form.attr("action"),
                type: "PUT",
                data: form.serialize(),
                success: function (res) {

                    if (res.status) {

                        senderAssignment.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: res.message || "ویرایش با موفقیت انجام شد."
                        });
                    }
                    else {
                        $(".edit-form .error").html(res.message || "خطا در ویرایش.");
                    }
                },
                error: function () {
                    $(".edit-form .error").html("خطا در ارتباط با سرور");
                }
            });

            return false;
        }
    },

    /* ============================
       REVOKE
    ============================ */

    revoke: {

        open: function (id) {

            Swal.fire({
                title: "لغو تخصیص سرشماره",
                input: "textarea",
                inputLabel: "علت لغو (اختیاری، ولی بهتر است پر شود)",
                inputPlaceholder: "علت لغو را بنویسید...",
                showCancelButton: true,
                confirmButtonText: "لغو تخصیص",
                confirmButtonColor: "#d33",
                cancelButtonText: "انصراف"
            }).then(function (result) {

                if (!result.isConfirmed) return;

                senderAssignment.revoke.exec(id, result.value);
            });
        },

        exec: function (id, reason) {

            $.ajax({
                url: "/Project/SenderNumberAssignment/Revoke",
                type: "PUT",
                data: {
                    id: id,
                    reason: reason
                },
                success: function (res) {

                    if (res.status) {

                        senderAssignment.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: res.message || "تخصیص با موفقیت لغو شد."
                        });
                    }
                    else {
                        Swal.fire({
                            icon: "error",
                            title: "خطا",
                            text: res.message || "خطا در لغو تخصیص."
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
        }
    }
};

$(document).ready(function () {
    senderAssignment.list.initial();
});
