function Add() {
    var src = document.getElementsByName("src")[0].value;
    var name = document.getElementsByName("name")[0].value;
    var description = document.getElementsByName("description")[0].value;

    var data = new FormData();
    data.append("src", src);
    data.append("name", name);
    data.append("description", description);

    $.ajax({
        url         : "./ajax/add.php",
        type        : 'POST',
        data        : data,
        cache       : false,
        dataType    : 'html',
        processData : false,
        contentType : false,
        success     : Success,
        error       : Error
    });
}
function Success(data) {
   alert("Запись добавлена");
}
function Error() {

}