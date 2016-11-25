<?php
$con=mysqli_connect("localhost","root","Baobao2$","ChatAppDB");

//Get the login user's username and password  (CHANGE LATER ON ACCORDINGLY)
$Username = $_POST["Username"];
$Password = $_POST["Password"];

//SLECT the record from database (CHANGE LATER ON ACCORDINGLY)
$statement = mysqli_prepare($con,"SELECT (Username, Password) FROM User WHERE Username = ? AND Password = ?");
mysqli_stmt_bind_param($statement,"ss", $Username, $Password);
mysqli_stmt_execute($statement);

mysqli_stmt_store_result($statement);
mysqli_stmt_bind_result($statement, $Username, $Password);

$user = array();

while(mysqli_stmt_fetch($statement)){
    $user[Username] = $Username;
    $user[Password] = $Password;
}

//RETURN JSON object to client app
echo json_encode($user);

mysqli_stmt_close($statement);

mysqli_close($con);

?>