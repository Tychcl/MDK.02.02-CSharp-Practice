<?php
    include_once("./config.php");
    include_once("./settings/connect.php");
    # Проверяем что пользователь авторизован
    if(isset($_COOKIE['token'])) {
        $token = $_COOKIE['token'];

        # Отправляем запрос в для того чтобы понять что пользователь авторизован
        $query = "SELECT * FROM `users_tokens` WHERE `token` = '".$token."';";
        $token_query = $mysqli->query($query);
        if($token_query->num_rows > 0) {
            $token_read = $token_query->fetch_assoc();
            header("Location: /main");
        }
    }
    
?>
<html>
    <head>
        <link rel="stylesheet" href="/css/style.css">
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
        <script src="./js/login.js"></script>
    </head>
    <body>
        <div class="main">
            <img src="/img/logo.png"/>
            <br><input name="login" placeholder="login"/>
            <input name="password" placeholder="password"/>
            <div class="button" onclick="Login()" >Login</div>
        </div>
    </body>
</html>