--TeamTeamwork ER to Relational Table Statements

CREATE TABLE Users(
	userId VARCHAR(50),
	name VARCHAR(50),
	yelpingSince DATE,
	reviewCount INTEGER,
	fansCount INTEGER,
	averageStars FLOAT,
	lat FLOAT,
	long FLOAT,
	votesFunny INTEGER,
	votesCool INTEGER,
	votesUseful INTEGER,
	PRIMARY KEY (userId)
);

CREATE TABLE Friends (
	friendID VARCHAR(50), --friends id
	userId VARCHAR(50), --logged in person id
	PRIMARY KEY (userId, friendID),
	FOREIGN KEY (userId) REFERENCES Users(userId),
	FOREIGN KEY (friendID) REFERENCES Users(userId)
);

CREATE TABLE Reviews (
	reviewID VARCHAR(50),
	reviewDate DATE,
	starCount INTEGER,
	text VARCHAR(2000),
	votesUseful INTEGER,
	votesCool INTEGER,
	votesFunny INTEGER,
	PRIMARY KEY (reviewID)
);

CREATE TABLE Businesses (
	businessID VARCHAR(50),
	name VARCHAR(128),
	reviewCount INTEGER,
	starRating FLOAT,
	isActive BOOL,
	zip	INTEGER,
	stateAbbrev CHAR(2),
	city VARCHAR(50),
	checkinCount INTEGER,
	street VARCHAR(128),
	lat FLOAT,
	long FLOAT,
	PRIMARY KEY (businessID)
);

CREATE TABLE FavoriteBusiness ( --identical to Friends relation?
	businessID VARCHAR(50),
	userId VARCHAR(50),
	PRIMARY KEY (userId, businessID),
	FOREIGN KEY (userId) REFERENCES Users(userId),
	FOREIGN KEY (businessID) REFERENCES Businesses(businessID)
);

CREATE TABLE WritesFor (
	reviewID VARCHAR(50),
	userId VARCHAR(50),
	businessID VARCHAR(50),
	PRIMARY KEY (userId, reviewID, BusinessID),
	FOREIGN KEY (reviewID) REFERENCES Reviews(reviewID),
	FOREIGN KEY (userId) REFERENCES Users(userId),
	FOREIGN KEY (businessID) REFERENCES Businesses(businessID)
);

CREATE TABLE Attributes (
	businessID VARCHAR(50),
	name VARCHAR(50),
	value VARCHAR(25), --bool types and varying types?
	PRIMARY KEY (businessID, name), 
	FOREIGN KEY (businessID) REFERENCES Businesses(businessID)
);

CREATE TABLE Categories (
	businessID VARCHAR(50),
	name VARCHAR(50),
	PRIMARY KEY (businessID, name), 
	FOREIGN KEY (businessID) REFERENCES Businesses(businessID)
);

CREATE TABLE Hours(
	businessID VARCHAR(50),
	weekday CHAR(9),
	openTime TIME,
	closeTime TIME,
	PRIMARY KEY (businessID, weekday), 
	FOREIGN KEY (businessID) REFERENCES Businesses(businessID)
);

CREATE TABLE Checkin (
	businessID VARCHAR(50),
	checkinDay CHAR(9),
	checkinTime TIME,
	checkinCount INTEGER,
	PRIMARY KEY (businessID, checkinDay, checkinTime), 
	FOREIGN KEY (businessID) REFERENCES Businesses(businessID)
);
