var securitypolicy = {

    form: null,
    btnSubmit: null,

    initial: function () {

        this.form = $(".security-policy-form");
        if (!this.form.length) return;

        this.btnSubmit = this.form.find("button[type='submit']");

        // فعال‌سازی validation
        this.form
            .removeData("validator")
            .removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse(this.form);

        // تنظیم اولیه Rotation
        this.toggleRotationDays();

        // رویداد تغییر RotationEnabled
        this.form.find("#RotationEnabled")
            .on("change", () => securitypolicy.toggleRotationDays());

        // جلوگیری از عدد منفی
        this.form.find("input[type='number']")
            .on("input", function () {
                if (this.value < 0) this.value = 0;
            });
    },

    toggleRotationDays: function () {

        var enabled = $("#RotationEnabled").is(":checked");
        var input = $("#RotationDays");

        if (!input.length) return;

        input.prop("disabled", !enabled);

        if (!enabled) {
            input.val(0);
        }
    },

    save: function (e) {
        e.preventDefault();

        var form = securitypolicy.form;
        form.validate();
        if (!form.valid()) return false;

        securitypolicy.btnSubmit.prop("disabled", true);

        $.post(form.attr("action"), form.serialize())

            .done(res => {

                if (res.status) {
                    Swal.fire({
                        icon: "success",
                        title: "ذخیره شد",
                        text: res.message || "سیاست‌های امنیتی با موفقیت ذخیره شد."
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "خطا",
                        html: res.message
                    });
                }
            })

            .fail(() => {
                Swal.fire({
                    icon: "error",
                    title: "خطا",
                    text: "در ارتباط با سرور خطایی رخ داد."
                });
            })

            .always(() => {
                securitypolicy.btnSubmit.prop("disabled", false);
            });

        return false;
    }
};

$(document).ready(function () {
    securitypolicy.initial();
});
