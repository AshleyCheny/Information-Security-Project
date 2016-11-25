<?php

// Load Request
$api_method = isset($_POST['api_method']) ? $_POST['api_method'] : '';
$api_data = isset($_POST['api_data']) ? $_POST['api_data'] : '';

// Validate Request
if (empty($api_method) || empty($api_data)) {
    API_Response(true, 'Invalid Request');
}
if (!function_exists($api_method)) {
    API_Response(true, 'API Method Not Implemented');
}

// Call API Method
call_user_func($api_method, $api_data);

/* Helper Function */

function API_Response($isError, $errorMessage, $responseData = '')
{
    exit(json_encode(array(
        'IsError' => $isError,
        'ErrorMessage' => $errorMessage,
        'ResponseData' => $responseData
    )));
}

function API_Response1($isError, $errorMessage, $UserName = '', $UserPassword = '')
{
    exit(json_encode(array(
        'IsError' => $isError,
        'ErrorMessage' => $errorMessage,
        'UserName' => $UserName,
        'UserPassword' => $UserPassword
    )));
}

/* API Methods */

function loginUser($api_data)
{
    // Decode Login Data
    $login_data = json_decode($api_data);
    $LoginUsername = $login_data->username;
    $LoginPassword = $login_data->password;

    $link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");

    //operate query and save the results in the variable $sql
    $sql = "SELECT `Username`,`Password` FROM `User` WHERE `Username` = '$LoginUsername' AND `Password` = '$LoginPassword'";
    $result = mysql_query($sql);
    $usernameandpassword = mysql_fetch_array($result);
    // Dummy Check
    if ($usernameandpassword!=null)
    {
        // Success
        API_Response(false, '', 'SUCCESS');
    }
    else
    {
        // Error
        API_Response(true, 'Invalid username and/or password or a new user?');
    }
}

function retrieveUser($api_data)
{
    // Decode Login Data
    $login_data = json_decode($api_data);

    // Dummy Check
    if ($login_data->username == 'Ying')
    {
	    $link=mysql_connect('localhost','root','Baobao2$');

        if (!$link)
            die('Could not connect to MySQL: ' . mysql_error());

        //select our project's database
        mysql_select_db("ChatServerDB");

        //operate query and save the results in the variable $sql
        $sql = "SELECT `Username` FROM `User` WHERE `Username` = 'aaa'";
        $result = mysql_query($sql);
        mysql_fetch_array($result);
        $username = $result;

        //operate query and save the results in the variable $sql
        $sql2 = "SELECT `Password` FROM `User` WHERE `Username` = 'aaa'";
        $result2 = mysql_query($sql2);
        mysql_fetch_array($result2);
        $userpassword = $result2;

        mysql_close($link);
        // Success
        API_Response1(false, '', $username, $userpassword);
    }
    else
    {
        // Error
        API_Response(true, 'Invalid username and/or password.');
    }
}

?>