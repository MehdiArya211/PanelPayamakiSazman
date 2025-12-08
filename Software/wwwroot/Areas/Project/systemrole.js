var systemrole = {

    list: {
        table: null,

        initial: function () {

            this.table = $('#datatables').DataTable({
                language: { url: "/assets/datatables/fa-lang.json" },

                ajax: {
                    url: "/Project/systemrole/GetList",
                    type: "POST"
                },

                columns: [
                    { data: "id" },
                    { data: "name" },
                    { data: "description" },
                    {
                        data: "isActive",
                        render: d => d
                            ? "<span class='text-success'>فعال</span>"
                            : "<span class='text-danger'>غیرفعال</span>"
                    },
                    {
                        data: null,
                        render: row => `


                                    <button onclick="systemrole.edit.loadForm('${row.id}')"
                                            class="btn btn-light action-btn"
                                            title="ویرایش">
                                        <i class="bi bi-pencil text-primary"></i>
                                    </button>

                            <button class="btn btn-light btn-sm"
                                onclick="systemrole.delete.confirm('${row.id}', '${row.name}')">
                                <i class="bi bi-trash text-danger"></i>
                            </button>
                        `
                    }
                ],

                serverSide: true,
                order: [0, "desc"]
            });
        },

        reload: function () {
            this.table.ajax.reload(null, false);
        }
    },

    create: {
        loadForm: function () {
            $.get("/Project/systemrole/LoadCreateForm", function (res) {
                $("#modal-form").html(res);
                new bootstrap.Modal(document.getElementById("base-modal")).show();
            });
        },

        save: function (e) {
            e.preventDefault();

            let form = $(".create-form");

            $.post(form.attr("action"), form.serialize())
                .done(res => {
                    if (res.status) {
                        systemrole.list.reload();
                        bootstrap.Modal.getInstance(document.getElementById("base-modal")).hide();
                        Swal.fire("ثبت شد", "نقش ایجاد شد.", "success");
                    }
                    else $(".error").html(res.message);
                });
        }
    },

    edit0: {

        loadForm: function (id) {
            $.get("/Project/systemrole/LoadEditForm", { id: id }, function (res) {
                $("#modal-form").html(res);
                new bootstrap.Modal(document.getElementById("base-modal")).show();
            });
        },

        save: function (e) {
            e.preventDefault();

            let form = $(".edit-form");

            $.ajax({
                url: form.attr("action"),
                type: "PUT",
                data: form.serialize(),
                success: function (res) {
                    if (res.status) {
                        systemrole.list.reload();
                        bootstrap.Modal.getInstance(document.getElementById("base-modal")).hide();
                        Swal.fire("ویرایش شد", "تغییرات ذخیره شد.", "success");
                    }
                    else $(".error").html(res.message);
                }
            });
        }
    },
    edit: {

        loadForm: function (id) {
            $.get("/Project/systemrole/LoadEditForm", { id: id }, function (res) {

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
                        systemrole.list.reload();

                        bootstrap.Modal.getInstance(document.getElementById("base-modal")).hide();

                        Swal.fire({
                            icon: "success",
                            title: "ویرایش شد",
                            text: "اطلاعات کاربر با موفقیت ویرایش شد."
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

        confirm: function (id, name) {
            Swal.fire({
                title: "حذف نقش",
                text: `حذف ${name} ؟`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله"
            }).then(r => {
                if (r.isConfirmed)
                    systemrole.delete.exec(id);
            });
        },

        exec: function (id) {
            $.post("/Project/systemrole/Delete", { id })
                .done(res => {
                    if (res.status) {
                        systemrole.list.reload();
                        Swal.fire("حذف شد", "عملیات با موفقیت انجام شد.", "success");
                    }
                    else Swal.fire("خطا", res.message, "error");
                });
        }
    }
};

$(document).ready(function () {
    systemrole.list.initial();
});
