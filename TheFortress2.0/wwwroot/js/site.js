// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
class ClassWatcher {

    constructor(targetNode, classToWatch, classAddedCallback, classRemovedCallback) {
        this.targetNode = targetNode;
        this.classToWatch = classToWatch;
        this.classAddedCallback = classAddedCallback;
        this.classRemovedCallback = classRemovedCallback;
        this.observer = null;
        this.lastClassState = targetNode.classList.contains(this.classToWatch);

        this.init()
    }

    init() {
        this.observer = new MutationObserver(this.mutationCallback);
        this.observe()
    }

    observe() {
        this.observer.observe(this.targetNode, {attributes: true})
    }

    disconnect() {
        this.observer.disconnect()
    }

    mutationCallback = mutationsList => {
        for (let mutation of mutationsList) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                let currentClassState = mutation.target.classList.contains(this.classToWatch);
                if (this.lastClassState !== currentClassState) {
                    this.lastClassState = currentClassState;
                    if (currentClassState) {
                        this.classAddedCallback()
                    } else {
                        this.classRemovedCallback()
                    }
                }
            }
        }
    }
}

//const swal = require('sweetalert2');
//import Swal from 'limonte-sweetalert2/sweetalert2.js'

// custom sleep function
function sleep(milliseconds) {
    const date = Date.now();
    let currentDate = null;
    do {
        currentDate = Date.now();
    } while (currentDate - date < milliseconds);
}

// AJAX success/failure fxns

sendMessageSuccess = function (response) {
    successToast.fire({
        title: 'Message sent successfully.',
        position: 'top-end',
        timer: 2000,
        onClose: () => {
            sleep(100);
            window.location.reload();
        }
    })

};
sendMessageFailure = function (response) {
    errorToast.fire({
        title: 'Something went wrong. Message not sent.',
        position: 'top-end',
        timer: 2000,
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

loginSuccess = function (response) {
    successToast.fire({
        title: 'Login successful',
        position: 'top-end',
        timer: 1000,
        onClose: () => {
            sleep(100);
            window.location.reload();
        }
    });
};
logoutSuccess = function (response) {
    successToast.fire({
        title: 'Logout successful.',
        position: 'top-end',
        timer: 1000,
        onClose: () => {
            sleep(100);
            window.location.reload();
        }
    });
};
loginFailure = function (xhr) {
    errorToast.fire({
        title: 'Login unsuccessful',
        onClose: () => {
            sleep(100)
        }
    });
};
registrationSuccess = function (response) {
    successToast.fire({
        title: 'Registration successful.',
        position: 'top-end',
        timer: 2000,
        onClose: () => {
            sleep(100);
            window.location.href = "/Manage/CheckEmail"
        }
    });
};

commentSuccess = function (response) {
    var commentid = response["0"];
    var username = response["1"];
    var eventid = response["2"];
    var content = response["3"];
    var parentid = response["4"];

    console.log(commentid);
    console.log(username);
    console.log(eventid);
    console.log(content);
    console.log(parentid);

    var insertThis = createCommentCard(commentid, username, eventid, content);

    if (parentid == 'null') {
        $('#addCommentModal').modal('toggle');
        $('#Content').val('');

        $('#' + eventid + ' ul:first').prepend(insertThis);
    } else {
        $('#replyModal').modal('toggle');
        $('#Content').val('');

        $('#' + parentid + ' ul:first').prepend(insertThis);
    }

    successToast.fire({
        title: 'Reply was successful',
        position: 'top-end',
        timer: 3000,
        onClose: () => {
            sleep(100);
        }
    });
};
const warningToast = Swal.mixin({
    toast: true,
    icon: 'warning',
    showConfirmButton: false,

});

// default Success toast
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

// Clear form functions
function clearConcertModal() {
    $('#Artists').val("");
    $('#VenueName').val("");
    $('#TimeStart').val("");
    $('#TimeEnd').val("");
    $('#FlyerUrl').val("");
    $('#IsApproved').val("");
}

// Pagination functions
function getPaginationMonths() {
    return $('#concertPaginationMonths').children('a');
}

function showConcertPagination(pagination_id) {
    const id = '#concert' + pagination_id;
    // console.log(id);

    $('.container-active').hide();
    $('.container-active').removeClass("container-active");
    $(id).show();
    $(id).addClass("container-active");

}


// file upload for house show
$("#showUploadButton").click(function () {
    $('#addToApprovalForm').hide();
    $('#loadingSkull').show();
    warningToast.fire({
        title: 'Verifying file; please wait. show',
        position: 'center',
        timer: 1800,
    });
    let file = document.getElementById("FlyerUrlUpload-show");
    let formData = new FormData();

    formData.append("file", file.files[0]);
    formData.append('artists', $('#sfArtists').val());
    formData.append('venue', $('#sfVenueName').val());
    formData.append('timeStart', $('#sfTimeStart').val());
    formData.append('timeEnd', $('#sfTimeEnd').val());
    console.log(file.files[0]);
    console.log(formData.get('artists'));
    console.log(formData.get('timeStart'));

    console.log(formData);
    $.ajax({
        url: "/Upload/UploadShowAjax",
        type: "POST",
        processData: false,
        contentType: false,
        data: formData,
        success: function () {
            //alert("File uploaded!");
            successToast.fire({
                title: 'Show added to queue successfully. Redirecting to Home page.',
                position: 'center',
                timer: 2000,
                onClose: () => {
                    sleep(100);
                    window.location.href = "/Home/Index"
                }
            });
        },
        error: function (xhr, options, error) {
            errorToast.fire({
                title: 'Something went wrong.',
                position: 'center',
                timer: 2000,
                onClose: () => {
                    sleep(100);
                    // window.location.reload();
                }
            });
            console.log("do some error handling here");
        }
    });
});
$("#concertUploadButton").click(function () {
    warningToast.fire({
        title: 'Verifying file; please wait.',
        position: 'center',
        timer: 1800,
    });
    $('#addToApprovalForm').hide();
    $('#loadingSkull').show();
    
    let file = document.getElementById("FlyerUrlUpload");
    let formData = new FormData();

    formData.append("file", file.files[0]);
    formData.append('artists', $('#cfArtists').val());
    formData.append('venue', $('#cfVenueName').val());
    formData.append('timeStart', $('#cfTimeStart').val());
    formData.append('timeEnd', $('#cfTimeEnd').val());
    console.log(file.files[0]);
    console.log(formData.get('artists'));
    console.log(formData.get('timeStart'));

    console.log(formData);
    $.ajax({
        url: "/Upload/UploadConcertAjax",
        type: "POST",
        processData: false,
        contentType: false,
        data: formData,
        success: function () {
            //alert("File uploaded!");
            successToast.fire({
                title: 'Concert added to queue successfully. Redirecting to Home page.',
                position: 'center',
                timer: 2000,
                onClose: () => {
                    sleep(100);
                    window.location.href = "/Home/Index"
                }
            });
        },
        error: function (xhr, options, error) {
            errorToast.fire({
                title: 'Something went wrong.',
                position: 'center',
                timer: 2000,
                onClose: () => {
                    sleep(100);
                    // window.location.reload();
                }
            });
            console.log("do some error handling here");
        }
    });
});


function createCommentCard(commentid, username, eventid, content) {
    var mainCard = $('<div></div>').addClass('card card-primary card-comment')
        .attr("id", commentid);

    var header = $('<div></div>').addClass('card-header');
    var title = $('<h3></h3>').addClass('card-title').text(username);
    header.append(title);

    var tools = $('<div></div>').addClass('card-tools');
    var button1 = $('<button></button>').attr('type', 'button')
        .addClass('btn btn-tool')
        .attr('data-card-widget', 'collapse');
    var fa1 = $('<i></i>').addClass('fas fa-minus');
    button1.append(fa1);
    tools.append(button1);
    var button2 = $('<button></button>').attr('type', 'button')
        .addClass('btn btn-tool')
        .attr('data-toggle', 'modal')
        .attr('data-parentid', commentid)
        .attr('data-eventid', eventid)
        .attr('href', '#replyModal');
    var fa2 = $('<i></i>').addClass('fas fa-reply');
    button2.append(fa2);
    tools.append(button2);

    header.append(tools);
    mainCard.append(header);

    var body = $('<div></div>').addClass('card-body div-gradient');
    var p = $('<p></p>').addClass('text-light').text(content);
    body.append(p);
    body.append('<br/>');
    body.append('<ul></ul>');

    mainCard.append(body);

    return mainCard;
}


function resetCommentModals(modalId) {
    console.log(modalId);
    $('#' + modalId)[0].reset();
}

$('#testBtn').click(function () {
    const element = document.getElementById('rowDelete');
    element.parentNode.removeChild(element);

});

$(function () {
    $('[data-toggle="tooltip"]').tooltip({
        animated: 'fade',
        placement: 'bottom',
        html: true,
        container: 'body',
    })
});

$(function () {
    $('[data-toggle="popover"]').popover({
        trigger: 'hover',
    })
});

$("#menu-toggle").click(function (e) {
    e.preventDefault();
    $("#wrapper").toggleClass("toggled");
});

// pass a value from button or element to a modal
$('#replyModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    var parentid = button.data('parentid'); // Extract info from data-* attributes
    var eventid = button.data('eventid'); // Extract info from data-* attributes
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this);
    modal.find('.modal-title').text('New reply');
    document.getElementById('eventIdInput').value = eventid;
    document.getElementById('parentIdInput').value = parentid;
});

$('#addCommentModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    var recipient = button.data('eventid'); // Extract info from data-* attributes
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this);
    console.log(recipient);
    modal.find('.modal-title').text('New reply');
    document.getElementById('eventIdInputNew').value = recipient;
});

function changeCommentSectionName(name) {
    console.log(name);
    document.getElementById('comment-section').innerHTML = name;
}

function showCommentsForEvent(eventId) {
    $('.comments-div').hide();
    // console.log(eventId);
    // console.log('#' + eventId);
    $('#' + eventId).show();
}

// this section is for listening to the body element 
let targetNode = document.getElementById('main-body-element');
console.log(targetNode.id);

// when showing the Sidebar (comments)
function onSidebarCollapseRemoval() {
    var buttons = document.getElementsByClassName('comment-show-button');

    for (let i = 0; i < buttons.length; i++) {
        buttons[i].removeAttribute('data-widget');
    }
}

// on closing the Sidebar
function onSidebarCollapseAdd() {
    var buttons = document.getElementsByClassName('comment-show-button');

    for (let i = 0; i < buttons.length; i++) {
        buttons[i].setAttribute('data-widget', 'pushmenu');
    }
}

let classWatcher = new ClassWatcher(targetNode, 'sidebar-collapse', onSidebarCollapseAdd, onSidebarCollapseRemoval);

$(window).scroll(function () {
    var ypos = $(window).scrollTop(); //pixels the site is scrolled down
    var visible = $(window).height() + 110; //visible pixels
    const img_height = 1536; //replace with height of your image
    var max_scroll = img_height - visible; //number of pixels of the image not visible at bottom
//change position of background-image as long as there is something not visible at the bottom  
    if (max_scroll > ypos) {
        $('body').css('background-position', "center -" + ypos + "px");
    } else {
        $('body').css('background-position', "center -" + max_scroll + "px");
    }
});

// Trusted account form switch
$('#showConcertForm').click(function () {
    $('#showForm').hide();
    $('#concertForm').show();
});
$('#showShowForm').click(function () {
    $('#showForm').show();
    $('#concertForm').hide();
});

$('#test').click(function(){
});
function loadingSkullModal() {
    $('#addToApprovalForm').hide();
    $('#loadingSkull').show();
    warningToast.fire({
        title: "Uploading file",
        position: "center",
        timer: 1800
    });
}
addConcertSuccess = function (response) {
    successToast.fire({
        title: 'Concert added successfully',
        position: 'top-end',
        timer: 2000,
        onClose: () => {
            sleep(100);
            window.location.reload();
        }
    });
};

$('#localConcertRadio').click(function () {
    $('#trustForm2').hide();
    $('#trustForm1').show();
});

$('#houseShowRadio').click(function () {
    $('#trustForm1').hide();
    $('#trustForm2').show();
});
