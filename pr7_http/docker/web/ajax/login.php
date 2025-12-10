<?php
    include_once("../config.php");
    include_once("../settings/connect.php");

    class response_error {
        public $status;
        public $messages;
    }

    # Получаем логин и пароль
    $login = $_POST['login'];
    $password = $_POST['password'];

    # Выполняем запрос к базе данных
    $query = "SELECT * FROM `users` WHERE `login` = '".$login."' AND `password` = '".$password."';";
    $user_query = $mysqli->query($query);
    if($user_query->num_rows > 0) {
        $user_read = $user_query->fetch_assoc();

        # Создаём токен
        $token = password_hash($user_read['login'], PASSWORD_DEFAULT);

        # Удаляем у пользователя токен, если он уже существует
        $mysqli->query("DELETE FROM `users_tokens` WHERE `id_user` = ".$user_read['id']);
        # Добавляем новый токен
        $mysqli->query("INSERT INTO `users_tokens`(`id_user`, `token`) VALUES (".$user_read['id'].", '".$token."')");
        # Сохраняем токен в Cookies
        setcookie("token", $token, time()+60*5, '/');

        $response_error = new response_error();
        $response_error->status = 200;
        $response_error->messages = "";
        http_response_code(200);
        echo json_encode($response_error, JSON_UNESCAPED_UNICODE);
    } else {
        $response_error = new response_error();
        $response_error->status = 403;
        $response_error->messages = "Отказано в доступе";
        http_response_code(403);
        echo json_encode($response_error, JSON_UNESCAPED_UNICODE);
    }
?>