var sms = {

    history: {
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
                    url: "/Project/SmsConsumer/GetHistory?onlyOutbound=true",
                    type: "POST",
                    dataType: "json"
                },

                columns: [
                   /* { data: "id" },*/
                    { data: "senderFullNumber" },
                    { data: "receiverNumber" },
                    {
                        data: "text",
                        render: function (d) {
                            if (!d) return "";
                            if (d.length <= 40) return d;
                            return d.substring(0, 40) + "…";
                        }
                    },
                    {
                        data: "status",
                        className: "text-center",
                        render: function (d) {
                            if (!d) return "";
                            // اینجا اگر enum دقیق داشتی، نقشه کن
                            return `<span class="badge bg-secondary">${d}</span>`;
                        }
                    },
                    { data: "channel", className: "text-center" },
                    {
                        data: "createdOn",
                        className: "text-center",
                        render: function (d) {
                            return d ? new Date(d).toLocaleString("fa-IR") : "";
                        }
                    },
                    {
                        data: "finalCost",
                        className: "text-center",
                        render: function (d) {
                            if (d === null || d === undefined) return "";
                            return Number(d).toLocaleString("fa-IR");
                        }
                    },
                    {
                        data: "failureReason",
                        render: function (d) {
                            return d ? `<span class="text-danger">${d}</span>` : "";
                        }
                    }
                ],

                serverSide: true,
                order: [[0, "desc"]]
            });
        },

        reload: function () {
            sms.history.table.ajax.reload(null, false);
        }
    },

    send: {

        loadForm: function () {
            $.get("/Project/SmsConsumer/LoadSendForm", function (res) {
                $("#send-form-area").html(res);

                // re-parse validation
                var forms = $("#send-form-area form");
                forms.each(function () {
                    $(this).removeData("validator").removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse($(this));
                });
            });
        },

        single: function (e) {
            e.preventDefault();
            var form = $(".single-form");
            form.validate();
            if (!form.valid()) return false;

            $.post(form.attr("action"), form.serialize())
                .done(res => {
                    if (res.status) {
                        sms.history.reload();
                        Swal.fire({ icon: "success", title: "ثبت شد", text: res.message || "پیامک ثبت شد." });
                        form[0].reset();
                    } else {
                        $(".error").html(res.message || "خطا در ارسال.");
                    }
                })
                .fail(() => $(".error").html("خطا در ارتباط با سرور"));

            return false;
        },

        bulk: function (e) {
            e.preventDefault();
            var form = $(".bulk-form");
            form.validate();
            if (!form.valid()) return false;

            $.post(form.attr("action"), form.serialize())
                .done(res => {
                    if (res.status) {
                        sms.history.reload();
                        Swal.fire({ icon: "success", title: "ثبت شد", text: res.message || "ارسال گروهی ثبت شد." });
                        form[0].reset();
                    } else {
                        $(".error").html(res.message || "خطا در ارسال.");
                    }
                })
                .fail(() => $(".error").html("خطا در ارتباط با سرور"));

            return false;
        },

        groups: function (e) {
            e.preventDefault();
            var form = $(".groups-form");
            form.validate();
            if (!form.valid()) return false;

            $.post(form.attr("action"), form.serialize())
                .done(res => {
                    if (res.status) {
                        sms.history.reload();
                        Swal.fire({ icon: "success", title: "ثبت شد", text: res.message || "ارسال به گروه‌ها ثبت شد." });
                        form[0].reset();
                    } else {
                        $(".error").html(res.message || "خطا در ارسال.");
                    }
                })
                .fail(() => $(".error").html("خطا در ارتباط با سرور"));

            return false;
        },

        file: function (e) {
            e.preventDefault();
            var form = $(".file-form");
            form.validate();
            if (!form.valid()) return false;

            var fd = new FormData(form[0]);

            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: fd,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.status) {
                        sms.history.reload();
                        Swal.fire({ icon: "success", title: "ثبت شد", text: res.message || "ارسال از فایل ثبت شد." });
                        form[0].reset();
                    } else {
                        $(".error").html(res.message || "خطا در ارسال.");
                    }
                },
                error: function () {
                    $(".error").html("خطا در ارتباط با سرور");
                }
            });

            return false;
        }
    }
};

$(document).ready(function () {
    sms.history.initial();
    sms.send.loadForm();
});
