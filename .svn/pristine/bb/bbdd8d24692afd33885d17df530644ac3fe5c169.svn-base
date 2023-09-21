var rsi_no = $("#RSI_NO").val();
var part_type = $('#PartType').val();
var phase_id = $('#phase_id').val();
var priceidentity = $('#priceidentity').val();
var pricetrendJson = $('#pricetrendJson').val();
var pricetrendArray = JSON.parse(pricetrendJson);
var mterialGroupPartsObject;

var tableFirstColumn = [
    { name: 'MTL_GROUP', data: 'MTL_GROUP', width: '55px', className: 'tooltipclass' },
    { name: 'PARTS_GROUP', data: 'PARTS_GROUP', width: '70px', className: 'tooltipclass' },
    { name: 'MTL_PARTS', data: 'MTL_PARTS', width: '50px', className: 'tooltipclass' },
    { name: 'PARENT', data: 'PARTNUMBER_PARENT', width: '120px' },
    {
        name: 'PART_NO', data: 'PART_NO', width: '120px',
        render: function (data, type, row) {
            var view = data;
            if (row.UNI_SPEC_STATUS == 'Y') {
                view += "<span class='text-danger' style='float: right; font-weight: bold;'>!</span>";
            }

            if (row.EOL_STATUS == 'Y') {
                view += "<span class='text-danger' style='float: right; font-weight: bold;'>@</span>";
            }
            return view;
        }
    },
    { name: 'ENGLISH_NAME', data: 'ENGLISH_NAME', width: '70px' },
    { name: 'PART_DESC', data: 'PART_DESC', width: '170px', className: 'text-ellipsis' },
    { name: 'MAKER_SOURCE', data: 'MAKER_SOURCE', width: '55px', visible: false, searchable: false },
    { name: 'MAKER_PART_NO', data: 'MAKER_PART_NO', width: '55px', visible: false, searchable: false },
    { name: 'PART_SPEC', data: 'PART_SPEC', width: '170px', searchable: false },
    { name: 'PART_TYPE', data: 'PART_TYPE', width: '80px', searchable: false },
    {
        name: 'RELEASE_DATE', data: 'RELEASE_DATE', width: '120px', visible: false, searchable: false,
        render: function (data, type, row) {
            var re = /-?\d+/;
            var m = re.exec(data);
            var d = new Date(parseInt(m[0]));
            var viewstring = '{0}/{1}/{2}';
            return viewstring.replace('{0}', d.getFullYear()).replace('{1}', d.getMonth() + 1).replace('{2}', d.getDate());
        }
    },
    { name: 'EOL_STATUS', data: 'EOL_STATUS', width: '120px', visible: false, searchable: false },
    { name: 'USAGE', data: 'USAGE', width: '70px', className: 'text-right', searchable: false },
    { name: 'SOURCER_OWNER', data: 'SOURCER_OWNER', width: '140px', className: 'text-ellipsis-owner', searchable: false },
    {
        name: 'PRICE', data: 'PRICE', width: '100px', className: 'text-right', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH') {
                if (data == null)
                    return "<input name='sourcerPrice' type='text' value='' class='form-control custom_tooltip' style='text-align:right; width:80px; font-size:12px;' disabled>";

                return "<input name='sourcerPrice' type='text' value='" + new Intl.NumberFormat('en-US').format(data) + "' class='form-control custom_tooltip' style='text-align:right; width:80px; font-size:12px;' disabled>";
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if (priceidentity === 'Part_No' || hasSubTotal) {
                if (data == null)
                    return "<p class='custom_tooltip'></p>";

                return "<p class='custom_tooltip'>" + new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data) + "</p>";
            }
            return null;
        }
    },
    {
        name: 'PRICE_PM', data: 'PRICE_PM', width: '120px', className: 'text-right', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH') {
                var result = '<input type="checkbox" name="isassigner" style="margin-right:3px;" />';
                if (row.ISASSIGNER === 'Y') {
                    result = '<input type="checkbox" name="isassigner" style="margin-right:3px;" checked="true" />';
                }
                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return result + "<input name='pmPrice' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
                return result + "<input name='pmPrice' type='text' value='" + new Intl.NumberFormat('en-US').format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if (priceidentity === 'Part_No' || hasSubTotal) {
                if (data !== null)
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }

            return null;
        }
    },
    {
        name: 'MOQ', data: 'MOQ', width: '80px', className: 'text-right', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH') {
                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return "<input name='moq' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
                return "<input name='moq' type='text' value='" + new Intl.NumberFormat('en-US').format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if (priceidentity === 'Part_No' || hasSubTotal) {
                if (data !== null)
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }
            return null;
        }
    },
    {
        name: 'MOCKUP', data: 'MOCKUP', width: '80px', className: 'text-right', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH') {
                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return "<input name='mockup' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
                return "<input name='mockup' type='text' value='" + new Intl.NumberFormat('en-US').format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if (priceidentity === 'Part_No' || hasSubTotal) {
                if (data !== null)
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }

            return null;
        }
    },
    {
        name: 'TOOLING', data: 'TOOLING', width: '80px', className: 'text-right', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH') {
                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return "<input name='tooling' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
                return "<input name='tooling' type='text' value='" + new Intl.NumberFormat('en-US').format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if (priceidentity === 'Part_No' || hasSubTotal) {
                if (data !== null)
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }
            return null;
        }
    },
    {
        name: 'FPCA/PCBA', data: 'FPCA_PCBA', width: '80px', className: 'text-right', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH') {
                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return "<input name='fpca_pcba' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
                return "<input name='fpca_pcba' type='text' value='" + new Intl.NumberFormat('en-US').format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if (priceidentity === 'Part_No' || hasSubTotal) {
                if (data !== null)
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }
            return null;
        }
    },
    {
        name: 'Valuation', data: 'ISCALCULATE', width: '50px', className: 'text-center', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y') {
                var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
                var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
                var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
                var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
                if (!hasSubTotal) {
                    if (data === "Y") {
                        return "<input type='checkbox' class='iscalculate' checked>";
                    }
                    return "<input type='checkbox' class='iscalculate'>";
                }
            }
            return null;
        }
    },
    {
        name: 'Approved', data: 'ISAPPROVED', width: '50px', className: 'text-center', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y' && data === 'Y')
                return '<i class="fa fa-2x fa-check-square-o" aria-hidden="true"></i>';
            return null;
        }
    },
    {
        name: 'Sourcer_Amount', data: 'SOURCERAMOUNT', defaultContent: '', width: '70px', className: 'text-right', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH' && (row.USAGE != null && row.PRICE != null)) {
                return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if ((priceidentity === 'Part_No' || hasSubTotal) && (row.USAGE != null && row.PRICE != null))
                return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            return null;
        }
    },
    {
        name: 'PM_Amount', data: 'PMAMOUNT', defaultContent: '', width: '70px', className: 'text-right', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH' && (row.USAGE != null && row.PRICE != null)) {
                return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if ((priceidentity === 'Part_No' || hasSubTotal) && (row.USAGE != null && row.PRICE_PM != null))
                return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            return null;
        }
    }
];
$.each(pricetrendArray, function (index, value) {
    var priceColumn = {
        name: value, data: 'pricetrend[]', defaultContent: '', className: 'text-right', render: function (data, type, row) {
            if (data.length == 0)
                data = null;
            else {
                var item = data.filter(function (d) { return d.ID == value });
                if (item.length > 0)
                    data = item[0].PRICE;
                else
                    data = null;
            }

            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH') {
                if (data == null)
                    return null;
                return new Intl.NumberFormat('en-US').format(data);
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if (priceidentity === 'Part_No' || hasSubTotal) {
                if (data == null)
                    return null;
                return new Intl.NumberFormat('en-US').format(data);
            }

            return null;
        }
    };
    tableFirstColumn.push(priceColumn);
});
var tableOtherColumn = [
    { name: 'SOURCE', data: 'SOURCE', width: '80px', searchable: false },
    { name: 'REMARK', data: 'REMARK', width: '80px', searchable: false },
    {
        name: 'REMARK_PM', data: 'REMARK_PM', width: '120px', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            data = (data == null) ? '' : data;
            if (row.ISMODIFY === "Y") {
                var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
                var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
                var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
                var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
                if (hasSubTotal)
                    return null;

                return "<textarea name='REMARK_PM' style='width:100%; height:70px; font-size:12px;'>" + data + "</textarea>";
            }
            return null;
        }
    },
    {
        name: 'REMARK_PUR', data: 'REMARK_PUR', width: '120px', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            data = (data == null) ? '' : data;
            if (row.ISMODIFY === "Y") {
                var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
                var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
                var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
                var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
                if (hasSubTotal)
                    return null;

                return "<textarea name='REMARK_PUR' style='width:100%; height:70px; font-size:12px;'>" + data + "</textarea>";
            }
            return null;
        }
    },
    {
        name: 'FILE_STATUS', data: 'FILE_STATUS', width: '55px', className: 'text-center', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (data === null)
                return null;

            if (row.ISMODIFY === "Y") {
                var icon = data === 'Y' ? 'fa-file-text-o' : 'fa-upload';
                var color = data === 'Y' ? ' btn-success' : 'btn-info';
                return '<button class="btn ' + color + ' btn-sm"  onclick="fileManagement(' + row.SN + ')")><i class="fa ' + icon + '"></i></button>';
            }
            return null;
        }
    },
    {
        name: 'UPDATED_DATE', data: 'UPDATED_DATE', visible: false, searchable: false,
        render: function (data, type, row) {
            var re = /-?\d+/;
            var m = re.exec(data);
            var d = new Date(parseInt(m[0]));
            var viewstring = '{0}/{1}/{2}';
            return viewstring.replace('{0}', d.getFullYear()).replace('{1}', d.getMonth() + 1).replace('{2}', d.getDate());
        }
    },
    { name: 'EMP_NAME', data: 'EMP_NAME', visible: false, searchable: false },
    {
        name: 'Return', data: null, defaultContent: '', width: '70px', className: 'text-center', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY !== 'Y')
                return null;
            if (row.MTL_GROUP === 'MOH') {
                if (row.return != '' && row.return != undefined) {
                    return "<input type='checkbox' class='editor-active return flat-blue' checked>";
                }
                return "<input type='checkbox' class='editor-active return flat-blue'>";
            }

            var isMaterialParts = row.MTL_PARTS !== null ? row.MTL_PARTS === '小計' : false;
            var isPartsGroup = row.PARTS_GROUP !== null ? row.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = row.MTL_GROUP != null ? row.MTL_GROUP === '小計' || row.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            if (!hasSubTotal) {
                if (row.return != '' && row.return != undefined) {
                    return "<input type='checkbox' class='editor-active return flat-blue' checked>";
                }
                return "<input type='checkbox' class='editor-active return flat-blue'>";
            }
            return null;
        }
    },
    { name: 'PRICE_HIS_H', data: 'PRICE_HIS_H', visible: false, searchable: false },
    { name: 'PRICE_HIS_L', data: 'PRICE_HIS_L', visible: false, searchable: false },
    { name: 'ISMODIFY', data: 'ISMODIFY', visible: false, searchable: false },
    { name: 'UNI_SPEC_STATUS', data: 'UNI_SPEC_STATUS', visible: false, searchable: false },
    { name: 'SN', data: 'SN', visible: false, searchable: false },
    { name: 'MODIFY_TYPE', data: 'MODIFY_TYPE', visible: false, searchable: false }
];
var tableColumn = tableFirstColumn.concat(tableOtherColumn);

var table_object = {
    paging: false,
    info: false,
    ordering: false,
    scrollY: '600px',
    scrollX: true,
    scrollCollapse: true,
    fixedColumns: {
        leftColumns: 6
    },
    select: {
        style: 'os',
        selector: 'td:not(:last-child)'
    },
    columns: tableColumn,
    buttons: [
        {
            text: '<i class="fa fa-pencil-square-o"></i>',
            titleAttr: '細項編輯',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                SaveData();
                var data = dt.row({ selected: true }).data();
                if (data == undefined) {
                    $("#alertModal .modal-body").html("Please slect one item!");
                    $("#alertModal").modal('show');
                    return false;
                }

                if (data.ISMODIFY === 'Y') {
                    GetEnglishName(data.PART_LEVEL);
                    var parentpartlevel = data.PARTNUMBER_PARENT.substring(0, 2);
                    BindMaterialGroupParts(parentpartlevel, data.PART_LEVEL, data.ENGLISH_NAME, data.MTL_GROUP, data.PARTS_GROUP, data.MTL_PARTS, data.SN);
                    setTimeout(function () {
                        $('.modal-edit #partlevel').val(data.PART_LEVEL);
                        $('.modal-edit #englishname').val(data.ENGLISH_NAME);
                        $('.modal-edit #sn').val(data.SN);
                    }, 500);


                    $('.modal-edit').modal('show');
                }
                else {
                    $("#alertModal .modal-body").html("沒有權限修改");
                    $("#alertModal").modal('show');
                }
            }
        }
    ],
    rowCallback: function (row, data, index) {
        //$(row).find('td').removeClass('warning');
        //if (data.MODIFY_TYPE == "A" || data.MODIFY_TYPE == "NEW" || data.return == "return") {
        //    $(row).find('td').addClass('warning');
        //}

        $(row).find('td').css('background-color', '');
        var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
        var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
        var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
        var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
        if (data.ISAPPROVED !== 'Y' && !hasSubTotal && data.PART_TYPE == part_type && data.MTL_PARTS !== '-') {
            $(row).find('td').css('background-color', '#FFE8E8');
        }
    },
    drawCallback: function (settings) {
        $('input[type="checkbox"].flat-blue').iCheck({
            checkboxClass: 'icheckbox_flat-blue'
        });
    },
    initComplete: function (settings, json) {
        var tableid = $(this).attr('id');
        var appendDOM = '#' + tableid + '_wrapper .col-sm-6:eq(0)';
        $(this).DataTable().buttons().container()
            .appendTo(appendDOM);

        $('[data-toggle="tooltip"]').tooltip();

        var table_data = $(this).DataTable().rows().data();
        var iscalculate_all_show = table_data.filter(function (data) {
            var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
            var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            return !hasSubTotal && data.ISMODIFY === 'Y';
        }).length === 0;
        if (iscalculate_all_show)
            $('.iscalculate_all[data-tableid="#' + tableid + '"]').addClass('hidden');

        var iscalculate_all_status = table_data.filter(function (data) {
            var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
            var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            return !hasSubTotal && data.ISMODIFY === 'Y';
        }).length === table_data.filter(function (data) {
            var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
            var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            return !hasSubTotal && data.ISMODIFY === 'Y' && data.ISCALCULATE === 'Y'
        }).length;
        if (iscalculate_all_status)
            $('.iscalculate_all[data-tableid="#' + tableid + '"]').prop('checked', true);
        else
            $('.iscalculate_all[data-tableid="#' + tableid + '"]').prop('checked', false);

        var isassigner_all_show = table_data.filter(function (data) {
            var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
            var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            return !hasSubTotal && data.MTL_GROUP === 'MOH' && data.ISMODIFY === 'Y';
        }).length === 0;
        if (isassigner_all_show)
            $('.isassigner_all[data-tableid="#' + tableid + '"]').addClass('hidden');

        var isassigner_all_status = table_data.filter(function (data) {
            var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
            var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            return !hasSubTotal && data.MTL_GROUP === 'MOH' && data.ISMODIFY === 'Y'
        }).length === table_data.filter(function (data) {
            var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
            var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
            var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
            var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
            return !hasSubTotal && data.MTL_GROUP === 'MOH' && data.ISMODIFY === 'Y' && data.ISASSIGNER === 'Y';
        }).length;
        if (isassigner_all_status)
            $('.isassigner_all[data-tableid="#' + tableid + '"]').prop('checked', true);
        else
            $('.isassigner_all[data-tableid="#' + tableid + '"]').prop('checked', false);
    }
};

$(function () {
    domTableLoad();
    var bu = $('#BU').val();
    var phase_id = $('#phase_id').val();
    GetPartLevelArray();

    var table_ajax = {
        url: 'ProductReview_GetNormalParts',
        method: 'POST',
        data: {
            rsi_no: rsi_no,
            bu: bu,
            phase_id: phase_id,
            part_type: part_type
        },
        dataSrc: ''
    };
    table_object.ajax = table_ajax;
    var table = $('#normalPartsTable').DataTable(table_object);

    var specialPartsTable = $('#specialContent table');
    $.each(specialPartsTable, function (index, value) {
        var group_id = $(value).data('groupId');
        var specialtableajax = {
            url: 'ProductReview_GetSpecialParts',
            method: 'POST',
            data: {
                rsi_no: rsi_no,
                part_type: part_type,
                group_id: group_id
            },
            dataSrc: ''
        };
        table_object.ajax = specialtableajax;
        var special_table = $(value).DataTable(table_object);
    });
});

var normalTableObject = {
    destroy: true,
    paging: false,
    info: false,
    ordering: false,
    scrollY: "500px",
    scrollX: true,
    scrollCollapse: true,
    processing: true,
    language: {
        loadingRecords: '&nbsp;',
        processing: '<i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i>'
    },
    select: {
        style: 'os',
        selector: 'td:not(:first-child,:last-child)'
    },
    ajax: {
        url: 'SourcerReview_GetBOMTableData',
        method: 'POST',
        data: {
            rsi_no: rsi_no,
            part_type: part_type,
            phase_id: phase_id
        },
        dataSrc: ''
    },
    columns: [
        { name: 'icon', data: null, defaultContent: '', className: 'text-center' },
        { name: '', data: 'PARTNUMBER_TOP', visible: false },
        { name: '', data: 'PARTNUMBER_PARENT', visible: false },
        { name: '', data: 'PARTNUMBER_CHILD', visible: false },
        {
            name: '', data: 'DISPLAYPARTNO', className: 'whitespacepre',
            render: function (data, type, row, meta) {
                if (row.EOL_STATUS === 'Y')
                    data = data + '<span class="text-danger">@</span>';
                if (row.UNI_SPEC_STATUS === 'Y')
                    data = data + '<span class="text-danger">!</span>';
                return data;
            }
        },
        { name: '', data: 'BOM_PATH', visible: false },
        { name: '', data: 'BOM_LEVEL' },
        { name: 'MTL_GROUP', data: 'MTL_GROUP' },
        { name: 'PARTS_GROUP', data: 'PARTS_GROUP' },
        { name: 'MTL_PARTS', data: 'MTL_PARTS' },
        { name: 'PART_TYPE', data: 'PART_TYPE' },
        { name: 'ENGLISH_NAME', data: 'ENGLISH_NAME' },
        { name: 'PART_LEVEL', data: 'PART_LEVEL', visible: false },
        { name: 'PART_DESC', data: 'PART_DESC', className: 'text-ellipsis' },
        { name: 'MAKER_SOURCE', data: 'MAKER_SOURCE', visible: false },
        { name: 'MAKER_PART_NO', data: 'MAKER_PART_NO', visible: false },
        { name: 'PART_SPEC', data: 'PART_SPEC' },
        { name: 'SPEC_DEF', data: 'SPEC_DEF', className: 'text-ellipsis', visible: false },
        {
            name: 'RELEASE_DATE', data: 'RELEASE_DATE', defaultContent: '', visible: false,
            render: function (data, type, row, meta) {
                var date = ConvertDate(data);
                if (date == '1/1/1') data = '';
                return date;
            }
        },
        { name: 'USAGE', data: 'USAGE', defaultContent: '', className: 'text-right' },
        { name: 'PART_UNIT', data: 'PART_UNIT' },
        { name: 'EOL_STATUS', data: 'EOL_STATUS', visible: false },
        { name: 'UNI_SPEC_STATUS', data: 'UNI_SPEC_STATUS', visible: false },
        { name: 'PRICE_PM', data: 'PRICE_PM', className: 'text-right', defaultContent: '' },
        { name: 'PMAMOUNT', data: 'PMAMOUNT', className: 'text-right', defaultContent: '' },
        {
            name: 'ISCALCULATE', data: 'ISCALCULATE', defaultContent: '', render: function (data, type, row, meta) {
                if (data === 'Y')
                    return '<i class="fa fa-2x fa-check-square-o" aria-hidden="true"></i>';

                if (data === 'N')
                    return '<i class="fa fa-2x fa-square-o" aria-hidden="true"></i>';
            }
        },
        { name: 'ISAPPROVED', data: 'ISAPPROVED' },
        { name: 'REMARK', data: 'REMARK' },
        {
            name: 'Download File', data: 'FILE_STATUS', className: 'text-center', defaultContent: '',
            render: function (data, type, row, meta) {
                if (data === "Y") {
                    return "<button class='btn btn-success btn-sm' onclick='fileManagement(" + row.SN + ")'><i class='fa fa-file-text-o'></i></button>";
                }
                return null;
            }
        },
        { name: 'MODIFY_TYPE', data: 'MODIFY_TYPE', visible: false },
        { name: 'SN', data: 'SN', defaultContent: '', visible: false },
        { name: 'REASSIGN_FROM', data: 'REASSIGN_FROM', visible: false, searchable: false },
        { name: 'REASSIGN_TO', data: 'REASSIGN_TO', visible: false, searchable: false },
        { name: 'REASSIGN_DATE', data: 'REASSIGN_DATE', visible: false, searchable: false }
    ],
    buttons: [
        {
            text: 'Edit Valuation',
            titleAttr: 'Edit Valuation',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                var data = dt.row({ selected: true }).data();
                if (data == undefined) {
                    $("#alertModal .modal-body").html("Please slect one item!");
                    $("#alertModal").modal('show');
                    return false;
                }

                if (data.ISMODIFY === 'Y') {
                    var sn = data.SN;
                    var url = "/RSI/Sourcer/ProductValuation?rsi_no={0}&part_type={1}&phase_id={2}&sn={3}";
                    url = url.replace("{0}", rsi_no).replace("{1}", part_type).replace("{2}", phase_id).replace("{3}", sn);
                    window.open(url, "_blank", "width=1000,height=800,scrollbars=yes,resizable=yes");
                }
                else {
                    $("#alertModal .modal-body").html("沒有權限修改");
                    $("#alertModal").modal('show');
                }
            }
        }
    ],
    drawCallback: function (settings) {
        var api = this.api();
        var datas = api.rows({ page: 'current' }).data();
        var minus = $('.fa-changeall').hasClass('fa-minus-circle');
        for (var i = 0; i < datas.length; i++) {
            var node = api.rows({ page: 'current' }).nodes()[i];
            $(node).find('td:not(:eq(0))').removeClass('text-bold');
            if (i + 1 !== datas.length) {
                var currentLevel = parseInt(datas[i].BOM_LEVEL);
                var nextLevel = parseInt(datas[i + 1].BOM_LEVEL);
                if (currentLevel + 1 === nextLevel) {
                    if (currentLevel != 0) {
                        if (minus)
                            $(node).find('td:eq(0)').empty().append('<i class="fa fa-minus-circle" aria-hidden="true" data-row=' + i + '></i>');
                        else
                            $(node).find('td:eq(0)').empty().append('<i class="fa fa-plus-circle" aria-hidden="true" data-row=' + i + '></i>');
                    }
                    else {
                        $(node).find('td:eq(0)').empty().append('<i class="fa fa-minus-circle" aria-hidden="true" data-row=' + i + '></i>');
                    }

                    $(node).find('td:eq(1)').addClass('text-bold');
                }
            }

            var modify_type = datas[i].MODIFY_TYPE;
            $(node).removeClass('success warning danger');
            if (modify_type === 'A' || modify_type === 'NEW')
                $(node).addClass('success');

            if (modify_type === 'U')
                $(node).addClass('warning');

            if (modify_type === 'S')
                $(node).addClass('danger');
        }
    },
    initComplete: function (settings, json) {
        $(this).DataTable().buttons().container()
            .appendTo('#bomTable_wrapper .col-sm-6:eq(0)');

        setTimeout(function () {
            $('.fa-minus-circle.fa-changeall').click();
        }, 500);
    }
};

function domTableLoad() {
    $('#bomTable').DataTable(normalTableObject);
}

function ConvertDate(data) {
    var re = /-?\d+/;
    var m = re.exec(data);
    var d = new Date(parseInt(m[0]));
    var viewstring = '{0}/{1}/{2}';
    return viewstring.replace('{0}', d.getFullYear()).replace('{1}', d.getMonth() + 1).replace('{2}', d.getDate());
}

$(document).on("ifToggled", "input[type = 'checkbox'].return", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    if (o_data.return != "") {
        o_data.return = "";
    } else {
        o_data.return = "return";
    }
    thisTable.DataTable().row(thisTr).data(o_data).draw();
});

$(document).on("click", ".btn-return", function () {
    var tableId = $(this).data('tableid');
    var thisTable = $(tableId).DataTable();
    var o_data = thisTable.rows().data();

    var status = o_data.filter(function (data) { return data.ISMODIFY === 'Y' }).length == o_data.filter(function (data) { return data.return === 'return' && data.ISMODIFY === 'Y' }).length;
    var new_data = [];
    if (status) {
        $.each(o_data, function (index, value) {
            if (value.ISMODIFY === 'Y') value.return = '';
            new_data.push(value);
        });
    }
    else {
        $.each(o_data, function (index, value) {
            if (value.ISMODIFY === 'Y') value.return = "return";
            new_data.push(value);
        });
    }
    thisTable.clear();
    thisTable.rows.add(new_data).draw();
});

$(document).on("mouseover", ".custom_tooltip", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    $(this).tooltip({
        'placement': 'bottom',
        'title': '<table><thead><tr><th colspan="2">歷史參考價</th></tr></thead><tbody><tr><td style="word-break: break-all;">最高價</td><td>' + new Intl.NumberFormat('en-US').format(o_data.PRICE_HIS_H) + '</td></tr><tr><td style="word-break: break-all;">最低價</td><td>' + new Intl.NumberFormat('en-US').format(o_data.PRICE_HIS_L) + '</td></tr></tbody></table>',
        'html': true,
        'container': 'body',
        'viewport': { 'selector': this, 'padding': 0 }
    });
    $(this).tooltip('show');
});

$(document).on("mouseout", ".custom_tooltip", function () {
    $(this).tooltip('hide');
});

$(document).on("mouseover", "td.tooltipclass", function () {

    var thisTr = $(this).closest("tr");
    var o_data = $('#normalPartsTable').DataTable().row(thisTr).data();
    if (o_data.MTL_PARTS == 'RD DEFINE' && o_data.REASSIGN_FROM != null && o_data.REASSIGN_FROM != '') {
        var title = '<table><thead><tr><th colspan="2">轉簽資訊:</th></tr></thead><tbody><tr><td style="word-break: break-all;">Reassign From:</td><td>{0}</td></tr><tr><td style="word-break: break-all;">Reassign To:</td><td>{1}</td></tr><tr><td style="word-break: break-all;">Reassign Date:</td><td>{2}</td></tr></tbody></table>';
        if (o_data.REASSIGN_FROM != null)
            title = title.replace('{0}', o_data.REASSIGN_FROM);
        else
            title = title.replace('{0}', '');
        if (o_data.REASSIGN_TO != null)
            title = title.replace('{1}', o_data.REASSIGN_TO);
        else
            title = title.replace('{1}', '');
        if (o_data.REASSIGN_DATE != null)
            title = title.replace('{2}', ConvertDate(o_data.REASSIGN_DATE));
        else
            title = title.replace('{2}', '');

        $(this).tooltip({
            'placement': 'top',
            'container': 'body',
            'title': title,
            'html': true,
            'viewport': { 'selector': this, 'padding': 0 }
        });
        $(this).tooltip('show');
    }
});

$(document).on("mouseout", "td.tooltipclass", function () {
    $(this).tooltip('hide');
});

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    var hrefTab = $(this).attr("href");
    var table = $(hrefTab).find("table").eq(1).DataTable();
    table.columns.adjust().fixedColumns().relayout();
});

$(document).on('click', '.fa-minus-circle:not(.fa-changeall, .specialicon)', function () {
    $(this).removeClass('fa-minus-circle').addClass('fa-plus-circle');
    var row = $(this).data('row');
    var datatable = $('#bomTable').DataTable();
    var rowdata = datatable.rows({ page: 'current' }).data()[row];
    var datas = datatable.rows({ page: 'current' }).data();
    var rowLevel = parseInt(rowdata.BOM_LEVEL);
    for (var i = row + 1; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        if (currentLevel > rowLevel) {
            var node = datatable.rows({ page: 'current' }).nodes()[i];
            $(node).addClass('hidden');
        }
        else {
            break;
        }
    }

    $('#bomTable').DataTable().columns.adjust();
});

$(document).on('click', '.fa-plus-circle:not(.fa-changeall, .specialicon)', function () {
    $(this).removeClass('fa-plus-circle').addClass('fa-minus-circle');
    var row = $(this).data('row');
    var datatable = $('#bomTable').DataTable();
    var rowdata = datatable.rows({ page: 'current' }).data()[row];
    var datas = datatable.rows({ page: 'current' }).data();
    var hasplusicon;
    var rowLevel = parseInt(rowdata.BOM_LEVEL);
    for (var i = row + 1; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        if (currentLevel > rowLevel) {
            var node = datatable.rows({ page: 'current' }).nodes()[i];
            if (currentLevel == rowLevel + 1) {
                $(node).removeClass('hidden');
                hasplusicon = $(node).find('.fa').hasClass('fa-plus-circle');
            }

            if (!hasplusicon)
                $(node).removeClass('hidden');
        }
        else {
            break;
        }
    }

    $('#bomTable').DataTable().columns.adjust();
});

$(document).on('click', '.fa-minus-circle.fa-changeall', function () {
    $(this).removeClass('fa-minus-circle').addClass('fa-plus-circle');
    var datatable = $('#bomTable').DataTable();
    var datas = $('#bomTable').DataTable().rows().data();
    for (var i = 0; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        var node = datatable.row(i).node();
        if (currentLevel !== 0 && currentLevel !== 1) {
            $(node).addClass('hidden');
        }

        var hasicon = $(node).find('td:eq(0)').has('i');
        if (hasicon && currentLevel !== 0) {
            $(node).find('td:eq(0) i').removeClass('fa-minus-circle').addClass('fa-plus-circle');
        }
    }

    $('#bomTable').DataTable().columns.adjust();
});

$(document).on('click', '.fa-plus-circle.fa-changeall', function () {
    $(this).removeClass('fa-plus-circle').addClass('fa-minus-circle');
    var datatable = $('#bomTable').DataTable();
    var datas = $('#bomTable').DataTable().rows().data();
    for (var i = 0; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        var node = datatable.row(i).node();
        if (currentLevel !== 0 && currentLevel !== 1) {
            $(node).removeClass('hidden');
        }

        var hasicon = $(node).find('td:eq(0)').has('i');
        if (hasicon && currentLevel !== 0) {
            $(node).find('td:eq(0) i').removeClass('fa-plus-circle').addClass('fa-minus-circle');
        }
    }

    $('#bomTable').DataTable().columns.adjust();
});

$(document).on("change", "input[name='isassigner']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var tableid = thisTable.attr('id');
    var o_data = thisTable.DataTable().row(thisTr).data();
    if ($(this).prop("checked")) {
        o_data.ISASSIGNER = "Y";
    } else {
        o_data.ISASSIGNER = "N";
    }

    var all_data = thisTable.DataTable().rows().data();
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
    var isassigner_all_status = all_data.filter(function (data) {
        var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
        var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
        var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
        var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
        return !hasSubTotal && data.MTL_GROUP === 'MOH' && data.ISMODIFY === 'Y';
    }).length === all_data.filter(function (data) {
        var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
        var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
        var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
        var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
        return !hasSubTotal && data.MTL_GROUP === 'MOH' && data.ISMODIFY === 'Y' && data.ISASSIGNER === 'Y';
    }).length;
    if (isassigner_all_status)
        $('.isassigner_all[data-tableid="#' + tableid + '"]').prop('checked', true);
    else
        $('.isassigner_all[data-tableid="#' + tableid + '"]').prop('checked', false);
});

$(document).on("change", "input[name='pmPrice']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    o_data.PRICE_PM = $(this).val();
    o_data.ISASSIGNER = 'Y';

    var rsi_no = $('#RSI_NO').val();
    o_data.pricetrend = [];
    $.each(pricetrendArray, function (index, value) {
        var pricetrendObject = {
            RSI_NO: rsi_no,
            SN: o_data.SN,
            ID: value,
            PRICE: o_data.PRICE_PM
        };
        o_data.pricetrend.push(pricetrendObject);
    });

    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on("change", "input[name='moq']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    o_data.MOQ = $(this).val();
    o_data.ISASSIGNER = 'Y';
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on("change", "input[name='mockup']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    o_data.MOCKUP = $(this).val();
    o_data.ISASSIGNER = 'Y';
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on("change", "input[name='tooling']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    o_data.TOOLING = $(this).val();
    o_data.ISASSIGNER = 'Y';
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on("change", "input[name='fpca_pcba']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    o_data.FPCA_PCBA = $(this).val();
    o_data.ISASSIGNER = 'Y';
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on("change", "textarea[name='REMARK_PM']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    o_data.REMARK_PM = $(this).val();
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on("change", "textarea[name='REMARK_PUR']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    o_data.REMARK_PUR = $(this).val();
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on("change", ".iscalculate", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var tableid = thisTable.attr('id');
    var o_data = thisTable.DataTable().row(thisTr).data();
    if ($(this).prop("checked")) {
        o_data.ISCALCULATE = "Y";
    } else {
        o_data.ISCALCULATE = "N";
    }
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);

    var all_data = thisTable.DataTable().rows().data();
    var iscalculate_all_status = all_data.filter(function (data) {
        var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
        var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
        var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
        var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
        return !hasSubTotal && data.ISMODIFY === 'Y';
    }).length === all_data.filter(function (data) {
        var isMaterialParts = data.MTL_PARTS !== null ? data.MTL_PARTS === '小計' : false;
        var isPartsGroup = data.PARTS_GROUP !== null ? data.PARTS_GROUP === '小計' : false;
        var isMaterialGroup = data.MTL_GROUP != null ? data.MTL_GROUP === '小計' || data.MTL_GROUP === '總計' : false;
        var hasSubTotal = isMaterialParts || isPartsGroup || isMaterialGroup;
        return !hasSubTotal && data.ISMODIFY === 'Y' && data.ISCALCULATE === 'Y';
    }).length;
    if (iscalculate_all_status)
        $('.iscalculate_all[data-tableid="#' + tableid + '"]').prop('checked', true);
    else
        $('.iscalculate_all[data-tableid="#' + tableid + '"]').prop('checked', false);
});

$(document).on('expanded.pushMenu', function () {
    console.log('expanded');
    setTimeout(function () {
        $('#bomTable').DataTable().columns.adjust();
        $('#normalPartsTable').DataTable().columns.adjust();
        var selector = 'div#specialContent div.active';
        $(selector).find('table:eq(1)').DataTable().columns.adjust();
    }, 500);
});

$(document).on('collapsed.pushMenu', function () {
    console.log('collapsed');
    setTimeout(function () {
        $('#bomTable').DataTable().columns.adjust();
        $('#normalPartsTable').DataTable().columns.adjust();
        var selector = 'div#specialContent div.active';
        $(selector).find('table:eq(1)').DataTable().columns.adjust();
    }, 500);
});

function GetPartLevelArray() {
    $.ajax({
        url: '/RSI/RD/PartNoSearch_PartLevel',
        type: 'POST',
        success: function (response) {
            $('#partlevel').empty();
            var optionString = '<option value=""></option>';
            $('#partlevel').append(optionString);
            $.each(response, function (index, value) {
                optionString = '<option value="{0}">{1}</option>'.replace('{0}', value).replace('{1}', value);
                $('#partlevel').append(optionString);
            });
        }
    });
}

function GetEnglishName(query) {
    $.ajax({
        url: '/RSI/RD/PartNoSeach_EnglishName',
        async: false,
        type: 'POST',
        data: {
            part_level: query
        },
        success: function (response) {
            $('#englishname').empty();
            var optionString = '<option value=""></option>';
            $('#englishname').append(optionString);
            $.each(response, function (index, value) {
                optionString = '<option value="{0}">{1}</option>'.replace('{0}', value).replace('{1}', value);
                $('#englishname').append(optionString);
            });
        }
    });
}

function BindMaterialGroupParts(parent_part_level, part_level, english_name, mtl_group, parts_group, mtl_parts, sn) {
    var rsi_no = $('#RSI_NO').val();
    var partlevel = part_level;
    var englishname = english_name;
    var parentpartlevel = parent_part_level;

    $.ajax({
        url: '/RSI/RD/RDReview_GetMaterialGroupMaterialParts',
        type: 'POST',
        data: {
            rsi_no: rsi_no,
            part_level: partlevel,
            english_name: englishname,
            parent_part_level: parentpartlevel,
            sn: sn
        },
        success: function (response) {
            $('.modal-edit #mtlgroup').empty();
            $('.modal-edit #partgroup').empty();
            $('.modal-edit #mtlparts').empty();
            if (response.length > 0) {
                mterialGroupPartsObject = response;
                BindMtlGroup();
                if (mtl_group != null) {
                    BindPartsGroup(mtl_group);
                    $('.modal-edit #mtlgroup').val(mtl_group);
                }

                if (mtl_group != null && parts_group != null) {
                    BindMtlParts(mtl_group, parts_group);
                    $('.modal-edit #partgroup').val(parts_group);
                }

                if (mtl_group != null && parts_group != null && mtl_parts != null) {
                    $('.modal-edit #mtlparts').val(mtl_parts);
                }
            }
            else {
                var optionString = '<option value="RD DEFINE">RD DEFINE</option>';
                $('.modal-edit #mtlgroup').append(optionString);
                $('.modal-edit #partgroup').append(optionString);
                $('.modal-edit #mtlparts').append(optionString);
            }
        }
    });
}

function BindMtlGroup() {
    var mtlGroup = [];
    $.each(mterialGroupPartsObject, function (index, value) {
        var itemMtlGroup = value.MTL_GROUP;
        if (mtlGroup.indexOf(itemMtlGroup) < 0)
            mtlGroup.push(itemMtlGroup);
    });

    $('#mtlgroup').empty();
    if (mtlGroup.length > 1) {
        $('#mtlgroup').append('<option value=""></option>');
    } else {
        BindPartsGroup(mtlGroup[0]);
    }
    $.each(mtlGroup, function (index, value) {
        var optionString = '<option value="{0}">{1}</option>';
        optionString = optionString.replace('{0}', value).replace('{1}', value);
        $('#mtlgroup').append(optionString);
    });
}

function BindPartsGroup(mtlgroup) {
    var partsgroup = [];
    var filterdata = mterialGroupPartsObject.filter(function (d) { return d.MTL_GROUP });
    if (mtlgroup != null)
        filterdata = mterialGroupPartsObject.filter(function (d) { return d.MTL_GROUP === mtlgroup });

    $.each(filterdata, function (index, value) {
        var item = value.PARTS_GROUP === null ? '' : value.PARTS_GROUP;
        if (partsgroup.indexOf(item) < 0)
            partsgroup.push(item);
    });

    $('select#partgroup').empty();
    if (partsgroup.length > 1) {
        $('select#partgroup').append('<option value=""></option>');
    } else {
        BindMtlParts(mtlgroup, partsgroup[0]);
    }
    $.each(partsgroup, function (index, value) {
        var optionString = '<option value="{0}">{1}</option>';
        optionString = optionString.replace('{0}', value).replace('{1}', value);
        $('select#partgroup').append(optionString);
    });
}

function BindMtlParts(mtlgroup, partsgroup) {
    var mtlParts = [];
    var filterdata = mterialGroupPartsObject.filter(function (d) { return d.MTL_GROUP });
    if (mtlgroup != null && partsgroup != null)
        filterdata = mterialGroupPartsObject.filter(function (d) { return d.MTL_GROUP === mtlgroup && d.PARTS_GROUP == partsgroup });

    $.each(filterdata, function (index, value) {
        var item = value.MTL_PARTS === null ? '' : value.MTL_PARTS;
        if (mtlParts.indexOf(item) < 0)
            mtlParts.push(item);
    });

    $('select#mtlparts').empty();
    if (mtlParts.length > 1) {
        $('select#mtlparts').append('<option value=""></option>');
    }

    $.each(mtlParts, function (index, value) {
        var optionString = '<option value="{0}">{1}</option>';
        optionString = optionString.replace('{0}', value).replace('{1}', value);
        $('select#mtlparts').append(optionString);
    });
}

$(document).on('change', '#mtlgroup', function () {
    var mtlgroup = $(this).val();
    BindPartsGroup(mtlgroup);
});

$(document).on('change', '#partgroup', function () {
    var mtlgroup = $('#mtlgroup').val();
    var partgroup = $(this).val();
    BindMtlParts(mtlgroup, partgroup);
});

$(document).on('click', '#btn-modal-save', function () {
    if (InspectPartObject()) {
        var model = BindPartObject();
        $.ajax({
            url: 'ModalEdit_Save',
            method: 'POST',
            data: {
                model: model
            },
            success: function (data) {
                if (data) {
                    $('.modal-edit').modal('hide');
                    reloadDataTable();
                    return;
                }
                alert('save fail');
            }
        });
    }
});

function InspectPartObject() {
    var mtl_group = $('#mtlgroup').val();
    var parts_group = $('#partgroup').val();
    var mtl_parts = $('#mtlparts').val();

    if (mtl_group == '') {
        $('#mtlgroup').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請選擇Material Group');
        return false;
    }

    if (parts_group == '') {
        $('#partgroup').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請選擇Part Groups');
        return false;
    }

    if (mtl_parts == '') {
        $('#mtlparts').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請選擇Material Parts');
        return false;
    }
    return true;
}

function BindPartObject() {
    var mtl_group = $('#mtlgroup').val();
    var parts_group = $('#partgroup').val();
    var mtl_parts = $('#mtlparts').val();
    var sn = $('#sn').val();

    var result = {
        MTL_GROUP: mtl_group,
        PARTS_GROUP: parts_group,
        MTL_PARTS: mtl_parts,
        SN: sn
    };
    return result;
}

function reloadDataTable() {
    $('#normalPartsTable').DataTable().ajax.reload();
    var selector = 'div#specialContent div.active';
    $(selector).find('table:eq(1)').DataTable().ajax.reload();
    $('.fa-changeall').removeClass('fa-plus-circle').addClass('fa-minus-circle');
    $('#bomTable').DataTable().ajax.reload();
}

