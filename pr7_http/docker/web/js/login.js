function Login() {
    var login = document.getElementsByName("login")[0].value;
    var password = document.getElementsByName("password")[0].value;

    var data = new FormData();
    data.append("login", login);
    data.append("password", password);

    $.ajax({
        url         : "./ajax/login.php",
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
    const response = JSON.parse(data);
    if(response["status"] == 200)
        window.location = "/main";
}
function Error() {
    console.log('Системная ошибка!');
}