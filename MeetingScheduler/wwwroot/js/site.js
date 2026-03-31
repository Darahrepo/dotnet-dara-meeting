
///////////////////////////////////////////////////////////
/////////// ======== Loader for forms ======== ////////////
///////////////////////////////////////////////////////////

$("#form").on('submit', function (e) {
    $(".overlay").show()
})



CheckMeetingAvailability();
Dropzone.autoDiscover = false;

$(function () {
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
    $("#IsShared_div").hide();
    $(".overlay").hide();

    $('.select2 , .select2bs4').select2({
        theme: 'bootstrap4'
    });

    $('#summernote').summernote({
        minHeight: 300,
        maxHeight: 500,
        callbacks: {
            onPaste: function (e) {
                var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
                e.preventDefault();
                document.execCommand('insertText', false, bufferText);
            }
        }
    });

    $('.duallistbox').bootstrapDualListbox()

    $("input[data-bootstrap-switch]").each(function () {
        $(this).bootstrapSwitch('state', $(this).prop('checked'));
    })


    $('.tab-action-btn-next').click((e) => {
        //var panelistAdded = AreMorePanelistsAdded();
        var attendeeAdded = true;
        //if (controllerName == "Meeting") {
        //    attendeeAdded = AreMoreAttendeesAdded();
        //}
        let elementTab = $(this).parents('.tab-pane');
        var isValid = $('#form').valid();
        let errors = elementTab.find('.required.error').length;
        if (errors > 0 || isValid == false || preventSubmit || !attendeeAdded || !timingAvailable) {
            $('#custom-tabs-three-tab > .active').nextAll('a').addClass('disabled');
            return false;
        } else {
            var test = $('#custom-tabs-three-tab > .active').next('a');
            $('#custom-tabs-three-tab > .active').next(' a').removeClass('disabled')[0].click();
            e.preventDefault();
            return false;
        }
    });


    /***********************************************/
    /*********          Start            ***********/
    /********* Date Time Picker Settings ***********/
    /***********************************************/

    $("#shareAttachments_div").hide();

    var timeFormat = 'HH:mm';
    var maxTime = "23:59";
    $.datepicker.setDefaults($.datepicker.regional["ar"]);

    $('#reservationdate').datetimepicker({
        format: 'YYYY-MM-DD',
        direction: 'ltr',
        locale: moment.locale('ar'),
        keepOpen: true,
        icons: {
            previous: 'fa fa-chevron-up',
            next: 'fa fa-chevron-down'
        },
        daysOfWeekDisabled: [],
        minDate: moment().format('YYYY-MM-DD')
    });

    //Timepicker
    $('#timepickerFrom').datetimepicker({
        format: 'LT',
        orientation: "auto",
        direction: 'ltr',
        rtl: false,
        format: timeFormat
    })

    var minStartTime = new Date(moment().add(5, 'minutes'));
    var minEndTime = new Date(moment(minStartTime).add(15, 'minutes'));

    $("#timepickerFrom").data('datetimepicker').minDate(minStartTime);


    $('#timepickerTo').datetimepicker({
        format: 'LT',
        orientation: "auto",
        rtl: false,
        direction: 'ltr',
        format: timeFormat,
        autoclose: false
    })

    $("#timepickerTo").data('datetimepicker').minDate(minEndTime);

    $('#timepickerFrom').on("change.datetimepicker", ({ date }) => {
        var minTime = new Date(moment(date).add(15, 'minutes'));
        $('#timepickerTo').data("datetimepicker").minDate(minTime);
    });


    $('#reservationdate').on("change.datetimepicker", function () {
        if ($("#date").val() === moment().format("YYYY-MM-DD")) {
            var minTime = moment().add(5, 'minutes');
            $('#timepickerFrom').data("datetimepicker").date(minStartTime);
            $('#timepickerFrom').data("datetimepicker").minDate(minStartTime);

            $('#timepickerTo').data("datetimepicker").date(minTime.add(15, 'minutes'));

        } else {
            $('#timepickerFrom').data("datetimepicker").minDate(moment({ hour: 08 }));
            $('#timepickerTo').data("datetimepicker").minDate(moment({ hour: 08, minute: 30 }));
        }
    });



    $('#timepickerFrom,#timepickerTo').on('show.datetimepicker', function () {

        if ($('.timepicker').is(':visible')) {
            var rows = $('.timepicker>.timepicker-picker>table>tr');
            var tr = '<tr class="customText"><td style= "height: 24px; line-height: 24px; color:grey">' + hoursTranslation + '</td><td class="separator"></td> <td style= "height: 24px; line-height: 24px; color:grey">' + minutesTranslation +'</td></tr>';
            $(rows[1]).after(tr);
        }
    });




    /***********************************************/
    /*******           Start                 *******/
    /******* hiding and show meeting room   ********/
    /***********************************************/

    var meetingRoomsCount = $('#MeetingRooms option').length;

    if (meetingRoomsCount <= 0) {
        $("input[type=radio][value='F']").attr("disabled", true);
        $("input[type=radio][value='B']").attr("disabled", true);
        $("#MeetingRoomDdl").hide();
    }
    $('input[name="MeetingLocation"]').trigger('click');

    $('input[name="MeetingLocation"]').click(function () {
        if ($(this).attr("value") == "F") {
            $("#MeetingRoomDdl").prop('required', true);
            $("#IsRecorded_div").find('input:checkbox:first').prop('checked', false);
            $("#IsRecorded_div").hide();
            $("#MeetingRoomDdl").show();
        }
        if ($(this).attr("value") == "O") {
            $("#MeetingRoomDdl").prop('required', false);
            $("#IsRecorded_div").show();
            $("#MeetingRoomDdl").hide();
        }
        if ($(this).attr("value") == "B") {
            id = "MeetingRoomDdl"
            $("#MeetingRoomDdl").show();
            $("#IsRecorded_div").show();
            $("#MeetingRoomDdl").prop('required', true);
        }
    });



    $('.fileinput-remove-button').on('click', function () {
        $("#IsShared_div").find('input:checkbox:first').prop('checked', false);
        $("#IsShared_div").hide();
    });

    $('input[type=file]').change(function () {
        fileCount = this.files.length;
        if (fileCount <= 0) {
            $("#IsShared_div").hide();
            $(".dz-message").show();
        } else {
            $("#IsShared_div").show();
            $(".dz-message").hide();
        }
    })

    $("#interpretersDiv").hide();
    $('input[name="IsTranslationNeeded"]').bind('load change',function (e) {
        if ($(this).is(":checked")) {
            $("#interpretersDiv").show();
            $('#divId').find(':input').addClass('required');
            $('.languageChange').trigger('load');
            $('#custom-tabs-three-tab a:first-child ').tab('show');
            $('#custom-tabs-three-tab > .active').nextAll('a').addClass('disabled');
        } else {
            $("#interpretersDiv").hide();
            preventSubmit = false;
            $('#divId').find(':input').removeClass('required');
        }

    });

    /***********************************************/
   /*******            End                 ********/
  /******* hiding and show meeting room   ********/
 /***********************************************/
});


function Details(id) {
    $(".overlay").show();
    $.get(ROOT + (controllerName + "/View/") + id ,
    function (data) {
        $('.modal-body div').html(data);
        $(".overlay").hide();
    });
}



function changeLanguage(index) {
    var message = $("#" + index);
    message.css("color", "red");

    if ($("#From_language_" + index).val() == $("#To_language_" + index).val()) {
        message.html(similarLanguageError);
        preventSubmit = true;
    } else {
        message.html("");
        preventSubmit = false;
    }
};
   /***********************************************/
  /*********           End             ***********/
 /********* Date Time Picker Settings ***********/
/***********************************************/




var typingTimer;
var doneTypingInterval = 1000;
var $input = $('#myInput');



function StartChangingValues() {
    clearTimeout(typingTimer);
    typingTimer = setTimeout(CheckMeetingAvailability, doneTypingInterval);
};



function CheckMeetingAvailability() {
    var result = true;
    var dateNow = new Date();
    var to = $('#Time_To').val();
    var from = $('#Time_From').val();
    var date = $('#date').val();
    var hostId = $("#hostId").val();
    var isWebex = $("#is-webex").prop('checked'); //will return true if webex
    var isCeo = $("#is-ceo").prop('checked'); //will return true if webex
    var url = '';
    var roomId = 0;

    if(isWebex === true) {
        //$("#IsCeo_div").hide();
        //$("#both").prop("checked",true);
        $("#is-webex").prop("disabled", true);
        //$("#faceToFace").prop("disabled", true);
        //$(".meeting-location").prop("disabled", true);
        //$('input:radio[name=MeetingLocation][value=B]').prop('checked', true);
    } else {
        $("#IsCeo_div").show();
        //$("#is-ceo").prop("checked", false);
        $(".meeting-location *").prop("disabled",false);
    }

    var location = $('input[name="MeetingLocation"]:checked').val();

    if (location == "B" || (location == "F")) {
        roomId = $("#MeetingRooms").val();
    } else {
        roomId = 0;
    };
    
    if (to != '' && from != '' && hostId > 0) {
        timingAvailable = false;
        $(".form-loader").show();
        $('#validate').addClass('disabled');
        var details = {
            to: to,
            from: from,
            date: date,
            roomId: roomId,
            hostId: hostId,
            locationType: location,
            isWebex: isWebex,
            isCeo: isCeo
        }

        $.ajax({
            type: "post",
            url: "/" + controllerName+"/CheckAvailability",
            data: details,
            dataType: 'json',
            success: function (response) {
                var message = $("#message");
                if (!response.meetingCanBeReserved)
                {
                    timingAvailable = false;
                    message.css("color", "red");
                    message.html(response.message);
                    $('#custom-tabs-three-tab > .active').nextAll('a').addClass('disabled');
                    $(".form-loader").hide();
                }
                else
                {
                    message.css("color", "red");
                    message.html("");
                    preventSubmit = false;
                    timingAvailable = true;
                    $('#validate').removeClass('disabled');
                    $(".form-loader").hide();
                }
            }
        });
        //$(".form-loader").hide();
    }
};


   /***********************************************/
  /*******           Start                 *******/
 /****** Checking Attendees Availability ********/
/***********************************************/
//function AreMorePanelistsAdded() {
//    $("#attendeesNotChosenMessage").html('');
//    var externalAdded = false;
//    var internalAdded = false;

//    $('input.externalPanelistsInput.email').each(function () {
//        if ($(this).val() != '') {
//            externalAdded = true;
//        }
//    });

//    internalAdded = $("#InternalWebinarPanelistIds")[0].value > 0; //$('div#ExternalWebinarPanelistsDiv :input[type ="email"]').val();;

//    $("#MeetingPanelists").removeClass("valid").addClass("invalid");

//    if (!externalAdded && !internalAdded) {
//        preventSubmit = true;
//        $(".overlay").hide();
//        message = $("#attendeesNotChosenMessage");
//        message.css("color", "red");
//        message.html('@localizer["PanelistsNotChosen"]');
//        $('html, body').animate
//            ({
//                scrollTop: message.offset().top - 123
//            }, 1000);

//    } else {
//        preventSubmit = false;
//    }

//    return externalAdded || internalAdded;
//}

function AreMoreAttendeesAdded() {
    var externalAdded = false;
    var internalAdded = false;
    // adding rules for inputs with class 'comment'
    $('input.externalAttendeesInput.email').each(function () {
        if ($(this).val() != '') {
            externalAdded = true;
        }
    });
    internalAdded = $("#MeetingInternalAttendeesId")[0].value > 0
    message = $("#attendeesNotChosenMessage");
    if (!externalAdded && !internalAdded) {
        preventSubmit = true;
        $(".overlay").hide();
        message.css("color", "red");
        message.html(attendeesNotChosen);
        $('html, body').animate({
            scrollTop: message.offset().top - 123
        }, 1000);
    } else {
        message.html("");
        preventSubmit = false;
    }

    
    return externalAdded || internalAdded;

   
}



   /***********************************************/
  /*******            End                  *******/
 /****** Checking Attendees Availability ********/
/***********************************************/




   /***********************************************/
  /*******              Start               ******/
 /*******  Adding and removing Requirements  ****/
/***********************************************/
$(document).on('click', '.addRequirements', function (e) {
    var type = $(this).attr('data-controller');
    $.ajax({
        url: ROOT + ("/" + type + "/AddRequirementsPartial/"),
        success: function (partialView) {
            $('#requirementsPartialDiv').append(partialView);
        }
    });
});

$(document).on('click', '.removeRequirements', function (e) {
    if ($("#requirementsPartialDiv > div").length > 1) {
        $(this).parent().parent().remove();
    } else {
        $(this).parent().parent().find('input:text').val('');
        $(this).parent().parent().find('textarea').val('');
    }
});
   /***********************************************/
  /*******               End                ******/
 /*******  Adding and removing Requirements  *******/
/***********************************************/


   /***********************************************/
  /*******              Start               ******/
 /******* Adding and removing panelists room ****/
/***********************************************/
$(document).on('click', '.addExternalPanelistField', function (e) {
    $.ajax({
        url: ROOT + ("/Webinar/AddPanelistsPartial/"),
        success: function (partialView) {
            $('#externalPanelistsPartialDiv').append(partialView);
        }
    });
});

$(document).on('click', '.removeExternalPanelistField', function (e) {
    if ($("#externalPanelistsPartialDiv > div").length > 1) {
        $(this).parent().parent().remove();
    } else {
        $(this).parent().parent().find('input:text').val('');
        $(this).parent().parent().find('textarea').val('');
    }
}); 
   /***********************************************/
  /*******               End                ******/
 /******* Adding and removing panelists room ****/
/***********************************************/




   /***********************************************/
  /*******              Start               ******/
 /******* Adding and removing panelists room ****/
/***********************************************/
$(document).on('click', '.addExternalAttendeeField', function (e) {
    $.ajax({
        url: ROOT + ("/Meeting/AddAttendeesPartial/"),
        success: function (partialView) {
            $('#externalAttendeesPartialDiv').append(partialView);
        }
    });
});

$(document).on('click', '.removeExternalAttendeeField', function (e) {
    if ($("#externalAttendeesPartialDiv > div").length > 1) {
        $(this).parent().parent().remove();
    } else {
        $(this).parent().parent().find('input:text').val('');
        $(this).parent().parent().find('textarea').val('');
    }
});
   /***********************************************/
  /*******               End                ******/
 /******* Adding and removing panelists room ****/
/***********************************************/



   /***********************************************/
  /*******              Start               ******/
 /******* Adding and removing Interpreters  *****/
/***********************************************/
    $(document).on('click', '.addInterpretersField', function (e) {
        $.ajax({
            url: ROOT + ("/Webinar/AddInterpretersPartial/"),
            success: function (partialView) {
                $('#interpretersDiv').append(partialView);
            }
        });
    });

    $(document).on('click', '.removeInterpretersField', function (e) {
        if ($("#interpretersDiv > div").length > 1) {
            $(this).parent().parent().remove();
        } else {
            $(this).parent().parent().find('input:text').val('');
            $(this).parent().parent().find('textarea').val('');
        }
    });
   /***********************************************/
  /*******               End                ******/
 /******* Adding and removing panelists room ****/
/***********************************************/
document.addEventListener("DOMContentLoaded", function () {

});


$(function ()
{
    $('#submit').on('click', function (e)
    {
        $(".overlay").show();
        e.preventDefault();

        if ($('form').valid()) {
            if (myDropzone.getQueuedFiles().length > 0) {
                $(".overlay").show();
                myDropzone.processQueue();
            }
            else {
                $("#is-webex").prop("disabled", false);
                var form = $("form");
                var testSerialize = form.serialize();
                var cntr = controllerName;
                $(".overlay").show();
                $.ajax({
                    type: 'POST',
                    url: "/" + controllerName + "/Create",
                    data: form.serialize(),
                    success: function (response) {

                        window.location.href = response.redirectToUrl;
                    }
                });
            }
        }
    });

    $("#is-webex").on('change', function () {

    });

    function myParamName() {
        return "Files";
    }

    myDropzone = new Dropzone('form', {
        url: "/" + controllerName + "/Create/",
        thumbnailWidth: 80,
        paramName: (myParamName),
        uploadMultiple: true,
        thumbnailHeight: 80,
        parallelUploads: 20,
        autoProcessQueue: false,
        previewsContainer: ".demo-upload",
        clickable: ".dz-clickable",
        addRemoveLinks: true,
        dictRemoveFile: "<i class='fa fa-times-circle'></i>",
        dictCancelUpload: "<i class='fa fa-times-circle'></i>",
        dictDefaultMessage: "<i class='fa fa-download' ></i>",
        success: function (file, response) {
            if (response.type == 'error') {

            } else {
                window.location.href = response.redirectToUrl;
            }
        },
    });

    myDropzone.on("addedfile", function (file) {
        $("#IsShared_div").show();
        $(".dz-message").hide();

        var ext = file.name.split('.').pop();

        if (ext == "pdf") {
            $(file.previewElement).find(".dz-image img").attr("src", ROOT + '/img/credit/pdf.png');
        } else if (ext.indexOf("doc") != -1) {
            $(file.previewElement).find(".dz-image img").attr("src", ROOT + '/img/credit/word.png');
        } else if (ext.indexOf("xls") != -1) {
            $(file.previewElement).find(".dz-image img").attr("src", ROOT + '/img/credit/excel.png');
        } else if (ext.indexOf("xlsx") != -1) {
            $(file.previewElement).find(".dz-image img").attr("src", ROOT + '/img/credit/excel.png');
        }
    });

    myDropzone.on("removedfile", function (file) {
        if (myDropzone.getAcceptedFiles().length <= 0) {
            $("#IsShared_div").hide();
            $(".dz-message").show();

        };
    });
});


$(function () {
    var Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000
    });


    $('#printAgenda').on('click', function (e) {
        var divToPrint = document.getElementById('Agenda');
        var popupWin = window.open('', '_blank');
        popupWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</html>');
        popupWin.document.close();
    });


});
function CopyUrl(url, culture) {

    var textToCopy = url;

    // navigator clipboard api needs a secure context (https)
    if (navigator.clipboard && window.isSecureContext) {
        //navigator clipboard api method
        var alertText = culture.toLowerCase() === "en" ? "Copied to clipboard" : "تم النسخ";
        toastr['info']
            (alertText);
        toastr.options.timeOut = 1000;
        (async () => {
            await navigator.clipboard.writeText(textToCopy);
        })();
    } else {
        
        var textArea = document.createElement("textarea");
        textArea.textContent  = textToCopy;
        textArea.style.position = "fixed";  //avoid scrolling to bottom
        document.body.appendChild(textArea);
        var selection = document.getSelection();
        var range = document.createRange();
        //  range.selectNodeContents(textarea);
        range.selectNode(textArea);
        selection.removeAllRanges();
        selection.addRange(range);


        try {
            var successful = document.execCommand('copy');
            var msg = successful ? 'successful' : 'unsuccessful';
            var alertText = '';
            if (successful) {
                alertText = culture.toLowerCase() === "en" ? "Copied to clipboard" : "تم النسخ";
                toastr['info']
                    (alertText);
                toastr.options.timeOut = 1000;
            } else {
                alertText = culture.toLowerCase() === "en" ? "Error Copying" : "حصل خطأ عند النسخ";
                toastr['danger']
                    (alertText);
                toastr.options.timeOut = 1000;
            }
        } catch (err) {
            var alertText = culture.toLowerCase() === "en" ? "Error Copying" : "حصل خطأ عند النسخ";
            toastr['danger']
                (alertText);
            toastr.options.timeOut = 1000;
        }
        selection.removeAllRanges();
        document.body.removeChild(textArea)
        return;
    }

};
