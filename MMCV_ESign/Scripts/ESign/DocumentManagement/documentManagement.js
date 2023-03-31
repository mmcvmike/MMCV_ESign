/*
* variables
* */

var baseUrl = "/DocumentManagement/";
var activeTabLocalStorage = "docMgrActiveTab";
var tabs = {
    meSign: "#meSign",
    sent: "#sent",
    draft: "#draft",
    cancel: "#cancel"
}

/*
* methods
* */

$(document).ready(() => {
    showActiveTab();
})

function showActiveTab() {
    if (!localStorage.getItem(activeTabLocalStorage))
        localStorage.setItem(activeTabLocalStorage, tabs.meSign);

    var activeTab = localStorage.getItem(activeTabLocalStorage);
    if (activeTab) {
        $(".tab-pane").removeClass("active");
        $(".nav-link").removeClass("active");
        $(activeTab).addClass("active");
        let currentTab = $(document.querySelector(`[href='${activeTab}']`));
        currentTab.click();
        currentTab.addClass('active');
    }
}

function getMeSignDocuments() {
    let action = baseUrl + 'GetMeSignDocuments';
    LoadingShow();
    localStorage.setItem(activeTabLocalStorage, tabs.meSign);
    GetDataAjax(action, function (response) {
        LoadingHide();
        if (response.rs) {
            let data = response.data;
            setTimeout(() => {
                bindMeSignDocumentDataTable(data);
            }, 200);
        }
        else {
            toastr.error(response.msg, "Warning");
        }
    });
}

function getSentDocuments() {
    let action = baseUrl + 'GetSentDocuments';
    LoadingShow();
    localStorage.setItem(activeTabLocalStorage, tabs.sent);
    GetDataAjax(action, function (response) {
        LoadingHide();
        if (response.rs) {
            let data = response.data;
            setTimeout(() => {
                bindSentDocumentDataTable(data);
            }, 200);
        }
        else {
            toastr.error(response.msg, "Warning");
        }
    });
}

function getDraftDocuments() {
    let action = baseUrl + 'GetDraftDocuments';
    LoadingShow();
    localStorage.setItem(activeTabLocalStorage, tabs.draft);
    GetDataAjax(action, function (response) {
        LoadingHide();
        if (response.rs) {
            let data = response.data;
            setTimeout(() => {
                bindDraftDocumentDataTable(data);
            }, 200);
        }
        else {
            toastr.error(response.msg, "Warning");
        }
    });
}

function getCancelDocuments() {
    let action = baseUrl + 'GetCancelDocuments';
    LoadingShow();
    localStorage.setItem(activeTabLocalStorage, tabs.cancel);
    GetDataAjax(action, function (response) {
        LoadingHide();
        if (response.rs) {
            let data = response.data;
            setTimeout(() => {
                bindCancelDocumentDataTable(data);
            }, 200);
        }
        else {
            toastr.error(response.msg, "Warning");
        }
    });
}

function getNameOfStatus(data) {
    let nameOfStatus = "";
    let initial = "<span class='badge badge-primary'>Initial</span>";
    let completed = "<span class='badge badge-success'>Completed</span>";
    let decline = "<span class='badge badge-warning'>Decline</span>";
    let draft = "<span class='badge badge-default'>Draft</span>";
    let cancel = "<span class='badge badge-danger'>Cancel</span>";

    nameOfStatus = data == 0 ? initial : (data == 1 ? completed : (data == 2 ? decline : (data == 3 ? cancel : draft )));

    return nameOfStatus;
}

function getNameOfSignerStatus(data) {
    let nameOfStatusSign = "";
    let signed = "<span class='badge badge-primary'>Signed</span>";
    let declined = "<span class='badge badge-danger'>Declined</span>";
    let initialSign = "<span class='badge badge-ligth'>No action</span>";

    nameOfStatus = data == 0 ? initialSign : (data == 1 ? signed : declined);

    return nameOfStatus;
}

function bindMeSignDocumentDataTable(data) {
    $("#meSignDocumentDT").DataTable({
        "destroy": true,
        "lengthChange": false,
        "pageLength": 10,
        "data": data,
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "sort": false,
        "columns": [
            { "data": "DocumentID", "name": "DocumentID", "autoWidth": false },
            { "data": "ReferenceCode", "name": "ReferenceCode", "autoWidth": false },
            { "data": "Title", "name": "Title", "autoWidth": false },
            {
                data: 'Status',
                render: function (data, type, row) {                  
                    return getNameOfStatus(data);
                },
            },
            {
                data: 'SignerStatus',
                render: function (data, type, row) {
                    return getNameOfSignerStatus(data);
                },
            },
            {
                data: 'CreatedDate',
                render: function (data, type, row) {
                    return data != "/Date(-62135596800000)/" ? formatDDMMYYHHMMSS(justNumbers(data)) : "";
                },
            },
            {
                data: 'Action',
                render: function (data, type, row) {
                    let html = `
                                <a href='/DocumentManagement/DetailView?docId=${row.DocumentID}' class='btn btn-light btn-sm'><i class='fa fa-info text-primary'></i>&nbsp;Detail</a>
                                <a href='/Home/PdfPage?docId=${row.DocumentID}' class='btn btn-light btn-sm'><i class='fa fa-sign text-success'></i>&nbsp;Sign</a>
                                <a href='javascript:void(0)' onclick='quickView(${row.DocumentID})' class='btn btn-light btn-sm'><i class='fa fa-eye text-default'></i>&nbsp;Quick view</a>
                            `;
                    return html;
                },
            },
        ],
        "columnDefs": [
            { "className": "text-center", "targets": "_all" }, //{ "className": "text-center", "targets": [0,1,2,3,4] },
        ],
        "language": {
            "info": "Show _START_ - _END_ / _TOTAL_",
            "paginate": {
                'previous': '<span class="la flaticon-left-arrow"></span>',
                'next': '<span class="la flaticon-right-arrow"></span>'
            },
        },
        scrollX: true,
    });
}

function bindSentDocumentDataTable(data) {
    $("#sentDocumentDT").DataTable({
        "dom": '<"top"i>rt<"bottom"flp><"clear">',
        "destroy": true,
        "lengthChange": false,
        "pageLength": 10,
        "data": data,
        "filter": false, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "sort": false,
        "columns": [
            { "data": "DocumentID", "name": "DocumentID", "autoWidth": false },
            { "data": "ReferenceCode", "name": "ReferenceCode", "autoWidth": false },
            { "data": "Title", "name": "Title", "autoWidth": false },
            {
                data: 'Status',
                render: function (data, type, row) {
                    return getNameOfStatus(data);
                },
            },
            {
                data: 'CreatedDate',
                render: function (data, type, row) {
                    return data != "/Date(-62135596800000)/" ? formatDDMMYYHHMMSS(justNumbers(data)) : "";
                },
            },
            {
                data: 'Action',
                render: function (data, type, row) {
                    let html = `
                                <a href='/DocumentManagement/DetailView?docId=${row.DocumentID}' class='btn btn-light btn-sm'><i class='fa fa-info text-primary'></i>&nbsp;Detail</a>
                                <a href='/Home/PdfPage?docId=${row.DocumentID}' class='btn btn-light btn-sm'><i class='fa fa-sign text-success'></i>&nbsp;Sign</a>
                                <a href='javascript:void(0)' onclick='quickView(${row.DocumentID})' class='btn btn-light btn-sm'><i class='fa fa-eye text-default'></i>&nbsp;Quick view</a>
                            `;
                    return html;
                },
            },
        ],
        "columnDefs": [
            { "className": "text-center", "targets": "_all" },
        ],
        "language": {
            "info": "Show _START_ - _END_ / _TOTAL_",
            "paginate": {
                'previous': '<span class="la flaticon-left-arrow"></span>',
                'next': '<span class="la flaticon-right-arrow"></span>'
            },
        },
        scrollX: true,
    });
}

function bindDraftDocumentDataTable(data) {
    $("#draftDocumentDT").DataTable({
        "destroy": true,
        "lengthChange": false,
        "pageLength": 10,
        "data": data,
        "filter": false, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "sort": false,
        "columns": [
            { "data": "DocumentID", "name": "DocumentID", "autoWidth": false },
            { "data": "ReferenceCode", "name": "ReferenceCode", "autoWidth": false },
            { "data": "Title", "name": "Title", "autoWidth": false },
            {
                data: 'Status',
                render: function (data, type, row) {
                    return getNameOfStatus(data);
                },
            },
            {
                data: 'CreatedDate',
                render: function (data, type, row) {
                    return data != "/Date(-62135596800000)/" ? formatDDMMYYHHMMSS(justNumbers(data)) : "";
                },
            },
            {
                data: 'Action',
                render: function (data, type, row) {
                    let html = `
                                <a href='/DocumentManagement/DetailView?docId=${row.DocumentID}' class='btn btn-light btn-sm'><i class='fa fa-info text-primary'></i>&nbsp;Detail</a>
                                <a href='/Home/PdfPage?docId=${row.DocumentID}' class='btn btn-light btn-sm'><i class='fa fa-sign text-success'></i>&nbsp;Sign</a>
                                <a href='javascript:void(0)' onclick='quickView(${row.DocumentID})' class='btn btn-light btn-sm'><i class='fa fa-eye text-default'></i>&nbsp;Quick view</a>
                            `;
                    return html;
                },
            },
        ],
        "columnDefs": [
            { "className": "text-center", "targets": "_all" },
        ],
        "language": {
            "info": "Show _START_ - _END_ / _TOTAL_",
            "paginate": {
                'previous': '<span class="la flaticon-left-arrow"></span>',
                'next': '<span class="la flaticon-right-arrow"></span>'
            },
        },
        scrollX: true,
    });
}

function bindCancelDocumentDataTable(data) {
    $("#cancelDocumentDT").DataTable({
        "destroy": true,
        "lengthChange": false,
        "pageLength": 10,
        "data": data,
        "filter": false, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "sort": false,
        "columns": [
            { "data": "DocumentID", "name": "DocumentID", "autoWidth": false },
            { "data": "ReferenceCode", "name": "ReferenceCode", "autoWidth": false },
            { "data": "Title", "name": "Title", "autoWidth": false },
            {
                data: 'Status',
                render: function (data, type, row) {
                    return getNameOfStatus(data);
                },
            },
            {
                data: 'CreatedDate',
                render: function (data, type, row) {
                    return data != "/Date(-62135596800000)/" ? formatDDMMYYHHMMSS(justNumbers(data)) : "";
                },
            },
            {
                data: 'Action',
                render: function (data, type, row) {
                    let html = `
                                <a href='/DocumentManagement/DetailView?docId=${row.DocumentID}' class='btn btn-light btn-sm'><i class='fa fa-info text-primary'></i>&nbsp;Detail</a>
                                <a href='javascript:void(0)' onclick='quickView(${row.DocumentID})' class='btn btn-light btn-sm'><i class='fa fa-eye text-default'></i>&nbsp;Quick view</a>
                            `;
                    return html;
                },
            },
        ],
        "columnDefs": [
            { "className": "text-center", "targets": "_all" },
        ],
        "language": {
            "info": "Show _START_ - _END_ / _TOTAL_",
            "paginate": {
                'previous': '<span class="la flaticon-left-arrow"></span>',
                'next': '<span class="la flaticon-right-arrow"></span>'
            },
        },
        scrollX: true,
    });
}

function addDocument() {
    let action = baseUrl + 'AddDocument';
    var datasend = JSON.stringify({

    });
    LoadingShow();
    PostDataAjax(action, datasend, function (response) {
        LoadingHide();
        if (response.rs) {
            getDocuments();
            toastr.success(response.msg, "Success");
        }
        else {
            toastr.error(response.msg, "Warning");
        }
    });
}

function viewDocument() {
    let action = '/Home/PdfPage';
    var datasend = JSON.stringify({

    });
    LoadingShow();
    PostDataAjax(action, datasend, function (response) {
        LoadingHide();
        if (response.rs) {
            getDocuments();
            toastr.success(response.msg, "Success");
        }
        else {
            toastr.error(response.msg, "Warning");
        }
    });
}

function quickView(id) {
    LoadingShow();
    let action = baseUrl + 'GetDocumentBase64Data';
    var datasend = {
        docId: id
    };
    PostDataAjax(action, datasend, function (response) {
        if (response.rs) {
            let data = response.data;
            let mimeType = "data:application/pdf;base64,";
            let base64 = data.Base64File;

            $("#quickViewModal").modal("show");
            $("#qvFrame").attr("src", mimeType + base64);   
            let iframe = $('iframe');
            if (iframe.length) {
                $(iframe).on('load', function () {
                    setTimeout(() => {
                        LoadingHide();
                    })
                });
            }
        }
        else {
            LoadingHide();
            toastr.error(response.msg, "Warning");
        }
    });
}