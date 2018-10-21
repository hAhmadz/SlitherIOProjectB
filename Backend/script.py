#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun Sep 23 18:46:56 2018

@author: yjchua
"""
from operator import itemgetter
from flask import Flask
from flask import abort
from flask import request
from flask import jsonify
from flask import make_response
import mysql.connector
import hashlib
import uuid

app = Flask(__name__)

@app.route('/signup', methods=['POST'])
def signup():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)
        
    # extract user's details from the JSON object
    firstname = str(request.json['FirstName'])
    lastname = str(request.json['LastName'])
    username = str(request.json['Username'])
    password = str(request.json['Password'])
    
    # hashes the password
    hashed_password = hash_password(password)
    
    # 203.101.225.0
    
    # if user doesn't exist, create new user and write details to DB. Otherwise,
    # return a message saying that user already exists.
    if not isUserExists(username):
        parameters = '(firstname, lastname, username, password)'
        values = (firstname, lastname, username, hashed_password)
        
        write_details_to_db('users', parameters, values)
        
        return make_response(jsonify({'Message': 'Signup complete!'}), 201)

    else:
        return make_response(jsonify({'Message': 'User already exists!'}), 400)
    
'''
Login function.

If login is successful, the user's LoggedIn flag will be set to 1
to indicate that the user is logged in. It also returns a JSON object
containing all the user's details for later usage in the app.

If login fails, a message indicating the reason will be returned in a JSON
object.
'''
@app.route('/login', methods=['POST'])
def login():

    #establish connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)


    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extract login credentials from the JSON object
    username = str(request.json['Username'])
    password = str(request.json['Password'])
    
    # check if user exists first. If user exists, check the password.
    if isUserExists(username):
        query = "select password from users where username = '" + \
                username + "'"
        cursor.execute(query)
        result = cursor.fetchall()
        cnx.close()

        hashed_password = result[0]['password']

        # if password is correct, mark the user as logged in and return JSON
        # object containing the user's UserID for later usage
        if check_password(hashed_password, password):
            pair = 'loggedin = 1'
            condition = "username = '" + username + "'"
            update_row_in_db('users', pair, condition)
            
            parameters = 'userid'
            conditions = 'username = "%s"' % username

            result = retrieve_from_db('users', parameters, conditions)


            return make_response(jsonify(result[0]), 200)
        else:
            # return make_response(jsonify({"Message":"Login failed!"}), 401)
            return make_response(jsonify({"userid":-2}), 401)
    else:
        # user does not exist
        return make_response(jsonify({"userid":-1}), 401)
    
'''
Function to retrieve a user's own details.
'''
@app.route('/retrieve_self', methods=['POST'])
def retrieve_self():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts user's ID from the JSON object
    userid = str(request.json['UserID'])

    # if user is not logged in, return error message
    if not isLoggedIn(userid):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)

    # columns to retrieve from DB
    parameters = 'firstname, lastname, highestscore, mostrecentscore'

    # only return rows matching conditions
    conditions = 'UserID = "%s"' % userid

    result = retrieve_from_db('users', parameters, conditions)

    return make_response(jsonify(result[0]), 200)

'''
Function to retrieve all scores of other people
'''
@app.route('/retrieve_scores', methods=['POST'])
def retrieve_scores():
    # Connecting to DB stuff
    # Create persistent connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)
    
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts user's ID from the JSON object
    userid = str(request.json['UserID'])

    # if user is not logged in, return error message
    if not isLoggedIn(userid):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)
    
    # query = "select username, highestscore, mostrecentscore from users"
    query = "select username, highestscore from users"
    
    cursor.execute(query)

    results = cursor.fetchall()
    cnx.close()
    
    # take only top 10 highest scores
    nonull_results = []
    for result in results:
        if not result['highestscore']:
            pass
        else:
            nonull_results.append(result)
    
    sorted_results = sorted(nonull_results, key=itemgetter('highestscore'), reverse=True)
    sorted_results = sorted_results[0:10]

    # return make_response(jsonify({"Results":results}), 200)
    return make_response(jsonify(sorted_results), 200)
    

'''
Edit username function
'''
@app.route('/edit_username', methods=['POST'])
def edit_username():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts user's ID from the JSON object
    userid = str(request.json['UserID'])

    # if user is not logged in, return error message
    if not isLoggedIn(userid):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)
    
    # editable fields (front-end, please put as None/default if blank)
    # extract user's details from the JSON object
    username = str(request.json['Username'])
    
    if not isUserExists(username):
        pair = 'username = "%s"' % username
        condition = "userid = '" + userid + "'"
        update_row_in_db('users', pair, condition)
        
        return make_response(jsonify({'Message': 'Username changed!'}), 201)

    else:
        return make_response(jsonify({'Message': 'Username already taken!'}), 400)
    
'''
Update highscore function
'''
@app.route('/update_highscore', methods=['POST'])
def update_highscore():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts user's ID from the JSON object
    userid = str(request.json['UserID'])

    # if user is not logged in, return error message
    if not isLoggedIn(userid):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)
    
    # editable fields (front-end, please put as None/default if blank)
    # extract user's details from the JSON object
    highscore = str(request.json['Highestscore'])
    

    pair = 'highestscore = "%s"' % highscore
    condition = "userid = '" + userid + "'"
    update_row_in_db('users', pair, condition)
    
    return make_response(jsonify({'Message': 'Highest score updated!'}), 201)

'''
Update most recent score function
'''
@app.route('/update_mostrecentscore', methods=['POST'])
def update_mostrecentscore():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts user's ID from the JSON object
    userid = str(request.json['UserID'])

    # if user is not logged in, return error message
    if not isLoggedIn(userid):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)
    
    # editable fields (front-end, please put as None/default if blank)
    # extract user's details from the JSON object
    mostrecentscore = str(request.json['MostRecentScore'])
    

    pair = 'mostrecentscore = "%s"' % mostrecentscore
    condition = "userid = '" + userid + "'"
    update_row_in_db('users', pair, condition)
    
    return make_response(jsonify({'Message': 'Most recent score updated!'}), 201)

'''
Function to search for a user.

Returns: Search results - details of user(s) being searched in a JSON object.
'''
@app.route('/search_user', methods=['POST'])
def search_user():
    # Connecting to DB stuff
    # Create persistent connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)
    
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts search query and ID of user who is doing the searching, from the
    # JSON object
    SearchQuery = str(request.json['SearchQuery'])
    userid = str(request.json['UserID'])

    # if user is not logged in, return error message
    if not isLoggedIn(userid):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)

    # store all row temporarily first
    sql_query = "select * from all_no_password"

    cursor.execute(sql_query)

    result = cursor.fetchall()

    # list to store matching results
    search_results = []

    # split the search query into constituent parts
    search_term = SearchQuery.split()

    # iterate through search terms
    for word in search_term:
        # check through each row (json_obj)
        for json_obj in result:
            # for each row, check through each column
            for key in json_obj:
                # if content of column matches search query, add row to
                # search_results list if it's not already in it
                if (key != 'userid' and key != 'loggedin' and key != 'highestscore' and key != 'mostrecentscore')\
                and word.lower() in json_obj[key].lower():
                    # if search_results list is empty, append to it
                    if len(search_results) == 0:
                        search_results.append(json_obj)
                    # if not empty, execute this block
                    else:
                        # check if row is already in search_results list. If
                        # yes, skip. Else, add.
                        if (search_results[-1]['userid'] == json_obj['userid']):
                            continue
                        else:
                            search_results.append(json_obj)

    # iterate through search results. If the row containing yourself is in it,
    # remove from results. You obviously know what your own details are right?
    row_index = None
    for row in search_results:
        if str(row['userid']) == userid:
            row_index = search_results.index(row)
            break
    if row_index != None:
        del search_results[row_index]


    # return make_response(jsonify({"Results":search_results}), 200)
    return make_response(jsonify(search_results), 200)

'''
Function to add friend
'''
@app.route('/add_friend', methods=['POST'])
def add_friend():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts querist's ID and target ID from the JSON object
    UserOne = int(request.json['UserFrom'])
    UserTwo = int(request.json['UserTo'])

    # if user is not logged in, return error message
    if not isLoggedIn(UserOne):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)

    # things needed for SQL searching
    table = 'friends'
    parameters = '(userfrom, userto)'
    values = (UserOne, UserTwo)

    # if target user doesn't exist, return error message
    if not isUserExistsByID(UserTwo):
        return make_response(jsonify({"Message":"User does not exist!"}), 400)
    
    if isRequestExists(UserOne, UserTwo):
        return make_response(jsonify({"Message":"Request already sent!"}), 400)
    elif isFriends(UserOne, UserTwo):
        return make_response(jsonify({"Message":"Already friends!"}), 400)

    # send friend request
    write_details_to_db(table, parameters, values)

    return make_response(jsonify({"Message":"Friend request sent!"}), 201)

@app.route('/retrieve_friend_requests', methods=['POST'])
def retrieve_friend_requests():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts user's own ID from JSON object
    UserID = str(request.json['UserID'])

    # if user is not logged in, return error message
    if not isLoggedIn(UserID):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)

    # things needed for SQL searching. Retrieve user's list of friend requests
    # from DB
    table = 'friends'
    parameters = 'status, userfrom'
    condition1 = 'userto = %s' % UserID
    condition2 = 'status = 0'
    conditions = condition1 + ' AND ' + condition2

    # execute retrieving friend requests list
    result = retrieve_from_db(table, parameters, conditions)

    # return make_response(jsonify({"Requests":result}), 200)
    return make_response(jsonify(result), 200)

'''
Function to act upon friend requests - accept or reject
'''
@app.route('/friend_request_action', methods=['POST'])
def friend_request_action():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts ID of person who wants to be friends, and your own ID from JSON
    # object, as well as the action to take (0/1 for reject/accept)
    UserOne = str(request.json['UserRequesterID'])
    UserTwo = str(request.json['UserOwnID'])
    UserAction = str(request.json['UserAction'])

    # if user is not logged in, return error message
    if not isLoggedIn(UserTwo):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)

    # conditions for SQL query
    condition1 = 'userfrom = ' + UserOne
    condition2 = 'userto = ' + UserTwo
    conditions = condition1 + ' AND ' + condition2

    # reject friend request and delete friend request
    if UserAction == '0':
        delete_from_db('friends', conditions)
        return make_response(jsonify({"Message":"Friend request rejected!"}), 200)
    else:
        # accept friend request
        update_row_in_db('friends','status = 1', conditions)
        return make_response(jsonify({"Message":"Friend request accepted!"}), 200)
    
'''
Function to return a list of a user's friends and their details

Returns: JSON object containing details of user's friends
'''
@app.route('/retrieve_friends_details', methods=['POST'])
def retrieve_friends_details():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts user's own ID from JSON object
    User = int(request.json['UserID'])

    # if user is not logged in, return error message
    if not isLoggedIn(User):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)

    # things for SQL query
    table = 'friends'
    parameters = '*'
    condition1 = 'userto = %s' % User
    condition2 = 'userfrom = %s' % User
    conditions = condition1 + ' OR ' + condition2

    result = retrieve_from_db(table, parameters, conditions)

    search_results = []
    results_array = []

    # ascertain who the user's friends are and add to search_results list
    for json_obj in result:
        if (json_obj['userfrom'] == User or json_obj['userto'] == User) and json_obj['status'] == 1:
            if json_obj['userfrom'] != User:
                search_results.append(json_obj['userfrom'])
            elif json_obj['userto'] != User:
                search_results.append(json_obj['userto'])

    # if user has no friends, return a message saying that
    if len(search_results) == 0:
        return make_response(jsonify({"Message":"You have no friends yet!"}), 200)

    # retrieve friends' details (including location and time location was
    # logged)
    for id in search_results:
        condition = 'userid = %s' % id
        result = retrieve_from_db('all_no_password','*', condition)
        results_array.append(result[0])

    # return make_response(jsonify({"Results":results_array}), 200)
    return make_response(jsonify(results_array), 200)
    
'''
Function to send message to another user
'''
@app.route('/send_message', methods=['POST'])
def send_message():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts ID of person who wants to be friends, and your own ID from JSON
    # object, as well as the action to take (0/1 for reject/accept)
    UserFrom = str(request.json['UserFrom'])
    UserTo = str(request.json['UserTo'])
    MessageContent = str(request.json['MessageContent'])
    Timestamp = str(request.json['Timestamp'])
    
    # if user is not logged in, return error message
    if not isLoggedIn(UserFrom):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)
    
    parameters = '(userfrom, userto, status, message, timestamp)'
    values = (UserFrom, UserTo, '0', MessageContent, Timestamp)
    
    # make sure recipient is a friend
    if isFriends(UserFrom, UserTo):
        write_details_to_db('messages', parameters, values)
        return make_response(jsonify({'Message': 'Message sent!'}), 201)
    else:
        return make_response(jsonify({'Message': 'You both are not friends!'}), 201)
    
'''
Function to retrieve unread messages
'''
@app.route('/retrieve_messages', methods=['POST'])
def retrieve_messages():
    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)

    # extracts ID of person who wants to be friends, and your own ID from JSON
    # object, as well as the action to take (0/1 for reject/accept)
    UserTo = str(request.json['UserID'])
    
    # if user is not logged in, return error message
    if not isLoggedIn(UserTo):
        return make_response(jsonify({"Message":"You are not logged in!"}), 400)
    
    # things needed for SQL searching. Retrieve user's list of friend requests
    # from DB
    table = 'messages'
    parameters = 'messageid, userfrom, message, timestamp'
    condition1 = 'userto = %s' % UserTo
    condition2 = 'status = 0'
    conditions = condition1 + ' AND ' + condition2

    # execute retrieving friend requests list
    result = retrieve_from_db(table, parameters, conditions)
    
    msgids = []
    for json_obj in result:
        msgids.append(json_obj['messageid'])
        
    msgids = tuple(msgids)
    
    for id in msgids:
        pair = 'status = "%s"' % '1'
        condition = "messageid = '" + str(id) + "'"
        update_row_in_db('messages', pair, condition)
        
    
    # return make_response(jsonify({"Unread messages":result}), 200)
    return make_response(jsonify(result), 200)


'''
Helper function to see if two users are friends with each other, before being
able to commence certain operations.
'''
def isFriends(UserFrom, UserTo):
    condition1 = 'userfrom = %s' % str(UserFrom)
    condition2 = 'userto = %s' % str(UserTo)
    condition3 = 'status = 1'

    condition4 = 'userfrom = %s' % str(UserTo)
    condition5 = 'userto = %s' % str(UserFrom)

    condition6 = '(' + condition1 + ' AND ' + condition2 + ' AND ' + condition3 + ')'
    condition7 = '(' + condition4 + ' AND ' + condition5 + ' AND ' + condition3 + ')'

    conditions = condition6 + ' OR ' + condition7

    result = retrieve_from_db('friends', 'userfrom, userto', conditions)

    if len(result) == 0:
        return False
    else:
        return True


    
@app.route('/logout', methods=['POST'])
def logout():

    # ensures that it's a JSON object that's being sent in
    if not request.json:
        abort(400)
        
    userid = str(request.json['UserID'])
    pair = 'loggedin = 0'
    condition = "userid = '" + userid + "'"
    update_row_in_db('users', pair, condition)
    
    return make_response(jsonify({'Message': 'You are now logged out!'}), 201)

'''
Helper function to delete from database.
'''
def delete_from_db(table, conditions):
    # Connecting to DB stuff
    # Create persistent connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)
    
    query_l1 = "delete from %s" % table
    query_l2 = "where %s" % conditions

    sql_query = query_l1 + ' ' + query_l2

    cursor.execute(sql_query)

    cnx.commit()
    cnx.close()
    
'''
Helper function to check if user exists in database, based on username.

Used in signup.
'''
def isUserExists(username):
    # Connecting to DB stuff
    # Create persistent connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)

    query = "select * from users where username = '" + username + "'"

    cursor.execute(query)

    result = cursor.fetchall()

    if len(result) > 0:
        return True
    else:
        return False

    cnx.close()
    
def isRequestExists(userfrom, userto):
    
    table = 'friends'
    parameters = '*'
    condition1 = 'userfrom = %s' % userfrom
    condition2 = 'userto = %s' % userto
    condition3 = 'status = 0'
    
    conditions = condition1 + ' AND ' + condition2 + ' AND ' + condition3
    
    # execute retrieving friend requests list
    result = retrieve_from_db(table, parameters, conditions)
    
    if len(result) > 0:
        return True
    else:
        return False
'''
Helper function to check if user exists in database, by UserID.
'''
def isUserExistsByID(id):
    # Connecting to DB stuff
    # Create persistent connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)
    
    query = "select * from users where userid = '" + str(id) + "'"

    cursor.execute(query)

    result = cursor.fetchall()
    cnx.close()

    if len(result) > 0:
        return True
    else:
        return False

'''
Helper function to write details to database.
'''
def write_details_to_db(table, parameters, values):
    # Connecting to DB stuff
    # Create persistent connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)


    query_l1 = "INSERT INTO %s" % table
    query_l2 = parameters #tuple type
    query_l3 = "values %s" % (values,)

    sql_query = query_l1 + ' ' + query_l2 + ' ' + query_l3

    cursor.execute(sql_query)

    cnx.commit()
    cnx.close()
    
'''
Helper function to update row in database.
'''
def update_row_in_db(table, pair, condition):
    # Connecting to DB stuff
    # Create persistent connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)

    query_l1 = "UPDATE %s" % table
    query_l2 = "SET %s" % pair
    query_l3 = "WHERE %s" % condition

    sql_query = query_l1 + ' ' + query_l2 + ' ' + query_l3

    cursor.execute(sql_query)

    cnx.commit()
    cnx.close()
    
'''
Helper function to retrieve from database.
'''
def retrieve_from_db(table, parameters, conditions):
    # Connecting to DB stuff
    # Create persistent connection to DB
    cnx = mysql.connector.connect(host="203.101.225.0",
                                 user="snake",
                                 passwd="ssss",
                                 database="slither")
    cursor = cnx.cursor(dictionary=True)

    # parameters are like col1, col2...etc
    query_l1 = "select %s from" % parameters
    query_l2 = table
    query_l3 = "where %s" % conditions

    sql_query = query_l1 + ' ' + query_l2 + ' ' + query_l3

    cursor.execute(sql_query)

    result = cursor.fetchall()
    cnx.close()

    return result
    
'''
Helper function to hash password so that passwords aren't stored in plaintext.
'''
def hash_password(password):
    # uuid is used to generate a random number
    salt = uuid.uuid4().hex
    return hashlib.sha256(salt.encode() + password.encode()).hexdigest() + ':' + salt

'''
Helper function to check whether provided password matches when logging in.
'''
def check_password(hashed_password, user_password):
    password, salt = hashed_password.split(':')
    return password == hashlib.sha256(salt.encode() + \
        user_password.encode()).hexdigest()
    
'''
Helper function to make sure user is logged in before being permitted to do
operations.

If a user isn't logged in, the user isn't permitted to commence operations.
'''
def isLoggedIn(UserID):
    condition1 = 'userid = %s' % str(UserID)
    condition2 = 'loggedin = 1'
    conditions = condition1 + ' AND ' + condition2

    result = retrieve_from_db('users','UserID', conditions)

    if len(result) == 0:
        return False
    else:
        return True


if __name__ == '__main__':
    app.run(debug=True, host = 'localhost')
    
    