var smsAdmin = {

    /* ============================
       LIST (HISTORY - DATATABLE)
    ============================ */
    list: {

        table: null,

        initial: function () {

            this.table = $('#sms-history-table').DataTable({
                drawCallback: function () {
                    $('[data-toggle="tooltip"]').tooltip();
                },

                language: { url: "/assets/datatables/fa-lang.json" },
                pagingType: "full_numbers",
                responsive: true,

                ajax: {
                    url: "/Project/AdminSms/GetHistory",
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        d.Search = $('#filter-search').val();
                        d.SenderNumberId = $('#filter-senderNumber').val();
                        d.Status = $('#filter-status').val();
                        d.Channel = $('#filter-channel').val();

                        var from = $('#filter-from').val();
                        var to = $('#filter-to').val();

                        d.From = from || null;
                        d.To = to || null;
                        d.OnlyOutbound = $('#filter-onlyOutbound').is(':checked');
                    }
                },

                columns: [
                    { data: "id" },

                    { data: "senderNumber" },

                    {
                        data: null,
                        render: function (data, type, row) {
                            var name = row.requestedByFullName || "";
                            var user = row.requestedByUserName || "";
                            if (name && user) return name + " (" + user + ")";
                            if (name) return name;
                            if (user) return user;
                            return "";
                        }
                    },

                    { data: "receiverNumber" },

                    {
                        data: "text",
                        render: function (data) {
                            if (!data) return "";
                            if (data.length > 50)
                                return data.substring(0, 50) + " ...";
                            return data;
                        }
                    },

                    {
                        data: "status",
                        render: function (data) {
                            switch (data) {
                                case "Pending":
                                    return '<span class="badge bg-warning text-dark">در انتظار</span>';
                                case "Sent":
                                    return '<span class="badge bg-info text-dark">ارسال شده</span>';
                                case "Delivered":
                                    return '<span class="badge bg-success">تحویل شده</span>';
                                case "Failed":
                                    return '<span class="badge bg-danger">ناموفق</span>';
                                default:
                                    return data || "";
                            }
                        },
                        className: "text-center"
                    },

                    {
                        data: "direction",
                        render: function (data) {
                            if (data === "Outbound")
                                return '<span class="badge bg-primary">خروجی</span>';
                            if (data === "Inbound")
                                return '<span class="badge bg-secondary">ورودی</span>';
                            return data || "";
                        },
                        className: "text-center"
                    },

                    { data: "channel" },

                    {
                        data: "createdOn",
                        render: function (data) {
                            return data || "";
                        }
                    },

                    {
                        data: "sentOn",
                        render: function (data) {
                            return data || "";
                        }
                    },

                    {
                        data: "deliveredOn",
                        render: function (data) {
                            return data || "";
                        }
                    },

                    {
                        data: "reservedCost",
                        render: function (data) {
                            if (data == null) return "";
                            return data.toLocaleString("fa-IR");
                        },
                        className: "text-end"
                    },

                    {
                        data: "finalCost",
                        render: function (data) {
                            if (data == null) return "";
                            return data.toLocaleString("fa-IR");
                        },
                        className: "text-end"
                    },

                    { data: "failureReason" }
                ],

                serverSide: true,
                order: [[8, "desc"]] // createdOn
            });
        },

        reload: function () {
            if (smsAdmin.list.table) {
                smsAdmin.list.table.ajax.reload(null, false);
            }
        }
    },

    /* ============================
       FILTER
    ============================ */
    filter: {

        apply: function () {
            smsAdmin.list.reload();
        },

        reset: function () {
            $('#filter-senderNumber').val("").trigger('change');
            $('#filter-status').val("").trigger('change');
            $('#filter-channel').val("").trigger('change');
            $('#filter-search').val("");
            $('#filter-from').val("");
            $('#filter-to').val("");
            $('#filter-from-view').val("");
            $('#filter-to-view').val("");
            $('#filter-onlyOutbound').prop("checked", true);

            smsAdmin.list.reload();
        }
    },

    /* ============================
       DATEPICKER (PERSIAN)
    ============================ */
    datepicker: {

        init: function () {

            // از تاریخ
            $("#filter-from-view").persianDatepicker({
                format: "YYYY/MM/DD",
                initialValue: false,
                autoClose: true,
                calendar: {
                    persian: { locale: "fa" }
                },
                onSelect: function (unix) {
                    if (!unix) {
                        $("#filter-from").val("").trigger('change');
                        return;
                    }
                    var g = new persianDate(unix).toCalendar("gregorian").toDate();
                    var year = g.getFullYear();
                    var month = (g.getMonth() + 1).toString().padStart(2, "0");
                    var day = g.getDate().toString().padStart(2, "0");
                    $("#filter-from").val(year + "-" + month + "-" + day).trigger('change');
                }
            });

            // تا تاریخ
            $("#filter-to-view").persianDatepicker({
                format: "YYYY/MM/DD",
                initialValue: false,
                autoClose: true,
                calendar: {
                    persian: { locale: "fa" }
                },
                onSelect: function (unix) {
                    if (!unix) {
                        $("#filter-to").val("").trigger('change');
                        return;
                    }
                    var g = new persianDate(unix).toCalendar("gregorian").toDate();
                    var year = g.getFullYear();
                    var month = (g.getMonth() + 1).toString().padStart(2, "0");
                    var day = g.getDate().toString().padStart(2, "0");
                    $("#filter-to").val(year + "-" + month + "-" + day).trigger('change');
                }
            });
        }
    },

    /* ============================
       SINGLE SEND
    ============================ */
    single: {

        loadForm: function () {

            $.get("/Project/AdminSms/LoadSingleForm", function (res) {

                $("#modal-form").html(res);

                var modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                var form = $(".single-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(form);
                }
            });
        },

        save: function (e) {

            e.preventDefault();

            var form = $(".single-form");

            if ($.validator && $.validator.unobtrusive) {
                form.validate();
                if (!form.valid()) return false;
            }

            $.post(form.attr("action"), form.serialize())
                .done(function (res) {

                    if (res.status) {

                        smsAdmin.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: res.message || "پیامک تکی با موفقیت ثبت شد."
                        });
                    } else {
                        $(".single-form .error").html(res.message || "خطا در ارسال پیامک.");
                    }
                })
                .fail(function () {
                    $(".single-form .error").html("خطا در ارتباط با سرور");
                });

            return false;
        }
    },

    /* ============================
       BULK SEND (LIST)
    ============================ */
    bulk: {

        loadForm: function () {

            $.get("/Project/AdminSms/LoadBulkForm", function (res) {

                $("#modal-form").html(res);

                var modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                var form = $(".bulk-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(form);
                }
            });
        },

        save: function (e) {

            e.preventDefault();

            var form = $(".bulk-form");

            var sender = form.find('select[name="SenderNumberId"]').val();
            var numbers = form.find('textarea[name="NumbersText"]').val();
            var text = form.find('textarea[name="Text"]').val();

            if (!sender || !numbers || !text) {
                $(".bulk-form .error").html("سرشماره، متن و فهرست شماره‌ها الزامی است.");
                return false;
            }

            $.post(form.attr("action"), form.serialize())
                .done(function (res) {

                    if (res.status) {

                        smsAdmin.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: res.message || "پیامک‌های گروهی ثبت شدند."
                        });
                    } else {
                        $(".bulk-form .error").html(res.message || "خطا در ارسال پیامک‌های گروهی.");
                    }
                })
                .fail(function () {
                    $(".bulk-form .error").html("خطا در ارتباط با سرور");
                });

            return false;
        }
    },

    /* ============================
       GROUPS SEND
    ============================ */
    groups: {

        loadForm: function () {

            $.get("/Project/AdminSms/LoadGroupsForm", function (res) {

                $("#modal-form").html(res);

                var modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                var form = $(".groups-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(form);
                }
            });
        },

        save: function (e) {

            e.preventDefault();

            var form = $(".groups-form");

            if ($.validator && $.validator.unobtrusive) {
                form.validate();
                if (!form.valid()) return false;
            }

            $.post(form.attr("action"), form.serialize())
                .done(function (res) {

                    if (res.status) {

                        smsAdmin.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: res.message || "پیامک‌ها برای گروه‌ها ثبت شدند."
                        });
                    } else {
                        $(".groups-form .error").html(res.message || "خطا در ارسال به گروه‌ها.");
                    }
                })
                .fail(function () {
                    $(".groups-form .error").html("خطا در ارتباط با سرور");
                });

            return false;
        }
    },

    /* ============================
       FILE SEND
    ============================ */
    file: {

        loadForm: function () {

            $.get("/Project/AdminSms/LoadFileForm", function (res) {

                $("#modal-form").html(res);

                var modal = new bootstrap.Modal(document.getElementById("base-modal"));
                modal.show();

                var form = $(".file-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(form);
                }
            });
        },

        save: function (e) {

            e.preventDefault();

            var form = $(".file-form")[0];

            var fd = new FormData(form);

            $.ajax({
                url: $(form).attr("action"),
                type: "POST",
                data: fd,
                processData: false,
                contentType: false,
                success: function (res) {

                    if (res.status) {

                        smsAdmin.list.reload();

                        var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                        modal.hide();

                        Swal.fire({
                            icon: "success",
                            title: "موفق",
                            text: res.message || "فایل دریافت شد و پیامک‌ها ثبت شدند."
                        });
                    } else {
                        $(".file-form .error").html(res.message || "خطا در پردازش فایل.");
                    }
                },
                error: function () {
                    $(".file-form .error").html("خطا در ارتباط با سرور");
                }
            });

            return false;
        }
    }
};


$(document).ready(function () {

    smsAdmin.list.initial();
    smsAdmin.datepicker.init();

    $('#sms-filter-form input, #sms-filter-form select').on('change keyup', function (e) {
        if (e.type === "change" || e.keyCode === 13) {
            smsAdmin.filter.apply();
        }
    });
});
