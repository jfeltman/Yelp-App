import json

# Team Teamwork
# -------------
# Joshua Feltman
# Matthew Johnson
# Andrew Fallin

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def parseBusinessData():
    #read the JSON file
    with open('Yelp-CptS451-2019/yelp_business.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile =  open('Parsed_Data/business.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)

            outfile.write(cleanStr4SQL(data['business_id'])+', ') #business id
            outfile.write(cleanStr4SQL(data['name'])+', ') #name
            outfile.write(cleanStr4SQL(data['address'])+', ') #full_address
            outfile.write(cleanStr4SQL(data['state'])+', ') #state
            outfile.write(cleanStr4SQL(data['city'])+', ') #city
            outfile.write(cleanStr4SQL(data['postal_code']) + ', ')  #zipcode
            outfile.write(str(data['latitude'])+', ') #latitude
            outfile.write(str(data['longitude'])+', ') #longitude
            outfile.write(str(data['stars'])+', ') #stars
            outfile.write(str(data['review_count'])+', ') #reviewcount
            outfile.write(str(data['is_open'])+'\n') #openstatus
            outfile.write("Categories: " + str([item for item in  data['categories']])+'\n') #category list
            outfile.write("Attributes: "  + parseAttributes(data) + "\n") # write your own code to process attributes
            outfile.write("Hours: " + parseHours(data)) # write your own code to process hours
            outfile.write('\n');

            line = f.readline()
            count_line +=1
    print(count_line)
    outfile.close()
    f.close()

def parseAttributes(data):
    s = "["

    for item in data['attributes']:
        if type(data['attributes'][str(item)]) is dict:
            for value in data['attributes'][str(item)]:
                s += "(" + value + ", " + str(data['attributes'][str(item)][str(value)]) + ")"
        else:
           s += "(" + item + ", " + str(data['attributes'][str(item)]) + ")"

    s += "]"
    return str(s)

def parseHours(data):
    s = "["

    for item in data['hours']:

        temp = str(data['hours'][str(item)]).replace("-", ",")
        s += "(" + item + ", " + temp + ")"

    s += "]"
    return str(s)

def parseUserData():
    #read the JSON file
    with open('Yelp-CptS451-2019/yelp_user.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile =  open('Parsed_Data/user.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)

            outfile.write(str(data['user_id']) + ', ') # user id
            outfile.write(str(data['name']) + ', ') # name
            outfile.write(str(data['yelping_since']) + ', ') # yelping_since
            outfile.write(str(data['review_count']) + ', ') # review_count
            outfile.write(str(data['fans']) + ', ') # fans
            outfile.write(str(data['average_stars']) + ', ') # average_stars
            outfile.write(str(data['funny']) + ', ') # funny
            outfile.write(str(data['useful']) + ', ') # useful
            outfile.write(str(data['cool']) + '\n') # cool
            outfile.write("Friends: " + str([item for item in data['friends']])) # friends
            outfile.write('\n');

            line = f.readline()
            count_line +=1
    print(count_line)
    outfile.close()
    f.close()

def parseCheckinData():
    #read the JSON file
    with open('Yelp-CptS451-2019/yelp_checkin.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile =  open('Parsed_Data/checkin.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)

            outfile.write(str(data['business_id']) + '\n') # business_id
            outfile.write(parseCheckinDay(data)) # day of week/time

            line = f.readline()
            count_line +=1
    print(count_line)
    outfile.close()
    f.close()
    pass

def parseCheckinDay(data):
    s = ""

    for day in data['time']:
        s += "\t" + str(day) + ": "
        for time in data['time'][str(day)]:
            s += "(" + str(time) + ", " + str(data['time'][str(day)][str(time)]) + ")"
        s += "\n"

    return str(s)

def parseReviewData():
    #read the JSON file
    with open('Yelp-CptS451-2019/yelp_review.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile =  open('Parsed_Data/review.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)

            outfile.write(str(data['review_id']) + ', ') # review_id
            outfile.write(str(data['user_id']) + ', ') # user_id
            outfile.write(str(data['business_id']) + ', ') # business_id
            outfile.write(cleanStr4SQL(data['text']) + ', ') # text
            outfile.write(str(data['stars']) + ', ') # stars
            outfile.write(str(data['date']) + ', ') # date
            outfile.write(str(data['funny']) + ', ') # funny
            outfile.write(str(data['useful']) + ', ') # useful
            outfile.write(str(data['cool'])) # cool
            outfile.write('\n');

            line = f.readline()
            count_line +=1
    print(count_line)
    outfile.close()
    f.close()

parseBusinessData()
#parseUserData()
parseCheckinData()
#parseReviewData()