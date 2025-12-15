var rolePermission = {

    all: [],

    load: function () {

        const roleName = $("#roleName").val();

        $.get("/Project/RolePermission/GetPermissions",
            { roleName: roleName },
            function (res) {

                rolePermission.all = res || [];
                rolePermission.render(rolePermission.all);
            });
    },

    render: function (list) {

        let html = "";

        (list || []).forEach((item, index) => {

            const actionsValue = (item.actions || []).join(',');

            html += `
                <tr data-index="${index}">
                    <td>
                        <input type="text"
                               class="form-control route-key"
                               value="${item.routeKey || ''}" />
                    </td>
                    <td>
                        <input type="text"
                               class="form-control actions"
                               value="${actionsValue}" />
                    </td>
                    <td class="text-center">
                        <button type="button"
                                class="btn btn-light"
                                title="حذف"
                                onclick="rolePermission.removeRow(this)">
                            <i class="bi bi-trash text-danger"></i>
                        </button>
                    </td>
                </tr>`;
        });

        $("#permission-body").html(html);
    },

    addRow: function () {

        rolePermission.all.push({ routeKey: "", actions: [] });
        rolePermission.render(rolePermission.all);
    },

    removeRow: function (btn) {

        $(btn).closest("tr").remove();
    },

    collect: function () {

        let permissions = [];

        $("#permission-body tr").each(function () {

            const routeKey = $(this).find(".route-key").val().trim();

            const actions = $(this)
                .find(".actions")
                .val()
                .split(',')
                .map(x => x.trim())
                .filter(x => x.length > 0);

            // routeKey خالی را نفرست
            if (routeKey.length === 0) return;

            permissions.push({
                routeKey: routeKey,
                actions: actions
            });
        });

        return permissions;
    },

    save: function () {

        const roleName = $("#roleName").val();

        if (!roleName) {
            Swal.fire("خطا", "نام نقش مشخص نشده است", "error");
            return;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();
        const permissions = rolePermission.collect();

        $.ajax({
            url: `/Project/RolePermission/Save?roleName=${encodeURIComponent(roleName)}`,
            type: "POST",
            contentType: "application/json",
            headers: {
                "RequestVerificationToken": token
            },
            data: JSON.stringify(permissions),
            success: function (res) {
                if (res.status) {
                    Swal.fire("موفق", res.message, "success");
                } else {
                    Swal.fire("خطا", res.message, "error");
                }
            }
        });
    }

    ,
    filter: function () {

        const q = ($("#filter_routeKey").val() || "").trim().toLowerCase();

        if (!q) {
            rolePermission.render(rolePermission.all);
            return;
        }

        const filtered = (rolePermission.all || []).filter(x =>
            (x.routeKey || "").toLowerCase().includes(q)
        );

        rolePermission.render(filtered);
    },

    resetFilter: function () {

        $("#filter_routeKey").val("");
        rolePermission.render(rolePermission.all);
    }
};

$(document).ready(function () {

    rolePermission.load();

    $("#btnFilter").on("click", function () {
        rolePermission.filter();
    });

    $("#btnResetFilter").on("click", function () {
        rolePermission.resetFilter();
    });
});
