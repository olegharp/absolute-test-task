// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('#poster-container').click(function (e) {
    $('#PosterFile').click();
});

$('#PosterFile').change(function(){
    var reader = new FileReader();
    reader.readAsDataURL(this.files[0]);
    reader.onload = function () {
        $('#poster-container').css('background', `url(${reader.result})`)
    };

});
