var startSubmition = false;

var sendnumberspeciality = {

    /* ============================
       LIST (DATATABLE)
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
                    url: "/Project/SenderNumberSpeciality/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id"},

                    { data: "code" },
                    { data: "title" },
                    { data: "description" },

                    {
                        data: null,
                        className: "text-left",
                        render: function (data, type, row) {
                            return `
                <div class="d-flex justify-content-center gap-2">

                    <button onclick="sendnumberspeciality.edit.loadForm('${row.id}')"
                            class="btn btn-light action-btn"
                            title="ویرایش">
                        <i class="bi bi-pencil text-primary"></i>
                    </button>

                    <button onclick="sendnumberspeciality.delete.confirm('${row.id}', '${row.title}')"
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
            sendnumberspeciality.list.table.ajax.reload(null, false);
        }
    },


    /* ============================
       CREATE
    ============================ */

    create: {

        loadForm: function () {

            $.get("/Project/SenderNumberSpeciality/LoadCreateForm", function (res) {

                $("#modal-form").html(res);

                const modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                // فعال‌سازی دوباره اعتبارسنجی
                var form = $(".create-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                $.validator.unobtrusive.parse(form);
            });
        },

        save: function (e) {

            e.preventDefault();
            startSubmition = true;

            var form = $(".create-form");

            form.validate();
            if (!form.valid()) {
                startSubmition = false;
                return;
            }

            $.post(form.attr("action"), form.serialize())
                .done(res => {

                    startSubmition = false;

                    if (res.status) {

                        sendnumberspeciality.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: "با موفقیت ایجاد شد."
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
        }
    },


    /* ============================
       EDIT (LOAD + SAVE)
    ============================ */

    edit: {

        loadForm: function (id) {

            $.get("/Project/SenderNumberSpeciality/LoadEditForm", { id: id }, function (res) {

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
            if (!form.valid()) return;

            // گرفتن آنتی فورجری توکن
            var token = form.find('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: form.attr("action"),
                type: "PUT",
                data: form.serialize(), // شامل توکن هم هست
                success: function (res) {

                    if (res.status) {

                        sendnumberspeciality.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: "ویرایش با موفقیت انجام شد."
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
        }

    },


    /* ============================
       DELETE (CONFIRM + EXEC)
    ============================ */

    delete: {

        confirm: function (id, title) {

            Swal.fire({
                title: "حذف آیتم",
                text: `آیا از حذف «${title}» مطمئن هستید؟`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "انصراف"
            }).then(result => {

                if (result.isConfirmed) {
                    sendnumberspeciality.delete.exec(id);
                }
            });
        },

        exec: function (id) {

            $.post("/Project/SenderNumberSpeciality/Delete", { id: id })

                .done(res => {

                    if (res.status) {

                        sendnumberspeciality.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "حذف شد",
                            text: "آیتم با موفقیت حذف شد."
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
    }
};


/* ============================
   EXECUTE ON PAGE LOAD
============================ */
$(document).ready(function () {
    sendnumberspeciality.list.initial();
});
