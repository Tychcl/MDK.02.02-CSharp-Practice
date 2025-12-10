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

            $user_query = $mysqli->query("SELECT * FROM `users` WHERE `id` = " . $token_read['id_user']);
            $user_read = $user_query->fetch_assoc();
            
            if($user_read['roll'] != "1")
                header("Location: /main");
        } else {
            header("Location: /login");
        }
    } else {
        header("Location: /login");
    }
?>
<html>
    <head>
        <link rel="stylesheet" href="/css/style.css">
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
        <script src="./js/add.js"></script>

    </head>
    <body>
        <div class="main">
            Ссылка на изображение с оф. сайта
            <br><input name="src"/>
            <br>Наименование
            <br><input name="name"/>
            <br>Описание
            <br><input name="description"/>
            <div class="button" onclick="Add()" >Добавить</div>
        </div>
    </body>
</html>