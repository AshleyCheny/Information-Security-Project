<?php>
// Connection with the database

try{
$bdd = new PDO("mysql:host=localhost;dbname=ChatServerDB","root","");
}catch(Exception $e){
die("ERROR ".$e->getMessage());
}

?>