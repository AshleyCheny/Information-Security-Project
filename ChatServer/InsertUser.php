<?php
include "conn.php";

if(isset($_POST['UserName']) && isset($_POST['UserPassword']) && isset($_POST['UserMail']) && isset($_POST['UserImage'])){

$req=$bdd->prepare("Insert INTO user(UserName,UserMail,UserPassword,UserImage) VALUES (:UserName,:UserMail,:UserPassword,:UserImage) ")
$req->execute(array(
'UserName'=>$_POST['UserName'],
'UserMail'=>$_POST['UserMail'],
'UserPassword'=>sha1($_POST['UserPassword']),
'UserImage'=>$_POST['UserImage']
));

}

// We will insert the image into the UserImage folder
//move_uploaded_file($_FILES['file']['tmp_name'], "UserImage/", $_FILES['file']['name'])

?>