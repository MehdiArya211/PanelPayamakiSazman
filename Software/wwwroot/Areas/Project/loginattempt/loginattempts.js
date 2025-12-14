/* =========================================
 * Login & Security Question Attempts
 * Area   : Project
 * Controller : LoginAttempt
 * ========================================= */

var loginAttemptModule = {

    initLoginAttempts: function () {

        var table = $('#tblLoginAttempts');

        if (!table.length) return;

        table.DataTable({
            processing: true,
            serverSide: true,
            searching: false,
            ordering: true,
            lengthMenu: [10, 20, 50],
            pageLength: 20,

            ajax: {
                url: '/Project/LoginAttempt/GetLoginAttempts',
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
                { data: 'ipAddress' },
                {
                    data: 'attemptDate',
                    render: function (data) {
                        if (!data) return '';
                        return new Date(data).toLocaleString('fa-IR');
                    }
                },
                {
                    data: 'failureReason',
                    defaultContent: '-'
                }
            ]
        });
    },

    initSecurityQuestionAttempts: function () {

        var table = $('#tblSecurityQuestions');

        if (!table.length) return;

        table.DataTable({
            processing: true,
            serverSide: true,
            searching: false,
            ordering: true,
            lengthMenu: [10, 20, 50],
            pageLength: 20,

            ajax: {
                url: '/Project/LoginAttempt/GetSecurityQuestionAttempts',
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
                        if (!data) return '';
                        return new Date(data).toLocaleString('fa-IR');
                    }
                }
            ]
        });
    },

    init: function () {
        this.initLoginAttempts();
        this.initSecurityQuestionAttempts();
    }
};

$(document).ready(function () {
    loginAttemptModule.init();
});
