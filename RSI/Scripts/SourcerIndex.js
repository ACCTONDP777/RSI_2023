var rsi_no = $("#RSI_NO").val();
var part_type = $('#PartType').val();
var phase_id = $('#phase_id').val();
var bu = $('#BU').val();
var projectname = $('#PROJECT_NAME').val();
var priceidentity = $('#priceidentity').val();
var pricetrendJson = $('#pricetrendJson').val();
var pricetrendArray = JSON.parse(pricetrendJson);

var tableFirstColumn = [
    //{
    //    name: 'ReAssign', data: null, defaultContent: '', width: '50px', className: 'text-center',
    //    render: function (data, type, row) {
    //        //if (row.ISMODIFY === 'Y' && row.MODIFY_TYPE === 'NEW')
    //        if (row.ISMODIFY === 'Y' && row.MTL_PARTS === 'RD DEFINE') {
    //            return '<i class="fa fa-user-circle-o" aria-hidden="true" style="cursor:pointer; font-size:25px;" onclick="showReAssign(\'' + row.MTL_PARTS + '\', \'' + row.PART_TYPE + '\', ' + row.SN + ')"></i>';
    //        }
    //        return null;
    //    }
    //},
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
        name: 'PRICE', data: 'PRICE', width: '80px', className: 'text-right', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y') {
                if (row.MTL_GROUP === '總計')
                    return new Intl.NumberFormat('en-US').format(data);

                if (data == null)
                    return "<input name='sourcerPrice' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px;' disabled>";

                return "<input name='sourcerPrice' type='text' value='" + new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px;' disabled>";
            }
            return null;
        }
    },
    {
        name: 'PRICE_PM', data: 'PRICE_PM', width: '120px', className: 'text-right', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y') {
                if (row.MTL_GROUP === '總計')
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);

                var result = '<input type="checkbox" name="isassigner" style="margin-right:3px;" />';
                if (row.ISASSIGNER === 'Y') {
                    result = '<input type="checkbox" name="isassigner" style="margin-right:3px;" checked="true" />';
                }

                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return result + "<input name='pmPrice' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";

                return result + "<input name='pmPrice' type='text' value='" + new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }
            return null;
        }
    },
    {
        name: 'MOQ', data: 'MOQ', width: '80px', className: 'text-right', defaultContent: '', searchable: false, visible: (pmprice_config > 0) ? true : false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y') {
                if (row.MTL_GROUP === '總計')
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);

                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return "<input name='moq' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";

                return "<input name='moq' type='text' value='" + new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }
            return null;
        }
    },
    {
        name: 'MOCKUP', data: 'MOCKUP', width: '80px', className: 'text-right', defaultContent: '', searchable: false, visible: (pmprice_config > 1) ? true : false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y') {
                if (row.MTL_GROUP === '總計')
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);

                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return "<input name='mockup' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";

                return "<input name='mockup' type='text' value='" + new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }
            return null;
        }
    },
    {
        name: 'TOOLING', data: 'TOOLING', width: '80px', className: 'text-right', defaultContent: '', searchable: false, visible: (pmprice_config > 2) ? true : false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y') {
                if (row.MTL_GROUP === '總計')
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);

                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return "<input name='tooling' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";

                return "<input name='tooling' type='text' value='" + new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }
            return null;
        }
    },
    {
        name: 'FPCA/PCBA', data: 'FPCA_PCBA', width: '80px', className: 'text-right', defaultContent: '', searchable: false, visible: (pmprice_config > 2) ? true : false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y') {
                if (row.MTL_GROUP === '總計')
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);

                var color = row.PRICE != data ? "red" : "black";
                if (data == null)
                    return "<input name='fpca_pcba' type='text' value='' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";

                return "<input name='fpca_pcba' type='text' value='" + new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data) + "' class='form-control' style='text-align:right; width:80px; font-size:12px; color:" + color + "'>";
            }
            return null;
        }
    },
    {
        name: 'Valuation', data: 'ISCALCULATE', width: '50px', className: 'text-center', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY === "Y") {
                if (row.MTL_GROUP === '總計')
                    return null;
                if (data === "Y") {
                    return "<input type='checkbox' class='iscalculate' checked>";
                }
                return "<input type='checkbox' class='iscalculate'>";
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
            if (row.ISMODIFY == "Y") {
                if (row.USAGE != null && row.PRICE != null)
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }
            return null;
        }
    },
    {
        name: 'PM_Amount', data: 'PMAMOUNT', defaultContent: '', width: '70px', className: 'text-right', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY == "Y") {
                if (row.USAGE != null && row.PRICE != null)
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }
            return null;
        }
    }
];
$.each(pricetrendArray, function (index, value) {
    var priceColumn = {
        name: value, data: 'pricetrend[]', className: 'text-right', defaultContent: '', render: function (data, type, row) {
            if (data.length == 0)
                data = row.PRICE;
            else {
                var item = data.filter(function (d) { return d.ID == value });
                if (item.length > 0)
                    data = item[0].PRICE;
                else
                    data = row.PRICE;
            }

            if (row.ISMODIFY === 'Y') {
                if (row.MTL_GROUP === '總計')
                    return null;
                var name = "pricetrend" + value;
                if (data == null)
                    return "<input name='" + name + "' type='text' value='' class='form-control pricetrend' style='text-align:right; width:80px; font-size:12px;'>";
                return "<input name='" + name + "' type='text' value='" + new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data) + "' class='form-control pricetrend' style='text-align:right; width:80px; font-size:12px;'>";
            }
            return null;
        }
    };
    tableFirstColumn.push(priceColumn);
});
var tableOtherColumn = [
    { name: 'SOURCE', data: 'SOURCE', width: '80px', searchable: false },
    { name: 'SOURCE_NO', data: 'SOURCE_NO', width: '80px', searchable: false, },
    { name: 'REMARK', data: 'REMARK', width: '80px' },
    {
        name: 'REMARK_PM', data: 'REMARK_PM', width: '120px', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            data = (data == null) ? '' : data;
            if (row.ISMODIFY === "Y") {
                if (row.MTL_GROUP === '總計')
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
                if (row.MTL_GROUP === '總計')
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
        name: 'UPDATED_DATE', data: 'UPDATED_DATE', searchable: false,
        render: function (data, type, row) {
            var date = ConvertDate(data);
            if (date == '1/1/1') date = '';
            return date;
        }
    },
    { name: 'EMP_NAME', data: 'EMP_NAME', searchable: false },
    {
        name: 'Return', data: null, defaultContent: '', width: '70px', className: 'text-center', defaultContent: '', searchable: false,
        render: function (data, type, row) {
            if (row.ISMODIFY === 'Y') {
                if (row.MTL_GROUP === '總計') return null;

                if (row.return != '' && row.return != undefined) {
                    return "<input type='checkbox' class='editor-active return flat-blue' checked>";
                }
                return "<input type='checkbox' class='editor-active return flat-blue'>";
            }
            return null;
        }
    },
    { name: 'BU', data: 'BU', visible: false, searchable: false },
    { name: 'SN', data: 'SN', visible: false, searchable: false },
    { name: 'CURRENCY', data: 'CURRENCY', visible: false, searchable: false },
    { name: 'RATE', data: 'RATE', visible: false, searchable: false },
    { name: 'PRICE_HIS_H', data: 'PRICE_HIS_H', visible: false, searchable: false },
    { name: 'PRICE_HIS_L', data: 'PRICE_HIS_L', visible: false, searchable: false },
    { name: 'PHASE_ID', data: 'PHASE_ID', visible: false, searchable: false },
    { name: 'ISMODIFY', data: 'ISMODIFY', visible: false, searchable: false },
    { name: 'UNI_SPEC_STATUS', data: 'UNI_SPEC_STATUS', visible: false, searchable: false },
    { name: 'errorInfo', data: null, defaultContent: '', visible: false, searchable: false },
    { name: 'ISASSIGNER', data: 'ISASSIGNER', visible: false, searchable: false },
    { name: 'MODIFY_TYPE', data: 'MODIFY_TYPE', visible: false, searchable: false },
    { name: 'REASSIGN_FROM', data: 'REASSIGN_FROM', visible: false, searchable: false },
    { name: 'REASSIGN_TO', data: 'REASSIGN_TO', visible: false, searchable: false },
    { name: 'REASSIGN_DATE', data: 'REASSIGN_DATE', visible: false, searchable: false }
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
    columns: tableColumn,
    buttons: [
        {
            text: '<i class="fa fa-exchange"></i>',
            titleAttr: 'Reassign',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                MaterialSourcerReassign();
            }
        }
    ],
    rowCallback: function (row, data, index) {
        $(row).find('td').removeClass('success danger warning errorInfo');
        if (data.MODIFY_TYPE === 'A' || data.return === 'return' || data.MODIFY_TYPE === 'NEW') {
            $(row).find('td').addClass('warning');
        }

        if (data.PHASE_ID === 40) {
            $(row).find('td').addClass('warringInfo');
        }

        if (data.errorInfo != undefined) {
            //$(row).find('td').addClass('errorInfo');danger
            $(row).find('td').addClass('danger');
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
        var assign = table_data.filter(function (data) { return data.ISMODIFY === 'Y' && data.MTL_PARTS == 'RD DEFINE' }).length;
        //var assign = table_data.filter(function (data) { return data.ISMODIFY === 'Y' && data.MODIFY_TYPE == 'NEW' }).length;
        if (assign == 0)
            $(this).DataTable().columns([0]).visible(false);  //判斷是否要長出Reassign欄位

        var iscalculate_all_show = table_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' }).length === 0;
        if (iscalculate_all_show)
            $('.iscalculate_all[data-tableid="#' + tableid + '"]').addClass('hidden');

        var iscalculate_all_status = table_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' }).length === table_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' && data.ISCALCULATE === 'Y' }).length;
        if (iscalculate_all_status)
            $('.iscalculate_all[data-tableid="#' + tableid + '"]').prop('checked', true);
        else
            $('.iscalculate_all[data-tableid="#' + tableid + '"]').prop('checked', false);

        var isassigner_all_show = table_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' }).length === 0;
        if (isassigner_all_show)
            $('.isassigner_all[data-tableid="#' + tableid + '"]').addClass('hidden');

        var isassigner_all_status = table_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' }).length === table_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' && data.ISASSIGNER === 'Y' }).length;
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

    var table_ajax = {
        url: 'Sourcer_TableView',
        method: 'POST',
        data: {
            rsi_no: rsi_no,
            part_type: part_type,
            bu: bu,
            phase_id: phase_id
        },
        dataSrc: 'normal_Detail'
    };
    table_object.ajax = table_ajax;
    var table = $('#normalPartsTable').DataTable(table_object);

    var specialPartsTable = $('#specialContent table');
    $.each(specialPartsTable, function (index, value) {
        var group_id = $(value).data('groupId');
        var specialtableajax = {
            url: 'Sourcer_GetSpecialPartsData',
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

    $('[data-toggle="tooltip"]').tooltip();

    var chk_hidden = $("#file").hasClass('hidden');
    if (chk_hidden)
        $("#file + .input-group").addClass('hidden');
});

$(document).on("mouseover", "input[name='sourcerPrice']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    var title = '<table><thead><tr><th colspan="2">歷史參考價</th></tr></thead><tbody><tr><td style="word-break: break-all;">最高價</td><td>{0}</td></tr><tr><td style="word-break: break-all;">最低價</td><td>{1}</td></tr></tbody></table>';
    if (o_data.PRICE_HIS_H != null)
        title = title.replace('{0}', new Intl.NumberFormat('en-US').format(o_data.PRICE_HIS_H));
    else
        title = title.replace('{0}', '');
    if (o_data.PRICE_HIS_L != null)
        title = title.replace('{1}', new Intl.NumberFormat('en-US').format(o_data.PRICE_HIS_L));
    else
        title = title.replace('{1}', '');
    $(this).tooltip({
        'placement': 'bottom',
        'container': 'body',
        'title': title,
        'html': true,
        'viewport': { 'selector': this, 'padding': 0 }
    });
    $(this).tooltip('show');
});

$(document).on("mouseout", "input[name='sourcerPrice']", function () {
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
            title = title.replace('{2}', ConvertDateTime(o_data.REASSIGN_DATE));
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

$(document).on("change", "input[name='pmPrice']", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    o_data.PRICE_PM = $(this).val() == '' ? null : $(this).val();
    if (o_data.PRICE_PM != null) {
        o_data.ISASSIGNER = 'Y';
    }

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
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);

    var all_data = thisTable.DataTable().rows().data();
    var isassigner_all_status = all_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' }).length === all_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' && data.ISASSIGNER === 'Y' }).length;
    if (isassigner_all_status)
        $('.isassigner_all[data-tableid="#' + tableid + '"]').prop('checked', true);
    else
        $('.isassigner_all[data-tableid="#' + tableid + '"]').prop('checked', false);
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

$(document).on("ifToggled", "input[type = 'checkbox'].return", function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    if (o_data.return != "" && o_data.return != undefined) {
        o_data.return = "";
    } else {
        o_data.return = "return";
    }
    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on("click", ".btn-return", function () {
    var tableId = $(this).data('tableid');
    var thisTable = $(tableId).DataTable();
    var o_data = thisTable.rows().data();

    var status = o_data.filter(function (data) { return data.ISMODIFY === 'Y' && (data.MTL_GROUP !== '總計') }).length == o_data.filter(function (data) { return data.return === 'return' && data.ISMODIFY === 'Y' && (data.MTL_GROUP !== '總計') }).length;
    var new_data = [];
    if (status) {
        $.each(o_data, function (index, value) {
            if (value.ISMODIFY === 'Y') value.return = '';
            new_data.push(value);
        });
    }
    else {
        $.each(o_data, function (index, value) {
            if (value.ISMODIFY === 'Y') value.return = 'return';
            new_data.push(value);
        });
    }
    thisTable.clear();
    thisTable.rows.add(new_data).draw(false);
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
    var iscalculate_all_status = all_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' }).length === all_data.filter(function (data) { return data.MTL_GROUP !== '總計' && data.ISMODIFY === 'Y' && data.ISCALCULATE === 'Y' }).length;
    if (iscalculate_all_status)
        $('.iscalculate_all[data-tableid="#' + tableid + '"]').prop('checked', true);
    else
        $('.iscalculate_all[data-tableid="#' + tableid + '"]').prop('checked', false);
});

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    var hrefTab = $(this).attr("href");
    var table = $(hrefTab).find("table").eq(1).DataTable();
    table.columns.adjust().fixedColumns().relayout();
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
        {
            name: 'USAGE', data: 'USAGE', defaultContent: '', className: 'text-right'
        },
        { name: 'PART_UNIT', data: 'PART_UNIT' },
        { name: 'EOL_STATUS', data: 'EOL_STATUS', visible: false },
        { name: 'UNI_SPEC_STATUS', data: 'UNI_SPEC_STATUS', visible: false },
        {
            name: 'PRICE_PM', data: 'PRICE_PM', className: 'text-right', defaultContent: '', render: function (data, type, row) {
                if (data == null)
                    return null;
                return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }
        },
        {
            name: 'PMAMOUNT', data: 'PMAMOUNT', className: 'text-right', defaultContent: '', render: function (data, type, row) {
                if (row.USAGE != null && row.PRICE_PM != null)
                    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 6 }).format(data);
            }
        },
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
        {
            name: 'SN', data: 'SN', defaultContent: '', visible: false
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
            .appendTo('#normalTable_wrapper .col-sm-6:eq(0)');

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

function ConvertDateTime(data) {
    var re = /-?\d+/;
    var m = re.exec(data);
    var d = new Date(parseInt(m[0]));
    var viewstring = '{0}/{1}/{2} {3}:{4}';
    return viewstring.replace('{0}', d.getFullYear()).replace('{1}', d.getMonth() + 1).replace('{2}', d.getDate()).replace('{3}', d.getHours()).replace('{4}', d.getMinutes());
}

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

$(document).on('change', '.pricetrend', function () {
    var thisTable = $(this).closest("table");
    var thisTr = $(this).closest("tr");
    var o_data = thisTable.DataTable().row(thisTr).data();
    var name = $(this).attr('name');
    var id = name.replace('pricetrend', '');
    var rsi_no = $('#RSI_NO').val();
    var sn = o_data.SN;
    var price = $(this).val() == '' ? null : $(this).val();
    if (o_data.pricetrend.length > 0) {
        var item = o_data.pricetrend.filter(function (d) { return d.ID == id });
        if (item.length > 0) {
            for (var i = 0; i < o_data.pricetrend.length; i++) {
                if (o_data.pricetrend[i].ID == id) {
                    o_data.pricetrend[i].PRICE = price;
                }
            }
        }
        else {
            var object = {
                RSI_NO: rsi_no,
                SN: sn,
                ID: id,
                PRICE: price
            };
            o_data.pricetrend.push(object);
        }
    } else {
        var object = {
            RSI_NO: rsi_no,
            SN: sn,
            ID: id,
            PRICE: price
        };
        o_data.pricetrend.push(object);
    }


    thisTable.DataTable().row(thisTr).data(o_data).draw(false);
});

$(document).on('expanded.pushMenu', function () {
    setTimeout(function () {
        $('#bomTable').DataTable().columns.adjust();
        $('#normalPartsTable').DataTable().columns.adjust();
        var selector = 'div#specialContent div.active';
        $(selector).find('table:eq(1)').DataTable().columns.adjust();
    }, 500);
});

$(document).on('collapsed.pushMenu', function () {
    setTimeout(function () {
        $('#bomTable').DataTable().columns.adjust();
        $('#normalPartsTable').DataTable().columns.adjust();
        var selector = 'div#specialContent div.active';
        $(selector).find('table:eq(1)').DataTable().columns.adjust();
    }, 500);
});

function MaterialSourcerReassign() {
    var url = 'MaterialSourcerReassign?rsi_no=' + rsi_no + '&part_type=' + part_type + '&phase_id=' + phase_id + '&projectname=' + projectname;
    window.open(url, '', 'scrollbars=yes,resizable=yes, width=1200, height=800');
}

