var route = {

    list: {

        table: null,

        initial: function () {

            this.table = $('#datatables').DataTable({

                language: { url: "/assets/datatables/fa-lang.json" },
                serverSide: true,

                ajax: {
                    url: "/Project/RouteDefinition/GetList",
                    type: "POST"
                },

                columns: [

                    { data: "routeKey" },

                    {
                        data: "actions",
                        render: d => d ? d.join(", ") : ""
                    },

                    {
                        data: "requiresFreshAuth",
                        render: d =>
                            d
                                ? '<span class="badge bg-success">بله</span>'
                                : '<span class="badge bg-secondary">خیر</span>'
                    },

                    {
                        data: null,
                        render: row => `
                            <div class="d-flex gap-2 justify-content-center">

                                <button class="btn btn-light"
                                        onclick="route.edit.loadForm('${row.routeKey}')">
                                    <i class="bi bi-pencil text-primary"></i>
                                </button>

                                <button class="btn btn-light"
                                        onclick="route.delete.confirm('${row.routeKey}')">
                                    <i class="bi bi-trash text-danger"></i>
                                </button>

                            </div>`
                    }
                ]
            });
        },

        reload: function () {
            route.list.table.ajax.reload(null, false);
        }
    },

    edit: {

        loadForm: function (routeKey) {

            $.get("/Project/RouteDefinition/LoadEditForm",
                { routeKey: routeKey },
                function (res) {

                    $("#modal-form").html(res);

                    const modal =
                        new bootstrap.Modal(document.getElementById("base-modal"));
                    modal.show();
                });
        },

        save: function (e) {

            e.preventDefault();

            var form = $(".edit-form");

            $.ajax({
                url: form.attr("action"),
                type: "PUT",
                data: form.serialize(),
                success: function (res) {

                    if (res.status) {

                        route.list.reload();

                        bootstrap.Modal
                            .getInstance(document.getElementById("base-modal"))
                            .hide();

                        Swal.fire("موفق", "ویرایش انجام شد", "success");
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

        confirm: function (routeKey) {

            Swal.fire({
                title: "حذف Route",
                text: `آیا از حذف ${routeKey} مطمئن هستید؟`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله",
                cancelButtonText: "انصراف"
            }).then(r => {
                if (r.isConfirmed)
                    route.delete.exec(routeKey);
            });
        },

        exec: function (routeKey) {

            $.post("/Project/RouteDefinition/Delete",
                { routeKey: routeKey },
                function (res) {

                    if (res.status) {
                        route.list.reload();
                        Swal.fire("حذف شد", "", "success");
                    }
                    else {
                        Swal.fire("خطا", res.message, "error");
                    }
                });
        }
    }
};

$(document).ready(function () {
    route.list.initial();
});
