<?php
	$mysqli = new mysqli($server, $user, $password, $database);
	// Check connection
	if ($mysqli->connect_error) {
	  die("Connection failed: " . $conn->connect_error);
	}
?>