<?php
//connect the database
// give your username & password
$link=mysql_connect('localhost','root','Baobao2$');
if (!$link) {
    die('Could not connect to MySQL: ' . mysql_error());
}

//select our project's database
mysql_select_db("ChatServerDB");

//operate query and save the results in the variable $sql
$sql=mysql_query("Insert INTO user(UserName,UserMail,UserPassword,UserImage) VALUES ('".$_REQUEST['UserName']."','".$_REQUEST['UserMail']."','".$_REQUEST['UserPassword']."','".$_REQUEST['UserImage']."'");

//verify the username and password
//if (mysql_num_rows($sql) > 0) {
    //if the user name and password is valid, then return success response to LoginViewModel in client and go to Conversations page.
    //echo "success";
    //$this->response('success');
//}else{
    //if they are not valid, return login failed to client.
  //  echo "login failed";
    //$this->response('login failed');
//}
mysql_close($link);
?>