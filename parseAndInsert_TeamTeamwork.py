import json
import psycopg2

# Team Teamwork Milestone 2 Data Insertion
# NOTE: Change dbname and password for each connection statement to fit your local db

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def int2BoolStr (value):
    if value == 0:
        return 'False'
    else:
        return 'True'

def insert2UsersTable():
    with open('Yelp-CptS451-2019/yelp_user.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='****' user='postgres' host='localhost' password='****'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)

            userid = str(data['user_id'])
            name = str(data['name'])
            yelping_since = str(data['yelping_since'])
            review_count = str(data['review_count'])
            fans = str(data['fans'])
            avg_stars = str(data['average_stars'])
            funny = str(data['funny'])
            useful = str(data['useful'])
            cool = str(data['cool']) 
            # lat and long coordinates not given, so set them to null in database, user will be able to edit them later

            sql = 'INSERT INTO users (userid, name, yelpingsince, reviewcount, fanscount, averagestars, votesfunny, votescool, votesuseful) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)'
            sql_data = (userid, name, yelping_since, review_count, fans, avg_stars, funny, cool, useful)

            try:
                cur.execute(sql, sql_data)
            except:
                print("insert into users table failed!")
            conn.commit()

            line = f.readline()
            count_line += 1

        cur.close()
        conn.close()

    print(count_line)
    f.close()


def insert2FriendsTable():
    with open('Yelp-CptS451-2019/yelp_user.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='****' user='postgres' host='localhost' password='****'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)

            userid = str(data['user_id'])
            friends = [item for item in data['friends']]

            for friendid in friends:
                sql = 'INSERT INTO friends (friendid, userid) VALUES (%s, %s)'
                sql_data = (friendid, userid)

                try:
                    cur.execute(sql, sql_data)
                except:
                    print("insert into users table failed!")
                conn.commit()

            line = f.readline()
            count_line += 1

        cur.close()
        conn.close()

    print(count_line)
    f.close()


def insert2BusinessTable():
    #reading the JSON file
    with open('Yelp-CptS451-2019/yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='****' user='postgres' host='localhost' password='****'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)

            inbusinessid = data['business_id']
            inname = data['name']
            inreviewcount = data['review_count']
            instarrating = data['stars']
            inisactive = data['is_open']
            inzip = data['postal_code']
            instateabbrev = data['state']
            incity = data['city']
            incheckincount = 0
            instreet = data['address']
            inlat = data['latitude']
            inlong = data['longitude']

            trystring = 'INSERT INTO businesses (businessid, name, reviewcount, starrating, isactive, zip, stateabbrev, city, checkincount, street, lat, long) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)'
            trydata = (cleanStr4SQL(inbusinessid), cleanStr4SQL(inname), inreviewcount, instarrating, int2BoolStr(inisactive), cleanStr4SQL(inzip), cleanStr4SQL(instateabbrev), cleanStr4SQL(incity), incheckincount, cleanStr4SQL(instreet), inlat, inlong)
            # Generate the INSERT statement for the current business
            try:
                cur.execute(trystring, trydata)
            except:
                print("Insert to businessesTABLE failed!") #will still insert PROPERLY anyway, no idea why this is thrown
            conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2TableAttributes():
    #reading the JSON file
    with open('Yelp-CptS451-2019/yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='****' user='postgres' host='localhost' password='****'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            attributeList = {}
            hasAttributes = 1
            if(data['attributes']):
                for item in data['attributes']:
                    if type(data['attributes'][str(item)]) is dict:
                        for value in data['attributes'][str(item)]:
                            attributeList[value] = data['attributes'][str(item)][str(value)]
                    else:
                        attributeList[item] = data['attributes'][str(item)]
            else:
                hasAttributes = 0

            inbusinessid = data['business_id']
            #print(attributeList)
            for attrKey, attrVal in attributeList.items():
                inname = attrKey
                inval = attrVal
                trystring = 'INSERT INTO attributes (businessid, name, value) VALUES (%s, %s, %s)'
                trydata = (inbusinessid, inname, inval)
                # Generate the INSERT statement for the current business
                try:
                    cur.execute(trystring, trydata)
                except:
                    print("Insert to attributeTABLE failed!") #will still insert PROPERLY anyway, no idea why this is thrown
                conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2CategoriesTable():
    with open('Yelp-CptS451-2019/yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='****' user='postgres' host='localhost' password='****'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            businessid = data['business_id']
            hoursList = {}

            if (data['categories']):
                for category in data['categories']:
                    sqlString = 'INSERT INTO categories (businessid, name) VALUES (%s, %s);'
                    sqlData = (businessid, category)
                    try:
                        cur.execute(sqlString, sqlData)
                    except:
                        print("Insert into categories table failed")
                    conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2HoursTable():
        #reading the JSON file
    with open('Yelp-CptS451-2019/yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='****' user='postgres' host='localhost' password='****'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            businessid = data['business_id']
            hoursList = {}

            if(data['hours']):
                for day in data['hours']:
                    hoursList[day] = str(data['hours'][day]).split("-")                    

            #print(hoursList)

            for day, hours in hoursList.items():
                sqlString = 'INSERT INTO hours (businessid, weekday, opentime, closetime) VALUES (%s, %s, %s, %s);'
                sqlData = (businessid, day, hours[0], hours[1])
                # Generate the INSERT statement for the current business
                try:
                    cur.execute(sqlString, sqlData)
                except:
                    print("Insert to hours table failed!")
                conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2CheckinTable():
    with open('Yelp-CptS451-2019/yelp_checkin.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='****' user='postgres' host='localhost' password='****'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            businessid = data['business_id']
            checkinList = {}

            for day in data['time']:
                for time in data['time'][str(day)]:
                    count = data['time'][day][time]
                    sqlString = 'INSERT INTO checkin (businessid, checkinday, checkintime, checkincount) VALUES (%s, %s, %s, %s);'
                    sqlData = (businessid, day, time, count)
                    try:
                        cur.execute(sqlString, sqlData)
                    except:
                        print("Insert to checkin table failed!") 
                    conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2ReviewsTable():
    with open('Yelp-CptS451-2019/yelp_review.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='****' user='postgres' host='localhost' password='****'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)

            reviewid = str(data['review_id'])
            userid = str(data['user_id'])
            businessid = str(data['business_id'])
            reviewtext = cleanStr4SQL(data['text'])
            date = str(data['date'])
            stars = str(data['stars'])
            funny = str(data['funny'])
            useful = str(data['useful']) 
            cool = str(data['cool'])

            # insert into review table
            sqlReviewsString = 'INSERT INTO reviews (reviewid, reviewdate, starcount, text, votesuseful, votescool, votesfunny) VALUES (%s, %s, %s, %s, %s, %s, %s);'
            sqlReviewsData = (reviewid, date, stars, reviewtext, useful, cool, funny)

            try:
                cur.execute(sqlReviewsString, sqlReviewsData)
            except:
                print("insert into reviews table failed!")
            conn.commit()

            #insert into writesfor table
            sqlWritesForString = 'INSERT INTO writesfor (reviewid, userid, businessid) VALUES (%s, %s, %s);'
            sqlWritesForData = (reviewid, userid, businessid)

            try:
                cur.execute(sqlWritesForString, sqlWritesForData)
            except:
                print("insert into writesfor table failed!")
            conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()


#insert2UsersTable()
#insert2FriendsTable()
#insert2BusinessTable()
#insert2TableAttributes()
#insert2CategoriesTable()
#insert2HoursTable()
#insert2CheckinTable()
#insert2ReviewsTable()
