
jQuery(document).ready(function () {

	$(".doc").fileinput({
		required: false,
		browseClass: "btn btn-secondary",
		browseIcon: "",
		removeClass: "btn btn-danger",
		removeLabel: "",
		removeIcon: "<i class='icon-trash-alt1'></i>",
		maxFileSize: '1000000',
		showUpload: false
	});

	//$(".file-loading").fileinput({
	//	theme: "fa",
	//	browseOnZoneClick: true,
	//	showUpload: false,
	//	dropZoneEnabled: true,
	//	overwriteInitial: false,
	//	allowedFileExtensions: ['jpg', 'png', 'gif'],
	//	required: false,
	//	showRemove = true,
	//	browseClass: "btn btn-secondary",
	//	browseIcon: "",
	//	removeClass: "btn btn-danger",
	//	removeLabel: "",
	//	removeIcon: "<i class='icon-trash-alt1'></i>",
	//	maxFileSize: '20000',
	//	fileActionSettings:
	//	{
	//		showUpload: false,
	//		showRemove = true,
	//	},
	//});

});