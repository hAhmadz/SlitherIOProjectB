All addresses start with 203.101.225.0:5000
Example: /signup denotes 203.101.225.0:5000/signup
***************************************************
/signup
Input:
{
    "FirstName":<string>,
    "LastName":<string>,
    "Username":<string>,
    "Password":<string>
}

Output:
{
    "Message":"Signup complete!"
}
OR
{
    "Message":"User already exists!"
}
****************************************************
/login
Input:
{
    "Username":<string>,
    "Password":<string>
}

Output:
{
    "userid":<int>
}
If "userid" == -1 -> user does not exist
If "userid" == -2 -> either wrong username or wrong password
Otherwise, it returns the userid. Example of successful login:
{
    "userid":1
}
Once logged in, take note of the userid and store it somewhere. It's used for a lot of functions.

****************************************************
/logout
Input:
{
    "UserID":<int>
}

Output:
{
    "Message": "You are now logged out!"
}
****************************************************
All functions below this line also return
{
    "Message": "You are not logged in!"
}
In the event that the user is not logged in.
****************************************************
/retrieve_self
Input:
{
    "UserID":<int>
}
Note that it's "UserId" with uppercase to POST to the server. JSON objects used to POST has uppercase, while JSON objects returned from server are all lowercase in their attributes.

Output:
{
    "firstname":<string>,
    "highestscore":<int>,
    "lastname":<string>,
    "level":<int>
}
highestscore and level can be null.
Example of output:
{
    "firstname": "YJ",
    "highestscore": 100,
    "lastname": "Chua",
    "level": null
} 
****************************************************
/retrieve_scores
Input:
{
    "UserID":<int>
}
*this is just to check whether you're logged in*
Output:
[
    {
        "highestscore":<int>,
        "username":<string>
    }
]
NOTE: Output is in an array this time, because there can be multiple results
Example output:
[
    {
        "highestscore": 500,
        "username": "jclark"
    },
    {
        "highestscore": 200,
        "username": "yjchua"
    }
]
****************************************************
/edit_username
Input:
{
    "UserID":<int>,
    "Username":<string>
}

Output:
{
    "Message": "Username already taken!"
}
OR
{
    "Message": "Username changed!"
}
****************************************************
/update_highscore
Input:
{
    "UserID":<int>,
    "Highestscore":<int>
}

Output:
{
    "Message": "Highest score updated!"
}
****************************************************
/update_mostrecentscore
Input:
{
    "UserID":<int>,
    "MostRecentScore":<int>
}

Output:
{
    "Message": "Most recent score updated!"
}
****************************************************
/search_user
{
    "SearchQuery":<string>,
    "UserID":<int>
}
Note: UserID is used to check if you're logged in

Sample input:
{
    "UserID": 1,
    "SearchQuery":"david clark"
}

Corresponding sample output:
[
    {
        "firstname": "David",
        "highestscore": null,
        "lastname": "Ripper",
        "loggedin": 1,
        "mostrecentscore": null,
        "userid": 3,
        "username": "dripper"
    },
    {
        "firstname": "Joshua",
        "highestscore": 500,
        "lastname": "Clark",
        "loggedin": 1,
        "mostrecentscore": 300,
        "userid": 2,
        "username": "jclark"
    }
]
****************************************************
/add_friend
Input:
{
    "UserFrom":<int>,
    "UserTo":<int>
}
Sample input:
{
    "UserFrom": 1,
    "UserTo": 2
}

Sample output:
{
    "Message": "Friend request sent!"
}
OR
{
    "Message": "Request already sent!"
}
OR
{
    "Message": "User does not exist!"
}
****************************************************
/retrieve_friend_requests
Input:
{
    "UserID":<int>
}
Note: UserID is your own UserID

Sample output:
[
    {
        "status": 0,
        "userfrom": 1
    },
    {
        "status": 0,
        "userfrom": 3
    }
]
****************************************************
/friend_request_action
{
    "UserRequesterID":<int>,
    "UserOwnID":<int>,
    "UserAction":<int>
}
Note: UserAction can be either 0 (reject and delete request) or 1 (accept request)

Sample output:
{
    "Message": "Friend request accepted!"
}
OR
{
    "Message": "Friend request rejected!"
}
****************************************************
/retrieve_friends_details
{
    "UserID":<int>
}

Sample output:
[
    {
        "firstname": "Joshua",
        "highestscore": 500,
        "lastname": "Clark",
        "loggedin": 1,
        "mostrecentscore": 300,
        "userid": 2,
        "username": "jclark"
    },
    {
        "firstname": "David",
        "highestscore": null,
        "lastname": "Ripper",
        "loggedin": 1,
        "mostrecentscore": null,
        "userid": 3,
        "username": "dripper"
    }
]***********************************************
/send_message:
Input:
{
    "UserFrom":<int>,
    "UserTo":<int>,
    "MessageContent":<string>,
    "Timestamp":<string>
}
Note: Get the timestamp from your device's local time

Sanple output:
{
    "Message": "You both are not friends!"
}
OR
{
    "Message": "Message sent!"
}
****************************************************
/retrieve_messages
Input:
{
    "UserID":<int>
}
Note: This is your own UserID

Sample output:
[
    {
        "message": "Screw you!",
        "messageid": 4,
        "timestamp": "1041 Oct 21 2018",
        "userfrom": 1
    },
    {
        "message": "I'll eat you!",
        "messageid": 5,
        "timestamp": "1041 Oct 21 2018",
        "userfrom": 1
    }
]
PS - you can sort the messages according to messageid, more recent ones have a larger ID (I used auto increment in the DB). The timestamp here is just a sample timestamp, can be any format depending on how you generate the timestamp when message is sent from client.
Once messages are retrieved, you can't retrieve them again (but it's possible to retrieve read messages, they're always stored in the DB. I can write a function for this if needed).

If no unread messages:
{
    "Unread messages": []
}

