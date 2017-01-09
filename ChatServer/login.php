<?php
//connect the database
// give your username & password
$link=mysql_connect('localhost','root','Baobao2$');
if (!$link) {
    die('Could not connect to MySQL: ' . mysql_error());
}

//select our project's database
mysql_select_db("chatappdb");

//operate query and save the results in the variable $sql
$sql=mysql_query("SELECT * FROM users where username = '".$_REQUEST['username']."' and password = '".$_REQUEST['password']."'");

//verify the username and password
if (mysql_num_rows($sql) > 0) {
    //if the user name and password is valid, then return success response to LoginViewModel in client and go to Conversations page.
    echo "success";
    //$this->response('success');
}else{
    //if they are not valid, return login failed to client.
    echo "login failed";
    //$this->response('login failed');
}
mysql_close($link);
?>