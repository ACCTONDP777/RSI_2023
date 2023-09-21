var normalTable = $('#normalTable');
var rsi_no = $('#RSI_NO').val();
var part_type = $('#PartType').val();
var mterialGroupPartsObject;
var unitObject;

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
        url: 'RDReview_NormalParts',
        method: 'POST',
        data: {
            rsi_no: rsi_no,
            part_type: part_type
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
        { name: 'PART_LEVEL', data: 'PART_LEVEL' },
        { name: 'MTL_GROUP', data: 'MTL_GROUP' },
        { name: 'MTL_PARTS', data: 'MTL_PARTS' },
        { name: 'PART_TYPE', data: 'PART_TYPE' },
        { name: 'ENGLISH_NAME', data: 'ENGLISH_NAME' },
        { name: 'PART_DESC', data: 'PART_DESC', className: 'text-ellipsis' },
        { name: 'MAKER_SOURCE', data: 'MAKER_SOURCE', visible: false },
        { name: 'MAKER_PART_NO', data: 'MAKER_PART_NO', visible: false },
        { name: 'PART_SPEC', data: 'PART_SPEC' },
        { name: 'SPEC_DEF', data: 'SPEC_DEF', className: 'text-ellipsis', visible: false },
        {
            name: 'RELEASE_DATE', data: 'RELEASE_DATE', defaultContent: '', visible: false,
            render: function (data, type, row, meta) {
                return ConvertDate(data);
            }
        },
        {
            name: 'USAGE', data: 'USAGE', defaultContent: '', className: 'text-right'
        },
        { name: 'PART_UNIT', data: 'PART_UNIT' },
        { name: 'EOL_STATUS', data: 'EOL_STATUS', visible: false },
        { name: 'UNI_SPEC_STATUS', data: 'UNI_SPEC_STATUS', visible: false },
        { name: 'REMARK', data: 'REMARK' },
        {
            name: 'Upload File', data: 'FILE_STATUS', defaultContent: '', className: 'text-center',
            render: function (data, type, row, meta) {
                if (data === null)
                    return null;

                var icon = data === 'Y' ? 'fa-file-text-o' : 'fa-upload';
                var color = data === 'Y' ? ' btn-success' : 'btn-info';
                return '<button class="btn ' + color + ' btn-sm"  onclick="fileManagement(' + row.SN + ')")><i class="fa ' + icon + '"></i></button>';
            }
        },
        { name: 'MODIFY_TYPE', data: 'MODIFY_TYPE', visible: false },
        {
            name: 'SN', data: 'SN', defaultContent: '', visible: false
        }
    ], buttons: [
        {
            text: '<i class="fa addparent"></i>',
            titleAttr: 'Add Parent Part No',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function () {
                var data = $(normalTable).DataTable().row({ selected: true }).data();
                if (data === undefined) {
                    $('.modal-selectitem').modal('show');
                    return false;
                }

                if (data.MODIFY_TYPE === 'S') {
                    $('.modal-nopermission').modal('show');
                    return false;
                }

                $('.has-error').removeClass('has-error');
                $('#errormessage').text('');
                $('#editTitle').text('Add Parent Part No');
                $('.modal-edit #mtlgroup').empty();
                $('.modal-edit #partgroup').empty();
                $('.modal-edit select#mtlparts').empty();
                $('.modal-edit #partnomfg option').remove();
                $('.modal-edit #partnomfg').append("<option value=''></option>");

                $('#radio1, #radio0').attr('disabled', false);
                $('#radio1').click();


                setTimeout(function () {
                    $('.modal-edit #parentpartno').val(data.PARTNUMBER_PARENT);
                    $('.modal-edit #partno').val('').attr('disabled', false);
                    $('.modal-edit #partnomfg').val('').attr('disabled', false);
                    $('.modal-edit .btn-partnosearch').attr('disabled', false);
                    $('.modal-edit #partlevel').val('');
                    $('.modal-edit #englishname').val('');
                    $('.modal-edit #usage').val('');
                    $('.modal-edit #partdesc').val('');
                    $('.modal-edit #unit').val('PCS');
                    $('.modal-edit #remark').val('');
                    $('.modal-edit #modifytype').val('A');
                    $('.modal-edit #parent_sn').val(data.PARENT_SN);
                    $('.modal-edit #mtltype').val('Normal');
                    $('.modal-edit #groupid').val('');
                    $('.modal-edit #parttype').val(data.PART_TYPE);

                }, 500);

                $('.modal-edit').modal('show');
            }
        },
        {
            text: '<i class="fa addchild"></i>',
            titleAttr: 'Add Child Part No',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function () {
                var data = $(normalTable).DataTable().row({ selected: true }).data();
                if (data === undefined) {
                    $('.modal-selectitem').modal('show');
                    return false;
                }

                if (data.MODIFY_TYPE === 'S') {
                    $('.modal-nopermission').modal('show');
                    return false;
                }

                $('.has-error').removeClass('has-error');
                $('#errormessage').text('');
                $('#editTitle').text('Add Child Part No');

                $('#radio1, #radio0').attr('disabled', false);
                $('#radio1').click();

                $('.modal-edit #mtlgroup').empty();
                $('.modal-edit #partgroup').empty();
                $('.modal-edit select#mtlparts').empty();
                $('.modal-edit #partnomfg option').remove();
                $('.modal-edit #partnomfg').append("<option value=''></option>");

                setTimeout(function () {
                    $('.modal-edit #parentpartno').val(data.PART_NO == null ? data.PART_LEVEL : data.PART_NO);
                    if (data.BOM_LEVEL == 0)
                        $('.modal-edit #parentpartno').val($('#REF_PRODUCT').val());
                    $('.modal-edit #partno').val('').attr('disabled', false);
                    $('.modal-edit #partnomfg').val('').attr('disabled', false);
                    $('.modal-edit .btn-partnosearch').attr('disabled', false);
                    $('.modal-edit #partlevel').val('');
                    $('.modal-edit #englishname').val('');
                    $('.modal-edit #usage').val('');
                    $('.modal-edit #partdesc').val('');
                    $('.modal-edit #unit').val('PCS');
                    $('.modal-edit #remark').val('');
                    $('.modal-edit #modifytype').val('A');
                    $('.modal-edit #parent_sn').val(data.SN);
                    $('.modal-edit #mtltype').val('Normal');
                    $('.modal-edit #groupid').val('');
                    $('.modal-edit #parttype').val(data.PART_TYPE);
                }, 500);

                $('.modal-edit').modal('show');
            }
        },
        {
            //text: 'Edit',
            text: '<i class="fa fa-pencil-square-o"></i>',
            titleAttr: 'Edit Part No：細項編輯',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                var data = $(normalTable).DataTable().row({ selected: true }).data();
                if (data === undefined) {
                    $('.modal-selectitem').modal('show');
                    return false;
                }

                if (data.MODIFY_TYPE === 'S') {
                    $('.modal-nopermission').modal('show');
                    return false;
                }

                $('#radio1, #radio0').attr('disabled', false);
                if (data.PART_NO != null)
                    $('#radio1').click();
                else
                    $('#radio0').click();
                $('#radio1, #radio0').attr('disabled', true);

                $('.modal-edit #partnomfg option').remove();
                $('.modal-edit #partnomfg').append("<option value=''></option>");

                $('.has-error').removeClass('has-error');
                $('#errormessage').text('');
                $('#editTitle').text('Edit Part No：細項編輯');

                $('.modal-edit #partlevel').val(data.PART_LEVEL);
                $('.modal-edit #parentpartno').val(data.PARTNUMBER_PARENT);
                GetEnglishName(data.PART_LEVEL);
                BindMaterialGroupParts(data.PART_LEVEL, data.ENGLISH_NAME, data.MTL_GROUP, data.PARTS_GROUP, data.MTL_PARTS, data.SN);
                setTimeout(function () {
                    $('.modal-edit #partno').val(data.PART_NO);
                    $('.modal-edit #englishname').val(data.ENGLISH_NAME);
                    $('.modal-edit #partdesc').val(data.PART_DESC);
                    $('.modal-edit #vendor').val(data.MAKER_SOURCE);
                    $('.modal-edit #makerpn').val(data.MAKER_PART_NO);
                    $('.modal-edit #releasedate').val(ConvertDate(data.RELEASE_DATE));
                    $('.modal-edit #specdef').text(data.SPEC_DEF);
                    $('.modal-edit #itemspec').val(data.PART_SPEC);
                    $('.modal-edit #usage').val(data.USAGE);
                    $('.modal-edit #unit').val(data.PART_UNIT);
                    $('.modal-edit #remark').val(data.REMARK);
                    $('.modal-edit #modifytype').val('U');
                    $('.modal-edit #mtltype').val('Normal');
                    $('.modal-edit #groupid').val('');
                    $('.modal-edit #parttype').val(data.PART_TYPE);
                    $('.modal-edit #sn').val(data.SN);
                    $('.modal-edit #parent_sn').val(data.PARENT_SN);

                    if (data.PART_NO !== null && data.PART_SPEC === null) {
                        $('.modal-edit #partno').attr('disabled', false);
                        $('.modal-edit #partnomfg').attr('disabled', false);
                        $('.modal-edit .btn-partnosearch').attr('disabled', false);
                    }

                    if (data.PART_NO === null && data.PART_SPEC !== null) {
                        $('.modal-edit #partno').attr('disabled', true);
                        $('.modal-edit #partnomfg').attr('disabled', true);
                        $('.modal-edit .btn-partnosearch').attr('disabled', true);
                    }
                }, 500);

                $('.modal-edit').modal('show');
            }
        },
        {
            //text: 'Change',
            text: '<i class="fa fa-exchange"></i>',
            titleAttr: 'Exchange：主件抽換',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                //do something
                var rsi_no = $("#RSI_NO").val();
                var bu = $('#BU').val();
                var url = 'RDReview_Change?rsi_no={0}&bu={1}';
                url = url.replace('{0}', rsi_no).replace('{1}', bu);
                window.open(url, '', 'scrollbars=yes,resizable=yes');
            }
        },
        {
            //text: 'Delete',
            text: '<i class="fa fa-trash-o"></i>',
            titleAttr: 'Delete Part No',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                var data = $(normalTable).DataTable().row({ selected: true }).data();
                $('.modal-edit #mtltype').val('Normal');
                $('.modal-edit #groupid').val('');
                if (data === undefined) {
                    $('.modal-selectitem').modal('show');
                    return false;
                }

                if (data.MODIFY_TYPE === 'S') {
                    $('.modal-nopermission').modal('show');
                    return false;
                }

                $('.modal-delete').modal('show');
            }
        },
        {
            text: '<i class="fa specialimg"></i>',
            //text: 'Special',
            titleAttr: 'Choose Special Portfolio ',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                //do something
                var rsi_no = $("#RSI_NO").val();
                var bu = $('#BU').val();
                var group_id = '';
                var specialTabs = $('div#specialTabs ul li');
                if (specialTabs.length > 0)
                    group_id = $(specialTabs).eq(0).data('groupId');
                var url = 'RDReview_SpecialTransaction?rsi_no={0}&group_id={1}&bu={2}';
                url = url.replace('{0}', rsi_no).replace('{1}', group_id).replace('{2}', bu);
                window.open(url, '', 'scrollbars=yes,resizable=yes');
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
                    else
                    {
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

        //自動縮合
        //setTimeout(function () {
        //    $('.fa-minus-circle.fa-changeall').click();
        //}, 500);
    },
    initComplete: function (settings, json) {
        $(this).DataTable().buttons().container()
            .appendTo('#normalTable_wrapper .col-sm-6:eq(0)');

        $('[data-toggle="tooltip"]').tooltip();
        var chk_hidden = $("#uploadExcel").hasClass('hidden');
        if (chk_hidden)
            $("#uploadExcel + .input-group").addClass('hidden');
    }
};

var specialTableObject = {
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
        selector: 'td'
    },
    columns: [
        { name: 'icon', data: null, defaultContent: '', className: 'text-center', visible: false },
        { name: '', data: 'PARTNUMBER_TOP', visible: false },
        { name: '', data: 'PARTNUMBER_PARENT' },
        { name: '', data: 'PARTNUMBER_CHILD', visible: false },
        { name: '', data: 'DISPLAYPARTNO' },
        { name: '', data: 'BOM_PATH', visible: false },
        { name: '', data: 'BOM_LEVEL', visible: false },
        { name: 'MTL_GROUP', data: 'MTL_GROUP' },
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
                return ConvertDate(data);
            }
        },
        {
            name: 'USAGE', data: 'USAGE', defaultContent: '', className: 'text-right'
        },
        { name: 'PART_UNIT', data: 'PART_UNIT' },
        { name: 'EOL_STATUS', data: 'EOL_STATUS', visible: false },
        { name: 'UNI_SPEC_STATUS', data: 'UNI_SPEC_STATUS', visible: false },
        { name: 'REMARK', data: 'REMARK' },
        {
            name: 'Upload File', data: 'FILE_STATUS', defaultContent: '', className: 'text-center',
            render: function (data, type, row, meta) {
                if (data === null)
                    return null;

                var icon = data === 'Y' ? ' fa-file-text-o' : 'fa-upload';
                var color = data === 'Y' ? ' btn-success' : 'btn-info';
                return '<button class="btn ' + color + ' btn-sm" onclick="fileManagement(' + row.SN + ')")><i class="fa ' + icon + '"></i></button>';
            }
        },
        { name: 'MODIFY_TYPE', data: 'MODIFY_TYPE', visible: false },
        {
            name: 'SN', data: 'SN', defaultContent: '', visible: false
        }
    ], buttons: [
        {
            text: '<i class="fa fa-plus-square-o" aria-hidden="true"></i>',
            titleAttr: 'Add Part No',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function () {
                var data = $('.specialTable').DataTable().row({ selected: true }).data();
                if (data === undefined) {
                    $('.modal-selectitem').modal('show');
                    return false;
                }

                $('.has-error').removeClass('has-error');
                $('#errormessage').text('');
                $('#editTitle').text('Add Part No');

                $('.modal-edit #mtlgroup').empty();
                $('.modal-edit #partgroup').empty();
                $('.modal-edit select#mtlparts').empty();

                $('#radio1, #radio0').attr('disabled', false);
                $('#radio1').click();

                setTimeout(function () {
                    $('.modal-edit #parentpartno').val(data.PARTNUMBER_PARENT);
                    $('.modal-edit #partno').val('').attr('disabled', false);
                    $('.modal-edit #partnomfg').val('').attr('disabled', false);
                    $('.modal-edit .btn-partnosearch').attr('disabled', false);
                    $('.modal-edit #partlevel').val('');
                    $('.modal-edit #englishname').val('');
                    $('.modal-edit #itemspec').val('');
                    $('.modal-edit #usage').val('');
                    $('.modal-edit #partdesc').val('');
                    $('.modal-edit #unit').val('PCS');
                    $('.modal-edit #remark').val('');
                    $('.modal-edit #modifytype').val('A');
                    $('.modal-edit #mtltype').val('Special');
                    var group_id = $('div#specialContent .specialtabContent.active').attr('id');
                    $('.modal-edit #groupid').val(group_id);
                    $('.modal-edit #parttype').val(data.PART_TYPE);
                }, 500);

                $('.modal-edit').modal('show');
            }
        },
        {
            //text: 'Edit',
            text: '<i class="fa fa-pencil-square-o"></i>',
            titleAttr: 'Edit Part No',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                var data = $('.specialTable').DataTable().row({ selected: true }).data();
                if (data === undefined) {
                    $('.modal-selectitem').modal('show');
                    return false;
                }


                $('.has-error').removeClass('has-error');
                $('#errormessage').text('');
                $('#editTitle').text('Edit Part No');

                $('#radio1, #radio0').attr('disabled', false);
                if (data.PART_NO != null)
                    $('#radio1').click();
                else
                    $('#radio0').click();

                $('#radio1, #radio0').attr('disabled', true);
                GetEnglishName(data.PART_LEVEL);
                BindMaterialGroupParts(data.PART_LEVEL, data.ENGLISH_NAME, data.MTL_GROUP, data.PARTS_GROUP, data.MTL_PARTS, data.SN);
                setTimeout(function () {
                    $('.modal-edit #parentpartno').val(data.PARTNUMBER_PARENT);
                    $('.modal-edit #partno').val(data.PART_NO);
                    $('.modal-edit #partlevel').val(data.PART_LEVEL);
                    $('.modal-edit #englishname').val(data.ENGLISH_NAME);
                    $('.modal-edit #partdesc').val(data.PART_DESC);
                    $('.modal-edit #vendor').val(data.MAKER_SOURCE);
                    $('.modal-edit #makerpn').val(data.MAKER_PART_NO);
                    $('.modal-edit #releasedate').val(ConvertDate(data.RELEASE_DATE));
                    $('.modal-edit #specdef').text(data.SPEC_DEF);
                    $('.modal-edit #itemspec').val(data.PART_SPEC);
                    $('.modal-edit #usage').val(data.USAGE);
                    $('.modal-edit #unit').val(data.PART_UNIT);
                    $('.modal-edit #remark').val(data.REMARK);
                    $('.modal-edit #modifytype').val('U');
                    $('.modal-edit #mtltype').val('Special');
                    var group_id = $('div#specialContent .specialtabContent.active').attr('id');
                    $('.modal-edit #groupid').val(group_id);
                    $('.modal-edit #parttype').val(data.PART_TYPE);
                    $('.modal-edit #sn').val(data.SN);

                    if (data.PART_NO !== null && data.PART_SPEC === null) {
                        $('.modal-edit #partno').attr('disabled', false);
                        $('.modal-edit #partnomfg').attr('disabled', false);
                        $('.modal-edit .btn-partnosearch').attr('disabled', false);
                    }

                    if (data.PART_NO === null && data.PART_SPEC !== null) {
                        $('.modal-edit #partno').attr('disabled', true);
                        $('.modal-edit #partnomfg').attr('disabled', true);
                        $('.modal-edit .btn-partnosearch').attr('disabled', true);
                    }
                }, 500);

                $('.modal-edit').modal('show');
            }
        },
        {
            //text: 'Delete',
            text: '<i class="fa fa-trash-o"></i>',
            titleAttr: 'Delete Part No',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                var data = $('.specialTable').DataTable().row({ selected: true }).data();
                $('.modal-edit #mtltype').val('Special');
                var group_id = $('div#specialContent .specialtabContent.active').attr('id');
                $('.modal-edit #groupid').val(group_id);

                if (data === undefined) {
                    $('.modal-selectitem').modal('show');
                    return false;
                }

                $('.modal-delete').modal('show');
            }
        }
    ],
    drawCallback: function (settings) {
        var api = this.api();
        var datas = api.rows({ page: 'current' }).data();
        for (var i = 0; i < datas.length; i++) {
            var node = api.row(i).node();
            var modify_type = datas[i].MODIFY_TYPE;

            $(node).removeClass('success');
            if (modify_type === 'A' || modify_type === 'U')
                $(node).addClass('success');
        }
    },
    initComplete: function (settings, json) {
        var tableid = $(this).attr('id');
        var selector = '#{0}_wrapper .col-sm-6:eq(0)'.replace('{0}', tableid);
        $(this).DataTable().buttons().container().appendTo(selector);

        $('[data-toggle="tooltip"]').tooltip();
    }
};

$(function () {
    var table = $(normalTable).DataTable(normalTableObject);
    GetPartLevelArray();
    GetUnit();

    BindRefModel();

    GetSpecialPartsGroups();

    RDMemberDisabled();

    $('.typeahead').typeahead({
        minLength: 2,
        source: function (query, process) {
            $.ajax({
                url: 'RDReview_AutoComplatePartNo',
                type: 'POST',
                dataType: "json",
                data: {
                    query: query
                },
                success: function (response) {
                    var result = [];
                    $.each(response, function (index, value) {
                        var object = {
                            id: value.part_desc,
                            name: value.part_no,
                            english: value.english_name
                        };
                        result.push(object);
                    });
                    return process(result);
                }
            });
        },
        afterSelect: function (item) {
            $('#zhezhao').show();
            $('#login').show();
            $('#partdesc').val(item.id);
            var part_no = item.name;
            var part_level = part_no.substring(0, 2);
            $('select#partlevel').val(part_level);
            GetEnglishName(part_level); 
            $('#zhezhao').show();
            $('#login').show();
            GetPartNoMfg(part_no);
            var english_name = item.english;
            $('select#englishname').val(english_name);
            BindMaterialGroupParts();

            //setTimeout(function () {
            //    var english_name = item.english;
            //    $('select#englishname').val(english_name);
            //    BindMaterialGroupParts();
            //}, 500);
        }
    });
});

$(document).on('click', '.fa-minus-circle:not(.fa-changeall, .specialicon)', function () {
    $(this).removeClass('fa-minus-circle').addClass('fa-plus-circle');
    var row = $(this).data('row');
    var datatable = $(normalTable).DataTable();
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
    $(normalTable).DataTable().columns.adjust();
});

$(document).on('click', '.fa-plus-circle:not(.fa-changeall, .specialicon)', function () {
    $(this).removeClass('fa-plus-circle').addClass('fa-minus-circle');
    var row = $(this).data('row');
    var datatable = $(normalTable).DataTable();
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
    $(normalTable).DataTable().columns.adjust();
});

$(document).on('click', '.fa-minus-circle.fa-changeall', function () {
    $(this).removeClass('fa-minus-circle').addClass('fa-plus-circle');
    var datatable = $(normalTable).DataTable();
    var datas = $(normalTable).DataTable().rows().data();
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

    $(normalTable).DataTable().columns.adjust();
});

$(document).on('click', '.fa-plus-circle.fa-changeall', function () {
    $(this).removeClass('fa-plus-circle').addClass('fa-minus-circle');
    var datatable = $(normalTable).DataTable();
    var datas = $(normalTable).DataTable().rows().data();
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
    $(normalTable).DataTable().columns.adjust();
});

$(document).on('click', '#btn-save', function () {
    if (InspectPartObject()) {
        var model = BindPartObject();
        var resultSN = 0;
        if (model.MTL_TYPE === 'Normal')
            resultSN = GetUpdateDeleteParentSN();
        var parent_no = $('#REF_PRODUCT').val();
        $.ajax({
            url: 'RDReview_Save',
            method: 'POST',
            data: {
                model: model,
                resultSN: resultSN,
                parent_no: parent_no
            },
            success: function (data) {
                if (data) {
                    $('.modal-edit').modal('hide');
                    reloadDataTable();
                    return;
                }
                alert('save fail');
            },
            error: function(){
                alert('資料有誤，請聯繫IT人員');
                $('.modal-edit').modal('hide');
            }
        });
    }
});

$(document).on('click', '#btn-delete', function () {
    var rsi_no = $('#RSI_NO').val();
    var part_type = $('#PartType').val();
    var mtltype = $('#mtltype').val();
    var parent_no = $('#REF_PRODUCT').val();
    var sn;
    var part_no;
    var parent_part_no;
    var resultSN;
    var group_id;
    if (mtltype === 'Normal') {
        sn = $(normalTable).DataTable().row({ selected: true }).data().SN;
        part_no = $(normalTable).DataTable().row({ selected: true }).data().PART_NO;
        parent_part_no = $(normalTable).DataTable().row({ selected: true }).data().PARTNUMBER_PARENT;
        resultSN = GetUpdateDeleteParentSN();
    }

    if (mtltype === 'Special') {
        var thisTable = $('div#specialTabs div#specialContent div.specialtabContent.active table:eq(1)');
        sn = $(thisTable).DataTable().row({ selected: true }).data().SN;
        part_no = $(thisTable).DataTable().row({ selected: true }).data().PART_NO;
        parent_part_no = $(thisTable).DataTable().row({ selected: true }).data().PARTNUMBER_PARENT;
        group_id = $(thisTable).data('groupId');
    }

    var modify_type = 'D';
    $.ajax({
        url: 'RDReview_Save',
        method: 'POST',
        data: {
            model: {
                RSI_NO: rsi_no,
                PART_NO: part_no,
                PARTNUMBER_PARENT: parent_part_no,
                PART_TYPE: part_type,
                MODIFY_TYPE: modify_type,
                MTL_TYPE: mtltype,
                SN: sn
            },
            resultSN: resultSN,
            parent_no: parent_no,
            group_id: group_id
        },
        success: function (data) {
            if (data) {
                $('.modal-delete').modal('hide');
                reloadDataTable();
                return;
            }
            alert('save fail');
        }
    });
});

function GetUnit() {
    $.ajax({
        url: 'GetUnit',
        method: 'POST',
        success: function (data) {
            unitObject = data;
            BindUnit();
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
        $('#specdef').text('');
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
        $('#specdef').text('');
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
        $('#specdef').text('');
        $('select#mtlparts').append('<option value=""></option>');
    } else {
        BindSpecDef(mtlgroup, partsgroup, mtlParts[0]);
    }
    $.each(mtlParts, function (index, value) {
        var optionString = '<option value="{0}">{1}</option>';
        optionString = optionString.replace('{0}', value).replace('{1}', value);
        $('select#mtlparts').append(optionString);
    });
}

function BindSpecDef(mtlgroup, partsgroup, mtlparts) {
    var filterdata = mterialGroupPartsObject.filter(function (d) { return d.MTL_GROUP === mtlgroup && d.PARTS_GROUP === partsgroup && d.MTL_PARTS === mtlparts })[0];
    var spec_def = filterdata === undefined ? '' : filterdata.SPEC_DEF;
    var part_type = filterdata === undefined ? '' : filterdata.PART_TYPE;
    part_type = (part_type === '' || part_type === null) ? $('#PartType').val() : part_type;
    $('#specdef').text(spec_def);
    $('#parttype').val(part_type);
}

function BindUnit() {
    $('#unit').empty();
    $.each(unitObject, function (index, value) {
        var optionString = '<option value="{0}">{1}</option>';
        optionString = optionString.replace('{0}', value).replace('{1}', value);
        $('#unit').append(optionString);
    });
}

function SetPartNoInfo(item_no, english_name, item_desc, vendor, maker_pn, release_date, part_level, material_group, material_part) {
    $('select#partlevel').val(part_level);
    GetEnglishName(part_level);

    setTimeout(function () {
        $('select#englishname').val(english_name);
        $('#partno').val(item_no);
        $('#partdesc').val(item_desc);
        $('#vendor').val(vendor);
        $('#makerpn').val(maker_pn);
        $('#releasedate').val(release_date);
        BindMaterialGroupParts();
    }, 500);
}

function BindPartObject() {
    var rsi_no = $('#RSI_NO').val();
    var mtl_type = $('#mtltype').val();
    var group_id = $('#groupid').val();
    var mtl_group = $('#mtlgroup').val();
    var mtl_parts = $('#mtlparts').val();
    var part_type = $('#parttype').val();
    var parent_partno = $('#parentpartno').val();
    var part_no = $('#partno').val();
    var part_no_mfg = $('#partnomfg').val();
    var part_desc = $('#partdesc').val();
    var part_level = $('#partlevel').val();
    var english_name = $('#englishname').val();
    var vendor = $('#vendor').val();
    var maker_pn = $('#makerpn').val();
    var partspec = $('#itemspec').val();
    var release_date = $('#releasedate').val();
    var usage = $('#usage').val();
    var unit = $('#unit').val();
    var remark = $('#remark').val();
    var modify_type = $('#modifytype').val();
    var bu = $('#BU').val();
    var sn = $('#sn').val();
    var parent_sn = $('#parent_sn').val();
    var partgroup = $('#partgroup').val();

    var header_part_type = $('#PartType').val();
    if (mtl_group === 'RD DEFINE') {
        //modify_type = 'NEW';
        part_type = $("#PartType").val();
    }
    else if (header_part_type.substring(0, 2) == 'TP' && part_type !== '' && part_type.substring(0, 2) !== 'TP')
    {
        part_type = 'TP' + part_type;
    }


    var result = {
        RSI_NO: rsi_no,
        MTL_TYPE: mtl_type,
        GROUP_ID: group_id,
        MTL_GROUP: mtl_group,
        MTL_PARTS: mtl_parts,
        PART_TYPE: part_type,
        PARTNUMBER_PARENT: parent_partno,
        PART_NO: part_no,
        PART_NO_MFG: part_no_mfg,
        PART_DESC: part_desc,
        PART_LEVEL: part_level,
        ENGLISH_NAME: english_name,
        MAKER_SOURCE: vendor,
        MAKER_PART_NO: maker_pn,
        PART_SPEC: partspec,
        RELEASE_DATE: release_date,
        USAGE: usage,
        PART_UNIT: unit,
        REMARK: remark,
        MODIFY_TYPE: modify_type,
        BU: bu,
        SN: sn,
        PARENT_SN: parent_sn,
        PARTS_GROUP: partgroup
    };
    return result;
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

$(document).on('change', '#mtlparts', function () {
    var mtlgroup = $('#mtlgroup').val();
    var partgroup = $('#partgroup').val();
    var mtlparts = $(this).val();
    BindSpecDef(mtlgroup, partgroup, mtlparts);
});

$(document).on('click', '.btn-partnosearch', function () {
    var partlevel = $('#partlevel').val();
    var englishname = $('#englishname').val();
    var material_parts = $('#mtlparts.form-control:not(.hidden)').val();
    var url = 'PartNoSearch?part_level={0}&english_name={1}&martial_parts={2}';
    url = url.replace('{0}', partlevel).replace('{1}', englishname).replace('{2}', material_parts);
    window.open(url, 'PartNoSearch', 'scrollbars=yes, width=1000, height=800');
});

function ConvertDate(data) {
    var re = /-?\d+/;
    var m = re.exec(data);
    var d = new Date(parseInt(m[0]));
    var viewstring = '{0}/{1}/{2}';
    return viewstring.replace('{0}', d.getFullYear()).replace('{1}', d.getMonth() + 1).replace('{2}', d.getDate());
}

function InspectPartObject() {
    var radioValue = $('input[name="haspartno"]:checked').val();
    var part_no = $('#partno').val();
    var part_level = $('#partlevel').val();
    var english_name = $('#englishname').val();
    var partspec = $('#itemspec').val();
    var usage = $('#usage').val();
    var mtl_group = $('#mtlgroup').val();
    var parts_group = $('#partgroup').val();
    var mtl_parts = $('#mtlparts').val();
    $('#partno').parents('.form-group').removeClass('has-error');
    $('#partlevel').parents('.form-group').removeClass('has-error');
    $('#englishname').parents('.form-group').removeClass('has-error');
    $('#itemspec').parents('.form-group').removeClass('has-error');
    $('#usage').parents('.form-group').removeClass('has-error');
    $('#mtlgroup').parents('.form-group').removeClass('has-error');
    $('#partgroup').parents('.form-group').removeClass('has-error');
    $('#mtlparts').parents('.form-group').removeClass('has-error');
    $('#errormessage').text('');

    if (radioValue == 1 && part_no === '') {
        $('#partno').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請輸入Part No');
        return false;
    }

    //if (part_no !== '' && part_no.length !== 12) {
    //    $('#partno').parents('.form-group').addClass('has-error');
    //    $('#errormessage').text('Part No需要12碼');
    //    return false;
    //}

    if (part_no !== '' && part_level === null) {
        $('#partlevel').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請選擇Part Level');
        return false;
    }

    if (part_no !== '' && part_level !== null) {
        var substr = part_no.substring(0, 2);
        if (substr !== part_level) {
            $('#partlevel').parents('.form-group').addClass('has-error');
            $('#errormessage').text('請確認Part Level');
            return false;
        }
    }

    if (english_name === null) {
        $('#englishname').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請選擇English Name');
        return false;
    }

    if (part_no != '' && partspec == '') {
        var hasPartNo = false;
        $.ajax({
            url: 'RDReview_AutoComplatePartNo',
            type: 'POST',
            dataType: "json",
            async: false,
            data: {
                query: part_no
            },
            success: function (response) {
                hasPartNo = response.length > 0;
            }
        });

        if (!hasPartNo) {
            $('#partno').parents('.form-group').addClass('has-error');
            $('#errormessage').text('查無此Part No');
            return false;
        }
    }

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

    if (radioValue == 0 && partspec === '') {
        $('#itemspec').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請輸入Part Spec');
        return false;
    }

    var numberUsage = Number(usage);
    if (isNaN(numberUsage) || numberUsage <= 0) {
        $('#usage').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請確認Usage');
        return false;
    }
    return true;
}

var fileTable = $('#fileTable');
function fileManagement(sn) {
    var rsi_no = $('#RSI_NO').val();
    $("input[name='sn']").val(sn);
    $("#fileManagement").modal('show');
    $(fileTable).DataTable({
        destroy: true,
        ajax: {
            url: '/RSI/RD/FileManagement',
            method: 'GET',
            data: {
                rsi_no: rsi_no,
                sn: sn
            },
            dataSrc: ''
        },
        columns: [
            {
                data: 'FILE_ID',
                render: function (data, type, row) {
                    return '<button class="btn btn-danger" data-file-id="' + data + '" onclick="fileDelete(this);"><i class="fa fa-trash-o" aria-hidden="true"></i> Delete</button>';
                }
            },
            {
                data: 'FILE_NAME',
                render: function (data, type, row) {
                    return '<a href="/RSI/RSI/GetFile?file_id=' + row["FILE_ID"] + '&rsi_no=' + rsi_no + '&sn=' + sn + '">' + data + '</button>';
                }
            },
            { data: 'FILE_SIZE', class: 'text-right' },
            { data: 'CREATED_BY' },
            { data: 'CREATED_DATE' },
            { data: 'REMARK' }
        ]
    });
}

function fileUpload() {
    var data = new FormData();
    data.append("rsi_no", $("#RSI_NO").val());
    data.append("sn", $("input[name='sn']").val());
    data.append("file", $("#file").get(0).files[0]);
    data.append("remark", $("#fileRemark").val());
    $.ajax({
        type: "POST",
        url: "/RSI/RD/FileManagement",
        contentType: false,
        processData: false,
        dataType: "json",
        data: data,
        success: function (data) {
            $('#file').val('').clone(true);
            $('#fileRemark').val('');
            $('.bootstrap-filestyle input.form-control').val('');
            $(fileTable).DataTable().ajax.reload(null, false);
            reloadDataTable();
        }
    })
}

function fileDelete(btn) {
    var file_id = $(btn).data("fileId");
    $.ajax({
        type: "POST",
        url: "/RSI/RD/DeleteFile",
        dataType: "json",
        data: {
            file_id: file_id
        },
        success: function (data) {
            $(fileTable).DataTable().ajax.reload(null, false);
            reloadDataTable();
        }
    })
}

$('#fileManagement').on('hidden.bs.modal', function () {
    reloadDataTable();
});

function reloadDataTable() {
    $('i.fa-changeall').removeClass('fa-plus-circle').addClass('fa-minus-circle');
    $(normalTable).DataTable().ajax.reload();
    reloadSpecialDataTable();
}

function reloadSpecialDataTable() {
    var liLength = $('div#specialTabs ul').find('li').length;
    if (liLength === 0)
        GetSpecialPartsGroups();
    if (liLength !== 0) {
        var contents = $('div#specialContent .specialtabContent');
        $.each(contents, function (index, value) {
            $(value).find('table').eq(1).DataTable().ajax.reload();
        });
    }
}

function BindRefModel() {
    var modelpart = $('#REF_PRODUCT').val();
    $('.rsi-model').text(modelpart);
}

$(document).on('shown.bs.tab', 'a[data-toggle="tab"]', function (e) {
    var hrefTab = $(this).attr("href");
    if (hrefTab === '#tab1') {
        $(normalTable).DataTable().columns.adjust();
        $('.btn-downloadExcel, #uploadExcel+.bootstrap-filestyle, .btn-uploadExcel').removeClass('hidden');
        return false;
    }

    if (hrefTab === '#tab2') {
        var specialActiveTab = $('#tab2 div#specialTabs ul li.active');
        if (specialActiveTab.length === 0)
            specialActiveTab = $('#tab2 div#specialTabs ul li:eq(0)');
        $(specialActiveTab).removeClass('active');
        $(specialActiveTab).find('a').tab('show');
        $('.btn-downloadExcel, #uploadExcel+.bootstrap-filestyle, .btn-uploadExcel').addClass('hidden');
        return false;
    }

    var selector = 'div{0}.active'.replace('{0}', hrefTab);
    $(selector).find('table').eq(1).DataTable().columns.adjust();
});

function specialNameChange(element) {
    var specialName = $(element).val();
    if (specialName.length > 20) {
        alert('最多輸入20字');
        return false;
    }

    var rsi_no = $("#RSI_NO").val();
    var group_id = $(element).parents('div.form-horizontal').parent().attr('id');
    $.ajax({
        url: "RDReview_SetSpecialName",
        method: "POST",
        data: { group_name: specialName, rsi_no: rsi_no, group_id: group_id },
        success: function (response) {
            if (response) {
                $('div#specialTabs ul li.active a').text(specialName);
            }
        }
    })
}

function specialDescChange(element) {
    var specialDesc = $(element).val();
    var rsi_no = $("#RSI_NO").val();
    var group_id = $(element).parents('div.form-horizontal').parent().attr('id');
    $.ajax({
        url: "RDReview_SetSpecialDesc",
        method: "POST",
        data: { group_desc: specialDesc, rsi_no: rsi_no, group_id: group_id },
        success: function (response) {
            //success
        }
    })
}

function GetSpecialPartsGroups() {
    var rsi_no = $('#RSI_NO').val();
    $.ajax({
        url: '/RSI/RSI/GetSpecialPartsGroups',
        type: 'POST',
        data: {
            rsi_no: rsi_no
        },
        success: function (response) {
            if (response.length === 0)
                return false;

            $.each(response, function (index, value) {
                AppendSpecialPartsTab(value);
            });
        }
    });
}

function AppendSpecialPartsTab(group_id) {
    var liString =
        '<li data-group-id="{0}">' +
        '<a href="#{1}" data-toggle="tab" style = "display:inline-block; border-right:0px;"></a>' +
        '<i class="fa fa-plus-circle specialicon" style="cursor:pointer; font-size:20px; color:#00a65a;" onclick="creatSpecailPartTab(this)"></i>' +
        '<i class="fa fa-times-circle" style="cursor:pointer; font-size:20px; color: #dd4b39;" onclick="specialdeleteModal(this)" data-group-id="{2}"></i>' +
        '</li>';
    liString = liString.replace('{0}', group_id).replace('{1}', group_id).replace('{2}', group_id);
    $('div#specialTabs ul').append(liString);

    AppendSpecialPartsContent(group_id);

}

function AppendSpecialPartsContent(group_id) {
    var divString =
        '<div id = "{0}" class="specialtabContent tab-pane">' +
        '<div class="form-horizontal">' +
        '<div class="form-group">' +
        '<label class="col-sm-2 control-label" style="margin-right:0px; text-align:left;">Portfolio Name</label>' +
        '<div class="col-sm-5">' +
        '<input type="text" class="form-control" name="groupname" onchange="specialNameChange(this)">' +
        '</div>' +
        '</div>' +
        '<div class="form-group">' +
        '<label class="col-sm-2 control-label" style="margin-right:0px; text-align:left;">Portfolio Description</label>' +
        '<div class="col-sm-5">' +
        '<input type="text" class="form-control" name="specialdesc" onchange="specialDescChange(this)">' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div class="table-response">' +
        '<table class="table table-bordered table-hover specialTable" style="width:100%;" data-group-id="{1}">' +
        '<thead>' +
        '<tr class="bg-light-blue">' +
        '<th class="text-center" style="vertical-align:middle;"><i class="fa fa-minus-circle fa-changeall" aria-hidden="true"></i></th>' +
        '<th class="text-center" style="vertical-align:middle;">Partnumber_Top</th>' +
        '<th class="text-center" style="vertical-align:middle;">Partnumber Parent</th>' +
        '<th class="text-center" style="vertical-align:middle;">Partnumber_Child</th>' +
        '<th class="text-center" style="vertical-align:middle;">Part No</th>' +
        '<th class="text-center" style="vertical-align:middle;">Partpath</th>' +
        '<th class="text-center" style="vertical-align:middle;">Level</th>' +
        '<th class="text-center" style="vertical-align:middle;">Material Group</th>' +
        '<th class="text-center" style="vertical-align:middle;">Material Parts</th>' +
        '<th class="text-center" style="vertical-align:middle;">Part Type</th>' +
        '<th class="text-center" style="vertical-align:middle;">English Name</th>' +
        '<th class="text-center" style="vertical-align:middle;">Part Level</th>' +
        '<th class="text-center" style="vertical-align:middle;">Part Description</th>' +
        '<th class="text-center" style="vertical-align:middle;">Vendor</th>' +
        '<th class="text-center" style="vertical-align:middle;">Maker PN</th>' +
        '<th class="text-center" style="vertical-align:middle;">Part Spec</th>' +
        '<th class="text-center" style="vertical-align:middle;">Spec Def</th>' +
        '<th class="text-center" style="vertical-align:middle;">Release Date</th>' +
        '<th class="text-center" style="vertical-align:middle;">Usage</th>' +
        '<th class="text-center" style="vertical-align:middle;">Unit</th>' +
        '<th class="text-center" style="vertical-align:middle;">Eol</th>' +
        '<th class="text-center" style="vertical-align:middle;">Uni Spec Stutas</th>' +
        '<th class="text-center" style="vertical-align:middle;">Remark</th>' +
        '<th class="text-center" style="vertical-align:middle;">Upload File</th>' +
        '<th class="text-center" style="vertical-align:middle;">Modify Type</th>' +
        '<th class="text-center" style="vertical-align:middle;">SN</th>' +
        '</tr>' +
        '</thead>' +
        '</table>' +
        '</div>' +
        '</div>';
    divString = divString.replace('{0}', group_id).replace('{1}', group_id);
    $('div#specialTabs div#specialContent.tab-content').append(divString);

    BindSpecialTableData(group_id);
    BindSpecialGroupName(group_id);
    BindSpecialGroupDesc(group_id);
}

function BindSpecialGroupName(group_id) {
    var rsi_no = $('#RSI_NO').val();
    $.ajax({
        url: '/RSI/RSI/GetSpecialPartsGroupName',
        type: 'POST',
        data: {
            rsi_no: rsi_no,
            group_id: group_id
        },
        success: function (response) {
            if (response === '')
                response = 'SpecialPartsGroup';

            var selectorTab = 'div#specialTabs ul li a[href="#{0}"]'.replace('{0}', group_id);
            $(selectorTab).text(response);
            var selector = 'div#{0}'.replace('{0}', group_id);
            $(selector).find('input[name="groupname"]').val(response);

        }
    });
}

function BindSpecialGroupDesc(group_id) {
    var rsi_no = $('#RSI_NO').val();
    $.ajax({
        url: '/RSI/RSI/GetSpecialPartsGroupDesc',
        method: 'POST',
        data: {
            rsi_no: rsi_no,
            group_id: group_id
        },
        success: function (response) {
            var selector = 'div#{0}'.replace('{0}', group_id);
            $(selector).find('input[name="specialdesc"]').val(response);
        }
    });
}

function BindSpecialTableData(group_id) {
    specialTableObject.ajax = {
        url: 'RDReview_SpecialParts',
        method: 'POST',
        data: {
            rsi_no: rsi_no,
            group_id: group_id
        },
        dataSrc: ''
    };
    var selector = 'div#{0} table.specialTable'.replace('{0}', group_id);
    var specialsTable = $(selector).DataTable(specialTableObject);
}

function creatSpecailPartTab(element) {
    var thisHref = $(element).prev().attr('href');
    var selector = 'div#specialContent div{0}'.replace('{0}', thisHref);
    var datatable = $(selector).find('table').eq(1);
    var datas = $(datatable).DataTable().rows().data();
    var group_name = $(element).prev().text();

    var specialObjectData = BindSpecialObject(datas);
    $.ajax({
        url: 'RDReview_CreateSpecialParts',
        type: 'POST',
        data: {
            rsi_no: rsi_no,
            sendData: specialObjectData,
            group_name: group_name
        },
        success: function (group_id) {
            AppendSpecialPartsTab(group_id);
        }
    });
}

function specialdeleteModal(element) {
    var group_id = $(element).data('groupId');
    $('.modal-specialdelete input[name="group_id"]').val(group_id);
    $('.modal-specialdelete').modal('show');
}

$(document).on('click', '#btn-specialdelete', function () {
    var group_id = $('.modal-specialdelete input[name="group_id"]').val();
    $.ajax({
        url: 'RDReview_DeleteSpecialParts',
        type: 'POST',
        data: {
            rsi_no: rsi_no,
            group_id: group_id
        },
        success: function (response) {
            RemoveSpecialTabContent(response);
            $('.modal-specialdelete').modal('hide');
        }
    });
});

function BindSpecialObject(sendData) {
    var result = [];
    var bu = $('#BU').val();
    $.each(sendData, function (index, value) {
        value.BU = bu;
        result.push(value);
    });
    return result;
}

function RemoveSpecialTabContent(group_id) {
    var selectorContent = 'div#specialTabs div#specialContent div#{0}'.replace('{0}', group_id);
    $(selectorContent).remove();
    var selectorTab = 'div#specialTabs ul li[data-group-id="{0}"]'.replace('{0}', group_id);
    $(selectorTab).remove();

    var selectorTabFirst = 'div#specialTabs ul li:eq(0)';
    $(selectorTabFirst).find('a').tab('show');
    reloadDataTable();
}

function RDMemberDisabled() {
    var part_type = $('#PartType').val().toUpperCase();
    if (part_type == 'ACD')
        $('#ACDRD_NAME').next().find('.btn').attr('disabled', false);

    if (part_type == 'EE')
        $('#EERD_NAME').next().find('.btn').attr('disabled', false);


    if (part_type == 'OM')
        $('#OMRD_NAME').next().find('.btn').attr('disabled', false);


    if (part_type == 'PACKING')
        $('#PACKINGRD_NAME').next().find('.btn').attr('disabled', false);

    if (part_type == 'TPACD')
        $('#TP_ACDRD_NAME').next().find('.btn').attr('disabled', false);

    if (part_type == 'TPEE')
        $('#TP_EERD_NAME').next().find('.btn').attr('disabled', false);


    if (part_type == 'TPOM')
        $('#TP_OMRD_NAME').next().find('.btn').attr('disabled', false);
}

//function RDMemberLinkToRFQ() {
//    var url = 'http://myauo/';
//    window.open(url, '_blank');
//}

$(document).on('expanded.pushMenu', function () {
    console.log('expanded');
    setTimeout(function () {
        $(normalTable).DataTable().columns.adjust();
        var selector = 'div#specialContent div.specialtabContent.active';
        $(selector).find('table:eq(1)').DataTable().columns.adjust();
    }, 500);
});

$(document).on('collapsed.pushMenu', function () {
    console.log('collapsed');
    setTimeout(function () {
        $(normalTable).DataTable().columns.adjust();
        var selector = 'div#specialContent div.specialtabContent.active';
        $(selector).find('table:eq(1)').DataTable().columns.adjust();
    }, 500);
});

$(document).on('click', '.btn-exportExcel', function () {
    var rsi_no = $("#RSI_NO").val();
    $("#exportExcel #rsi_no").val(rsi_no);
    var projectname = $("#PROJECT_NAME").val();
    $("#exportExcel #projectname").val(projectname);
    var bu = $('#BU').val();
    var part_type = $('#PartType').val();
    var phase_id = 10;
    $("#exportExcel #bu").val(bu);
    $("#exportExcel #part_type").val(part_type);
    $("#exportExcel #phase_id").val(phase_id);
    $("#exportExcel").submit();
});

$(document).on('click', '.btn-downloadExcel', function () {
    var rsi_no = $("#RSI_NO").val();
    var part_type = $('#PartType').val();
    var project_name = $('#PROJECT_NAME').val();

    $("#downloadExcel #downloadExcel_rsi_no").val(rsi_no);
    $("#downloadExcel #downloadExcel_part_type").val(part_type);
    $("#downloadExcel #downloadExcel_project_name").val(project_name);

    $("#downloadExcel").submit();
});

$(document).on('click', '.btn-uploadExcel', function () {
    var rsi_no = $('#RSI_NO').val();
    var parttype = $('#PartType').val();
    var data = new FormData();
    data.append('rsi_no', rsi_no);
    data.append('parttype', parttype);
    var files = $('#uploadExcel').get(0).files;
    if (files.length > 0) {
        data.append("uploadfile", files[0]);
    }
    else {
        alert('請選擇上傳的檔案');
        return false;
    }

    $('#zhezhao').show();
    $('#login').show();

    $.ajax({
        url: '/RSI/RD/UploadExcel',
        type: 'POST',
        contentType: false,
        processData: false,
        dataType: "json",
        data: data,
        success: function (response) {
            $('#zhezhao').hide();
            $('#login').hide();
            var status = response.status;
            $("#uploadExcel").filestyle('clear');
            if (status) {
                var message = response.message;
                alert(message);
                location.reload();
            }
            else {
                var message = response.message;
                var file_name = response.attr1;
                $('#downloadErrorExcel #file_name').val(file_name);
                alert(message);
                $('.btn-downloadErrorExcel').click();
            }
        },
        error: function (response) {
            $('#zhezhao').hide();
            $('#login').hide();
            alert('檔案解析錯誤，請確認上傳檔案是否正確');
        }
    });
});

$(document).on('click', '.btn-downloadErrorExcel', function () {
    $("#downloadErrorExcel").submit();
});

function GetPartLevelArray(query) {
    $.ajax({
        url: 'PartNoSearch_PartLevel',
        type: 'POST',
        success: function (response) {
            $('#partlevel').empty();
            var optionString = '<option value=""></option>';
            $('#partlevel').append(optionString);
            $.each(response, function (index, value) {
                optionString = '<option value="{0}">{1}</option>'.replace('{0}', value).replace('{1}', value);
                if (query !== undefined)
                    $(optionString).attr("selected", true);
                $('#partlevel').append(optionString);
            });
        }
    });
}

function GetEnglishName(query) {
    $.ajax({
        url: 'PartNoSeach_EnglishName',
        async: false,
        type: 'POST',
        data: {
            part_level: query
        },
        success: function (response) {
            $('#englishname').empty();
            var optionString = '<option value=""></option>';
            if (response.length > 1) {
                $('#englishname').append(optionString);
            } else {
                BindMaterialGroupParts(query, response[0]);
            }

            $.each(response, function (index, value) {
                optionString = '<option value="{0}">{1}</option>'.replace('{0}', value).replace('{1}', value);
                $('#englishname').append(optionString);
            });
        }
    });
}

$(document).on('change', 'select#partlevel', function () {
    var query = $(this).val();
    $('#mtlgroup').empty();
    $('#partgroup').empty();
    $('select#mtlparts').empty();
    GetEnglishName(query);
    $('#specdef').text('');
});

$(document).on('change', 'select#englishname', function () {
    $('#specdef').text('');
    BindMaterialGroupParts();
});

function BindMaterialGroupParts(part_level, english_name, mtl_group, parts_group, mtl_parts, sn) {
    var rsi_no = $('#RSI_NO').val();
    var partlevel = $('select#partlevel').val();
    var englishname = $('select#englishname').val();
    var parentpartlevel = $('input#parentpartno').val().substring(0, 2);
    if (part_level != null) partlevel = part_level;
    if (english_name != null) englishname = english_name;

    $.ajax({
        url: 'RDReview_GetMaterialGroupMaterialParts',
        type: 'POST',
        data: {
            rsi_no: rsi_no,
            part_level: partlevel,
            english_name: englishname,
            parent_part_level: parentpartlevel,
            sn : sn
        },
        success: function (response) {
            $('.modal-edit #mtlgroup').empty();
            $('.modal-edit #partgroup').empty();
            $('.modal-edit #mtlparts').empty();
            if (response.length > 0) {
                mterialGroupPartsObject = response;
                BindMtlGroup();
                if (mtl_group != null) {
                    $('.modal-edit #mtlgroup').val(mtl_group);
                    BindPartsGroup(mtl_group);
                }

                if (mtl_group != null && parts_group != null) {
                    $('.modal-edit #partgroup').val(parts_group);
                    BindMtlParts(mtl_group, parts_group);
                }

                if (mtl_group != null && parts_group != null && mtl_parts != null) {
                    $('.modal-edit #mtlparts').val(mtl_parts);
                    BindSpecDef(mtl_group, parts_group, mtl_parts);
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

function GetUpdateDeleteParentSN() {
    var selectedIndex = $(normalTable).DataTable().row({ selected: true }).index();
    var nowpartlevel = $(normalTable).DataTable().row({ selected: true }).data().BOM_LEVEL;
    var resultSN = [];
    for (var i = selectedIndex - 1; i >= 0; i--) {
        var data = $(normalTable).DataTable().row(i).data();
        if (nowpartlevel === undefined || nowpartlevel > data.BOM_LEVEL) {
            nowpartlevel = data.BOM_LEVEL;
            resultSN.push(data.SN);
        }
    }

    var data = $(normalTable).DataTable().row({ selected: true }).data();
    var nowpartno = data.PART_NO;
    if (data.BOM_LEVEL == 0)
        nowpartno = $('#REF_PRODUCT').val();
    var nowsn = data.SN;
    var parent_no = $('#parentpartno').val();
    if (nowpartno == parent_no) {
        resultSN.push(nowsn);
    }
    return resultSN;
}

$(document).on('change', '#radio1', function () {
    $('.div_partno').removeClass('hidden');
    $('.modal-edit .div_partnomfg').removeClass('hidden');
    $('.modal-edit #partnomfg option').remove();
    $('.modal-edit #partnomfg').append("<option value=''></option>");
    $('.div_specdef, .div_itemspec, .div_partgroup').addClass('hidden');
    $('#itemspec').val('');
    $('.modal-edit #partlevel').val('').attr('disabled', true);
    $('.modal-edit #englishname').val('').attr('disabled', true);
    $('.modal-edit #mtlgroup').empty();
    $('.modal-edit #mtlparts').empty();
    $('.modal-edit #partgroup').empty();
    $('.modal-edit #specdef').text('');

    $('#partno').parents('.form-group').removeClass('has-error');
    $('#partlevel').parents('.form-group').removeClass('has-error');
    $('#englishname').parents('.form-group').removeClass('has-error');
    $('#itemspec').parents('.form-group').removeClass('has-error');
    $('#usage').parents('.form-group').removeClass('has-error');
    $('#mtlgroup').parents('.form-group').removeClass('has-error');
    $('#partgroup').parents('.form-group').removeClass('has-error');
    $('#mtlparts').parents('.form-group').removeClass('has-error');
    $('#errormessage').text('');
});

$(document).on('change', '#radio0', function () {
    $('.div_specdef, .div_itemspec, .div_partgroup').removeClass('hidden');
    $('.div_partno').addClass('hidden');
    $('#partno').val('');
    $('.modal-edit .div_partnomfg').addClass('hidden');
    $('.modal-edit #partnomfg option').remove();
    $('.modal-edit #partlevel').val('').attr('disabled', false);
    $('.modal-edit #englishname').val('').attr('disabled', false);
    $('.modal-edit #mtlgroup').empty();
    $('.modal-edit #mtlparts').empty();
    $('.modal-edit #partgroup').empty();
    $('.modal-edit #specdef').text('');

    $('#partno').parents('.form-group').removeClass('has-error');
    $('#partlevel').parents('.form-group').removeClass('has-error');
    $('#englishname').parents('.form-group').removeClass('has-error');
    $('#itemspec').parents('.form-group').removeClass('has-error');
    $('#usage').parents('.form-group').removeClass('has-error');
    $('#mtlgroup').parents('.form-group').removeClass('has-error');
    $('#partgroup').parents('.form-group').removeClass('has-error');
    $('#mtlparts').parents('.form-group').removeClass('has-error');
    $('#errormessage').text('');
});

function GetPartNoMfg(part_no) {
    $.ajax({
        url: 'GetPartNoMfg',
        type: 'POST',
        data: {
            part_no: part_no
        },
        success: function (response) {
            $('#partnomfg option').remove();

            if (response != null)
            {
                $.each(response, function () {
                    var mfg = this.split('-')[1];
                    var option_string = "<option value='" + this + "'>" + mfg + "</option>";

                    $('#partnomfg').append(option_string);
                });
            }

            $('#partnomfg').append("<option value=''></option>");
        }
    });
}
