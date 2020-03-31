﻿addCodeSuccess = function (response) {
    let code = {
        0: response["0"],
        1: $('#CodeString').val().toUpperCase(),
        2: 0,
        3: $('#MaxTimesUsed').val()
    };
    const p = ["codeId=" + code[0]];
    const deleteForm = createDeleteBtn("Admin", "DeleteCodeAJAX",
        p, 'deleteCodeSuccess', 'deleteCodeFailure');
    addTableRows("codesTable", code, deleteForm);
    successToast.fire({
        title: 'Code added successfully',
        position: 'top-end',
        timer: 3000
    });


    $('#addCodeModal').modal('toggle');
    $('#addCodeForm').trigger('reset');
};

queueApproveSuccess = function (response) {
    const element = document.getElementById(response["0"]);
    element.parentNode.removeChild(element);
    // TODO insert row to upcoming local shows table
    successToast.fire({
        title: 'Queue item approved successfully',
        position: 'top-end',
        timer: 3000
    })
};
addConcertSuccess = function (response) {
    let concerts = {
        0: response["1"],
        1: $('#Artists').val(),
        2: $('#TimeStart').val(),
        3: $('#VenueName').val(),
        4: $('#IsApproved').val(),
    };

    const param = ['localConcertId=' + response["1"]];

    const deleteForm = createDeleteBtn("Admin", "DeleteConcertAJAX", param, 'deleteConcertSuccess',);

    addTableRows("localConcertsTable", concerts, deleteForm);

    successToast.fire({
        title: 'Concert added successfully',
        position: 'top-end',
        timer: 3000
    });

    $('#addConcertModal').modal('toggle');
    $('#addConcertForm').trigger('reset');
};

deleteTableRowSuccess = function (response) {
    // the element to delete
    const element = document.getElementById(response["0"]);
    element.parentNode.removeChild(element);
    // notify success
    successToast.fire({
        title: 'Deleted successfully',
        position: 'top-end',
        timer: 3000
    })
};
genericSuccess = function (response) {
    successToast.fire({
        title: 'Operation was successful.',
        position: 'top-end',
        timer: 2000,
        onClose: () => {
            sleep(100);
            window.location.reload();
        }
    })
};
genericFailure = function (response) {
    errorToast.fire({
        title: 'Something went wrong.',
        position: 'top-end',
        timer: 2000,
    })

};
const warningToast = Swal.mixin({
    toast: true,
    icon: 'warning',
    showConfirmButton: false,
});
const successToast = Swal.mixin({
    toast: true,
    icon: 'success',
    showConfirmButton: false,
    timerProgressBar: true,
    onOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer);
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    },

});
// default error toast
const errorToast = Swal.mixin({
    toast: true,
    position: 'top-end',
    icon: 'error',
    showConfirmButton: false,
    onOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer);
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    },

});

function createDeleteBtn(controller, action, params, onSuccess, onFailure) {
    const deleteForm = document.createElement('form');
    let paramsString = '';
    for (let i = 0; i < params.length; i++) {
        if (i > 0) {
            paramsString = paramsString + '&' + params[i];
            continue;
        }
        paramsString = paramsString + params[i];
    }
    console.log(paramsString);

    deleteForm.setAttribute('action',
        '/' + controller + '/' + action + '?' + paramsString);
    deleteForm.setAttribute('data-ajax', 'true');
    deleteForm.setAttribute('data-ajax-method', 'GET');
    deleteForm.setAttribute('data-ajax-success', onSuccess);
    deleteForm.setAttribute('data-ajax-failure', onFailure);
    deleteForm.setAttribute('method', 'post');

    const deleteBtn = document.createElement('button');
    deleteBtn.className = 'btn badge-danger text-light';
    deleteBtn.innerHTML = 'Delete';

    deleteForm.appendChild(deleteBtn);

    return deleteForm;

}

function addTableRows(tableId, data, deleteBtn) {
    const tableRef = document.getElementById(tableId).getElementsByTagName('tbody')[0];
    const colCount = document.getElementById(tableId).rows[0].cells.length;

    const newRow = tableRef.insertRow();
    newRow.id = data[0];
    // column 0 is reserved for id
    for (let i = 0; i < colCount; i++) {
        // if last row add a delete button
        if (i == colCount - 1) {
            var cell = newRow.insertCell(i);
            cell.appendChild(deleteBtn);
            break;
        }
        var cell = newRow.insertCell(i);

        // Append a text node to the cell
        const newText = document.createTextNode(data[i]);
        cell.appendChild(newText);
    }
}

function loadingSkullModal() {
    $('#modalForm').hide();
    $('#loadingSkull').show();
    warningToast.fire({
        title: "Uploading file",
        position: "center",
        timer: 1800
    });
}
