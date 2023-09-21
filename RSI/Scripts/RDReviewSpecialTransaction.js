$(function () {
    window.moveTo(0, 0);
    window.resizeTo(screen.width, screen.height);

    BindNormalTable();
    BindSpecialTable();
});

var normalTable = $('#normalTable');
var specialTable = $('#specialTable');
function BindNormalTable() {
    var rsi_no = $('#rsi_no').val();
    $(normalTable).DataTable({
        destroy: true,
        paging: false,
        info: false,
        ordering: false,
        searching: false,
        scrollY: "700px",
        scrollX: true,
        scrollCollapse: true,
        ajax: {
            url: 'RDReview_NormalParts',
            method: 'POST',
            data: {
                rsi_no: rsi_no
            },
            dataSrc: ''
        },
        columns: [
            { name: '', data: null, defaultContent: '', className: 'text-center' },
            { name: '', data: null, defaultContent: '', className: 'text-center' },
            { name: '', data: 'DISPLAYPARTNO', className: 'whitespacepre' },
            { name: 'Level', data: 'BOM_LEVEL' },
            { name: 'English Name', data: 'ENGLISH_NAME' },
            { name: 'Part Type', data: 'PART_TYPE' }
        ],
        drawCallback: function (settings) {
            var api = this.api();
            var datas = api.rows({ page: 'current' }).data();
            for (var i = 0; i < datas.length; i++) {
                var node = api.row(i).node();
                var currentLevel = parseInt(datas[i].BOM_LEVEL);
                var modify_type = datas[i].MODIFY_TYPE;
                if (currentLevel !== 0) {
                    if (modify_type !== 'S')
                        $(node).find('td:eq(0)').empty().append('<input type="checkbox" name="normalCheckbox" data-row="' + i + '" value="' + datas[i].PART_NO + '">');
                    if (modify_type === 'S') {
                        $(node).addClass('danger');
                        $(node).find('td:eq(0)').empty().append('<input type="checkbox" name="normalCheckbox" data-row="' + i + '" value="' + datas[i].PART_NO + '" checked>');
                    }

                }


                $(node).find('td:not(:eq(0))').removeClass('text-bold');
                if (i + 1 !== datas.length) {
                    var nextLevel = parseInt(datas[i + 1].BOM_LEVEL);
                    if (currentLevel + 1 === nextLevel) {
                        $(node).find('td:eq(1)').empty().append('<i class="fa fa-minus-circle" aria-hidden="true" data-row=' + i + '></i>');
                        $(node).find('td:eq(2)').addClass('text-bold');
                    }
                }
            }

            $('input[name="normalCheckbox"]').iCheck({
                checkboxClass: 'icheckbox_minimal-red'
            });
        }
    });
}

function BindSpecialTable() {
    var rsi_no = $('#rsi_no').val();
    var group_id = $('#group_id').val();
    $(specialTable).DataTable({
        destroy: true,
        paging: false,
        info: false,
        ordering: false,
        searching: false,
        scrollY: "700px",
        scrollX: true,
        scrollCollapse: true,
        ajax: {
            url: 'RDReview_SpecialParts',
            method: 'POST',
            data: {
                rsi_no: rsi_no,
                group_id: group_id
            },
            dataSrc: ''
        },
        columns: [
            { name: '', data: 'PART_NO', className: 'whitespacepre' },
            { name: 'Level', data: 'BOM_LEVEL', visible: false},
            { name: 'English Name', data: 'ENGLISH_NAME' },
            { name: 'Part Type', data: 'PART_TYPE' }
        ]
    });
}

$(document).on('ifChanged', 'input[name="normalCheckbox"]', function () {

    var datatable = $(normalTable).DataTable();
    var row = $(this).data('row');
    var rowdata = datatable.row(row).data();

    var rownode = datatable.row(row).node();
    var checkparameter = $(rownode).hasClass('danger') ? 'uncheck' : 'check';
    if ($(rownode).hasClass('danger'))
        $(rownode).removeClass('danger');
    else
        $(rownode).addClass('danger');
    SpecialPartsDataChange(rowdata, checkparameter);

    var datas = datatable.rows().data();
    var rowLevel = parseInt(rowdata.BOM_LEVEL);
    for (var i = row + 1; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        if (currentLevel > rowLevel) {
            var node = datatable.row(i).node();
            $(node).find('input[name="normalCheckbox"]').iCheck(checkparameter);
        }
        else {
            return false;
        }
    }
});

function SpecialPartsDataChange(data, toggle) {
    if (toggle === 'check') {
        var datatable = $(specialTable).DataTable();
        datatable.row.add(data).draw(false);
    }
    else {
        var datatable = $(specialTable).DataTable();
        var rowindex;
        datatable.rows().data().each(function (value, index) {
            var exist = (value.PARTNUMBER_PARENT === data.PARTNUMBER_PARENT) && (value.PART_NO === data.PART_NO);
            if (exist) {
                datatable.row(index).remove().draw(false);
                return false;
            }
        });
    }
}

$(document).on('click', '.fa-minus-circle:not(.fa-changeall)', function () {
    $(this).removeClass('fa-minus-circle').addClass('fa-plus-circle');
    var row = $(this).data('row');
    var thisTable = $(this).closest('table');
    var datatable = $(thisTable).DataTable();
    var rowdata = datatable.row(row).data();
    var datas = $(thisTable).DataTable().rows().data();
    var rowLevel = parseInt(rowdata.BOM_LEVEL);
    for (var i = row + 1; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        if (currentLevel > rowLevel) {
            var node = datatable.row(i).node();
            $(node).addClass('hidden');
        }
        else {
            return false;
        }
    }
});

$(document).on('click', '.fa-plus-circle:not(.fa-changeall)', function () {
    $(this).removeClass('fa-plus-circle').addClass('fa-minus-circle');
    var row = $(this).data('row');
    var thisTable = $(this).closest('table');
    var datatable = $(thisTable).DataTable();
    var rowdata = datatable.row(row).data();
    var datas = $(thisTable).DataTable().rows().data();
    var hasplusicon;
    var rowLevel = parseInt(rowdata.BOM_LEVEL);
    for (var i = row + 1; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        if (currentLevel > rowLevel) {
            var node = datatable.row(i).node();
            if (currentLevel == rowLevel + 1) {
                $(node).removeClass('hidden');
                hasplusicon = $(node).find('.fa').hasClass('fa-plus-circle');
            }

            if (!hasplusicon)
                $(node).removeClass('hidden');
        }
        else {
            return false;
        }
    }
});

$(document).on('click', '.btn-cancel', function () {
    window.close();
});

$(document).on('click', '.btn-confirm', function () {
    var rsi_no = $('#rsi_no').val();
    var group_id = $('#group_id').val();
    var datatable = $(specialTable).DataTable();
    var sendData = BindSpecialObject(datatable.rows().data());
    $.ajax({
        url: 'RDReview_SpecialTransaction',
        type: 'POST',
        dataType: "json",
        data: {
            specialData: sendData,
            rsi_no: rsi_no,
            group_id: group_id
        }
        , success: function (response) {
            if (response) {
                window.opener.reloadDataTable();
                window.close();
            }
        }
    });
});

function BindSpecialObject(sendData) {
    var result = [];
    var bu = $('#bu').val();
    $.each(sendData, function (index, value) {
        value.BU = bu;
        result.push(value);
    });
    return result;
}