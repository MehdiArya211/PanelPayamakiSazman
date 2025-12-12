var consumerSenderNumber = {

    list: {
        table: null,

        initial: function () {
            this.table = $('#datatables').DataTable({
                drawCallback: function () {
                    $('[data-toggle="tooltip"]').tooltip();
                },

                language: { url: "/assets/datatables/fa-lang.json" },
                pagingType: "full_numbers",
                responsive: true,

                ajax: {
                    url: "/Project/ConsumerSenderNumber/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" },

                    { data: "fullNumber" },
                    { data: "fixedPrefix" },

                    {
                        data: "status",
                        className: "text-center",
                        render: function (data) {
                            if (!data) return "";
                            // وضعیت‌های سرشماره همون enum قبلیه
                            switch (data) {
                                case "Purchasing": return '<span class="badge bg-warning text-dark">در حال خرید</span>';
                                case "CompletingDocuments": return '<span class="badge bg-info text-dark">تکمیل مدارک</span>';
                                case "ReviewingDocuments": return '<span class="badge bg-secondary">بررسی مدارک</span>';
                                case "Active": return '<span class="badge bg-success">فعال</span>';
                                case "Inactive": return '<span class="badge bg-dark">غیرفعال</span>';
                                default: return data;
                            }
                        }
                    },

                    {
                        data: "walletBalance",
                        className: "text-center",
                        render: function (data) {
                            if (data === null || data === undefined) return "";
                            // نمایش ریالی سه‌تا سه‌تا
                            return Number(data).toLocaleString("fa-IR");
                        }
                    },

                    { data: "description" },

                    {
                        data: null,
                        className: "text-center",
                        render: function (data, type, row) {
                            // فعلاً فقط یک دکمه نمایشی
                            return `
                                <button class="btn btn-light action-btn"
                                        data-toggle="tooltip"
                                        title="انتخاب برای ارسال"
                                        onclick="consumerSenderNumber.pick('${row.senderNumberId || row.id}', '${row.fullNumber || ''}')">
                                    <i class="bi bi-send text-primary"></i>
                                </button>
                            `;
                        }
                    }
                ],

                serverSide: true,
                order: [[0, "desc"]]
            });
        },

        reload: function () {
            consumerSenderNumber.list.table.ajax.reload(null, false);
        }
    },

    pick: function (id, fullNumber) {
        // اینجا بعداً می‌تونی به صفحه ارسال پیامک وصلش کنی
        console.log("picked:", id, fullNumber);
        Swal.fire({
            icon: "info",
            title: "انتخاب شد",
            text: `سرشماره: ${fullNumber}`
        });
    }
};

$(document).ready(function () {
    consumerSenderNumber.list.initial();
});
