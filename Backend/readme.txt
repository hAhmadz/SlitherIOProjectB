All addresses start with 203.101.225.0:5000
Example: /signup denotes 203.101.225.0:5000/signup
script-nectar.py is used for deploying on the remote Nectar server
script.py is if you want to run the server locally
Both are identical except for the hosting address
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
    "mostrecentscore":<int>
}
highestscore and level can be null.
Example of output:
{
    "firstname": "YJ",
    "highestscore": 100,
    "lastname": "Chua",
    "mostrecentscore": null
} 
This function retrieves your own details (for example, to be displayed in the 'About yourself' page
****************************************************
/retrieve_scores
Input:
{
    "UserID":<int>
}
*this is just to check whether you're logged in*
Output:
{
    "Results": [
        {
            "highestscore":<int>,
            "mostrecentscore":<int>,
            "username":<string>
        }
    ]
}

Example output:
{
    "Results": [
        {
            "highestscore": 200,
            "mostrecentscore": null,
            "username": "yjchua"
        },
        {
            "highestscore": 500,
            "mostrecentscore": 300,
            "username": "jclark"
        },
        {
            "highestscore": null,
            "mostrecentscore": null,
            "username": "dripper"
        },
        {
            "highestscore": null,
            "mostrecentscore": null,
            "username": "haarisa"
        }
    ]
}
Usage: Use this in a global leaderboards page
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
Usage:
When you first log in, the backend returns all details about you. Keep them somewhere, it includes your highest score. After each game, compare that score with the highest score retrieved from the database earlier. If it's higher, call this function to update it.
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
Usage:
After every game, call this function to upload your most recent score to the DB
****************************************************
/search_user
{
    "SearchQuery":<string>,
    "UserID":<int>
}
Note: UserID is used to check if you're logged in

Sample output:
{
    "Results": [
        {
            "firstname": "Joshua",
            "highestscore": 500,
            "lastname": "Clark",
            "level": null,
            "loggedin": 1,
            "userid": 2,
            "username": "jclark"
        },
        {
            "firstname": "David",
            "highestscore": 750,
            "lastname": "Ripper",
            "level": null,
            "loggedin": 1,
            "userid": 3,
            "username": "dripper"
        }
    ]
}
Usage:
Search engine to find people and add them as friends
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
Usage:
Send a friend request to someone
****************************************************
/retrieve_friend_requests
Input:
{
    "UserID":<int>
}
Note: UserID is your own UserID

Sample output:
{
    "Requests": [
        {
            "status": 0,
            "userfrom": 1
        },
        {
            "status": 0,
            "userfrom": 3
        }
    ]
}
Usage:
See who wants to make friends with you
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
Usage:
Act upon friend requests
****************************************************
/retrieve_friends_details
{
    "UserID":<int>
}

Sample output:
{
    "Results": [
        {
            "firstname": "Joshua",
            "highestscore": 500,
            "lastname": "Clark",
            "level": null,
            "loggedin": 1,
            "userid": 2,
            "username": "jclark"
        },
        {
            "firstname": "David",
            "highestscore": null,
            "lastname": "Ripper",
            "level": null,
            "loggedin": 1,
            "userid": 3,
            "username": "dripper"
        }
    ]
}
Usage:
Call this function in a friends page or something like that. This function retrieves details of all your friends.
****************************************************
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
Usage:
Send a message to a friend
****************************************************
/retrieve_messages
Input:
{
    "UserID":<int>
}
Note: This is your own UserID

Sample output:
{
    "Unread messages": [
        {
            "message": "Message 1!",
            "messageid": 2,
            "timestamp": "1425 4 Oct 2018",
            "userfrom": 1
        },
        {
            "message": "Message 2!",
            "messageid": 3,
            "timestamp": "1425 4 Oct 2018",
            "userfrom": 1
        }
    ]
}
PS - you can sort the messages according to messageid, more recent ones have a larger ID (I used auto increment in the DB). The timestamp here is just a sample timestamp, can be any format depending on how you generate the timestamp when message is sent from client.
Once messages are retrieved, you can't retrieve them again (but it's possible to retrieve read messages, they're always stored in the DB. I can write a function for this if needed).

If no unread messages:
{
    "Unread messages": []
}

Note: Please keep the messageid for each message for ordering/sorting purposes. This function only returns unread messages.
