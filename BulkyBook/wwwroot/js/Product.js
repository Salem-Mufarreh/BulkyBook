var dataTable;
$(document).ready(function () {
    loadDataTables();
});

function loadDataTables() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "16%" },
            { "data": "isbn", "width": "16%" },
            { "data": "price", "width": "16%" },
            { "data": "author", "width": "16%" },
            { "data": "category.name", "width": "16%" },
            {
                "data": "id", "render": function (data) {
                    return ` <div class="text-center">
                             <a href='/Admin/Product/Upsert/${data}' class="btn btn-success text-white" style="cursor:pointer"><i class="fas fa-edit"></i></a>
                             <a onclick=Delete("/Admin/Product/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"><i class="fas fa-trash-alt"></i></a>
                             </div>
                            `
                }
            }
        ],
    });
}

function Delete(url) {
    swal({
        title: "Are You Sure You Want To Delete?",
        text: "You will not be able to restore the Data!",
        icon: "warning",
        buttons: true,
        dangerMode: true

    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}