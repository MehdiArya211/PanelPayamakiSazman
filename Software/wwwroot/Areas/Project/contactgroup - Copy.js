var startSubmition = false;

var contactGroup = {

    /* ============================
       GROUPS LIST (DATATABLE)
    ============================ */
    list: {

        table: null,

        initial: function () {

            this.table = $('#datatables').DataTable({
                "drawCallback": function () {
                    $('[data-toggle="tooltip"]').tooltip();
                },

                language: { url: "/assets/datatables/fa-lang.json" },
                pagingType: "full_numbers",
                responsive: true,

                ajax: {
                    url: "/Project/ContactGroup/GetGroups",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" },
                    { data: "name" },
                    { data: "description" },
                    {
                        data: "contactCount",
                        className: "text-center",
                        render: function (d) {
                            return (d ?? 0).toLocaleString("fa-IR");
                        }
                    },
                    {
                        data: null,
                        className: "text-center",
                        render: function (data, type, row) {
                            return `
                                <div class="d-flex justify-content-center gap-2 flex-wrap">

                                    <button onclick="contactGroup.contacts.loadModal('${row.id}', '${row.name}')"
                                            class="btn btn-light action-btn"
                                            title="مدیریت مخاطبین">
                                        <i class="bi bi-person-lines-fill text-success"></i>
                                    </button>

                                    <button onclick="contactGroup.edit.loadForm('${row.id}')"
                                            class="btn btn-light action-btn"
                                            title="ویرایش">
                                        <i class="bi bi-pencil text-primary"></i>
                                    </button>

                                    <button onclick="contactGroup.delete.confirm('${row.id}', '${row.name}')"
                                            class="btn btn-light action-btn"
                                            title="حذف">
                                        <i class="bi bi-trash text-danger"></i>
                                    </button>

                                </div>`;
                        }
                    }
                ],

                serverSide: true,
                order: [0, "desc"]
            });
        },

        reload: function () {
            contactGroup.list.table.ajax.reload(null, false);
        }
    },

    /* ============================
       CREATE
    ============================ */
    create: {

        loadForm: function () {

            $.get("/Project/ContactGroup/LoadCreateForm", function (res) {

                $("#modal-form").html(res);

                const modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                var form = $(".create-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                $.validator.unobtrusive.parse(form);
            });
        },

        save: function (e) {

            e.preventDefault();
            if (startSubmition) return false;
            startSubmition = true;

            var form = $(".create-form");

            form.validate();
            if (!form.valid()) {
                startSubmition = false;
                return false;
            }

            $.post(form.attr("action"), form.serialize())
                .done(res => {

                    startSubmition = false;

                    if (res.status) {

                        contactGroup.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: res.message || "گروه ایجاد شد."
                        });
                    }
                    else {
                        $(".create-form .error").html(res.message);
                    }
                })
                .fail(() => {
                    startSubmition = false;
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

            $.get("/Project/ContactGroup/LoadEditForm", { id: id }, function (res) {

                $("#modal-form").html(res);

                const modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                var form = $(".edit-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                $.validator.unobtrusive.parse(form);

            });
        },

        save: function (e) {

            e.preventDefault();

            var form = $(".edit-form");

            form.validate();
            if (!form.valid()) return false;

            $.ajax({
                url: form.attr("action"),
                type: "PUT",
                data: form.serialize(),
                success: function (res) {

                    if (res.status) {

                        contactGroup.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: res.message || "ویرایش انجام شد."
                        });
                    }
                    else {
                        $(".edit-form .error").html(res.message);
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
       DELETE
    ============================ */
    delete: {

        confirm: function (id, title) {

            Swal.fire({
                title: "حذف گروه",
                text: `آیا از حذف «${title}» مطمئن هستید؟`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "انصراف"
            }).then(result => {

                if (result.isConfirmed) {
                    contactGroup.delete.exec(id);
                }
            });
        },

        exec: function (id) {

            $.post("/Project/ContactGroup/Delete", { id: id })

                .done(res => {

                    if (res.status) {

                        contactGroup.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "حذف شد",
                            text: res.message || "گروه حذف شد."
                        });
                    }
                    else {
                        Swal.fire({
                            icon: "error",
                            title: "خطا",
                            text: res.message
                        });
                    }
                })

                .fail(() => {
                    Swal.fire({
                        icon: "error",
                        title: "خطا",
                        text: "در ارتباط با سرور خطایی رخ داد."
                    });
                });
        }
    },

    /* ============================================================
       CONTACTS (MODAL + DATATABLE + FORMS)
    ============================================================ */
    contacts: {

        loadModal: function (groupId, groupName) {

            $.get("/Project/ContactGroup/LoadContactsModal",
                { groupId: groupId, groupName: groupName },
                function (res) {

                    $("#modal-form").html(res);

                    const modal = new bootstrap.Modal(document.getElementById("base-modal"));
                    modal.show();

                    // اعتبارسنجی فرم‌ها
                    $(".add-single-form, .add-text-form, .csv-form").each(function () {
                        var f = $(this)
                            .removeData("validator")
                            .removeData("unobtrusiveValidation");

                        $.validator.unobtrusive.parse(f);
                    });
                });
        },

        list: {

            table: null,

            initial: function (groupId) {

                // اگر قبلاً ساخته شده بود، نابودش کن
                if (contactGroup.contacts.list.table) {
                    contactGroup.contacts.list.table.destroy();
                    contactGroup.contacts.list.table = null;
                }

                contactGroup.contacts.list.table = $('#contactsTable').DataTable({
                    "drawCallback": function () {
                        $('[data-toggle="tooltip"]').tooltip();
                    },

                    language: { url: "/assets/datatables/fa-lang.json" },
                    pagingType: "full_numbers",
                    responsive: true,

                    ajax: {
                        url: "/Project/ContactGroup/GetContacts",
                        type: "POST",
                        dataType: "json",
                        data: function (d) {
                            d.groupId = groupId;
                        }
                    },

                    columns: [
                        { data: "id" },
                        { data: "phoneNumber" },
                        { data: "firstName" },
                        { data: "lastName" },
                        { data: "nationalCode" },
                        { data: "birthDate" },
                        {
                            data: "customFields",
                            render: function (data) {
                                if (!data || !data.length) return "";
                                return data.map(x => `${x.key}:${x.value}`).join(" | ");
                            }
                        }
                    ],

                    serverSide: true,
                    order: [0, "desc"]
                });
            },

            reload: function () {
                if (contactGroup.contacts.list.table)
                    contactGroup.contacts.list.table.ajax.reload(null, false);

                // تعداد مخاطب گروه هم ممکنه تغییر کنه
                contactGroup.list.reload();
            }
        },

        addSingle: {

            save: function (e) {

                e.preventDefault();

                var form = $(".add-single-form");

                form.validate();
                if (!form.valid()) return false;

                $.post(form.attr("action"), form.serialize())
                    .done(res => {
                        if (res.status) {
                            form[0].reset();
                            $(".error-single").html("");

                            contactGroup.contacts.list.reload();

                            Swal.fire({
                                icon: "success",
                                title: "موفق",
                                text: res.message || "مخاطب اضافه شد."
                            });
                        } else {
                            $(".error-single").html(res.message);
                        }
                    })
                    .fail(() => {
                        $(".error-single").html("خطا در ارتباط با سرور");
                    });

                return false;
            }
        },

        addText: {

            save: function (e) {

                e.preventDefault();

                var form = $(".add-text-form");

                form.validate();
                if (!form.valid()) return false;

                $.post(form.attr("action"), form.serialize())
                    .done(res => {
                        if (res.status) {
                            form.find("textarea").val("");
                            $(".error-text").html("");

                            contactGroup.contacts.list.reload();

                            Swal.fire({
                                icon: "success",
                                title: "موفق",
                                text: res.message || "شماره‌ها ثبت شدند."
                            });
                        } else {
                            $(".error-text").html(res.message);
                        }
                    })
                    .fail(() => {
                        $(".error-text").html("خطا در ارتباط با سرور");
                    });

                return false;
            }
        },

        csv: {

            save: function (e) {

                e.preventDefault();

                var form = $(".csv-form")[0];
                var fd = new FormData(form);

                $.ajax({
                    url: $(form).attr("action"),
                    type: "POST",
                    data: fd,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        if (res.status) {
                            form.reset();
                            $(".error-csv").html("");

                            contactGroup.contacts.list.reload();

                            Swal.fire({
                                icon: "success",
                                title: "موفق",
                                text: res.message || "فایل پردازش شد."
                            });
                        } else {
                            $(".error-csv").html(res.message);
                        }
                    },
                    error: function () {
                        $(".error-csv").html("خطا در ارتباط با سرور");
                    }
                });

                return false;
            }
        },

        deduplicate: function (groupId) {

            Swal.fire({
                title: "حذف تکراری‌ها",
                text: "شماره‌های تکراری این گروه حذف شود؟",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله",
                cancelButtonText: "انصراف"
            }).then(r => {

                if (!r.isConfirmed) return;

                $.post("/Project/ContactGroup/Deduplicate", { groupId: groupId })
                    .done(res => {
                        if (res.status) {
                            contactGroup.contacts.list.reload();
                            Swal.fire({
                                icon: "success",
                                title: "موفق",
                                text: res.message || "تکراری‌ها حذف شدند."
                            });
                        } else {
                            Swal.fire({
                                icon: "error",
                                title: "خطا",
                                text: res.message
                            });
                        }
                    })
                    .fail(() => {
                        Swal.fire({
                            icon: "error",
                            title: "خطا",
                            text: "خطا در ارتباط با سرور"
                        });
                    });
            });
        }
    }
};

$(document).ready(function () {
    contactGroup.list.initial();
});
