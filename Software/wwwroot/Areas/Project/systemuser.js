var systemuser = {

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
                    url: "/Project/SystemUser/GetList",
                    type: "POST",
                    dataType: "json",
                    data: function (d) {

                        d.extra_filter_username = $("#filter_username").val();
                        d.extra_filter_fullname = $("#filter_fullname").val();
                        d.extra_filter_mobile = $("#filter_mobile").val();
                    }
                },

                columns: [
                    { data: "id" },
                    { data: "userName" },
                    { data: "fullName" },
                    { data: "mobileNumber" },
                    { data: "nationalCode" },
                    {
                        data: "isActive",
                        render: function (val) {
                            return val ? "<span class='text-success'>فعال</span>"
                                : "<span class='text-danger'>غیرفعال</span>";
                        }
                    },
                    {
                        data: null,
                        className: "text-center",
                        render: function (data, type, row) {
                            return `
                                <div class="d-flex justify-content-center gap-2">

                                    <button onclick="systemuser.edit.loadForm('${row.id}')"
                                            class="btn btn-light action-btn"
                                            title="ویرایش">
                                        <i class="bi bi-pencil text-primary"></i>
                                    </button>

                                    <button onclick="systemuser.delete.confirm('${row.id}', '${row.fullName}')"
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
            systemuser.list.table.ajax.reload(null, false);
        }
    },

    create: {

        loadForm: function () {
            $.get("/Project/SystemUser/LoadCreateForm", function (res) {
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
                        systemuser.list.reload();
                        bootstrap.Modal.getInstance(document.getElementById("base-modal")).hide();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: "کاربر با موفقیت ایجاد شد."
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
            $.get("/Project/SystemUser/LoadEditForm", { id: id }, function (res) {

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
                        systemuser.list.reload();

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
                title: "حذف کاربر",
                text: `آیا از حذف «${name}» مطمئن هستید؟`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "انصراف"
            }).then(result => {

                if (result.isConfirmed) {
                    systemuser.delete.exec(id);
                }
            });
        },

        exec: function (id) {

            $.post("/Project/SystemUser/Delete", { id: id })

                .done(res => {
                    if (res.status) {
                        systemuser.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "حذف شد",
                            text: "کاربر با موفقیت حذف شد."
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
    ,
    password: {

        toggle: function (inputId, btn) {
            const input = document.getElementById(inputId);
            const icon = btn.querySelector('i');

            if (!input) return;

            if (input.type === 'password') {
                input.type = 'text';
                icon.classList.remove('bi-eye');
                icon.classList.add('bi-eye-slash');
            } else {
                input.type = 'password';
                icon.classList.remove('bi-eye-slash');
                icon.classList.add('bi-eye');
            }
        },

        checkStrength: function (password) {

            let strength = 0;

            if (password.length >= 8) strength++;
            if (/[a-z]/.test(password)) strength++;
            if (/[A-Z]/.test(password)) strength++;
            if (/[0-9]/.test(password)) strength++;
            if (/[^A-Za-z0-9]/.test(password)) strength++;

            const bar = document.getElementById('passwordStrengthBar');
            const text = document.getElementById('passwordStrengthText');

            if (!bar || !text) return;

            let percent = 0;
            let color = '';
            let message = '';

            switch (strength) {
                case 0:
                case 1:
                    percent = 20;
                    color = 'bg-danger';
                    message = 'بسیار ضعیف';
                    break;
                case 2:
                    percent = 40;
                    color = 'bg-warning';
                    message = 'ضعیف';
                    break;
                case 3:
                    percent = 60;
                    color = 'bg-info';
                    message = 'متوسط';
                    break;
                case 4:
                    percent = 80;
                    color = 'bg-primary';
                    message = 'خوب';
                    break;
                case 5:
                    percent = 100;
                    color = 'bg-success';
                    message = 'قوی';
                    break;
            }

            bar.style.width = percent + '%';
            bar.className = 'progress-bar ' + color;
            text.textContent = message;
        }
    },

};

$(document).ready(function () {
    systemuser.list.initial();

    $("#btnFilter").click(() => systemuser.list.reload());
    $("#btnResetFilter").click(() => {
        $("#filter_username,#filter_fullname,#filter_mobile").val("");
        systemuser.list.reload();
    });
});
