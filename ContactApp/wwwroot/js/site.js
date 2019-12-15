// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function() {

    //Init tooltips
    $('[data-toggle="tooltip"]').tooltip();

    //Index data table init
    $('#phones-table').DataTable();

    //Delete ajax alert action
    $("#phones-table").on("click",".delete-contact", function() {

        var deleteId = $(this).attr("data-deleteid");
        
        //Popup Alert Question
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    cache: false,
                    type: "DELETE",
                    url: "/contact/delete/" + deleteId,
                    success: function(data, textStatus, jqXHR) {
                        location.reload();
                    },
                    error: function(jqXHR, textStatus, errorThrown) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            html:'Something went wrong! <br/>' +
                                 '<div>' + jqXHR.responseText 
                                +'</div>'
                        });
                    }
                });
            }
        });
    });
});