<?php
    include_once("../config.php");
    include_once("../settings/connect.php");

    # Получаем логин и пароль
    $src = $_POST['src'];
    $name = $_POST['name'];
    $description = $_POST['description'];

    $mysqli->query("INSERT INTO `news`(`img`, `name`, `description`) VALUES ('".$src."','".$name."','".$description."');");
?>