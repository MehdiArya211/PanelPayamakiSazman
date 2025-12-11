var startSubmition = false;
var unit = {

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
                    url: "/Project/Unit/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" },         // برای sort و استفاده داخلی

                    { data: "code" },
                    { data: "name" },
                    { data: "description" },

                    {
                        data: "isActive",
                        render: function (data, type, row) {
                            if (data)
                                return '<span class="badge bg-success">فعال</span>';
                            return '<span class="badge bg-secondary">غیرفعال</span>';
                        },
                        className: "text-center"
                    },

                    {
                        data: null,
                        className: "text-center",
                        render: function (data, type, row) {
                            return `
                                        <div class="d-flex justify-content-center gap-2">

                                            <button onclick="unit.edit.loadForm('${row.id}')"
                                                    class="btn btn-light action-btn"
                                                    data-toggle="tooltip"
                                                    title="ویرایش">
                                                <i class="bi bi-pencil text-primary"></i>
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
            unit.list.table.ajax.reload(null, false);
        }
    },


    /* ============================
       CREATE
    ============================ */

    create: {

        loadForm: function () {

            $.get("/Project/Unit/LoadCreateForm", function (res) {

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

                        unit.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: "واحد با موفقیت ایجاد شد."
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

            $.get("/Project/Unit/LoadEditForm", { id: id }, function (res) {

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

            var token = form.find('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: form.attr("action"),
                type: "PUT", // دقت کن: اکشن کنترلر باید [HttpPut] باشد
                data: form.serialize(),
                success: function (res) {

                    if (res.status) {

                        unit.list.reload();

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

    }
};

$(document).ready(function () {
    unit.list.initial();
});
