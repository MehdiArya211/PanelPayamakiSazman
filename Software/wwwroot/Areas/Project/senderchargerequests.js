var chargeRequests = {

    list: {

        table: null,

        initial: function () {

            this.table = $('#charge-requests-table').DataTable({
                drawCallback: function () {
                    $('[data-toggle="tooltip"]').tooltip();
                },

                language: { url: "/assets/datatables/fa-lang.json" },
                pagingType: "full_numbers",
                responsive: true,

                ajax: {
                    url: "/Project/SenderChargeRequest/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" },              // برای sort داخلی یا نمایش کد درخواست

                    {
                        data: null,
                        render: function (data, type, row) {
                            if (row.senderNumberFullNumber) {
                                return row.senderNumberFullNumber;
                            }
                            // اگر به هر دلیلی fullNumber نیامده بود، حداقل id را نشان بده
                            return row.senderNumberId || "";
                        }
                    },

                    {
                        data: null,
                        render: function (data, type, row) {
                            if (row.walletTitle) {
                                return row.walletTitle;
                            }
                            return row.walletId || "";
                        }
                    },

                    {
                        data: null,
                        render: function (data, type, row) {
                            if (row.requestedByUserFullName) {
                                return row.requestedByUserFullName;
                            }
                            return row.requestedByUserId || "";
                        }
                    },

                    {
                        data: "amount",
                        render: function (data) {
                            if (data == null) return "";
                            return data.toLocaleString("fa-IR");
                        },
                        className: "text-end"
                    },

                    {
                        data: "status",
                        className: "text-center",
                        render: function (data) {
                            if (!data) return "";
                            switch (data) {
                                case "Pending":
                                    return '<span class="badge bg-warning text-dark">در انتظار بررسی</span>';
                                case "Approved":
                                    return '<span class="badge bg-success">تایید شده</span>';
                                case "Rejected":
                                    return '<span class="badge bg-danger">رد شده</span>';
                                default:
                                    return '<span class="badge bg-light text-dark">' + data + '</span>';
                            }
                        }
                    },

                    { data: "paymentDescription" },
                    { data: "bankAccountNumber" },

                    {
                        data: "createdOn",
                        render: function (data) {
                            if (!data) return "";
                            return data; // اگر سمت سرور فرمت فارسی بدهی بهتر است
                        }
                    },

                    {
                        data: "receiptImageUrl",
                        render: function (data) {
                            if (!data) return "";
                            return '<a href="' + data + '" target="_blank">مشاهده رسید</a>';
                        },
                        className: "text-center"
                    },

                    // ستون عملیات (قبلی) بدون تغییر خاص، فقط همین می‌ماند
                    {
                        data: null,
                        className: "text-center",
                        orderable: false,
                        render: function (data, type, row) {
                            var isPending = row.status === "Pending";

                            var approveBtn = `
                <button onclick="chargeRequests.action.openApprove('${row.id}')"
                        class="btn btn-light action-btn"
                        data-toggle="tooltip"
                        title="تایید"
                        ${isPending ? "" : "disabled"}>
                    <i class="bi bi-check-lg text-success"></i>
                </button>`;

                            var rejectBtn = `
                <button onclick="chargeRequests.action.openReject('${row.id}')"
                        class="btn btn-light action-btn"
                        data-toggle="tooltip"
                        title="رد"
                        ${isPending ? "" : "disabled"}>
                    <i class="bi bi-x-lg text-danger"></i>
                </button>`;

                            return `
                <div class="d-flex justify-content-center gap-2">
                    ${approveBtn}
                    ${rejectBtn}
                </div>`;
                        }
                    }
                ],

                serverSide: true,
                order: [[0, "desc"]]
            });
        },

        reload: function () {
            if (chargeRequests.list.table) {
                chargeRequests.list.table.ajax.reload(null, false);
            }
        }
    },

    action: {

        openApprove: function (id) {

            Swal.fire({
                title: "تایید درخواست شارژ",
                input: "textarea",
                inputLabel: "یادداشت (اختیاری)",
                inputPlaceholder: "توضیحی برای تایید این درخواست بنویسید...",
                inputAttributes: {
                    "aria-label": "یادداشت"
                },
                showCancelButton: true,
                confirmButtonText: "تایید",
                cancelButtonText: "انصراف"
            }).then(function (result) {

                if (!result.isConfirmed) return;

                chargeRequests.action.approve(id, result.value);
            });
        },

        approve: function (id, note) {

            $.ajax({
                url: "/Project/SenderChargeRequest/Approve",
                type: "PUT",
                data: {
                    id: id,
                    note: note
                },
                success: function (res) {

                    if (res.status) {
                        chargeRequests.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: res.message || "درخواست با موفقیت تایید شد."
                        });
                    } else {
                        Swal.fire({
                            icon: "error",
                            title: "خطا",
                            text: res.message || "خطا در تایید درخواست."
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: "error",
                        title: "خطا",
                        text: "در ارتباط با سرور خطایی رخ داد."
                    });
                }
            });
        },

        openReject: function (id) {

            Swal.fire({
                title: "رد درخواست شارژ",
                input: "textarea",
                inputLabel: "علت رد (اختیاری، ولی بهتر است پر شود)",
                inputPlaceholder: "علت رد درخواست را بنویسید...",
                inputAttributes: {
                    "aria-label": "یادداشت"
                },
                showCancelButton: true,
                confirmButtonText: "رد درخواست",
                confirmButtonColor: "#d33",
                cancelButtonText: "انصراف"
            }).then(function (result) {

                if (!result.isConfirmed) return;

                chargeRequests.action.reject(id, result.value);
            });
        },

        reject: function (id, note) {

            $.ajax({
                url: "/Project/SenderChargeRequest/Reject",
                type: "PUT",
                data: {
                    id: id,
                    note: note
                },
                success: function (res) {

                    if (res.status) {
                        chargeRequests.list.reload();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: res.message || "درخواست با موفقیت رد شد."
                        });
                    } else {
                        Swal.fire({
                            icon: "error",
                            title: "خطا",
                            text: res.message || "خطا در رد درخواست."
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: "error",
                        title: "خطا",
                        text: "در ارتباط با سرور خطایی رخ داد."
                    });
                }
            });
        }
    }
};


$(document).ready(function () {
    chargeRequests.list.initial();
});
