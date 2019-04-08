-- Team Teamwork
-- Milestone 2 Trigger Statements

-- ========================================================
-- Trigger 1
-- When review is added for business, update the businesses review count and its avg star rating
CREATE OR REPLACE FUNCTION updateReviewCountAndRating() RETURNS trigger AS '
DECLARE
	sumRating FLOAT;
	revCount INT;
	avgRating FLOAT;
BEGIN
	sumRating = (SELECT SUM(starcount) FROM reviews, writesfor WHERE reviews.reviewid = writesfor.reviewid AND writesfor.businessid = NEW.businessid);
	revCount = (SELECT COUNT(writesfor.businessid) FROM writesfor WHERE writesfor.businessid = NEW.businessid);
	avgRating = sumRating / revCount;
	UPDATE businesses
	SET reviewcount = revCount, starrating = ROUND(CAST(avgRating as numeric), 1)
	WHERE businesses.businessid = NEW.businessid;
	RETURN NEW;
END
' LANGUAGE plpgsql;

CREATE TRIGGER reviewCountAndRating
AFTER INSERT ON writesfor
FOR EACH ROW
WHEN (NEW.businessid IS NOT NULL)
EXECUTE PROCEDURE updateReviewCountAndRating();

-- Trigger 1 tests, after all inserts, reviewcount should be 3 and rating should be 3.7
INSERT INTO users (userid) VALUES ('randomuserid');
INSERT INTO businesses (businessid, name, reviewcount, starrating) VALUES ('randombusinessid', 'test business', 0, 0);
INSERT INTO reviews (reviewid, starcount) VALUES ('randomreviewid', 4);
INSERT INTO writesfor (reviewid, userid, businessid) VALUES ('randomreviewid', 'randomuserid', 'randombusinessid');

INSERT INTO reviews (reviewid, starcount) VALUES ('randomreviewid2', 2);
INSERT INTO writesfor (reviewid, userid, businessid) VALUES ('randomreviewid2', 'randomuserid', 'randombusinessid');

INSERT INTO reviews (reviewid, starcount) VALUES ('randomreviewid3', 5);
INSERT INTO writesfor (reviewid, userid, businessid) VALUES ('randomreviewid3', 'randomuserid', 'randombusinessid');
SELECT * FROM businesses WHERE businessid = 'randombusinessid';

-- Clean up tests
DELETE FROM writesfor WHERE reviewid = 'randomreviewid';
DELETE FROM writesfor WHERE reviewid = 'randomreviewid2';
DELETE FROM writesfor WHERE reviewid = 'randomreviewid3';
DELETE FROM reviews WHERE reviewid = 'randomreviewid';
DELETE FROM reviews WHERE reviewid = 'randomreviewid2';
DELETE FROM reviews WHERE reviewid = 'randomreviewid3';
DELETE FROM businesses WHERE businessid = 'randombusinessid';
DELETE FROM users WHERE userid = 'randomuserid';


-- ========================================================
-- Trigger 2
-- When a customer check ins to a business, update the businesses checkin count (ON NEW CHECKIN INSERT)
CREATE OR REPLACE FUNCTION updateCheckinCountInsert() RETURNS trigger AS '
BEGIN
	UPDATE businesses
	SET checkincount = (SELECT SUM(checkincount) FROM checkin WHERE businessid = NEW.businessid)
	WHERE businesses.businessid = NEW.businessid;
	RETURN NEW;
END
' LANGUAGE plpgsql;

CREATE TRIGGER checkinCountInsert
AFTER INSERT ON checkin
FOR EACH ROW
WHEN (NEW.businessid IS NOT NULL)
EXECUTE PROCEDURE updateCheckinCountInsert();

-- tests, checkincount should be 2
INSERT INTO businesses (businessid, name, checkincount) VALUES ('randombusinessid', 'test business', 0);
INSERT INTO checkin (businessid, checkinday, checkintime, checkincount) VALUES ('randombusinessid', 'Monday', '12:00:00', 1);
INSERT INTO checkin (businessid, checkinday, checkintime, checkincount) VALUES ('randombusinessid', 'Tuesday', '14:55:00', 1);
SELECT businessid, checkincount FROM businesses WHERE businessid = 'randombusinessid';

-- clean up
DELETE FROM checkin WHERE businessid = 'randombusinessid';
DELETE FROM businesses WHERE businessid = 'randombusinessid';

-- ========================================================
-- Trigger 3
-- When a customer check ins to a business, update the businesses checkin count (ON CHECKIN UPDATE)
CREATE OR REPLACE FUNCTION updateCheckinCount() RETURNS trigger AS '
BEGIN
	UPDATE businesses
	SET checkincount = (SELECT SUM(checkincount) FROM checkin WHERE businessid = NEW.businessid)
	WHERE businesses.businessid = NEW.businessid;
	RETURN NEW;
END
' LANGUAGE plpgsql;

CREATE TRIGGER checkinCountUpdate
AFTER UPDATE OF checkincount ON checkin
FOR EACH ROW
WHEN (NEW.businessid IS NOT NULL AND NEW.checkincount > 0)
EXECUTE PROCEDURE updateCheckinCount();

-- tests, checkincount should be 2
INSERT INTO businesses (businessid, name, checkincount) VALUES ('randombusinessid', 'test business', 0);
INSERT INTO checkin (businessid, checkinday, checkintime, checkincount) VALUES ('randombusinessid', 'Monday', '12:00:00', 1);
UPDATE checkin SET checkincount = 2 WHERE businessid = 'randombusinessid' AND checkinday = 'Monday' AND checkintime = '12:00:00';
SELECT businessid, checkincount FROM businesses WHERE businessid = 'randombusinessid';

-- clean up
DELETE FROM checkin WHERE businessid = 'randombusinessid';
DELETE FROM businesses WHERE businessid = 'randombusinessid';
