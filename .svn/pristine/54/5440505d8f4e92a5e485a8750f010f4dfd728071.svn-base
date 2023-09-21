
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
        url: '/RSI/RD/RDBossReview_NormalParts',
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
            name: 'Download File', data: 'FILE_STATUS', defaultContent: '', className:'text-center',
            render: function (data, type, row, meta) {
                if (data === "Y") {
                    return "<button class='btn btn-success btn-sm' onclick='fileManagement(" + rsi_no + "," + row.SN + ")'><i class='fa fa-file-text-o'></i></button>";
                }
                return null;
            }
        },
        { name: 'MODIFY_TYPE', data: 'MODIFY_TYPE', visible: false },
        {
            name: 'SN', data: 'SN', defaultContent: '', visible: false
        }
    ],
    buttons: [
        {
            //text: 'Edit',
            text: '<i class="fa fa-pencil-square-o"></i>',
            titleAttr: 'Edit Part No：細項編輯',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom',
                'data-container': 'body'
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

                if (data.PHASE_ID !== '10') {
                    $('.modal-nopermission').modal('show');
                    return false;
                }

                $('#radio1, #radio0').attr('disabled', false);
                if (data.PART_NO != null)
                    $('#radio1').click();
                else
                    $('#radio0').click();
                $('#radio1, #radio0').attr('disabled', true);

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
                        $('.modal-edit .btn-partnosearch').attr('disabled', false);
                    }

                    if (data.PART_NO === null && data.PART_SPEC !== null) {
                        $('.modal-edit #partno').attr('disabled', true);
                        $('.modal-edit .btn-partnosearch').attr('disabled', true);
                    }
                }, 500);

                $('.modal-edit').modal('show');
            }
        }
    ],
    drawCallback: function (settings) {
        var api = this.api();
        var datas = api.rows({ page: 'current' }).data();
        for (var i = 0; i < datas.length; i++) {
            var node = api.row(i).node();
            $(node).find('td:not(:eq(0))').removeClass('text-bold');
            if (i + 1 !== datas.length) {
                var currentLevel = parseInt(datas[i].BOM_LEVEL);
                var nextLevel = parseInt(datas[i + 1].BOM_LEVEL);
                if (currentLevel + 1 === nextLevel) {
                    $(node).find('td:eq(0)').empty().append('<i class="fa fa-minus-circle" aria-hidden="true" data-row=' + i + '></i>');
                    $(node).find('td:eq(1)').addClass('text-bold');
                }
            }

            var modify_type = datas[i].MODIFY_TYPE;

            if (modify_type === 'S')
                $(node).addClass('danger');

            if (datas[i].PHASE_ID === '10') {
                $(node).addClass('bg-yellow');
            }
        }
    },
    initComplete: function (settings, json) {
        $(this).DataTable().buttons().container()
            .appendTo('#normalTable_wrapper .col-sm-6:eq(0)');

        $('[data-toggle="tooltip"]').tooltip();
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
        { name: '', data: 'PART_NO' },
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
            name: 'Download File', data: 'FILE_STATUS', defaultContent: '', className: 'text-center',
            render: function (data, type, row, meta) {
                if (data === "Y") {
                    return "<button class='btn btn-success btn-sm' onclick='fileManagement(" + rsi_no + "," + row.SN + ")'><i class='fa fa-file-text-o'></i></button>";
                }
                return null;
            }
        },
        { name: 'MODIFY_TYPE', data: 'MODIFY_TYPE', visible: false },
        {
            name: 'SN', data: 'SN', defaultContent: '', visible: false
        }
    ],
    buttons: [
        {
            //text: 'Edit',
            text: '<i class="fa fa-pencil-square-o"></i>',
            titleAttr: 'Edit Part No：細項編輯',
            attr: {
                'data-toggle': 'tooltip',
                'data-placement': 'bottom'
            },
            action: function (e, dt, node, config) {
                var data = dt.row({ selected: true }).data();
                console.log(data);
                if (data === undefined) {
                    $('.modal-selectitem').modal('show');
                    return false;
                }

                if (data.MODIFY_TYPE === 'S') {
                    $('.modal-nopermission').modal('show');
                    return false;
                }

                if (data.PHASE_ID === null) {
                    $('.modal-nopermission').modal('show');
                    return false;
                }

                $('.has-error').removeClass('has-error');
                $('#errormessage').text('');
                $('#editTitle').text('Edit Part No：細項編輯');
                BindMtlGroup();
                GetPartLevelArray(data.PART_LEVEL);
                GetEnglishName(data.PART_LEVEL);

                setTimeout(function () {
                    BindMtlParts(data.MTL_GROUP);

                    $('.modal-edit #parentpartno').val(data.PARTNUMBER_PARENT);
                    $('.modal-edit #partno').val(data.PART_NO);
                    $('.modal-edit #partlevel').val(data.PART_LEVEL).attr('disabled', true);
                    $('.modal-edit #englishname').val(data.ENGLISH_NAME).attr('disabled', true);;
                    $('.modal-edit #partdesc').val(data.PART_DESC);
                    $('.modal-edit #vendor').val(data.MAKER_SOURCE);
                    $('.modal-edit #makerpn').val(data.MAKER_PART_NO);
                    $('.modal-edit #releasedate').val(ConvertDate(data.RELEASE_DATE));
                    $('.modal-edit #mtlgroup').val(data.MTL_GROUP).attr('disabled', true);
                    $('.modal-edit #mtlparts.form-control:not(.hidden)').val(data.MTL_PARTS).attr('disabled', true);
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

                    if (data.PART_NO !== null && data.PART_SPEC === null) {
                        $('.modal-edit #partno').attr('disabled', false);
                        $('.modal-edit .btn-partnosearch').attr('disabled', false);
                        $('.modal-edit #itemspec').attr('disabled', true);
                    }

                    if (data.PART_NO === null && data.PART_SPEC !== null) {
                        $('.modal-edit #partno').attr('disabled', true);
                        $('.modal-edit .btn-partnosearch').attr('disabled', true);
                        $('.modal-edit #itemspec').attr('disabled', false);
                    }
                }, 500);

                $('.modal-edit').modal('show');
            }
        }
    ],
    drawCallback: function (settings) {
        var api = this.api();
        var datas = api.rows({ page: 'current' }).data();
        for (var i = 0; i < datas.length; i++) {
            var node = api.row(i).node();
            var modify_type = datas[i].MODIFY_TYPE;

            if (datas[i].PHASE_ID === '10') {
                $(node).addClass('bg-yellow');
                api.row(i).select();
            }
        }
    },
    initComplete: function (settings, json) {
        var tableid = $(this).attr('id');
        var selector = '#{0}_wrapper .col-sm-6:eq(0)'.replace('{0}', tableid);
        $(this).DataTable().buttons().container().appendTo(selector);
    }
};

function fileTableLoading(rsi_no, sn) {
    $("#fileTable").DataTable({
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

function fileManagement(rsi_no, sn) {
    fileTableLoading(rsi_no, sn);
    $("#fileManagement").modal("show");
}

$(function () {
    window.moveTo(0, 0);
    window.resizeTo(screen.width, screen.height);
    GetPartLevelArray();
    GetMterialGroupParts();
    GetUnit();

    $('.container').css({ "background-image": "url(http://auhqhrap02.corpnet.auo.com/PB20/Webform/GetSpecialWaterMark.ashx)", "background-repeat": "repeat", "background-position": "center" });

    var table = $('#normalTable').DataTable(normalTableObject);
    BindRefModel();
    GetSpecialPartsGroups();

    $('.typeahead').typeahead({
        source: function (query, process) {
            $.ajax({
                url: '/RSI/RD/RDReview_AutoComplatePartNo',
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
            $('#partdesc').val(item.id);
            var part_no = item.name;
            var part_level = part_no.substring(0, 2);
            $('select#partlevel').val(part_level);
            GetEnglishName(part_level);

            setTimeout(function () {
                var english_name = item.english;
                $('select#englishname').val(english_name);
                BindMaterialGroupParts();
            }, 500);
        }
    });
});

function GetPartLevelArray(query) {
    $.ajax({
        url: '/RSI/RD/PartNoSearch_PartLevel',
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

function ConvertDate(data) {
    var re = /-?\d+/;
    var m = re.exec(data);
    var d = new Date(parseInt(m[0]));
    var viewstring = '{0}/{1}/{2}';
    return viewstring.replace('{0}', d.getFullYear()).replace('{1}', d.getMonth() + 1).replace('{2}', d.getDate());
}

$(document).on('click', '.fa-minus-circle:not(.fa-changeall, .specialicon)', function () {
    $(this).removeClass('fa-minus-circle').addClass('fa-plus-circle');
    var row = $(this).data('row');
    var datatable = $(normalTable).DataTable();
    var rowdata = datatable.row(row).data();
    var datas = $(normalTable).DataTable().rows().data();
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
    $(normalTable).DataTable().columns.adjust();
});

$(document).on('click', '.fa-plus-circle:not(.fa-changeall, .specialicon)', function () {
    $(this).removeClass('fa-plus-circle').addClass('fa-minus-circle');
    var row = $(this).data('row');
    var datatable = $(normalTable).DataTable();
    var rowdata = datatable.row(row).data();
    var datas = $(normalTable).DataTable().rows().data();
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

function BindRefModel() {
    var modelpart = $('#REF_PRODUCT').val();
    $('.rsi-model').text(modelpart);
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
        '<a href="#{1}" data-toggle="tab" style="display:inline-block; border-right:0px;"></a>' +
        '</li>';
    liString = liString.replace('{0}', group_id).replace('{1}', group_id);
    $('div#specialTabs ul').append(liString);

    AppendSpecialPartsContent(group_id);

}

function AppendSpecialPartsContent(group_id) {
    var divString =
        '<div id="{0}" class="specialtabContent tab-pane">' +
        '<div class="form-horizontal">' +
        '<div class="form-group">' +
        '<label class="col-sm-2 control-label" style="margin-right:0px; text-align:left;">Portfolio Name</label>' +
        '<div class="col-sm-5">' +
        '<input type="text" class="form-control" name="groupname" disabled>' +
        '</div>' +
        '</div>' +
        '<div class="form-group">' +
        '<label class="col-sm-2 control-label" style="margin-right:0px; text-align:left;">Portfolio Description</label>' +
        '<div class="col-sm-5">' +
        '<input type="text" class="form-control" name="specialdesc" disabled>' +
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
        '<th class="text-center" style="vertical-align:middle;">Download File</th>' +
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
        url: '/RSI/RD/RDBossReview_SpecialParts',
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

$(document).on('shown.bs.tab', 'a[data-toggle="tab"]', function (e) {
    var hrefTab = $(this).attr("href");
    if (hrefTab === '#tab1') {
        $(normalTable).DataTable().columns.adjust();
        return false;
    }

    if (hrefTab === '#tab2') {
        var specialActiveTab = $('#tab2 div#specialTabs ul li.active');
        if (specialActiveTab.length === 0)
            specialActiveTab = $('#tab2 div#specialTabs ul li:eq(0)');
        $(specialActiveTab).removeClass('active');
        $(specialActiveTab).find('a').tab('show');
        return false;
    }

    var selector = 'div{0}.active'.replace('{0}', hrefTab);
    $(selector).find('table').eq(1).DataTable().columns.adjust();
});

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

function BindMtlGroup() {
    var mtlGroup = [];
    $.each(mterialGroupPartsObject, function (index, value) {
        var itemMtlGroup = value.MTL_GROUP;
        if (mtlGroup.indexOf(itemMtlGroup) < 0)
            mtlGroup.push(itemMtlGroup);
    });

    $('#mtlgroup').empty();
    $.each(mtlGroup, function (index, value) {
        var optionString = '<option value="{0}">{1}</option>';
        optionString = optionString.replace('{0}', value).replace('{1}', value);
        $('#mtlgroup').append(optionString);
    });
}

function BindMtlParts(mtlgroup) {
    var mtlParts = [];
    var filterdata = mterialGroupPartsObject.filter(function (d) { return d.MTL_GROUP === mtlgroup });
    $.each(filterdata, function (index, value) {
        var itemMtlParts = value.MTL_PARTS === null ? '' : value.MTL_PARTS;
        if (mtlParts.indexOf(itemMtlParts) < 0)
            mtlParts.push(itemMtlParts);
    });

    if (mtlgroup === 'RD DEFINE') {
        $('input#mtlparts.form-control').removeClass('hidden').val('');
        $('select#mtlparts.form-control').addClass('hidden');
        return;
    }

    $('input#mtlparts.form-control').addClass('hidden').attr('disabled', false);
    $('select#mtlparts.form-control').removeClass('hidden').attr('disabled', false);
    $('select#mtlparts.form-control').empty();
    $.each(mtlParts, function (index, value) {
        var optionString = '<option value="{0}">{1}</option>';
        optionString = optionString.replace('{0}', value).replace('{1}', value);
        $('select#mtlparts.form-control').append(optionString);
    });
}

function GetMterialGroupParts() {
    var bu = $('#BU').val();
    $.ajax({
        url: '/RSI/RD/GetMterialGroupParts',
        method: 'POST',
        data: {
            bu: bu,
            part_type: part_type
        },
        success: function (data) {
            mterialGroupPartsObject = data;
        }
    });
}

function GetUnit() {
    $.ajax({
        url: '/RSI/RD/GetUnit',
        method: 'POST',
        success: function (data) {
            unitObject = data;
            BindUnit();
        }
    });
}

function GetPartLevelArray(query) {
    $.ajax({
        url: '/RSI/RD/PartNoSearch_PartLevel',
        type: 'POST',
        success: function (response) {
            $('#partlevel').empty();
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
        url: '/RSI/RD/PartNoSeach_EnglishName',
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

function BindUnit() {
    $('#unit').empty();
    $.each(unitObject, function (index, value) {
        var optionString = '<option value="{0}">{1}</option>';
        optionString = optionString.replace('{0}', value).replace('{1}', value);
        $('#unit').append(optionString);
    });
}

$(document).on('click', '.btn-partnosearch', function () {
    var partlevel = $('#partlevel').val();
    var englishname = $('#englishname').val();
    var material_parts = $('#mtlparts.form-control:not(.hidden)').val();
    var url = '/RSI/RD/PartNoSearch?part_level={0}&english_name={1}&martial_parts={2}';
    url = url.replace('{0}', partlevel).replace('{1}', englishname).replace('{2}', material_parts);
    window.open(url, 'PartNoSearch', 'scrollbars=yes, width=1000, height=800');
});

function SetPartNoInfo(item_no, english_name, item_desc, vendor, maker_pn, release_date, part_level, material_group, material_part) {
    GetEnglishName(part_level);
    //BindMtlParts(material_group);
    //BindSpecDef(material_group, material_part);

    setTimeout(function () {
        $('#partlevel').val(part_level);
        $('#englishname').val(english_name);
        $('#partno').val(item_no);
        $('#partdesc').val(item_desc);
        $('#vendor').val(vendor);
        $('#makerpn').val(maker_pn);
        $('#releasedate').val(release_date);
        //$('#mtlgroup').val(material_group);
        //$('#mtlparts.form-control:not(.hidden)').val(material_part);
        $('#itemspec').attr('disabled', true);
    }, 500);
}

$(document).on('click', '#btn-save', function () {
    var rsi_no = $('#RSI_NO').val();
    if (InspectPartObject()) {
        var model = BindPartObject();
        $.ajax({
            "url": "/RSI/RD/SaveForLayer1",
            "method": "POST",
            "data": { "h_Product_Details": model, "rsi_no": rsi_no },
            "success": function (data) {
                if (data) {
                    $('.modal-edit').modal('hide');
                    reloadDataTable();
                    //var url = '/RSI/';
                    //window.opener.location.href = url;
                    //return;
                }
                else {
                    alert('save fail');
                }
            }
        });
    }
});

$(document).on('click', '#btn-submit', function () {
    var rsi_no = $('#RSI_NO').val();
    var part_type = $('#PartType').val();
    var bu = $('#BU').val();
    var projectname = $('#PROJECT_NAME').val();
    var phase_id = $('#phase_id').val();
    $.ajax({
        "url": "/RSI/RD/SubmitForLayer1",
        "method": "POST",
        "data": { "rsi_no": rsi_no, "part_type": part_type, "bu": bu, "projectname": projectname, "phase_id": phase_id },
        "success": function (data) {
            if (data) {
                //$('.modal-edit').modal('hide');
                //reloadDataTable();
                var url = '/RSI/';
                window.opener.location.href = url;
                //alert("送出完成");
                $('#myModal').modal('hide');
                window.close();
            } else{
                alert('save fail');
            }
        }
    });
});

function InspectPartObject() {
    var part_no = $('#partno').val();
    var part_level = $('#partlevel').val();
    var english_name = $('#englishname').val();
    var partspec = $('#itemspec').val();
    var usage = $('#usage').val();
    $('#partno').parents('.form-group').removeClass('has-error');
    $('#partlevel').parents('.form-group').removeClass('has-error');
    $('#englishname').parents('.form-group').removeClass('has-error');
    $('#itemspec').parents('.form-group').removeClass('has-error');
    $('#usage').parents('.form-group').removeClass('has-error');
    $('#errormessage').text('');

    if (part_no === '' && partspec === '') {
        $('#partno').parents('.form-group').addClass('has-error');
        $('#itemspec').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請輸入Part No或Part Sepc');
        return false;
    }

    if (part_no !== '' && part_no.length !== 12) {
        $('#partno').parents('.form-group').addClass('has-error');
        $('#errormessage').text('Part No需要12碼');
        return false;
    }

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

    var numberUsage = Number(usage);
    if (isNaN(numberUsage) || numberUsage <= 0) {
        $('#usage').parents('.form-group').addClass('has-error');
        $('#errormessage').text('請確認Usage');
        return false;
    }

    return true;
}

function BindPartObject() {
    var rsi_no = $('#RSI_NO').val();
    var mtl_type = $('#mtltype').val();
    var group_id = $('#groupid').val();
    var mtl_group = $('#mtlgroup').val();
    var mtl_parts = $('#mtlparts.form-control:not(.hidden)').val();
    //var part_type = $('#PartType').val();
    var part_type = $('#parttype').val();
    var parent_partno = $('#parentpartno').val();
    var part_no = $('#partno').val();
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
    var partgroup = $('#partgroup').val();

    if (mtl_group === 'RD DEFINE') {
        //modify_type = 'NEW';
        part_type = $("#PartType").val();
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
        PARTS_GROUP: partgroup
    };
    return result;
}

function BindMaterialGroupParts(part_level, english_name, mtl_group, parts_group, mtl_parts, sn) {
    var rsi_no = $('#RSI_NO').val();
    var partlevel = $('select#partlevel').val();
    var englishname = $('select#englishname').val();
    var parentpartlevel = $('input#parentpartno').val().substring(0, 2);
    if (part_level != null) partlevel = part_level;
    if (english_name != null) englishname = english_name;

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
    $('#specdef').text(spec_def);
    $('#parttype').val(part_type);
}

$(document).on('change', '#radio1', function () {
    $('.div_partno').removeClass('hidden');
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