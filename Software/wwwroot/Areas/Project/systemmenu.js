var systemmenu = {

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
                    url: "/Project/SystemMenu/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" },
                    { data: "key" },
                    { data: "title" },
                    { data: "order" },
                    { data: "parentTitle" },
                    {
                        data: "isActive",
                        render: function (val) {
                            return val
                                ? "<span class='text-success'>فعال</span>"
                                : "<span class='text-danger'>غیرفعال</span>";
                        }
                    },
                    {
                        data: null,
                        className: "text-center",
                        render: function (data, type, row) {

                            return `
                                <button class="btn btn-light action-btn"
                                    onclick="systemmenu.edit.loadForm('${row.id}')">
                                    <i class="bi bi-pencil text-primary"></i>
                                </button>

                                <button class="btn btn-light action-btn"
                                    onclick="systemmenu.delete.confirm('${row.id}', '${row.title}')">
                                    <i class="bi bi-trash text-danger"></i>
                                </button>
                            `;
                        }
                    }
                ],

                serverSide: true,
                order: [0, "desc"]
            });
        },

        reload: function () {
            systemmenu.list.table.ajax.reload(null, false);
        }
    },

    create: {

        loadForm: function () {

            $.get("/Project/SystemMenu/LoadCreateForm", function (res) {

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

                        systemmenu.list.reload();
                        bootstrap.Modal.getInstance(document.getElementById("base-modal")).hide();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: "منو با موفقیت ایجاد شد."
                        });
                    }
                    else {
                        $(".create-form .error").html(res.message);
                    }
                })
                .fail(() => $(".create-form .error").html("خطا در ارتباط با سرور"));
        }
    },

    edit: {

        loadForm: function (id) {

            $.get("/Project/SystemMenu/LoadEditForm", { id: id }, function (res) {

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
                type: "PUT",
                data: form.serialize(),
                success: function (res) {

                    if (res.status) {

                        systemmenu.list.reload();
                        bootstrap.Modal.getInstance(document.getElementById("base-modal")).hide();

                        Swal.fire({
                            icon: "success",
                            title: "ویرایش شد",
                            text: "تغییرات با موفقیت ذخیره شد."
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
                title: "حذف منو",
                text: `آیا از حذف «${title}» مطمئن هستید؟`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "انصراف"
            }).then(result => {

                if (result.isConfirmed) {
                    systemmenu.delete.exec(id);
                }
            });
        },

        exec: function (id) {

            $.post("/Project/SystemMenu/Delete", { id: id })
                .done(res => {

                    if (res.status) {

                        systemmenu.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "حذف شد",
                            text: "منو با موفقیت حذف شد."
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
                .fail(() => Swal.fire({
                    icon: "error",
                    title: "خطا",
                    text: "در ارتباط با سرور خطایی رخ داد."
                }));
        }
    }
};

$(document).ready(function () {
    systemmenu.list.initial();
});
