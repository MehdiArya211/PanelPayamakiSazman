var serviceClient = {

    table: null,
    modal: null,

    /* ---------------------------
     * Init
     * --------------------------- */
    init: function () {

        this.modal = new bootstrap.Modal(
            document.getElementById('serviceClientModal')
        );

        this.initDataTable();
    },

    /* ---------------------------
     * DataTable
     * --------------------------- */
    initDataTable: function () {

        this.table = $('#tblServiceClients').DataTable({
            processing: true,
            serverSide: true,
            searching: false,
            ordering: true,
            pageLength: 20,

            ajax: {
                url: '/Project/ServiceClients/GetList',
                type: 'POST'
            },

            columns: [
                { data: 'displayName' },
                { data: 'unitName' },
                {
                    data: 'isActive',
                    render: function (data, type, row) {
                        return `
                            <input type="checkbox"
                                   ${data ? 'checked' : ''}
                                   onchange="serviceClient.changeStatus('${row.clientId}', this.checked)" />
                        `;
                    }
                },
                {
                    data: 'clientId',
                    orderable: false,
                    render: function (id) {
                        return `
                            <button class="btn btn-sm btn-warning me-1"
                                    onclick="serviceClient.loadEditForm('${id}')">
                                ویرایش
                            </button>
                            <button class="btn btn-sm btn-info"
                                    onclick="serviceClient.rotateSecret('${id}')">
                                Rotate Secret
                            </button>
                        `;
                    }
                }
            ]
        });
    },

    /* ---------------------------
     * Create
     * --------------------------- */
    loadCreateForm: function () {

        $('#serviceClientModalBody')
            .load('/Project/ServiceClients/LoadCreateForm', () => {
                this.modal.show();
            });
    },

    create: function () {

        var form = $('#serviceClientCreateForm');

        var roles = $('#roleIdsInput').val()
            .split(',')
            .map(x => x.trim())
            .filter(x => x);

        var data = form.serializeArray();

        roles.forEach(r => {
            data.push({ name: 'RoleIds', value: r });
        });

        $.post('/Project/ServiceClients/Create', $.param(data))
            .done(res => {

                if (!res || res.status !== true) {
                    Swal.fire('خطا', res?.message || 'خطای نامشخص', 'error');
                    return;
                }

                Swal.fire('موفق', res.message, 'success');

                this.modal.hide();
                this.table.ajax.reload(null, false);
            })
            .fail(() => {
                Swal.fire('خطا', 'خطا در ارتباط با سرور', 'error');
            });
    },

    /* ---------------------------
     * Edit (Roles)
     * --------------------------- */
    loadEditForm: function (clientId) {

        $('#serviceClientModalBody')
            .load('/Project/ServiceClients/LoadEditForm?clientId=' + clientId, () => {
                this.modal.show();
            });
    },

    updateRoles: function () {

        var clientId = $('#serviceClientEditForm input[name="clientId"]').val();

        var roles = $('#editRoleIdsInput').val()
            .split(',')
            .map(x => x.trim())
            .filter(x => x);

        $.ajax({
            url: '/Project/ServiceClients/UpdateRoles',
            type: 'PUT',
            data: {
                clientId: clientId,
                roleIds: roles
            }
        })
            .done(res => {

                if (!res || res.status !== true) {
                    Swal.fire('خطا', res?.message || 'خطای نامشخص', 'error');
                    return;
                }

                Swal.fire('موفق', res.message, 'success');

                this.modal.hide();
                this.table.ajax.reload(null, false);
            })
            .fail(() => {
                Swal.fire('خطا', 'خطا در ارتباط با سرور', 'error');
            });
    },

    /* ---------------------------
     * Rotate Secret
     * --------------------------- */
    rotateSecret: function (clientId) {

        Swal.fire({
            title: 'تولید کلید جدید',
            text: 'کلید جدید فقط یکبار نمایش داده می‌شود. ادامه می‌دهید؟',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'بله',
            cancelButtonText: 'انصراف'
        })
            .then(result => {

                if (!result.isConfirmed)
                    return;

                $.post('/Project/ServiceClients/RotateSecret', { clientId })
                    .done(res => {

                        if (!res || res.status !== true) {
                            Swal.fire('خطا', res?.message || 'خطای نامشخص', 'error');
                            return;
                        }

                        Swal.fire({
                            title: 'Client Secret',
                            html: `
                            <p class="text-danger fw-bold">
                                این کلید فقط یکبار نمایش داده می‌شود
                            </p>
                            <code style="user-select:all">
                                ${res.secret}
                            </code>
                        `,
                            icon: 'success'
                        });
                    })
                    .fail(() => {
                        Swal.fire('خطا', 'خطا در ارتباط با سرور', 'error');
                    });
            });
    },

    /* ---------------------------
     * Change Status
     * --------------------------- */
    changeStatus: function (clientId, isActive) {

        $.post('/Project/ServiceClients/ChangeStatus',
            { clientId: clientId, isActive: isActive })
            .fail(() => {
                Swal.fire('خطا', 'خطا در تغییر وضعیت', 'error');
                this.table.ajax.reload(null, false);
            });
    }
};

/* ---------------------------
 * Document Ready
 * --------------------------- */
$(document).ready(function () {
    serviceClient.init();
});
