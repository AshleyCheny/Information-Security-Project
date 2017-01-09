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

/* API Methods */

function registerUser($api_data)
{
    // Decode Login Data
    $register_data = json_decode($api_data);

    $link=mysql_connect('localhost','root','Baobao2$');
	if (!$link) {
    	die('Could not connect to MySQL: ' . mysql_error());
	}

	//select our project's database
	mysql_select_db("chatappdb");

	//operate query and save the results in the variable $sql
	$sql=mysql_query("Insert INTO users(RegistrationID, IdentityKey, SignedPreKey, OneTimePreKeyID) VALUES ($login_data->registrationId,$login_data->identityKey,$login_data->signedPreKey,$login_data->preStoreKeyId");

    //operate query and save the results in the variable $sql
    //$sql = "SELECT `UserID` FROM `users` WHERE `UserID` = 1";
    //$result = mysql_query($sql);
    //mysql_fetch_array($result);
    //$userid = $result;

    // Dummy Check
    if ($login_data->username == 'Ying' &&
        $login_data->password == '1234')
    {
        // Success
        API_Response(false, '', 'SUCCESS');
    }
    else
    {
        // Error
        API_Response(true, 'Invalid username and/or password.');
    }
}

?>