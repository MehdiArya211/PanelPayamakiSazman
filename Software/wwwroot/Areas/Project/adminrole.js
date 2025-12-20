// wwwroot/areas/admin/role/adminrole.js
var adminRole = {
    /* ============================
       DATATABLE INITIALIZATION
    ============================ */
    dataTable: null,

    initDataTable: function () {
        this.dataTable = $('#adminRoleTable').DataTable({
            language: { url: "/assets/datatables/fa-lang.json" },
            processing: true,
            serverSide: true,
            responsive: true,
            ajax: {
                url: "/Project/AdminRole/GetList",
                type: "POST"
            },
            columns: [
                {
                    data: "id",
                    visible: false // مخفی کردن شناسه
                },
                {
                    data: "name",
                    title: "نام نقش",
                    render: function (data, type, row) {
                        return `<strong>${data}</strong>`;
                    }
                },
                {
                    data: "description",
                    title: "توضیحات",
                    render: function (data) {
                        return data || '<span class="text-muted">---</span>';
                    }
                },
                {
                    data: "isActive",
                    title: "وضعیت",
                    className: "text-center",
                    render: function (data) {
                        if (data) {
                            return '<span class="badge bg-success">فعال</span>';
                        } else {
                            return '<span class="badge bg-danger">غیرفعال</span>';
                        }
                    }
                },
                {
                    data: "userCount",
                    title: "کاربران",
                    className: "text-center",
                    render: function (data) {
                        return `<span class="badge bg-info">${data || 0}</span>`;
                    }
                },
                {
                    data: "createdAt",
                    title: "تاریخ ایجاد",
                    render: function (data) {
                        if (!data) return '---';
                        var date = new Date(data);
                        return date;
                    }
                },
                {
                    data: "createdBy",
                    title: "ایجاد کننده",
                    render: function (data) {
                        return data || '<span class="">---</span>';
                    }
                },
                {
                    data: null,
                    title: "عملیات",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                            <div class="btn-group btn-group-sm" role="group">
                                <button type="button" class="btn btn-outline-primary" 
                                        onclick="adminRole.edit.loadForm('${row.id}')"
                                        title="ویرایش">
                                    <i class="bi bi-pencil"></i>
                                </button>
                                
                                <button type="button" class="btn btn-outline-success" 
                                        onclick="adminRole.toggleActive('${row.id}', ${!row.isActive})"
                                        title="${row.isActive ? 'غیرفعال کردن' : 'فعال کردن'}">
                                    <i class="bi ${row.isActive ? 'bi-toggle-off' : 'bi-toggle-on'}"></i>
                                </button>
                                
                                <button type="button" class="btn btn-outline-danger" 
                                        onclick="adminRole.delete.confirm('${row.id}', '${row.name}')"
                                        title="حذف">
                                    <i class="bi bi-trash"></i>
                                </button>
                            </div>
                        `;
                    }
                }
            ],
            order: [[1, 'asc']], // مرتب‌سازی بر اساس نام
            initComplete: function () {
                // اضافه کردن جستجو به صورت داینامیک
                $('#adminRoleTable_filter input').unbind();
                $('#adminRoleTable_filter input').bind('keyup', function (e) {
                    if (e.keyCode == 13) {
                        adminRole.dataTable.search(this.value).draw();
                    }
                });
            }
        });
    },

    /* ============================
       CREATE OPERATIONS
    ============================ */
    create: {
        loadForm: function () {
            $.get('/Project/AdminRole/LoadCreateForm', function (response) {
                $('#adminRoleModalContent').html(response);
                $('#adminRoleModal').modal('show');

                // فعال‌سازی validation
                $.validator.unobtrusive.parse('#createRoleForm');
            }).fail(function () {
                adminRole.showError('خطا در بارگذاری فرم');
            });
        },

        save: function (e) {
            e.preventDefault();

            var form = $('#createRoleForm');
            if (!form.valid()) {
                return false;
            }

            var submitBtn = $('#btnCreate');
            submitBtn.prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> در حال ایجاد...');

            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                data: form.serialize(),
                success: function (response) {
                    if (response.status) {
                        $('#adminRoleModal').modal('hide');
                        adminRole.showSuccess(response.message);
                        adminRole.reloadDataTable();
                    } else {
                        adminRole.showError(response.message, '#createErrorAlert');
                    }
                },
                error: function () {
                    adminRole.showError('خطا در ارتباط با سرور', '#createErrorAlert');
                },
                complete: function () {
                    submitBtn.prop('disabled', false).html('<i class="bi bi-check-circle"></i> ایجاد نقش');
                }
            });
        }
    },

    /* ============================
       EDIT OPERATIONS
    ============================ */
    edit: {
        loadForm: function (id) {
            $.get('/Project/AdminRole/LoadEditForm', { id: id }, function (response) {
                $('#adminRoleModalContent').html(response);
                $('#adminRoleModal').modal('show');

                // فعال‌سازی validation
                $.validator.unobtrusive.parse('#editRoleForm');
            }).fail(function () {
                adminRole.showError('خطا در بارگذاری فرم');
            });
        },

        save: function (e) {
            e.preventDefault();

            var form = $('#editRoleForm');
            if (!form.valid()) {
                return false;
            }

            var submitBtn = $('#btnEdit');
            submitBtn.prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> در حال ذخیره...');

            $.ajax({
                url: form.attr('action'),
                type: 'PUT',
                data: form.serialize(),
                success: function (response) {
                    if (response.status) {
                        $('#adminRoleModal').modal('hide');
                        adminRole.showSuccess(response.message);
                        adminRole.reloadDataTable();
                    } else {
                        adminRole.showError(response.message, '#editErrorAlert');
                    }
                },
                error: function () {
                    adminRole.showError('خطا در ارتباط با سرور', '#editErrorAlert');
                },
                complete: function () {
                    submitBtn.prop('disabled', false).html('<i class="bi bi-save"></i> ذخیره تغییرات');
                }
            });
        }
    },

    /* ============================
       DELETE OPERATIONS
    ============================ */
    delete: {
        confirm: function (id, name) {
            Swal.fire({
                title: 'حذف نقش',
                html: `آیا از حذف نقش <strong>${name}</strong> مطمئن هستید؟<br><small class="text-danger">این عمل قابل بازگشت نیست!</small>`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'بله، حذف شود',
                cancelButtonText: 'انصراف',
                confirmButtonColor: '#dc3545'
            }).then((result) => {
                if (result.isConfirmed) {
                    adminRole.delete.execute(id);
                }
            });
        },

        execute: function (id) {
            $.post('/Project/AdminRole/Delete', { id: id })
                .done(function (response) {
                    if (response.status) {
                        adminRole.showSuccess(response.message);
                        adminRole.reloadDataTable();
                    } else {
                        adminRole.showError(response.message);
                    }
                })
                .fail(function () {
                    adminRole.showError('خطا در ارتباط با سرور');
                });
        }
    },

    /* ============================
       TOGGLE ACTIVE/INACTIVE
    ============================ */
    toggleActive: function (id, newStatus) {
        var statusText = newStatus ? 'فعال' : 'غیرفعال';

        Swal.fire({
            title: 'تغییر وضعیت',
            text: `آیا می‌خواهید نقش را ${statusText} کنید؟`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'بله',
            cancelButtonText: 'انصراف'
        }).then((result) => {
            if (result.isConfirmed) {
                adminRole.toggleActiveExecute(id, newStatus);
            }
        });
    },

    toggleActiveExecute: function (id, isActive) {
        $.ajax({
            url: '/Project/AdminRole/ToggleActive',
            type: 'PUT',
            data: { id: id, isActive: isActive },
            success: function (response) {
                if (response.status) {
                    adminRole.showSuccess(response.message);
                    adminRole.reloadDataTable();
                } else {
                    adminRole.showError(response.message);
                }
            },
            error: function () {
                adminRole.showError('خطا در ارتباط با سرور');
            }
        });
    },

    /* ============================
       CHECK NAME UNIQUE
    ============================ */
    checkNameUnique: function (name, excludeId) {
        if (!name || name.length < 2) return;

        var url = '/Project/AdminRole/CheckNameUnique?name=' + encodeURIComponent(name);
        if (excludeId) {
            url += '&excludeId=' + encodeURIComponent(excludeId);
        }

        $.get(url, function (response) {
            var errorElement = excludeId ? $('#editNameUniqueError') : $('#nameUniqueError');
            if (!response.isUnique) {
                errorElement.removeClass('d-none');
            } else {
                errorElement.addClass('d-none');
            }
        });
    },

    /* ============================
       UTILITY FUNCTIONS
    ============================ */
    reloadDataTable: function () {
        if (this.dataTable) {
            this.dataTable.ajax.reload(null, false);
        }
    },

    showSuccess: function (message) {
        Swal.fire({
            icon: 'success',
            title: 'موفقیت',
            text: message,
            timer: 3000,
            showConfirmButton: false
        });
    },

    showError: function (message, elementId) {
        if (elementId) {
            $(elementId).html(message).removeClass('d-none');
        } else {
            Swal.fire({
                icon: 'error',
                title: 'خطا',
                text: message
            });
        }
    }
};

// Initialize when document is ready
$(document).ready(function () {
    adminRole.initDataTable();

    // Global form submission handlers
    $(document).on('submit', '#createRoleForm', function (e) {
        adminRole.create.save(e);
    });

    $(document).on('submit', '#editRoleForm', function (e) {
        adminRole.edit.save(e);
    });
});