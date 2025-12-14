var smsTariff = {

    table: null,
    modal: null,

    /* ---------------------------
     * Init
     * --------------------------- */
    init: function () {
        this.modal = new bootstrap.Modal(
            document.getElementById('smsTariffModal')
        );

        this.initDataTable();
    },

    /* ---------------------------
     * DataTable
     * --------------------------- */
    initDataTable: function () {

        this.table = $('#tblSmsTariffs').DataTable({
            processing: true,
            serverSide: true,
            searching: false,
            ordering: true,
            pageLength: 20,

            ajax: {
                url: '/Project/SmsTariff/GetList',
                type: 'POST'
            },

            columns: [
                { data: 'operator' },
                { data: 'persianPricePerSegment' },
                { data: 'englishPricePerSegment' },
                {
                    data: 'id',
                    orderable: false,
                    render: function (id) {
                        return `
                            <button class="btn btn-sm btn-warning me-1"
                                    onclick="smsTariff.loadEditForm('${id}')">
                                ویرایش
                            </button>
                            <button class="btn btn-sm btn-danger"
                                    onclick="smsTariff.remove('${id}')">
                                حذف
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
        $('#smsTariffModalBody')
            .load('/Project/SmsTariff/LoadCreateForm', () => {
                this.modal.show();
            });
    },

    create: function () {
        var form = $('#smsTariffCreateForm');

        $.post('/Project/SmsTariff/Create', form.serialize())
            .done(res => {
                if (!res.success) {
                    Swal.fire('خطا', res.message, 'error');
                    return;
                }

                Swal.fire('موفق', res.message, 'success');
                this.modal.hide();
                this.table.ajax.reload(null, false);
            });
    },

    /* ---------------------------
     * Edit
     * --------------------------- */
    loadEditForm: function (id) {
        $('#smsTariffModalBody')
            .load('/Project/SmsTariff/LoadEditForm?id=' + id, () => {
                this.modal.show();
            });
    },

    edit: function () {
        var form = $('#smsTariffEditForm');

        $.ajax({
            url: '/Project/SmsTariff/Edit',
            type: 'PUT',
            data: form.serialize()
        })
            .done(res => {
                if (!res.success) {
                    Swal.fire('خطا', res.message, 'error');
                    return;
                }

                Swal.fire('موفق', res.message, 'success');
                this.modal.hide();
                this.table.ajax.reload(null, false);
            });
    },

    /* ---------------------------
     * Delete
     * --------------------------- */
    remove: function (id) {

        Swal.fire({
            title: 'حذف تعرفه',
            text: 'آیا از حذف این تعرفه مطمئن هستید؟',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'بله',
            cancelButtonText: 'انصراف'
        })
            .then(result => {
                if (!result.isConfirmed) return;

                $.post('/Project/SmsTariff/Delete', { id: id })
                    .done(res => {
                        if (!res.success) {
                            Swal.fire('خطا', res.message, 'error');
                            return;
                        }

                        Swal.fire('حذف شد', res.message, 'success');
                        this.table.ajax.reload(null, false);
                    });
            });
    }
};

$(document).ready(function () {
    smsTariff.init();
});
