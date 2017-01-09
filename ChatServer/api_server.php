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

function loginUser($api_data)
{
    // Decode Login Data
    $login_data = json_decode($api_data);

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