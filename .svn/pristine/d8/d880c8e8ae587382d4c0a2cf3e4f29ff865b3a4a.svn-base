
var rsi_no = $('#RSI_NO').val();
var part_type = $('#PartType').val();

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
        url: 'PLReview_NormalParts',
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
        setTimeout(function () {
            $('.fa-minus-circle.fa-changeall').click();
        }, 500);
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
    ]
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
    var table = $('#normalTable').DataTable(normalTableObject);
    BindRefModel();
    GetSpecialPartsGroups();
});

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
        url: 'PLReview_SpecialParts',
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

$(document).on('click', '.btn-exportExcel', function () {
    var rsi_no = $("#RSI_NO").val();
    $("#exportExcel #rsi_no").val(rsi_no);
    var projectname = $("#PROJECT_NAME").val();
    $("#exportExcel #projectname").val(projectname);
    var bu = $('#BU').val();
    var part_type = $('#PartType').val();
    var phase_id = 30;
    $("#exportExcel #bu").val(bu);
    $("#exportExcel #part_type").val(part_type);
    $("#exportExcel #phase_id").val(phase_id);
    $("#exportExcel").submit();
});