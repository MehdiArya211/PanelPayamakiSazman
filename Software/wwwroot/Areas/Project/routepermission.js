// wwwroot/areas/admin/routepermission/routepermission.js
var routePermission = {
    /* ============================
       GLOBAL VARIABLES
    ============================ */
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    currentSearch: '',
    permissions: [],

    /* ============================
       INITIALIZATION
    ============================ */
    init: function () {
        this.loadPermissions();
        this.setupEventListeners();
    },

    setupEventListeners: function () {
        // Enter key in search
        $('#searchInput').keypress(function (e) {
            if (e.which === 13) { // Enter key
                routePermission.search();
            }
        });
    },

    /* ============================
       LOAD PERMISSIONS
    ============================ */
    loadPermissions: function (page = 1) {
        this.currentPage = page;

        var searchData = {
            draw: 1,
            start: (page - 1) * this.pageSize,
            length: this.pageSize,
            searchValue: this.currentSearch,
            sortColumnName: 'routeKey',
            sortDirection: 'asc'
        };

        $.ajax({
            url: '/Project/RoutePermission/GetList',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(searchData),
            success: function (response) {
                routePermission.permissions = response.data;
                routePermission.totalItems = response.recordsTotal;
                routePermission.renderTable();
                routePermission.renderPagination();
                routePermission.updateCount();
            },
            error: function () {
                routePermission.showError('خطا در بارگذاری داده‌ها');
            }
        });
    },

    /* ============================
       RENDER TABLE
    ============================ */
    renderTable: function () {
        var tbody = $('#permissionTableBody');
        tbody.empty();

        if (this.permissions.length === 0) {
            tbody.html(`
                <tr>
                    <td colspan="4" class="text-center text-muted py-4">
                        <i class="bi bi-inbox display-4 d-block mb-2"></i>
                        هیچ مجوزی یافت نشد
                    </td>
                </tr>
            `);
            return;
        }

        this.permissions.forEach(function (permission, index) {
            var row = `
                <tr class="permission-row">
                    <td>
                        <div class="route-key">${permission.routeKey}</div>
                    </td>
                    <td>
                        ${routePermission.renderActions(permission.actions)}
                    </td>
                    <td class="text-center">
                        ${permission.requiresFreshAuth ?
                    '<span class="badge bg-warning text-dark">بله</span>' :
                    '<span class="badge bg-success">خیر</span>'}
                    </td>
                    <td class="text-center">
                        <div class="btn-group btn-group-sm" role="group">
                            <button type="button" class="btn btn-outline-primary" 
                                    onclick="routePermission.edit.loadForm('${permission.routeKey}')"
                                    title="ویرایش">
                                <i class="bi bi-pencil"></i>
                            </button>
                            
                            <button type="button" class="btn btn-outline-danger" 
                                    onclick="routePermission.delete.confirm('${permission.routeKey}')"
                                    title="حذف">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    },

    renderActions: function (actions) {
        if (!actions || actions.length === 0) {
            return '<span class="text-muted">---</span>';
        }

        var badges = '';
        actions.forEach(function (action) {
            badges += `<span class="badge bg-primary action-badge">${action}</span>`;
        });
        return badges;
    },

    /* ============================
       PAGINATION
    ============================ */
    renderPagination: function () {
        var totalPages = Math.ceil(this.totalItems / this.pageSize);
        var pagination = $('#pagination');
        pagination.empty();

        if (totalPages <= 1) return;

        // Previous button
        var prevDisabled = this.currentPage === 1 ? 'disabled' : '';
        pagination.append(`
            <li class="page-item ${prevDisabled}">
                <a class="page-link" href="#" onclick="routePermission.loadPermissions(${this.currentPage - 1}); return false;">
                    <i class="bi bi-chevron-right"></i>
                </a>
            </li>
        `);

        // Page numbers
        for (var i = 1; i <= totalPages; i++) {
            var active = i === this.currentPage ? 'active' : '';
            pagination.append(`
                <li class="page-item ${active}">
                    <a class="page-link" href="#" onclick="routePermission.loadPermissions(${i}); return false;">
                        ${i}
                    </a>
                </li>
            `);
        }

        // Next button
        var nextDisabled = this.currentPage === totalPages ? 'disabled' : '';
        pagination.append(`
            <li class="page-item ${nextDisabled}">
                <a class="page-link" href="#" onclick="routePermission.loadPermissions(${this.currentPage + 1}); return false;">
                    <i class="bi bi-chevron-left"></i>
                </a>
            </li>
        `);
    },

    updateCount: function () {
        var start = (this.currentPage - 1) * this.pageSize + 1;
        var end = Math.min(this.currentPage * this.pageSize, this.totalItems);

        $('#permissionCount').html(`
            نمایش ${start} تا ${end} از ${this.totalItems} مجوز
        `);
    },

    /* ============================
       SEARCH
    ============================ */
    search: function () {
        this.currentSearch = $('#searchInput').val();
        this.loadPermissions(1);
    },

    /* ============================
       EDIT OPERATIONS
    ============================ */
    edit: {
        loadForm: function (routeKey) {
            $.get('/Project/RoutePermission/LoadEditForm', { routeKey: routeKey }, function (response) {
                $('#routePermissionModalContent').html(response);

                // قبل از نشان دادن modal، Select2 را initialize نکنید
                // این کار در ready خود partial view انجام می‌شود

                // Event handler برای زمانی که modal کاملاً show شد
                $('#routePermissionModal').on('shown.bs.modal', function () {
                    // مطمئن شویم Select2 قبلاً initialize نشده
                    $('.select2-multiple').select2('destroy');

                    // مجدداً initialize کنید
                    routePermission.initializeSelect2();
                });

                $('#routePermissionModal').modal('show');
            }).fail(function () {
                routePermission.showError('خطا در بارگذاری فرم');
            });
        },

        initializeSelect2: function () {
            // اگر element وجود داشت، اول destroy کنید
            if ($('.select2-multiple').hasClass('select2-hidden-accessible')) {
                $('.select2-multiple').select2('destroy');
            }

            // Initialize Select2
            $('.select2-multiple').select2({
                theme: 'bootstrap4',
                width: '100%',
                placeholder: 'Actionها را انتخاب کنید...',
                allowClear: true,
                dropdownParent: $('#routePermissionModal') // مهم: برای کار در modal
            });

            // Update preview when selection changes
            $('.select2-multiple').on('change', function () {
                routePermission.updateSelectedActionsPreview();
            });
        },

        save: function (e) {
            e.preventDefault();

            var form = $('#editPermissionForm');
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
                        // قبل از بستن modal، Select2 را destroy کنید
                        $('.select2-multiple').select2('destroy');

                        $('#routePermissionModal').modal('hide');
                        routePermission.showSuccess(response.message);
                        routePermission.loadPermissions(routePermission.currentPage);
                    } else {
                        routePermission.showError(response.message, '#editErrorAlert');
                    }
                },
                error: function () {
                    routePermission.showError('خطا در ارتباط با سرور', '#editErrorAlert');
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
        confirm: function (routeKey) {
            Swal.fire({
                title: 'حذف مجوز',
                html: `آیا از حذف مجوز <strong>${routeKey}</strong> مطمئن هستید؟<br>
                      <small class="text-danger">این عمل قابل بازگشت نیست!</small>`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'بله، حذف شود',
                cancelButtonText: 'انصراف',
                confirmButtonColor: '#dc3545'
            }).then((result) => {
                if (result.isConfirmed) {
                    routePermission.delete.execute(routeKey);
                }
            });
        },

        execute: function (routeKey) {
            $.ajax({
                url: '/Project/RoutePermission/Delete',
                type: 'DELETE',
                data: { routeKey: routeKey },
                success: function (response) {
                    if (response.status) {
                        routePermission.showSuccess(response.message);
                        routePermission.loadPermissions(routePermission.currentPage);
                    } else {
                        routePermission.showError(response.message);
                    }
                },
                error: function () {
                    routePermission.showError('خطا در ارتباط با سرور');
                }
            });
        }
    },

    /* ============================
       UTILITY FUNCTIONS
    ============================ */
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
    routePermission.init();

    // Global form submission handlers
    $(document).on('submit', '#editPermissionForm', function (e) {
        routePermission.edit.save(e);
    });
});