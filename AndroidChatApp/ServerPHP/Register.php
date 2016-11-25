<?php
$con=mysqli_connect("localhost","root","Baobao2$","ChatAppDB");

//GET the registered user's information
$Username = $_POST["Username"];
$Password = $_POST["Password"];
//  ** also get keys infomation from the registered user (CHANGE LATER ON ACCORDINGLY)
$RegistrationID = $_POST["RegistrationID"];
$IdentityKey = $_POST["IdentityKey"];
$PreKey = $_POST["PreKey"];
$SignedPreKey = $_POST["SignedPreKey"];

//SAVE the getting registered user's data and save it in the server database (CHANGE LATER ON ACCORDINGLY)
$statement = mysqli_prepare($con,"INSERT INTO User (Username,Password, RegistrationID, IdentityKey, PreKey, SignedPreKey) VALUES (?, ?, ?, ?, ?, ?) ");
mysqli_stmt_bind_param($statement,"ssisss", $Username, $Password, $RegistrationID, $IdentityKey, $PreKey, $SignedPreKey);
mysqli_stmt_execute($statement);

mysqli_stmt_close($statement);

mysqli_close($con);
?>