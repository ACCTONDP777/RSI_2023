
$(function () {
    window.moveTo(0, 0);
    window.resizeTo(screen.width, screen.height);

    BindChangeTable();
    BindOtherTable();

    $('#partno').typeahead({
        source: function (query, process) {
            $.ajax({
                url: 'RDReviewChange_AutoComplatePartNo',
                type: 'POST',
                dataType: "json",
                data: {
                    query: query
                },
                success: function (response) {
                    return process(response);
                }
            });
        },
        afterSelect: function (item) {
            $('#plant').empty();
            $.ajax({
                url: 'RDReview_AutoComplatePlant',
                type: 'POST',
                dataType: "json",
                data: {
                    query: item
                },
                success: function (response) {
                    var optionString = '<option value="{0}">{1}</option>';
                    $.each(response, function (index, value) {
                        var appendString = optionString.replace('{0}', value).replace('{1}', value);
                        $('#plant').append(appendString);
                    });
                }
            });
        }
    });

    $('#queryconditions').typeahead({
        source: function (query, process) {
            $.ajax({
                url: 'RDReviewChange_AutoComplateQueryConditions',
                type: 'POST',
                dataType: "json",
                data: {
                    query: query
                },
                success: function (response) {
                    var result = [];
                    $.each(response, function (index, value) {
                        var object = {
                            id: value.rsi_no,
                            name: value.query_result
                        };
                        result.push(object);
                    });
                    return process(result);
                }
            });
        },
        afterSelect: function (item) {
            $('#rsino').val(item.id);
        }
    });

    $('input[name="searchtype"]').iCheck({
        radioClass: 'iradio_minimal'
    });

    //introJs().start();
});

var chageTable = $('#changeTable');
var otherTable = $('#otherTable');
function BindChangeTable() {
    var rsi_no = $('#rsi_no').val();
    $(chageTable).DataTable({
        destroy: true,
        paging: false,
        info: false,
        ordering: false,
        searching: false,
        scrollY: "500px",
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
            { name: '', data: null, defaultContent: '', className: 'text-center', width: '5%' },
            { name: '', data: null, defaultContent: '', className: 'text-center', width: '5%' },
            { name: '', data: 'DISPLAYPARTNO', className: 'whitespacepre', width: '20%' },
            { name: 'Level', data: 'BOM_LEVEL', width: '5%' },
            { name: 'English Name', data: 'ENGLISH_NAME', width: '50%' },
            { name: 'Part Type', data: 'PART_TYPE', width: '15%' }
        ],
        drawCallback: function (settings) {
            var api = this.api();
            var datas = api.rows({ page: 'current' }).data();
            var first = 0;
            for (var i = 0; i < datas.length; i++) {
                var node = api.row(i).node();
                $(node).find('td:not(:eq(0))').removeClass('text-bold');
                if (i + 1 !== datas.length) {
                    var currentLevel = parseInt(datas[i].BOM_LEVEL);
                    var nextLevel = parseInt(datas[i + 1].BOM_LEVEL);
                    if (currentLevel + 1 === nextLevel) {
                        if (currentLevel !== 0) {
                            $(node).find('td:eq(0)').empty().append('<input type="radio" name="changeRadio" data-row="' + i + '" value="' + datas[i].PART_NO + '">');
                            first++;
                        }

                        if (first == 1) {
                            $(node).find('td:eq(0)').find('input').attr('data-step', '1').attr('data-intro', 'hello world!');
                        }
                            
                        $(node).find('td:eq(1)').empty().append('<i class="fa fa-minus-circle" aria-hidden="true" data-row=' + i + '></i>');
                        $(node).find('td:eq(2)').addClass('text-bold');
                    }
                }
            }

            $('input[name="changeRadio"]').iCheck({
                radioClass: 'iradio_flat-blue'
            });
        }
    });
}

function BindOtherTable() {
    $(otherTable).DataTable({
        destroy: true,
        paging: false,
        info: false,
        ordering: false,
        searching: false,
        scrollY: "500px",
        scrollX: true,
        scrollCollapse: true,
    });
}

$(document).on('ifClicked', 'input[name="changeRadio"]', function (event) {
    var radio = $(this);
    $(chageTable).find('tbody tr.info').removeClass('info');

    var changeTable = $(chageTable).DataTable();
    var row = $(this).data('row');
    var rowdata = changeTable.row(row).data();
    //if (rowdata.MODIFY_TYPE == 'S') {
    //    alert('"Special Parts不允許節點抽換，請先於Special Parts頁面移除設定。');
    //    return false;
    //}

    var datas = changeTable.rows().data();
    var rowLevel = parseInt(rowdata.BOM_LEVEL);
    var hasSpecial = false;
    for (var i = row + 1; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        if (currentLevel > rowLevel) {
            var item = changeTable.row(i).data();
            if (item.MODIFY_TYPE == "S") {
                alert("This material contains Special parts, please remove the Special parts before exchanging.");
                hasSpecial = true;
                break;
            }
        }
        else {
            break;
        }
    }

    if (!hasSpecial) {
        var partlevel = rowdata.PART_LEVEL;
        $('#partlevel').val(partlevel);

        var rownode = changeTable.row(row).node();
        $(rownode).addClass('info');

        for (var i = row + 1; i < datas.length; i++) {
            var currentLevel = parseInt(datas[i].BOM_LEVEL);
            if (currentLevel > rowLevel) {
                var node = changeTable.row(i).node();
                $(node).addClass('info');
            }
            else {
                break;
            }
        }
    }
    else {
        setTimeout(function () {
            $(radio).iCheck('uncheck');
        }, 50);
    }
    
});

$(document).on('click', '.btn-query', function () {
    var searchtype = $('input[name="searchtype"]:checked').val();
    var partno = $('#partno').val();
    var partlevel = $('#partlevel').val();
    var plant = $('#plant').val();
    var rsi_no = $('#rsi_no').val();
    var rsino = $('#rsino').val();
    var ajaxObject;
    $('.has-error').removeClass('has-error');
    $('#errormessage').text('');

    if (searchtype === 'bom' && partno === '') {
        $('#partno').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請輸入Part No');
        return false;
    }

    if (partlevel === '') {
        $('#partlevel').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請選擇Part Level');
        return false;
    }

    if (searchtype === 'bom' && plant === null) {
        $('#plant').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請確認Mgt Plant');
        return false;
    }

    if (searchtype === 'rsi' && rsino === '') {
        $('#queryconditions').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請輸入Query conditions');
        return false;
    }


    if (searchtype === 'bom') {
        ajaxObject = {
            url: 'RDReview_Change',
            type: 'POST',
            data: {
                rsi_no: rsi_no,
                partno: partno,
                partlevel: partlevel,
                plant: plant
            },
            dataSrc: ''
        };
    }

    if (searchtype === 'rsi') {
        ajaxObject = {
            url: 'RDReview_NormalParts',
            type: 'POST',
            data: {
                rsi_no: rsino,
            },
            dataSrc: ''
        };
    }

    $(otherTable).DataTable({
        destroy: true,
        paging: false,
        info: false,
        ordering: false,
        searching: false,
        scrollY: "500px",
        scrollX: true,
        scrollCollapse: true,
        ajax: ajaxObject,
        columns: [
            { name: '', data: null, defaultContent: '', className: 'text-center', width: '5%' },
            { name: '', data: null, defaultContent: '', className: 'text-center', width: '5%' },
            { name: '', data: 'DISPLAYPARTNO', className: 'whitespacepre', width: '20%' },
            { name: 'Level', data: 'BOM_LEVEL', width: '5%' },
            { name: 'English Name', data: 'ENGLISH_NAME', width: '50%' },
            { name: 'Part Type', data: 'PART_TYPE', width: '15%' }
        ],
        drawCallback: function (settings) {
            var api = this.api();
            var datas = api.rows({ page: 'current' }).data();
            for (var i = 0; i < datas.length; i++) {
                var node = api.row(i).node();
                $(node).find('td:not(:eq(0))').removeClass('text-bold');
                if (i + 1 !== datas.length) {
                    var currentLevel = parseInt(datas[i].BOM_LEVEL);
                    var currentPartLevel = parseInt(datas[i].PART_LEVEL);
                    var nextLevel = parseInt(datas[i + 1].BOM_LEVEL);
                    if (currentLevel + 1 === nextLevel) {
                        $(node).find('td:eq(1)').empty().append('<i class="fa fa-minus-circle" aria-hidden="true" data-row=' + i + '></i>');
                        $(node).find('td:eq(2)').addClass('text-bold');
                        if (parseInt(partlevel) === currentPartLevel)
                            $(node).find('td:eq(0)').empty().append('<input type="radio" name="otherRadio" data-row="' + i + '" value="' + datas[i].PART_NO + '">');
                    }
                }
            }

            $('input[name="otherRadio"]').iCheck({
                radioClass: 'iradio_flat-green'
            });
        }
    });
});

$(document).on('ifChanged', 'input[name="otherRadio"]', function () {
    $(otherTable).find('tbody tr.success').removeClass('success');

    var otherDataTable = $(otherTable).DataTable();
    var row = $(this).data('row');
    var rowdata = otherDataTable.row(row).data();

    var rownode = otherDataTable.row(row).node();
    $(rownode).addClass('success');

    var datas = otherDataTable.rows().data();
    var rowLevel = parseInt(rowdata.BOM_LEVEL);
    for (var i = row + 1; i < datas.length; i++) {
        var currentLevel = parseInt(datas[i].BOM_LEVEL);
        if (currentLevel > rowLevel) {
            var node = otherDataTable.row(i).node();
            $(node).addClass('success');
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
    var changeData = $(chageTable).find('tr.info');
    var otherData = $(otherTable).find('tr.success');
    var sendChangeData = SendChangeData(changeData);
    var sendOtherData = SendOtherData(otherData);

    if (sendChangeData.length > 0 && sendOtherData.length > 0) {
        document.getElementById("btnConfirm").disabled = true;
    }

    $.ajax({
        url: 'RDReview_ChangeData',
        type: 'POST',
        dataType: "json",
        data: {
            changeData: sendChangeData,
            otherData: sendOtherData
        },
        success: function (data) {
            window.opener.reloadDataTable();
            window.close();
        }
    });
});

function SendChangeData(data) {
    var changeData = [];
    var rsi_no = $('#rsi_no').val();
    var row = $(data).eq(0).find('input[name="changeRadio"]').data('row');
    $.each(data, function (index, value) {
        var datatable = $(chageTable).DataTable();
        var singleData = datatable.row(row).data();
        var object = {
            RSI_NO: rsi_no,
            PART_TYPE: singleData.PART_TYPE,
            MODIFY_TYPE: 'D',
            SN: singleData.SN
        };
        changeData.push(object);
        row++;
    });
    return changeData;
}

function SendOtherData(data) {
    var otherData = [];
    var rsi_no = $('#rsi_no').val();
    var bu = $('#bu').val();
    var row = $(data).eq(0).find('input[name="otherRadio"]').data('row');
    $.each(data, function (index, value) {
        var datatable = $(otherTable).DataTable();
        var singleData = datatable.row(row).data();
        var parent_no = singleData.PARTNUMBER_PARENT;
        var parent_sn = null;
        if (index === 0) {
            var changerow = $(chageTable).find('input[name="changeRadio"]:checked').data('row');
            parent_no = $(chageTable).DataTable().row(changerow).data().PARTNUMBER_PARENT;
            parent_sn = singleData.PARENT_SN;
        }

        var header_part_type = window.opener.document.getElementById('PartType').value;
        var detail_part_type = singleData.PART_TYPE;
        if (header_part_type.substring(0, 2) == 'TP' && singleData.PART_TYPE !== null)
            detail_part_type = 'TP' + singleData.PART_TYPE;

        var object = {
            RSI_NO: rsi_no,
            MTL_TYPE: singleData.MTL_TYPE,
            MTL_GROUP: singleData.MTL_GROUP,
            MTL_PARTS: singleData.MTL_PARTS,
            PART_TYPE: detail_part_type,
            PARTNUMBER_PARENT: parent_no,
            PART_NO: singleData.PART_NO,
            PART_DESC: singleData.PART_DESC,
            PART_LEVEL: singleData.PART_LEVEL,
            ENGLISH_NAME: singleData.ENGLISH_NAME,
            MAKER_SOURCE: singleData.MAKER_SOURCE,
            MAKER_PART_NO: singleData.MAKER_PN,
            PART_SPEC: singleData.PART_SPEC,
            RELEASE_DATE: ConvertDate(singleData.RELEASE_DATE),
            USAGE: singleData.USAGE,
            PART_UNIT: singleData.PART_UNIT,
            REMARK: singleData.REMARK,
            MODIFY_TYPE: 'A',
            BU: bu,
            PARENT_SN: parent_sn,
            PARTS_GROUP: singleData.PARTS_GROUP
        };
        otherData.push(object);
        row++;
    });
    return otherData;
}

function ConvertDate(data) {
    var re = /-?\d+/;
    var m = re.exec(data);
    var d = new Date(parseInt(m[0]));
    var viewstring = '{0}/{1}/{2}';
    return viewstring.replace('{0}', d.getFullYear()).replace('{1}', d.getMonth() + 1).replace('{2}', d.getDate());
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
            break;
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
            break;
        }
    }
});

$(document).on('ifChanged', 'input[name="searchtype"]', function () {
    var radioValue = $(this).val();
    if (radioValue === 'bom') {
        $('.bomcontent').removeClass('hidden');
        $('.rsicontent').addClass('hidden');
    }

    if (radioValue === 'rsi') {
        $('.rsicontent').removeClass('hidden');
        $('.bomcontent').addClass('hidden');
    }
});
