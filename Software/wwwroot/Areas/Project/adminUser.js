const adminUser = {
    modalId: 'adminUserModal',
    modalContentId: 'adminUserModalContent',

    create: {
        loadForm: function () {
            $.ajax({
                url: '/Project/AdminUser/LoadCreateForm',
                type: 'GET',
                success: function (html) {
                    $('#' + adminUser.modalContentId).html(html);
                    const bsModal = new bootstrap.Modal(document.getElementById(adminUser.modalId));
                    bsModal.show();
                },
                error: function (err) {
                    notifyError('خطا در بارگذاری فرم');
                    console.error('Error:', err);
                }
            });
        },

        save: function (event) {
            event.preventDefault();

            const form = document.getElementById('createAdminUserForm');
            if (!form) {
                notifyError('فرم یافت نشد');
                return false;
            }

            // جمع‌آوری نقش‌های انتخاب‌شده
            const roleIds = [];
            document.querySelectorAll('.role-checkbox:checked').forEach(function (checkbox) {
                roleIds.push(checkbox.value);
            });

            const formData = {
                userName: form.querySelector('[name="CreateModel.UserName"]').value,
                initialPassword: form.querySelector('[name="CreateModel.InitialPassword"]').value,
                firstName: form.querySelector('[name="CreateModel.FirstName"]').value,
                lastName: form.querySelector('[name="CreateModel.LastName"]').value,
                nationalCode: form.querySelector('[name="CreateModel.NationalCode"]').value,
                mobileNumber: form.querySelector('[name="CreateModel.MobileNumber"]').value,
                unitId: form.querySelector('[name="CreateModel.UnitId"]').value,
                roleIds: roleIds,
                securityQuestions: []
            };

            $.ajax({
                url: '/Project/AdminUser/Create',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(formData),
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (res) {
                    if (res.status) {
                        notifySuccess(res.message);
                        $('#' + adminUser.modalId).modal('hide');

                        setTimeout(function () {
                            adminUser.initDataTable();
                        }, 500);
                    } else {
                        notifyError(res.message);
                    }
                },
                error: function (xhr) {
                    notifyError('خطا در ارتباط با سرور');
                    console.error('Error:', xhr);
                }
            });

            return false;
        }
    },

    edit: {
        loadForm: function (id) {
            $.ajax({
                url: '/Project/AdminUser/LoadEditForm',
                type: 'GET',
                data: { id: id },
                success: function (html) {
                    $('#' + adminUser.modalContentId).html(html);
                    const bsModal = new bootstrap.Modal(document.getElementById(adminUser.modalId));
                    bsModal.show();
                },
                error: function (err) {
                    notifyError('خطا در بارگذاری فرم');
                    console.error('Error:', err);
                }
            });
        },

        save: function (event) {
            event.preventDefault();

            const form = document.getElementById('editAdminUserForm');
            if (!form) {
                notifyError('فرم یافت نشد');
                return false;
            }

            // جمع‌آوری نقش‌های انتخاب‌شده
            const roleIds = [];
            document.querySelectorAll('.role-checkbox:checked').forEach(function (checkbox) {
                roleIds.push(checkbox.value);
            });

            const formData = {
                id: form.querySelector('[name="EditModel.Id"]').value,
                firstName: form.querySelector('[name="EditModel.FirstName"]').value,
                lastName: form.querySelector('[name="EditModel.LastName"]').value,
                nationalCode: form.querySelector('[name="EditModel.NationalCode"]').value,
                mobileNumber: form.querySelector('[name="EditModel.MobileNumber"]').value,
                unitId: form.querySelector('[name="EditModel.UnitId"]').value,
                isActive: form.querySelector('[name="EditModel.IsActive"]').value === 'true',
                roleIds: roleIds
            };

            $.ajax({
                url: '/Project/AdminUser/Edit',
                type: 'PUT',
                contentType: 'application/json',
                data: JSON.stringify(formData),
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (res) {
                    if (res.status) {
                        notifySuccess(res.message);
                        $('#' + adminUser.modalId).modal('hide');

                        setTimeout(function () {
                            adminUser.initDataTable();
                        }, 500);
                    } else {
                        notifyError(res.message);
                    }
                },
                error: function (xhr) {
                    notifyError('خطا در ارتباط با سرور');
                    console.error('Error:', xhr);
                }
            });

            return false;
        }
    },

    delete: function (id) {
        if (!confirm('آیا از حذف این کاربر مطمئن هستید؟')) {
            return false;
        }

        $.ajax({
            url: '/Project/AdminUser/Delete',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ id: id }),
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                if (res.status) {
                    notifySuccess(res.message);

                    setTimeout(function () {
                        adminUser.initDataTable();
                    }, 500);
                } else {
                    notifyError(res.message);
                }
            },
            error: function (xhr) {
                notifyError('خطا در ارتباط با سرور');
                console.error('Error:', xhr);
            }
        });
    },

    initDataTable: function () {
        if ($.fn.DataTable.isDataTable('#adminUserTable')) {
            $('#adminUserTable').DataTable().destroy();
        }

        $('#adminUserTable').DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: '/Project/AdminUser/GetList',
                type: 'POST'
            },
            columns: [
                { data: 'id', title: 'شناسه', width: '10%' },
                { data: 'userName', title: 'نام کاربری', width: '15%' },
                { data: 'fullName', title: 'نام کامل', width: '20%' },
                { data: 'mobileNumber', title: 'موبایل', width: '15%' },
                { data: 'nationalCode', title: 'کد ملی', width: '12%' },
                {
                    data: 'lockoutEnd',
                    title: 'وضعیت',
                    width: '12%',
                    render: function (data) {
                        if (data) {
                            const lockDate = new Date(data);
                            if (lockDate > new Date()) {
                                return '<span class="badge bg-danger">قفل شده</span>';
                            }
                        }
                        return '<span class="badge bg-success">فعال</span>';
                    }
                },
                {
                    data: 'id',
                    title: 'عملیات',
                    width: '16%',
                    render: function (data, type, row) {
                        return `
                            <div class="btn-group btn-group-sm" role="group">
                                <button class="btn btn-warning" onclick="adminUser.edit.loadForm('${data}')" title="ویرایش کاربر">
                                    <i class="bi bi-pencil"></i> ویرایش
                                </button>
                                <button class="btn btn-danger" onclick="adminUser.delete('${data}')" title="حذف کاربر">
                                    <i class="bi bi-trash"></i> حذف
                                </button>
                            </div>
                        `;
                    },
                    orderable: false,
                    searchable: false
                }
            ],
            order: [[1, 'asc']],
            language: {
                url: '/assets/datatables/fa-lang.json'
            },
            responsive: true,
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>><"row"<"col-sm-12"tr>><"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            pageLength: 10
        });
    }
};

$(document).ready(function () {
    adminUser.initDataTable();
});