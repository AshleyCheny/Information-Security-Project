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
$link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");
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

function API_Response1($isError, $errorMessage, $new_array)
{
    exit(json_encode(array(
        'IsError' => $isError,
        'ErrorMessage' => $errorMessage,
        'Array' => $new_array
    )));
}

function API_Response2($isError, $errorMessage, $MessageID, $MessageSenderRegisID, $MessageReceiverRegisID, $MessageText, $MessageTimestamp)
{
    exit(json_encode(array(
        'IsError' => $isError,
        'ErrorMessage' => $errorMessage,
        'MessageID' => $MessageID,
        'MessageSenderRegisID' => $MessageSenderRegisID,
        'MessageReceiverRegisID' => $MessageReceiverRegisID,
        'MessageText' => $MessageText,
        'MessageTimestamp' => $MessageTimestamp
    )));
}

function API_Response3($isError, $errorMessage, $PreKeyID = '', $Prekey)
{
    exit(json_encode(array(
        'IsError' => $isError,
        'ErrorMessage' => $errorMessage,
        'PreKeyID' => $PreKeyID,
        'Prekey' => $Prekey
    )));
}

/* API Methods */

function loginUser($api_data)
{
    // Decode Login Data
    $login_data = json_decode($api_data);
    $LoginUsername = $login_data->Username;
    $LoginPassword = $login_data->Password;

    $link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");

    //operate query and save the results in the variable $sql
    $sql = "SELECT `RegistrationID` FROM `User` WHERE `Username` = '$LoginUsername' AND `Password` = '$LoginPassword'";
    $result = mysql_query($sql);
    $userRegistrationID = mysql_fetch_array($result);
    $regisID = $userRegistrationID['RegistrationID'];
    // Dummy Check
    if ($userRegistrationID!=null)
    {
        // Success
        API_Response(false, '', $regisID);
    }
    else
    {
        // Error
        API_Response(true, 'Invalid username and/or password or a new user?');
    }

    mysql_close($link);
}

function registerUser($api_data)
{
    $userRegisInfo = json_decode($api_data);
    $password = $userRegisInfo->Password;
    $username = $userRegisInfo->Username;
    $registrationID = $userRegisInfo->RegistrationID;
    $publicIdentityKey = $userRegisInfo->PublicIdentityKey;
    $publicSignedPreKeyID = $userRegisInfo->PublicSignedPreKeyID;
    $publicSignedPreKeySignature = $userRegisInfo->PublicSignedPreKeySignature;
    $publicSignedPreKey = $userRegisInfo->PublicSignedPreKey;

    $link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");

    $sql = "INSERT INTO `User`(`Username`, `Password`, `RegistrationID`, `IdentityKey`, `SignedPreKeyID`, `SignedPreKeySignature`, `SignedPreKey`) VALUES ('$username','$password','$registrationID','$publicIdentityKey','$publicSignedPreKeyID','$publicSignedPreKeySignature','$publicSignedPreKey')";

    if(mysql_query($sql,$link)){
        API_Response(false, '', 'SUCCESS');
    }else{
        API_Response(true, '', 'FAILED');
    }

    mysql_close($link);

}

function storePreKeys($api_data)
{
    $preKeyInfo = json_decode($api_data);
    $publicSignedPreKeyID = $preKeyInfo->PublicSignedPreKeyID;
    $publicPreKeyID = $preKeyInfo->PublicPreKeyID;
    $publicPreKey = $preKeyInfo->PublicPreKey;

    $link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");

    $sql = "INSERT INTO `Prekeys`(`SignedPreKeyID`, `PrekeyID`, `Prekey`) VALUES ('$publicSignedPreKeyID','$publicPreKeyID','$publicPreKey')";

    if(mysql_query($sql,$link)){
        API_Response(false, '', 'SUCCESS');
    }else{
        API_Response(true, '', 'FAILED');
    }

    mysql_close($link);

}

function getFriends($api_data1)
{
    // Decode Login Data
    $login_data = json_decode($api_data1);
    $LoginRegisID = $login_data->RegistrationID;

    $link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");

    //operate query and save the results in the variable $sql
    $sql = "SELECT * FROM `User` WHERE NOT `RegistrationID`='$LoginRegisID'";
    $result = mysql_query($sql);
    //$conversation = mysql_fetch_array($result);
    while($row = mysql_fetch_array($result,MYSQL_ASSOC))
    {
        $new_array[] = $row;
    }
    API_Response1(false, '', $new_array);

    mysql_close($link);
}

function sendMessages($api_data)
{
    // Decode Login Data
    $message = json_decode($api_data);
    $insertMessage = $message->message;
    $messageID = $insertMessage->MessageID;
    $messageSenderRegisID = $insertMessage->MessageSenderRegisID;
    $messageReceiverRegisID = $insertMessage->MessageReceiverRegisID;
    $messageText = $insertMessage->MessageText;
    $messageTimestamp = $insertMessage->MessageTimestamp;

    $link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");

    $sql = "INSERT INTO `Message`(`MessageID`,`MessageSenderRegisID`, `MessageReceiverRegisID`, `MessageText`, `MessageTimestamp`) VALUES ('$messageID','$messageSenderRegisID','$messageReceiverRegisID','$messageText','$messageTimestamp')";
    //API_Response(false, '', $messageReceiverRegisID);

    if(mysql_query($sql,$link)){
        API_Response(false, '', 'SUCCESS');
    }else{
        API_Response(true, '', 'FAILED');
    }

    mysql_close($link);
}

function getMessage($api_data)
{
    // Decode Login Data
    $message = json_decode($api_data);
    $insertMessage = $message->message;
    $messageSenderRegisID = $insertMessage->MessageSenderRegisID;
    $messageReceiverRegisID = $insertMessage->MessageReceiverRegisID;

    $link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");

    //operate query and save the results in the variable $sql
    //$sql = "SELECT * FROM `Message` WHERE `MessageReceiverRegisID`='$LoginRegisID' AND `Sent`=FALSE ORDER BY `MessageTimestamp` DESC LIMIT 1";
    $sql = "SELECT * FROM `Message` WHERE `MessageReceiverRegisID`='$messageReceiverRegisID' AND `MessageSenderRegisID`='$messageSenderRegisID' AND `Sent`=FALSE ORDER BY `MessageTimestamp` DESC";
    $result = mysql_query($sql);
    $sql1 = "UPDATE `Message` SET `Sent`=TRUE WHERE `MessageReceiverRegisID`='$messageReceiverRegisID' ORDER BY `MessageTimestamp` DESC";
    mysql_query($sql1);
    //$conversation = mysql_fetch_array($result);
    while($row = mysql_fetch_array($result,MYSQL_ASSOC))
    {
        $MessageID = $row['MessageID'];
        $MessageSenderRegisID = $row['MessageSenderRegisID'];
        $MessageReceiverRegisID = $row['MessageReceiverRegisID'];
        $MessageText = $row['MessageText'];
        $MessageTimestamp = $row['MessageTimestamp'];
        API_Response2(false, '', $MessageID, $MessageSenderRegisID, $MessageReceiverRegisID, $MessageText, $MessageTimestamp);
    }

    mysql_close($link);
}

function getPreKey($api_data)
{
    // Decode Login Data
    $PreKey_data = json_decode($api_data);
    $PublicSignedPreKeyID = $PreKey_data->PublicSignedPreKeyID;

    $link=mysql_connect('localhost','root','Baobao2$');

    if (!$link)
        die('Could not connect to MySQL: ' . mysql_error());

    //select our project's database
    mysql_select_db("ChatAppDB");

    //operate query and save the results in the variable $sql
    $sql = "SELECT `PreKeyID`,`Prekey` FROM `Prekeys` WHERE `SignedPreKeyID` = '$PublicSignedPreKeyID' LIMIT 1";
    $result = mysql_query($sql);
    $sql1 = "DELETE FROM `Prekeys` WHERE `PreKeyID`='$result->PreKeyID'";
    mysql_query($sql1);
    if ($result!=null)
    {
        $preKeys = mysql_fetch_array($result);
        $PreKeyID = $preKeys['PreKeyID'];
        $Prekey = $preKeys['Prekey'];
        // Success
        API_Response3(false, '', $PreKeyID, $Prekey);
    }
    else
    {
        // Error
        API_Response3(true, 'No Prekeys left in database');
    }

    mysql_close($link);
}

?>