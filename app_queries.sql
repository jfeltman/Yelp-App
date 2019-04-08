-- Queries to use for the milestone2 application as is in the requirements

-- 1: get list of states from busineses
SELECT DISTINCT stateabbrev FROM businesses ORDER BY stateabbrev;

-- 2: get cities within selected state
-- replace 'AZ' with state selected
SELECT DISTINCT city FROM businesses WHERE stateabbrev = 'AZ' ORDER BY city; 

-- 3: get zip from selected state and city
-- replace 'AZ' and 'Tempe' with state and city selected
SELECT DISTINCT zip FROM businesses WHERE stateabbrev = 'AZ' AND city = 'Tempe' ORDER BY zip;

-- 4: get categories for businesses within selected state, city, and zip code
-- replace state city and zip with those selected
SELECT DISTINCT categories.name
FROM businesses, categories 
WHERE businesses.businessid = categories.businessid AND zip = 85021
ORDER BY categories.name;

-- 5: get businesses within selected state, city, and zip code
SELECT DISTINCT *
FROM businesses
WHERE zip = 85021
ORDER BY name;

-- 6: Filter the businesses (from 5) based on any categories selected
SELECT DISTINCT *
FROM businesses, categories
WHERE businesses.businessid = categories.businessid AND 
	zip = 85021 AND (categories.name = 'Mexican' OR categories.name = 'Hotels') -- can add more based on the amount of categories selected
ORDER BY businesses.name;

-- 7: Get reviews for a selected business (using businessid)
SELECT DISTINCT *
FROM reviews, writesfor
WHERE reviews.reviewid = writesfor.reviewid AND writesfor.businessid = 'LhBP1BxWBM3aLLRR4zgQlw';

-- 8: Create a new review for a business
-- set votesuseful, cool, funny to 0 as a default
-- use selected businesses and create a string of random characters for the new reviewid
INSERT INTO reviews (reviewid, reviewdate, starcount, text, votesuseful, votescool, votesfunny) VALUES ('randomreviewid', '2019-03-19', 4, 'some long review text no more than 2000 characters.', 0, 0, 0);
INSERT INTO writesfor (reviewid, userid, businessid) VALUES ('samerandomreviewid', 'loggedinuserid', 'selectedbusinessid');
