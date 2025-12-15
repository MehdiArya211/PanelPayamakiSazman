var menu = {
    selectedMenu: null,
    menuData: [],

    /* ============================ INITIALIZATION ============================ */
    init: function () {
        this.loadMenus();
    },

    /* ============================ LOAD MENUS ============================ */
    loadMenus: function () {
        $('#loading').show();
        $('#menu-list-container').hide();
        $('#empty-state').hide();
        $('#actions-toolbar').hide();

        $.get('/Project/Menu/GetMenuTree', function (response) {
            $('#loading').hide();

            if (response.success && response.data && response.data.length > 0) {
                menu.menuData = response.data;
                menu.renderMenuList();
                $('#menu-list-container').show();
            } else {
                $('#empty-state').show();
            }
        }).fail(function () {
            $('#loading').hide();
            Swal.fire({
                icon: 'error',
                title: 'خطا',
                text: 'خطا در بارگذاری منوها'
            });
        });
    },

    /* ============================ RENDER MENU LIST ============================ */
    renderMenuList: function () {
        var container = $('#menu-list-container');
        var html = '';

        function renderNode(node, level = 0) {
            var hasChildren = node.children && node.children.length > 0;
            var levelClass = 'menu-item-level-' + Math.min(level + 1, 5);
            var isActive = node.isActive ? 'menu-status-active' : 'menu-status-inactive';
            var statusText = node.isActive ? 'فعال' : 'غیرفعال';
            var isSelected = menu.selectedMenu && menu.selectedMenu.id === node.id;

            html += `
                <div class="menu-item ${levelClass} ${isSelected ? 'active' : ''}"
                     data-id="${node.id}"
                     onclick="menu.selectMenu(event, '${node.id}')">
                    
                    ${hasChildren ?
                    `<button class="menu-toggle" onclick="menu.toggleChildren(event, '${node.id}')">
                            <i class="bi bi-chevron-down" id="toggle-icon-${node.id}"></i>
                        </button>` :
                    '<span style="width: 30px; display: inline-block;"></span>'
                }
                    
                    <span class="menu-icon">
                        ${node.icon ? `<i class="${node.icon}"></i>` : '<i class="bi bi-folder"></i>'}
                    </span>
                    
                    <span class="menu-title">${node.title}</span>
                    
                    <span class="menu-badge badge bg-light text-dark border">
                        ${node.key}
                    </span>
                    
                    <span class="menu-badge badge ${node.isActive ? 'bg-success' : 'bg-danger'}">
                        ${statusText}
                    </span>
                    
                    ${node.order ? `<span class="menu-badge badge bg-light text-dark border">${node.order}</span>` : ''}
                </div>
                
                <div id="children-container-${node.id}" class="children-container" style="display: block;">
            `;

            // رندر فرزندان
            if (hasChildren) {
                node.children.forEach(child => {
                    html += renderNode(child, level + 1);
                });
            }

            html += `</div>`;
            return html;
        }

        // پاک‌سازی و رندر جدید
        container.empty();

        // رندر همه منوهای ریشه
        menu.menuData.forEach(rootNode => {
            html = renderNode(rootNode, 0);
            container.append(html);
        });

        // برای اینکه html تکراری نباشه، تابع renderNode باید مقدار return داشته باشه
        // اما به روش زیر بهتر عمل می‌کنیم:
        container.html(this.buildMenuHTML(menu.menuData));
    },

    // تابع جدید برای ساخت HTML
    buildMenuHTML: function (nodes, level = 0) {
        var html = '';

        nodes.forEach(node => {
            var hasChildren = node.children && node.children.length > 0;
            var levelClass = 'menu-item-level-' + Math.min(level + 1, 5);
            var isActive = node.isActive ? 'menu-status-active' : 'menu-status-inactive';
            var statusText = node.isActive ? 'فعال' : 'غیرفعال';
            var isSelected = menu.selectedMenu && menu.selectedMenu.id === node.id;

            html += `
                <div class="menu-item ${levelClass} ${isSelected ? 'active' : ''}"
                     data-id="${node.id}"
                     onclick="menu.selectMenu(event, '${node.id}')">
                    
                    ${hasChildren ?
                    `<button class="menu-toggle" onclick="menu.toggleChildren(event, '${node.id}')">
                            <i class="bi bi-chevron-down" id="toggle-icon-${node.id}"></i>
                        </button>` :
                    '<span style="width: 30px; display: inline-block;"></span>'
                }
                    
                    <span class="menu-icon">
                        ${node.icon ? `<i class="${node.icon}"></i>` : '<i class="bi bi-folder"></i>'}
                    </span>
                    
                    <span class="menu-title">${node.title}</span>
                    
                    <span class="menu-badge badge bg-light text-dark border">
                        ${node.key}
                    </span>
                    
                    <span class="menu-badge badge ${node.isActive ? 'bg-success' : 'bg-danger'}">
                        ${statusText}
                    </span>
                    
                    ${node.order ? `<span class="menu-badge badge bg-light text-dark border">${node.order}</span>` : ''}
                </div>
                
                <div id="children-container-${node.id}" class="children-container" style="display: block;">
            `;

            // رندر فرزندان اگر وجود دارند
            if (hasChildren) {
                html += this.buildMenuHTML(node.children, level + 1);
            }

            html += `</div>`;
        });

        return html;
    },

    /* ============================ SELECT MENU ============================ */
    selectMenu: function (event, menuId) {
        event.stopPropagation();

        // حذف active از همه
        $('.menu-item').removeClass('active');

        // اضافه کردن active به منوی انتخاب شده
        $(event.currentTarget).addClass('active');

        // پیدا کردن اطلاعات منوی انتخاب شده
        var selectedNode = this.findNodeById(menu.menuData, menuId);
        if (selectedNode) {
            menu.selectedMenu = selectedNode;
            menu.showActionsToolbar(selectedNode);
        }
    },

    findNodeById: function (nodes, id) {
        for (var i = 0; i < nodes.length; i++) {
            var node = nodes[i];
            if (node.id === id) {
                return node;
            }
            if (node.children && node.children.length > 0) {
                var found = this.findNodeById(node.children, id);
                if (found) return found;
            }
        }
        return null;
    },

    showActionsToolbar: function (menuNode) {
        var statusText = menuNode.isActive ?
            '<span class="text-success">✓ فعال</span>' :
            '<span class="text-danger">✗ غیرفعال</span>';

        var parentInfo = '';
        if (menuNode.parentId) {
            var parentNode = this.findNodeById(menu.menuData, menuNode.parentId);
            if (parentNode) {
                parentInfo = `زیرمجموعه: <strong>${parentNode.title}</strong> | `;
            }
        }

        $('#selected-menu-title').html(`
            <i class="${menuNode.icon || 'bi bi-folder'}"></i>
            ${menuNode.title}
        `);

        $('#selected-menu-details').html(`
            ${parentInfo}
            کلید: <code>${menuNode.key}</code> | 
            مسیر: ${menuNode.routeKey ? `<code>${menuNode.routeKey}</code>` : 'تعیین نشده'} | 
            ترتیب: ${menuNode.order} | 
            وضعیت: ${statusText}
        `);

        $('#actions-toolbar').slideDown(300);
    },

    /* ============================ TOGGLE CHILDREN ============================ */
    toggleChildren: function (event, menuId) {
        event.stopPropagation();
        event.preventDefault();

        var $childrenContainer = $(`#children-container-${menuId}`);
        var $icon = $(`#toggle-icon-${menuId}`);

        if ($childrenContainer.length) {
            if ($childrenContainer.is(':visible')) {
                $childrenContainer.slideUp(200);
                $icon.removeClass('bi-chevron-down').addClass('bi-chevron-right');
            } else {
                $childrenContainer.slideDown(200);
                $icon.removeClass('bi-chevron-right').addClass('bi-chevron-down');
            }
        }
    },

    /* ============================ ACTIONS ============================ */
    addRoot: function () {
        menu.selectedMenu = null;
        $('#actions-toolbar').hide();
        $('.menu-item').removeClass('active');

        // بارگذاری فرم ایجاد منوی اصلی
        $.get('/Project/Menu/LoadCreateForm', function (res) {
            $("#modal-form").html(res);
            const modal = new bootstrap.Modal(document.getElementById("base-modal"));
            modal.show();

            var form = $(".create-form")
                .removeData("validator")
                .removeData("unobtrusiveValidation");
            if ($.validator && $.validator.unobtrusive) {
                $.validator.unobtrusive.parse(form);
            }
        });
    },

    addChild: function () {
        if (!menu.selectedMenu) {
            Swal.fire({
                icon: 'warning',
                title: 'لطفاً یک منو انتخاب کنید',
                text: 'برای افزودن زیرمنو، ابتدا یک منو را انتخاب کنید'
            });
            return;
        }

        // بارگذاری فرم ایجاد با والد انتخاب شده
        $.get('/Project/Menu/LoadCreateForm', function (res) {
            $("#modal-form").html(res);
            const modal = new bootstrap.Modal(document.getElementById("base-modal"));
            modal.show();

            // تنظیم والد انتخاب شده
            $('#CreateModel_ParentId').val(menu.selectedMenu.id);
            $('#CreateModel_ParentId').prop('disabled', true);

            // اضافه کردن متن به عنوان فرم
            $('.modal-title').html(`افزودن زیرمنو به "${menu.selectedMenu.title}"`);

            var form = $(".create-form")
                .removeData("validator")
                .removeData("unobtrusiveValidation");
            if ($.validator && $.validator.unobtrusive) {
                $.validator.unobtrusive.parse(form);
            }
        });
    },

    edit: function () {
        if (!menu.selectedMenu) {
            Swal.fire({
                icon: 'warning',
                title: 'لطفاً یک منو انتخاب کنید',
                text: 'برای ویرایش، ابتدا یک منو را انتخاب کنید'
            });
            return;
        }

        $.get("/Project/Menu/LoadEditForm", { id: menu.selectedMenu.id }, function (res) {
            $("#modal-form").html(res);
            const modal = new bootstrap.Modal(document.getElementById("base-modal"));
            modal.show();

            var form = $(".edit-form")
                .removeData("validator")
                .removeData("unobtrusiveValidation");
            if ($.validator && $.validator.unobtrusive) {
                $.validator.unobtrusive.parse(form);
            }
        });
    },

    delete: function () {
        if (!menu.selectedMenu) {
            Swal.fire({
                icon: 'warning',
                title: 'لطفاً یک منو انتخاب کنید',
                text: 'برای حذف، ابتدا یک منو را انتخاب کنید'
            });
            return;
        }

        Swal.fire({
            title: "حذف منو",
            html: `
                <p>آیا از حذف منوی <strong>${menu.selectedMenu.title}</strong> مطمئن هستید؟</p>
                <div class="text-danger small mt-2">
                    <i class="bi bi-exclamation-triangle"></i>
                    توجه: اگر این منو دارای زیرمنو باشد، آنها نیز حذف خواهند شد!
                </div>
            `,
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "بله، حذف شود",
            cancelButtonText: "انصراف",
            confirmButtonColor: "#dc3545"
        }).then(function (result) {
            if (result.isConfirmed) {
                $.post("/Project/Menu/Delete", { id: menu.selectedMenu.id })
                    .done(function (res) {
                        if (res.status) {
                            menu.selectedMenu = null;
                            $('#actions-toolbar').hide();
                            menu.loadMenus();
                            Swal.fire({
                                icon: "success",
                                title: "حذف شد",
                                text: "منو با موفقیت حذف شد."
                            });
                        } else {
                            Swal.fire({
                                icon: "error",
                                title: "خطا",
                                text: res.message || "خطا در حذف منو."
                            });
                        }
                    })
                    .fail(function () {
                        Swal.fire({
                            icon: "error",
                            title: "خطا",
                            text: "در ارتباط با سرور خطایی رخ داد."
                        });
                    });
            }
        });
    },

    clearSelection: function () {
        menu.selectedMenu = null;
        $('#actions-toolbar').slideUp(300);
        $('.menu-item').removeClass('active');
    },

    /* ============================ FORM HANDLERS ============================ */
    saveCreateForm: function (e) {
        e.preventDefault();
        var form = $(".create-form");

        if ($.validator && $.validator.unobtrusive) {
            form.validate();
            if (!form.valid()) {
                return;
            }
        }

        $.post(form.attr("action"), form.serialize())
            .done(function (res) {
                if (res.status) {
                    menu.loadMenus();
                    var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                    modal.hide();
                    Swal.fire({
                        icon: "success",
                        title: "عملیات موفق",
                        text: "منو با موفقیت ایجاد شد."
                    });
                } else {
                    $(".create-form .error").html(res.message || "خطا در ذخیره اطلاعات.");
                }
            })
            .fail(function () {
                $(".create-form .error").html("خطا در ذخیره اطلاعات.");
            });
    },

    saveEditForm: function (e) {
        e.preventDefault();
        var form = $(".edit-form");

        if ($.validator && $.validator.unobtrusive) {
            form.validate();
            if (!form.valid()) return;
        }

        $.ajax({
            url: form.attr("action"),
            type: "PUT",
            data: form.serialize(),
            success: function (res) {
                if (res.status) {
                    menu.loadMenus();
                    var modal = bootstrap.Modal.getInstance(document.getElementById("base-modal"));
                    modal.hide();
                    Swal.fire({
                        icon: "success",
                        title: "عملیات موفق",
                        text: "ویرایش منو با موفقیت انجام شد."
                    });
                } else {
                    $(".edit-form .error").html(res.message || "خطا در ویرایش اطلاعات.");
                }
            },
            error: function () {
                $(".edit-form .error").html("خطا در ارتباط با سرور");
            }
        });
    }
};

$(document).ready(function () {
    menu.init();
});