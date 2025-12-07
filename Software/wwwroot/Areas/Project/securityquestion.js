var securityQuestion = {

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
                    url: "/Project/SecurityQuestion/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" },
                    { data: "text" },
                    {
                        data: null,
                        className: "text-center",
                        render: function (data, type, row) {
                            return `
                            <div class="d-flex justify-content-center gap-2">

                                <button onclick="securityQuestion.edit.loadForm('${row.id}')"
                                        class="btn btn-light action-btn"
                                        title="ویرایش">
                                    <i class="bi bi-pencil text-primary"></i>
                                </button>

                                <button onclick="securityQuestion.delete.confirm('${row.id}', '${row.title}')"
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
            securityQuestion.list.table.ajax.reload(null, false);
        }
    },

    create: {

        loadForm: function () {

            $.get("/Project/SecurityQuestion/LoadCreateForm", function (res) {

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

            var form = $(".create-form");

            form.validate();
            if (!form.valid()) return;

            $.post(form.attr("action"), form.serialize())
                .done(res => {

                    if (res.status) {
                        securityQuestion.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: "سوال امنیتی با موفقیت ایجاد شد."
                        });
                    }
                    else {
                        $(".create-form .error").html(res.message);
                    }
                })
                .fail(() => {
                    $(".create-form .error").html("خطا در ذخیره اطلاعات.");
                });
        }
    },

    edit: {

        loadForm: function (id) {

            $.get("/Project/SecurityQuestion/LoadEditForm", { id: id }, function (res) {

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

            // آنتی‌فورجری توکن
            var token = form.find('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: form.attr("action"),
                type: "PUT",
                data: form.serialize(),
                success: function (res) {

                    if (res.status) {
                        securityQuestion.list.reload();

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

    delete: {

        confirm: function (id, title) {

            Swal.fire({
                title: "حذف سوال",
                text: `آیا از حذف «${title}» مطمئن هستید؟`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "انصراف"
            }).then(result => {

                if (result.isConfirmed) {
                    securityQuestion.delete.exec(id);
                }
            });
        },

        exec: function (id) {

            $.post("/Project/SecurityQuestion/Delete", { id: id })

                .done(res => {

                    if (res.status) {
                        securityQuestion.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "حذف شد",
                            text: "سوال با موفقیت حذف شد."
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
    securityQuestion.list.initial();

    $("#btnFilter").on("click", function () {
        securityQuestion.list.reload();
    });

    $("#btnResetFilter").on("click", function () {
        $("#filter_title").val("");
        securityQuestion.list.reload();
    });
});
