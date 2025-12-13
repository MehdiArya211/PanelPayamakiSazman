var roleAssignment = {

    users: [],
    roles: [],
    selectedRoleIds: [],   // نقش‌های فعلی/انتخابی کاربر

    init: function () {
        roleAssignment.loadUsers();
        roleAssignment.loadRoles();
        roleAssignment.loadUnitsIfExists();
        roleAssignment.bindEvents();
    },

    bindEvents: function () {
        $(document).on("change", "#userId", function () {
            roleAssignment.onUserChanged();
        });
    },

    /* =========================
       LOAD USERS
    ========================= */


    loadUsers0: function () {
        $.get("/Project/RoleAssignment/GetUserLookup", function (res) {

            let html = `<option value="">انتخاب کاربر...</option>`;

            (res || []).forEach(x => {
                const id = x.id || x.Id;
                const text = x.text || x.Text;
                html += `<option value="${id}">${text}</option>`;
            });

            $("#userId").html(html);
        });
    },


    loadRoles0: function () {
        $.get("/Project/RoleAssignment/GetRoleLookup", function (res) {

            let html = `<option value="">انتخاب نقش...</option>`;

            (res || []).forEach(x => {
                const id = x.id || x.Id;
                const text = x.text || x.Text;
                html += `<option value="${id}">${text}</option>`;
            });

            $("#roleId").length && $("#roleId").html(html);
            $("#unitRoleId").length && $("#unitRoleId").html(html);
        });
    },

    

    /* =========================
       LOAD UNITS (OPTIONAL)
       اگر endpoint واحدها رو داری، اینو فعال کن.
    ========================= */
    loadUnitsIfExists: function () {
        if ($("#unitId").length === 0) return;

        // اگر مسیر واحدها در پروژه‌ات فرق دارد، همین URL را عوض کن.
        $.get("/Project/Unit/GetAll")
            .done(function (res) {
                let options = `<option value="">انتخاب واحد...</option>`;
                (res || []).forEach(u => options += `<option value="${u.id}">${u.title}</option>`);
                $("#unitId").html(options);
            })
            .fail(function () {
                // اگر این endpoint نداری، صفحه واحد را فعلاً بدون لیست رها می‌کنیم
                $("#unitId").html(`<option value="">لیست واحدها در دسترس نیست</option>`);
            });
    },

    /* =========================
       USER CHANGE -> LOAD CURRENT ROLES
    ========================= */
    onUserChanged: function () {

        const userId = $("#userId").val();
        roleAssignment.selectedRoleIds = [];
        roleAssignment.renderUserRoles();

        if (!userId) return;

        // اگر API GET نقش‌های کاربر وجود داشته باشد:
        $.get("/Project/RoleAssignment/GetUserRoleIds", { userId: userId })
            .done(function (res) {
                roleAssignment.selectedRoleIds = (res && res.roleIds) ? res.roleIds : [];
                roleAssignment.renderUserRoles();
            })
            .fail(function () {
                // اگر GET وجود نداشته باشد، فقط خالی نمایش می‌دهیم
                roleAssignment.selectedRoleIds = [];
                roleAssignment.renderUserRoles();
            });
    },

    /* =========================
       RENDER ROLES TABLE
    ========================= */
    renderUserRoles: function () {
        if ($("#user-roles-body").length === 0) return;

        let html = "";

        const roleIdSet = new Set((roleAssignment.selectedRoleIds || []).map(x => ("" + x).toLowerCase()));

        // نمایش با نام نقش‌ها (از roles که قبلاً لود شده)
        roleAssignment.roles.forEach(r => {
            const rid = ("" + r.id).toLowerCase();
            if (!roleIdSet.has(rid)) return;

            html += `
                <tr>
                    <td>${r.name}</td>
                    <td>
                        <button class="btn btn-light action-btn"
                                title="حذف نقش"
                                onclick="roleAssignment.removeRole('${r.id}')">
                            <i class="bi bi-trash text-danger"></i>
                        </button>
                    </td>
                </tr>
            `;
        });

        if (!html) {
            html = `<tr><td colspan="2" class="text-muted text-center">نقشی برای نمایش وجود ندارد.</td></tr>`;
        }

        $("#user-roles-body").html(html);
    },

    /* =========================
       ASSIGN SINGLE ROLE (POST assign)
    ========================= */
    assignSingle: function () {

        const userId = $("#userId").val();
        const roleId = $("#roleId").val();

        if (!userId || !roleId) {
            Swal.fire("خطا", "کاربر و نقش را انتخاب کنید.", "error");
            return;
        }

        $.ajax({
            url: "/Project/RoleAssignment/AssignToUser",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ userId: userId, roleId: roleId }),
            success: function (res) {
                if (res.status) {
                    Swal.fire("موفق", res.message, "success");

                    // UI: نقش را به لیست انتخابی اضافه می‌کنیم
                    roleAssignment.addToSelected(roleId);
                    roleAssignment.renderUserRoles();
                } else {
                    Swal.fire("خطا", res.message, "error");
                }
            },
            error: function () {
                Swal.fire("خطا", "خطا در ارتباط با سرور", "error");
            }
        });
    },

    addToSelected: function (roleId) {
        const rid = ("" + roleId).toLowerCase();
        const list = roleAssignment.selectedRoleIds || [];
        if (!list.some(x => ("" + x).toLowerCase() === rid)) {
            list.push(roleId);
            roleAssignment.selectedRoleIds = list;
        }
    },

    /* =========================
       REMOVE ROLE (UI) -> Replace
    ========================= */
    removeRole: function (roleId) {
        const userId = $("#userId").val();
        if (!userId) {
            Swal.fire("خطا", "ابتدا کاربر را انتخاب کنید.", "error");
            return;
        }

        Swal.fire({
            title: "حذف نقش",
            text: "آیا از حذف این نقش مطمئن هستید؟",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "بله",
            cancelButtonText: "انصراف"
        }).then(r => {
            if (!r.isConfirmed) return;

            // حذف از لیست انتخابی
            roleAssignment.selectedRoleIds = (roleAssignment.selectedRoleIds || [])
                .filter(x => ("" + x).toLowerCase() !== ("" + roleId).toLowerCase());

            // Replace برای اعمال واقعی روی سرور
            roleAssignment.replaceAll();
        });
    },

    /* =========================
       REPLACE ALL ROLES (PUT users/{id}/roles)
    ========================= */
    replaceAll: function () {

        const userId = $("#userId").val();

        if (!userId) {
            Swal.fire("خطا", "ابتدا کاربر را انتخاب کنید.", "error");
            return;
        }

        // اگر لیست خالی باشد یعنی حذف همه نقش‌ها
        const body = { roleIds: (roleAssignment.selectedRoleIds && roleAssignment.selectedRoleIds.length) ? roleAssignment.selectedRoleIds : null };

        $.ajax({
            url: `/Project/RoleAssignment/ReplaceUserRoles?userId=${encodeURIComponent(userId)}`,
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify(body),
            success: function (res) {
                if (res.status) {
                    Swal.fire("موفق", res.message, "success");
                    roleAssignment.renderUserRoles();
                } else {
                    Swal.fire("خطا", res.message, "error");
                }
            },
            error: function () {
                Swal.fire("خطا", "خطا در ارتباط با سرور", "error");
            }
        });
    },

    clearSelected: function () {
        roleAssignment.selectedRoleIds = [];
        roleAssignment.renderUserRoles();
    },

    /* =========================
       ASSIGN TO UNIT
    ========================= */
    assignToUnit: function () {
        const unitId = $("#unitId").val();
        const roleId = $("#unitRoleId").val();

        if (!unitId || !roleId) {
            Swal.fire("خطا", "واحد و نقش را انتخاب کنید.", "error");
            return;
        }

        $.ajax({
            url: `/Project/RoleAssignment/AssignToUnit?unitId=${encodeURIComponent(unitId)}`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ roleId: roleId }),
            success: function (res) {
                if (res.status) Swal.fire("موفق", res.message, "success");
                else Swal.fire("خطا", res.message, "error");
            },
            error: function () {
                Swal.fire("خطا", "خطا در ارتباط با سرور", "error");
            }
        });
    }
};

$(document).ready(function () {
    roleAssignment.init();
});
