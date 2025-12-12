var startSubmition = false;

var senderChargeReq = {

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
                    url: "/Project/SenderChargeRequestConsumer/GetList",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                    { data: "id" },
                    { data: "senderNumberFullNumber" },
                    {
                        data: "amount",
                        className: "text-center",
                        render: function (d) {
                            if (d == null) return "";
                            return Number(d).toLocaleString("fa-IR");
                        }
                    },
                    {
                        data: "status",
                        className: "text-center",
                        render: function (d) {
                            switch (d) {
                                case "Pending":
                                    return '<span class="badge bg-warning text-dark">در انتظار</span>';
                                case "Approved":
                                    return '<span class="badge bg-success">تأیید شده</span>';
                                case "Rejected":
                                    return '<span class="badge bg-danger">رد شده</span>';
                                default:
                                    return d || "";
                            }
                        }
                    },
                    { data: "paymentDescription" },
                    { data: "bankAccountNumber" },
                    { data: "reviewNote" },
                    {
                        data: "createdOn",
                        className: "text-center",
                        render: function (d) {
                            return d || "";
                        }
                    },
                    {
                        data: "receiptImageUrl",
                        className: "text-center",
                        render: function (d) {
                            if (!d) return "";
                            // اگر URL کامل نیست، باید سمت بک‌اند کاملش کنید یا اینجا base بگذارید
                            return `<a href="${d}" target="_blank" class="btn btn-sm btn-outline-primary">مشاهده</a>`;
                        }
                    }
                ],

                serverSide: true,
                order: [[0, "desc"]]
            });
        },

        reload: function () {
            senderChargeReq.list.table.ajax.reload(null, false);
        }
    },

    create: {

        loadForm: function () {
            $.get("/Project/SenderChargeRequestConsumer/LoadCreateForm", function (res) {

                $("#modal-form").html(res);

                const modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();
                bindAmountFormatting();

                // رفرش ولیدیشن
                var form = $(".create-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                $.validator.unobtrusive.parse(form);
            });
        },

        save: function (e) {
            e.preventDefault();
            if (startSubmition) return false;
            startSubmition = true;

            var form = $(".create-form");

            form.validate();
            if (!form.valid()) {
                startSubmition = false;
                return false;
            }
            $("#amountRaw").val(toPlainNumber($("#amountDisplay").val()));
            var fd = new FormData(form[0]);

            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: fd,
                processData: false,
                contentType: false,
                success: function (res) {
                    startSubmition = false;

                    if (res.status) {

                        senderChargeReq.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: res.message || "درخواست ثبت شد."
                        });
                    }
                    else {
                        $(".create-form .error").html(res.message);
                    }
                },
                error: function () {
                    startSubmition = false;
                    $(".create-form .error").html("خطا در ارتباط با سرور");
                }
            });

            return false;
        }
    }
};

function toPlainNumber(str) {
    if (!str) return "";
    const fa = "۰۱۲۳۴۵۶۷۸۹";
    const ar = "٠١٢٣٤٥٦٧٨٩";
    let s = String(str);

    for (let i = 0; i < 10; i++) {
        s = s.replaceAll(fa[i], String(i)).replaceAll(ar[i], String(i));
    }

    return s.replace(/[^\d]/g, "");
}

function formatThousands(numStr) {
    if (!numStr) return "";
    numStr = numStr.replace(/^0+(?=\d)/, "");
    return numStr.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function bindAmountFormatting() {
    const $display = $("#amountDisplay");
    const $raw = $("#amountRaw");
    if ($display.length === 0 || $raw.length === 0) return;

    if ($display.data("bound")) return;
    $display.data("bound", true);

    $display.on("input", function () {
        const plain = toPlainNumber(this.value);
        this.value = formatThousands(plain);
        $raw.val(plain);
    });

    const initPlain = toPlainNumber($raw.val());
    if (initPlain) $display.val(formatThousands(initPlain));
}


$(document).ready(function () {
    senderChargeReq.list.initial();
});
