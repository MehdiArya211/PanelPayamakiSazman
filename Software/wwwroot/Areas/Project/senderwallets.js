var senderWallet = {

    list: {

        table: null,

        initial: function () {

            this.table = $('#wallet-transactions').DataTable({
                drawCallback: function () {
                    $('[data-toggle="tooltip"]').tooltip();
                },

                language: { url: "/assets/datatables/fa-lang.json" },
                pagingType: "full_numbers",
                responsive: true,

                ajax: {
                    url: "/Project/SenderWallet/GetTransactions",
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        d.senderNumberId = $('#SenderNumberFilter').val();
                    }
                },

                columns: [
                    { data: "id" },

                    {
                        data: "amount",
                        render: function (data, type, row) {
                            if (data == null) return "";
                            var sign = row.isCredit ? "+" : "-";
                            return sign + " " + data.toLocaleString("fa-IR");
                        },
                        className: "text-end"
                    },

                    {
                        data: "isCredit",
                        render: function (data) {
                            if (data === true)
                                return '<span class="badge bg-success">بستانکار</span>';
                            if (data === false)
                                return '<span class="badge bg-danger">بدهکار</span>';
                            return "";
                        },
                        className: "text-center"
                    },

                    { data: "type" },
                    { data: "reference" },
                    { data: "description" },

                    {
                        data: "createdOn",
                        render: function (data) {
                            if (!data) return "";
                            return data; // اگر سمت سرور فرمت فارسی بدهی بهتر است
                        }
                    }
                ],

                serverSide: true,
                order: [[6, "desc"]] // مرتب‌سازی روی createdOn
            });
        },

        reload: function () {
            if (senderWallet.list.table) {
                senderWallet.list.table.ajax.reload(null, false);
            }
        }
    },

    walletInfo: {

        load: function () {

            var id = $('#SenderNumberFilter').val();
            if (!id) {
                $('#wallet-balance').text("-");
                $('#btnCharge').prop("disabled", true);
                $('#btnTransfer').prop("disabled", true);
                return;
            }

            $.get("/Project/SenderWallet/GetWallet", { senderNumberId: id })
                .done(function (res) {

                    if (!res || res.balance == null) {
                        $('#wallet-balance').text("۰");
                    } else {
                        $('#wallet-balance').text(res.balance.toLocaleString("fa-IR"));
                    }

                    $('#btnCharge').prop("disabled", false);
                    $('#btnTransfer').prop("disabled", false);
                })
                .fail(function () {
                    $('#wallet-balance').text("-");
                    $('#btnCharge').prop("disabled", true);
                    $('#btnTransfer').prop("disabled", true);
                });
        }
    },

    filter: {

        onChange: function () {
            senderWallet.walletInfo.load();
            senderWallet.list.reload();
        }
    },

    charge: {

        loadForm: function () {

            var id = $('#SenderNumberFilter').val();
            if (!id) return;

            var text = $('#SenderNumberFilter option:selected').text();

            $.get("/Project/SenderWallet/LoadChargeForm",
                { senderNumberId: id, senderNumberText: text },
                function (res) {

                    $("#modal-form").html(res);

                    var modal = new bootstrap.Modal(document.getElementById("base-modal"));
                    modal.show();

                    var form = $(".charge-form")
                        .removeData("validator")
                        .removeData("unobtrusiveValidation");

                    if ($.validator && $.validator.unobtrusive) {
                        $.validator.unobtrusive.parse(form);
                    }
                });
        },

        save: function (e) {

            e.preventDefault();

            var form = $(".charge-form");

            if ($.validator && $.validator.unobtrusive) {
                form.validate();
                if (!form.valid()) return false;
            }

            $.post(form.attr("action"), form.serialize())
                .done(function (res) {

                    if (res.status) {

                        senderWallet.walletInfo.load();
                        senderWallet.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: res.message || "کیف پول با موفقیت شارژ شد."
                        });
                    } else {
                        $(".charge-form .error").html(res.message || "خطا در شارژ کیف پول.");
                    }
                })
                .fail(function () {
                    $(".charge-form .error").html("خطا در ارتباط با سرور");
                });

            return false;
        }
    },

    transfer: {

        loadForm: function () {

            var id = $('#SenderNumberFilter').val();
            if (!id) return;

            var text = $('#SenderNumberFilter option:selected').text();

            $.get("/Project/SenderWallet/LoadTransferForm",
                { fromSenderNumberId: id, fromSenderNumberText: text },
                function (res) {

                    $("#modal-form").html(res);

                    var modal = new bootstrap.Modal(document.getElementById("base-modal"));
                    modal.show();

                    var form = $(".transfer-form")
                        .removeData("validator")
                        .removeData("unobtrusiveValidation");

                    if ($.validator && $.validator.unobtrusive) {
                        $.validator.unobtrusive.parse(form);
                    }
                });
        },

        save: function (e) {

            e.preventDefault();

            var form = $(".transfer-form");

            if ($.validator && $.validator.unobtrusive) {
                form.validate();
                if (!form.valid()) return false;
            }

            $.post(form.attr("action"), form.serialize())
                .done(function (res) {

                    if (res.status) {

                        senderWallet.walletInfo.load();
                        senderWallet.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "عملیات موفق",
                            text: res.message || "انتقال با موفقیت انجام شد."
                        });
                    } else {
                        $(".transfer-form .error").html(res.message || "خطا در انتقال موجودی.");
                    }
                })
                .fail(function () {
                    $(".transfer-form .error").html("خطا در ارتباط با سرور");
                });

            return false;
        }
    }
};


$(document).ready(function () {

    senderWallet.list.initial();

    $('#SenderNumberFilter').on('change', function () {
        senderWallet.filter.onChange();
    });
});
