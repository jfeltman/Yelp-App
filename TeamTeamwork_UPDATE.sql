-- Businesses checkin count update
UPDATE businesses
SET checkincount = 
	(SELECT SUM(C1.checkincount) 
	 FROM checkin as C1 
	 WHERE businesses.businessid = C1.businessid)
WHERE EXISTS -- only update businesses that have checkins
	(SELECT * 
	 FROM checkin as C1 
	 WHERE businesses.businessid = C1.businessid);


-- Temp table for counts to speed up update query with reviews
CREATE TABLE tempCounts(
	businessid VARCHAR(50),
	reviewCount INTEGER,
	avgRating FLOAT,
	PRIMARY KEY (businessid),
	FOREIGN KEY (businessid) REFERENCES Businesses(businessid)
);

-- Get review count and avg rating for each business, then insert it into temp table
INSERT INTO tempCounts
	(SELECT businesses.businessid, COUNT(reviews.reviewid), ROUND(AVG(reviews.starcount), 1)
	 FROM reviews, writesfor, businesses
	 WHERE reviews.reviewid = writesfor.reviewid AND businesses.businessid = writesfor.businessid
	 GROUP BY businesses.businessid);

-- update business review count
UPDATE businesses
SET reviewcount =
	(SELECT reviewcount 
	 FROM tempcounts 
	 WHERE tempCounts.businessid = businesses.businessid);

-- update business starrating
UPDATE businesses
SET starrating =
	(SELECT avgrating 
	 FROM tempcounts 
	 WHERE tempCounts.businessid = businesses.businessid);

-- get rid of temp counts
DROP TABLE tempCounts;