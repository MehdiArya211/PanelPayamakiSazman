const rolePermission = {
    // ID بدون # علامت
    modalId: 'adminRolePermissionsModal',

    // Load form برای مدیریت مجوزهای نقش
    loadForm: function (id) {
        $.ajax({
            url: '/Project/AdminRole/LoadPermissionsForm',
            type: 'GET',
            data: { id: id },
            success: function (html) {
                // یافتن یا ایجاد modal
                let modal = $('#' + rolePermission.modalId);

                if (modal.length === 0) {
                    // اگر modal وجود ندارد، ایجاد کنید
                    $('body').append(`
                        <div class="modal fade" id="${rolePermission.modalId}" tabindex="-1" aria-hidden="true">
                            <div class="modal-dialog modal-lg modal-dialog-scrollable">
                                <div class="modal-content" id="adminRolePermissionsModalContent">
                                </div>
                            </div>
                        </div>
                    `);
                    modal = $('#' + rolePermission.modalId);
                }

                // قرار دادن محتوای HTML در modal-content
                $('#adminRolePermissionsModalContent').html(html);

                // نمایش modal
                const bsModal = new bootstrap.Modal(modal[0]);
                bsModal.show();
            },
            error: function (err) {
                showNotify('error', 'خطا در بارگذاری فرم مجوزهای نقش');
                console.error('Error:', err);
            }
        });
    },

    // انتخاب تمام مجوزها
    selectAll: function () {
        $('.action-checkbox').prop('checked', true);
        showNotify('success', 'تمام مجوزها انتخاب شدند');
    },

    // لغو انتخاب تمام مجوزها
    clearAll: function () {
        $('.action-checkbox').prop('checked', false);
        showNotify('warning', 'تمام مجوزها از حالت انتخاب خارج شدند');
    },

    // ذخیره مجوزهای نقش
    save: function (event, roleId, roleName) {
        event.preventDefault();

        console.log('Save function called');
        console.log('roleId:', roleId);
        console.log('roleName:', roleName);

        // اطمینان از وجود فرم
        const form = document.getElementById('permissionsForm');
        if (!form) {
            showNotify('error', 'فرم یافت نشد');
            return false;
        }

        console.log('Form found');

        // ایجاد آرایه‌ای از مجوزهای انتخاب‌شده
        const routeKeys = [];
        const actionMap = {};

        // جمع‌آوری تمام routeKey‌ها
        const routeKeyInputs = document.querySelectorAll('input[name="routeKey"]');
        console.log('Route keys found:', routeKeyInputs.length);

        routeKeyInputs.forEach(function (input) {
            const routeKey = input.value;
            if (routeKey && !routeKeys.includes(routeKey)) {
                routeKeys.push(routeKey);
                actionMap[routeKey] = [];
            }
        });

        // جمع‌آوری تمام مجوزهای انتخاب‌شده
        const checkedCheckboxes = document.querySelectorAll('.action-checkbox:checked');
        console.log('Checked checkboxes:', checkedCheckboxes.length);

        checkedCheckboxes.forEach(function (checkbox) {
            const routeKey = checkbox.getAttribute('data-route');
            const action = checkbox.value;

            console.log('Processing checkbox - Route:', routeKey, 'Action:', action);

            if (routeKey) {
                if (!actionMap[routeKey]) {
                    actionMap[routeKey] = [];
                }
                if (!actionMap[routeKey].includes(action)) {
                    actionMap[routeKey].push(action);
                }
            }
        });

        // اضافه کردن routeKey‌هایی که مجوز ندارند (خالی)
        routeKeys.forEach(function (routeKey) {
            if (!actionMap[routeKey]) {
                actionMap[routeKey] = [];
            }
        });

        // ایجاد آرایه‌ی نهایی
        const permissionsData = [];
        routeKeys.forEach(function (routeKey) {
            permissionsData.push({
                routeKey: routeKey,
                actions: actionMap[routeKey] || []
            });
        });

        console.log('Final permissions data:', permissionsData);
        console.log('Data to send:', {
            roleId: roleId,
            roleName: roleName,
            permissions: permissionsData
        });

        // ارسال درخواست AJAX
        $.ajax({
            url: '/Project/AdminRole/SavePermissions',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                roleId: roleId,
                roleName: roleName,
                permissions: permissionsData
            }),
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                console.log('Save response:', res);
                if (res.status) {
                    //showNotify('success', res.message);
                    Swal.fire({
                        icon: 'success',
                        title: 'موفقیت',
                        text: res.message,
                        timer: 3000,
                        showConfirmButton: false
                    });

                    // بستن modal
                    const modal = bootstrap.Modal.getInstance(document.getElementById(rolePermission.modalId));
                    if (modal) {
                        modal.hide();
                    }
                    //adminRole.initDataTable();

                    // بازخواند جدول
                    //if (typeof adminRole !== 'undefined' && adminRole.initDataTable) {
                    //    setTimeout(function () {
                    //        adminRole.initDataTable();
                    //    }, 500);
                    //}
                } else {
                    showNotify('error', res.message);
                    const errorDiv = form.querySelector('.error');
                    if (errorDiv) {
                        errorDiv.textContent = res.message;
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error:', {
                    status: status,
                    error: error,
                    response: xhr.responseText
                });
                showNotify('error', 'خطا در ارتباط با سرور:  ' + error);
            }
        });

        return false;
    }
};

function showNotify(type, message) {
    const colors = {
        'success': '#28a745',
        'error': '#dc3545',
        'warning': '#ffc107',
        'info': '#17a2b8'
    };

    const icons = {
        'success': '✓',
        'error': '✕',
        'warning': '⚠',
        'info': 'ℹ'
    };

    const notification = document.createElement('div');
    notification.className = 'custom-notification';
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        background-color: ${colors[type]};
        color: white;
        border-radius: 5px;
        box-shadow: 0 2px 10px rgba(0,0,0,0.2);
        z-index: 9999;
        animation: slideIn 0.3s ease-out;
        max-width: 400px;
        word-wrap: break-word;
        direction: rtl;
        text-align: right;
    `;

    notification.innerHTML = `
        <span style="font-weight: bold; font-size: 18px; margin-left: 10px;">
            ${icons[type]}
        </span>
        <span>${message}</span>
    `;

    document.body.appendChild(notification);

    // حذف پیام بعد از 4 ثانیه
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease-out';
        setTimeout(() => notification.remove(), 300);
    }, 4000);
}

// اضافه کردن CSS برای animation
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }

    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity:  1;
        }
        to {
            transform: translateX(400px);
            opacity:  0;
        }
    }
`;
document.head.appendChild(style);