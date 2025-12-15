$(document).ready(function () {

    $('#tblSecurityQuestions').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: true,
        lengthMenu: [10, 20, 50],
        pageLength: 20,

        ajax: {
            url: '/project/loginattemps/GetSecurityQuestionAttempts',
            type: 'POST'
        },

        columns: [
            { data: 'username' },
            {
                data: 'isSuccessful',
                render: function (data) {
                    return data
                        ? '<span class="badge bg-success">موفق</span>'
                        : '<span class="badge bg-danger">ناموفق</span>';
                }
            },
            { data: 'attemptCount' },
            { data: 'ipAddress' },
            {
                data: 'attemptDate',
                render: function (data) {
                    return data
                        ? new Date(data).toLocaleString('fa-IR')
                        : '';
                }
            }
        ]
    });

});
