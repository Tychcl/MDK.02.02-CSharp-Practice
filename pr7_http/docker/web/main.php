<?php
    include_once("./config.php");
    include_once("./settings/connect.php");
    # Проверяем что пользователь авторизован
    if(isset($_COOKIE['token'])) {
        $token = $_COOKIE['token'];

        # Отправляем запрос в для того чтобы понять что пользователь авторизован
        $query = "SELECT * FROM `users_tokens` WHERE `token` = '".$token."';";
        $token_query = $mysqli->query($query);
        if($token_query->num_rows == 0) {
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
    </head>
    <body>
        <?php
            $news_query = $mysqli->query("SELECT * FROM `news`;");
            while($news_read =  $news_query->fetch_assoc()) {
                ?>
                <div class="news">
                    <img class="image" src = "<?=$news_read['img'];?>">
                    <div class="name"><?=$news_read['name'];?></div>
                    <div><?=$news_read['description'];?></div>
                </div>
                <?php
            }
        ?>
    </body>
</html>