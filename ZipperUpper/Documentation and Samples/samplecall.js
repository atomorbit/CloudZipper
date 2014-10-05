//Make a POST call to the ZipApi with a stringified string array
//that consists of URL strings which are target files to be
//downloaded.

//If POST operation is successful, take the ticket number
//returned by the POST operation, and append it to a GET
//URL that is used to download the file.
function download(array) {
    $.ajax({
        url: "http://cloudzipper.azurewebsites.net/api/zip",
        type: "POST",
        data: { '': JSON.stringify(array) },
        success: function (data, textStatus, jqXHR) {
            location.href = "http://cloudzipper.azurewebsites.net/api/zip/" + data;
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
}