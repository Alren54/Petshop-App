drop table if exists application cascade;
drop table if exists products cascade;
drop table if exists notice_board cascade;
drop table if exists animal cascade;
drop table if exists user_info cascade;

drop sequence if exists ani_id cascade;
drop sequence if exists use_id cascade;
drop sequence if exists not_id cascade;
drop sequence if exists pro_id cascade;

drop function if exists trig_fonk_add_appl cascade;
drop function if exists trig_fonk_add_nid cascade;
drop function if exists trig_fonk_add_aid cascade;
drop function if exists trig_fonk_add_uid cascade;
drop function if exists trig_fonk_add_pid cascade;
drop function if exists trig_fonk_delete_notice_applicants cascade;

drop type if exists tumtip cascade;
drop type if exists protip cascade;
drop type if exists mynoticetip cascade;
drop type if exists myapplicanttip cascade;
drop type if exists nottip cascade;

-----------------------------------------------------------------
--Creating All Tables:

-- animal Table
CREATE TABLE animal (
    animal_id SERIAL PRIMARY KEY,
    species VARCHAR(20) UNIQUE 
);

-- user_info Table
CREATE TABLE user_info (
    user_id SERIAL PRIMARY KEY,
    user_name VARCHAR(20) NOT NULL UNIQUE,
    user_password VARCHAR(20) NOT NULL,
    city VARCHAR(20),
    address VARCHAR(100),
    f_name VARCHAR(20),
    l_name VARCHAR(20),
    birth_date DATE,
    CONSTRAINT consent_age CHECK (birth_date < CURRENT_DATE - INTERVAL '18 years')
);

-- notice_board Table
CREATE TABLE notice_board (
    notice_id SERIAL PRIMARY KEY,
    uid INT NOT NULL,
    CONSTRAINT f_key_uid FOREIGN KEY (uid) REFERENCES user_info(user_id),
    aid INT NOT NULL,
    CONSTRAINT f_key_aid FOREIGN KEY (aid) REFERENCES animal(animal_id) ON DELETE CASCADE,
    notice_date DATE,
	animal_name VARCHAR(20),
    weight INT,
    sex CHAR,
    city VARCHAR(20),
    birth_date DATE,
	breed VARCHAR(20),
	CONSTRAINT birth_date_ck CHECK (birth_date < CURRENT_DATE)
);

-- application Table
CREATE TABLE application (
    nid SERIAL,
    applicant_id INT NOT NULL,
    PRIMARY KEY (nid, applicant_id),
    CONSTRAINT f_key_nid FOREIGN KEY (nid) REFERENCES notice_board(notice_id),
    CONSTRAINT f_key_applicant FOREIGN KEY (applicant_id) REFERENCES user_info(user_id)
);

-- products Table
CREATE TABLE products (
    product_id SERIAL PRIMARY KEY,
    product_name VARCHAR(20),
    price INT,
    stock INT,
    upload_date DATE,
    a_species VARCHAR(20),
	a_aid INT,
	CONSTRAINT f_key_aid FOREIGN KEY (a_aid) REFERENCES animal(animal_id) ON DELETE CASCADE
);

-- Creating View:
CREATE VIEW applicant_info AS 
    SELECT f_name, l_name, user_id, address
    FROM user_info;

-- Creating Sequences:
CREATE SEQUENCE ani_id MINVALUE 1 MAXVALUE 20 INCREMENT BY 1;
CREATE SEQUENCE use_id MINVALUE 100 MAXVALUE 120 INCREMENT BY 1;
CREATE SEQUENCE not_id MINVALUE 200 MAXVALUE 220 INCREMENT BY 1;
CREATE SEQUENCE pro_id MINVALUE 300 MAXVALUE 320 INCREMENT BY 1;

---------------------------------------------------------------
--Creating Functions and Triggers:
---------------------------------------------------------------

-- If the same Application already exists:
CREATE OR REPLACE FUNCTION trig_fonk_add_appl()
RETURNS TRIGGER AS $$
BEGIN
    IF (EXISTS (SELECT 1 FROM application WHERE NEW.nid = nid AND NEW.applicant_id = applicant_id)) THEN
        RAISE EXCEPTION 'Bu ilana başvurdunuz';
    ELSE
        RETURN NEW;
    END IF;
END;
$$ LANGUAGE 'plpgsql';
---------------------------
CREATE TRIGGER add_appl
BEFORE INSERT
ON application
FOR EACH ROW 
EXECUTE FUNCTION trig_fonk_add_appl();
-----------------------------------------------------------------
-- New User's ID: Sequence Increaser
CREATE OR REPLACE FUNCTION trig_fonk_add_uid()
RETURNS TRIGGER AS $$
BEGIN
    IF (NEW.user_id = 0) THEN
        NEW.user_id := NEXTVAL('use_id');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE 'plpgsql';
---------------------------------------
CREATE TRIGGER add_uid
BEFORE INSERT
ON user_info
FOR EACH ROW 
EXECUTE FUNCTION trig_fonk_add_uid();
------------------------------------------------------------------
-- New Animal's ID: Sequence Increaser
CREATE OR REPLACE FUNCTION trig_fonk_add_aid()
RETURNS TRIGGER AS $$
BEGIN
    IF (NEW.animal_id = 0) THEN
        NEW.animal_id := NEXTVAL('ani_id');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE 'plpgsql';
---------------------------------------
CREATE TRIGGER add_aid
BEFORE INSERT
ON animal
FOR EACH ROW 
EXECUTE FUNCTION trig_fonk_add_aid();
------------------------------------------------------------------
-- New Notice's ID: Sequence Increaser
CREATE FUNCTION trig_fonk_add_nid()
RETURNS TRIGGER AS $$

BEGIN
    IF (new.notice_id = 0) THEN
		new.notice_id:=nextval('not_id');
	END IF;
	RETURN new;
END;
$$ LANGUAGE 'plpgsql';
-------------------------------
CREATE TRIGGER add_nid
BEFORE INSERT
ON notice_board
FOR EACH ROW EXECUTE FUNCTION trig_fonk_add_nid();
--------------------------------------------------------------------
-- New Product's ID: Sequence Increaser
CREATE FUNCTION trig_fonk_add_pid()
RETURNS TRIGGER AS $$

BEGIN
    IF (new.product_id = 0) THEN
		new.product_id:=nextval('pro_id');
	END IF;
	RETURN new;
END;
$$ LANGUAGE 'plpgsql';
--------------------------------
CREATE TRIGGER add_pid
BEFORE INSERT
ON products
FOR EACH ROW EXECUTE FUNCTION trig_fonk_add_pid();
-----------------------------------------------------------------
-- Product's Stock Decrementing or Deleting Checker:
CREATE OR REPLACE FUNCTION trig_fonk_delete_pid()
RETURNS TRIGGER AS $$
BEGIN
    RAISE NOTICE'Ürün başarılı bir şekilde alınmıştır.';
    IF (OLD.stock > 1) THEN
        UPDATE products
        SET stock = stock - 1
        WHERE product_id = OLD.product_id;
        RETURN NULL; -- Returning NULL prevents the actual deletion
    END IF;
    RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';
--------------------------------
CREATE TRIGGER delete_pid
BEFORE DELETE
ON products
FOR EACH ROW EXECUTE FUNCTION trig_fonk_delete_pid();
------------------------------------------------------------------------
-- Deleting every Applicant for a Notice once the Notice has been deleted:
CREATE OR REPLACE FUNCTION trig_fonk_delete_notice_applicants()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM application
    WHERE nid = OLD.notice_id;

    RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';
-------------------------------
CREATE TRIGGER delete_notice_applicants
BEFORE DELETE
ON notice_board
FOR EACH ROW
EXECUTE FUNCTION trig_fonk_delete_notice_applicants();
----------------------------------------------------------------------
--------------------------------------------Trigger Functions End-----
-- Inserting Database:
INSERT INTO user_info VALUES (0, 'SaintXI', 'icarus', 'Kayseri', 'Melikgazi', 'Aziz', 'Çifçibaşı', '2003-08-19');
INSERT INTO user_info VALUES (0, 'EKD', 'Kursat', 'Eskişehir', 'Kocaeli', 'Emin Kürşat', 'Doğan', '2002-01-17');
INSERT INTO user_info VALUES (0, 'Hollyali', '545454', 'Sakarya', 'Adapazarı', 'Ali Eren', 'Arık', '2003-10-20');
INSERT INTO user_info VALUES (0, 'kollessisopod', 'Golisopod', 'Balıkesir', '6 EYLÜL', 'Kemal Ata', 'Türkoğlu', '2003-06-03');
INSERT INTO user_info VALUES (0, 'MI99', 'aşkınolayım', 'Istanbul', 'Istanbul, Galatasaray HQ', 'Mauro', 'Icardi', '1993-02-19');
INSERT INTO user_info VALUES (0, 'Harry_Potter', 'gelemedimsanamedina', 'Istanbul', 'Istanbul, Galatasaray Yedek Kulübesi', 'Kerem', 'Aktürkoğlu', '1998-10-21');
INSERT INTO user_info VALUES (0, 'GalaxyInvader', 'X_Æ_A-12', 'Kayseri', 'Kayseri, Gesi Bağları', 'Elon', 'Tusk', '1971-06-28');
INSERT INTO user_info VALUES (0, 'A', 'B', 'Sample_Şehir', 'Sample_Adres', 'Sample_İsim', 'Sample_Soyisim', '2000-06-30');
INSERT INTO user_info VALUES (0, 'ED9', 'osenebusene', 'Istanbul', 'Kadıköy', 'Edin', 'Dzeko', '1986-03-17');
INSERT INTO user_info VALUES (0, 'csnaber', 'samed', 'Istanbul', 'Kadıköy', 'Can', 'Sungur', '1986-06-30');
INSERT INTO user_info VALUES (0, 'dost', 'asukathebest', 'Istanbul', 'Kadıköy', 'Dost', 'Kayaoğlu', '1989-09-18');
INSERT INTO user_info VALUES (0, 'CR7', 'siuuuuu', 'Riyad', 'Al-Nasr', 'Cristiano', 'Ronaldo', '1985-02-05');
INSERT INTO user_info VALUES (0, 'LM10', 'goat', 'Miami', 'Little Havana', 'Lionel', 'Messi', '1987-06-24'); 

INSERT INTO animal VALUES (0,'Köpek'); 			--1
INSERT INTO animal VALUES (0,'Kedi');			--2
INSERT INTO animal VALUES (0,'Kuş');			--3
INSERT INTO animal VALUES (0,'Hamster');		--4
INSERT INTO animal VALUES (0,'Gine Domuzu');	--5
INSERT INTO animal VALUES (0,'Tavşan');			--6
INSERT INTO animal VALUES (0,'Balık');			--7
INSERT INTO animal VALUES (0,'Kaplumbağa');		--8
INSERT INTO animal VALUES (0,'Mantis');			--9
INSERT INTO animal VALUES (0,'Tilki');			--10
INSERT INTO animal VALUES (0,'Zürafa');			--11
INSERT INTO animal VALUES (0,'Eşek');			--12
INSERT INTO animal VALUES (0,'Maymun');         --13

INSERT INTO notice_board VALUES (0, 100, 2, '2024-01-11', 'Khan', 10, 'M', 'Kayseri', '2022-01-01', 'Scottish');
INSERT INTO notice_board VALUES (0, 100, 2, '2024-01-11', 'Saint', 8, 'M', 'Kayseri', '2021-02-08', 'Siyam');
INSERT INTO notice_board VALUES (0, 100, 2, '2024-01-11', 'Eleanor', 12, 'F', 'Kayseri', '2020-08-23', 'Van Kedisi');
INSERT INTO notice_board VALUES (0, 100, 2, '2024-01-11', 'Viper', 9, 'M', 'Kayseri', '2022-12-30', 'Tekir');
INSERT INTO notice_board VALUES (0, 100, 2, '2024-01-11', 'Wi-Fi', 6, 'F', 'Kayseri', '2023-10-25', 'Sphinx');

INSERT INTO notice_board VALUES (0, 101, 10, '2024-01-09', 'Kızıl Kuyruk', 19, 'M', 'Eskişehir', '2022-07-05', 'Çöl Tilkisi');

INSERT INTO notice_board VALUES (0, 103, 9, '2024-01-08', 'Alien', 0, 'F', 'Balıkesir', '2020-06-03', 'Orkide Mantisi');
INSERT INTO notice_board VALUES (0, 103, 9, '2024-01-08', 'Predator', 0, 'F', 'Balıkesir', '2022-12-11', 'Yeşil Mantis');

INSERT INTO notice_board VALUES (0, 102, 1, '2023-12-16', 'İmparator', 60, 'M', 'Sakarya', '2019-09-09', 'Sivas Kangalı');
INSERT INTO notice_board VALUES (0, 102, 2, '2023-12-16', 'Kaplan', 0, 'F', 'Sakarya', '2023-10-16', 'Turuncu');
INSERT INTO notice_board VALUES (0, 102, 7, '2023-12-16', 'Nemo', 0, 'M', 'Sakarya', '2023-10-8', 'Palyaço Balığı');
INSERT INTO notice_board VALUES (0, 102, 7, '2023-12-16', 'Buffer', 1, 'M', 'Sakarya', '2023-09-15', 'Balon Balığı');
INSERT INTO notice_board VALUES (0, 102, 11, '2023-12-16', 'Cüce', 600, 'F', 'Sakarya', '2016-06-29', 'Afrika Zürafası');

INSERT INTO notice_board VALUES (0, 109, 2, '2024-01-10', 'Osman', 4, 'M', 'Istanbul', '2021-07-06', 'Tekir');
INSERT INTO notice_board VALUES (0, 109, 2, '2024-01-10', 'Kırpık', 3, 'F', 'Istanbul', '2022-11-12', 'Tekir');
INSERT INTO notice_board VALUES (0, 109, 2, '2024-01-10', 'Yumak', 3, 'F', 'Istanbul', '2023-08-17', 'Tekir');

INSERT INTO notice_board VALUES (0, 110, 7, '2023-12-31', 'Gömücü', 6, 'F', 'Istanbul', '2016-03-03', 'Müren');

INSERT INTO notice_board VALUES (0, 111, 13, '2022-11-27', 'CRJR', 17, 'M', 'Riyad', '2019-02-28', 'Şempanze');

INSERT INTO notice_board VALUES (0, 112, 1, '2022-11-27', 'Bodyguard', 17, 'M', 'Riyad', '2019-02-28', 'Doberman');

INSERT INTO application VALUES (213, 100);
INSERT INTO application VALUES (216, 102);
INSERT INTO application VALUES (217, 101);
INSERT INTO application VALUES (218, 101);

INSERT INTO application VALUES (200, 104);
INSERT INTO application VALUES (201, 104);
INSERT INTO application VALUES (202, 104);
INSERT INTO application VALUES (203, 104);
INSERT INTO application VALUES (204, 104);
INSERT INTO application VALUES (205, 104);
INSERT INTO application VALUES (206, 104);
INSERT INTO application VALUES (207, 104);
INSERT INTO application VALUES (208, 104);
INSERT INTO application VALUES (209, 104);
INSERT INTO application VALUES (210, 104);
INSERT INTO application VALUES (211, 104);
INSERT INTO application VALUES (212, 104);

INSERT INTO application VALUES (200, 101);
INSERT INTO application VALUES (200, 103);
INSERT INTO application VALUES (200, 105);
INSERT INTO application VALUES (200, 100);

INSERT INTO application VALUES (201, 101);
INSERT INTO application VALUES (201, 103);
INSERT INTO application VALUES (201, 105);
INSERT INTO application VALUES (201, 100);

INSERT INTO application VALUES (202, 102);
INSERT INTO application VALUES (202, 103);
INSERT INTO application VALUES (202, 105);

INSERT INTO application VALUES (208, 100);
INSERT INTO application VALUES (209, 100);
INSERT INTO application VALUES (210, 100);
INSERT INTO application VALUES (211, 100);
INSERT INTO application VALUES (212, 100);

INSERT INTO application VALUES (208, 106);
INSERT INTO application VALUES (209, 106);
INSERT INTO application VALUES (210, 106);
INSERT INTO application VALUES (211, 106);
INSERT INTO application VALUES (212, 106);


INSERT INTO products VALUES (0, 'Kuş Kafesi', 135, 3, '2024-01-05', 'Kuş', 3); 
INSERT INTO products VALUES (0, 'Akvaryum', 290, 1, '2024-01-05', 'Balık', 7); 
INSERT INTO products VALUES (0, 'Kuş Yemi 500 g', 70, 12, '2024-01-05', 'Kuş', 3); 
INSERT INTO products VALUES (0, 'Köpek Tasması', 50, 100 , now(), 'Köpek', 1);
INSERT INTO products VALUES (0, 'Kedi Tasması', 50, 100 , now(), 'Kedi', 2);
INSERT INTO products VALUES (0, 'Köpek Maması 15 Kg', 949, 30 , now(), 'Köpek', 1);
INSERT INTO products VALUES (0, 'Kedi Maması 3 Kg', 419, 30 , now(), 'Kedi', 2);
INSERT INTO products VALUES (0, 'Kedi Kumu 20 L', 129, 20 , now(), 'Kedi', 2);
INSERT INTO products VALUES (0, 'Hamster Kafesi', 1699, 10 , now(), 'Hamster', 4);
INSERT INTO products VALUES (0, 'Balık Yemi 250 mL', 75, 57 , now(), 'Balık', 7);
INSERT INTO products VALUES (0, 'Kedi Taşıma Kafesi', 369, 15 , now(), 'Kedi', 2);
INSERT INTO products VALUES (0, 'Hamster Talaşı 15 L', 299, 19 , now(), 'Hamster', 4);
INSERT INTO products VALUES (0, 'Hamster Yemi 1 Kg', 46, 32 , now(), 'Hamster', 4);
--------------------------------------------------------------------------------------
-- SEARCH ALGORITHMS -----------------------------------------------------------------
--------------------------------------------------------------------------------------

CREATE TYPE tumtip AS (noticeid INT, aid INT, notice_date DATE, sex CHAR, city VARCHAR(20), birth_date DATE, breed VARCHAR(20), user_name varchar(20), weight INT, species VARCHAR(20), animal_name VARCHAR(20));
CREATE TYPE protip AS (product_id INT, product_name VARCHAR(20), price INT, stock INT, upload_date DATE, a_species VARCHAR(20), a_aid INT);
CREATE TYPE mynoticetip AS (noticeid INT, uid INT, aid INT, notice_date DATE, animal_name VARCHAR(20), weight INT, sex char,city varchar(20),birthdate date,breed varchar(20));
CREATE TYPE myapplicanttip AS (user_city varchar(20), user_fname varchar(20), user_lname varchar(20), user_name varchar(20));
CREATE TYPE nottip AS (notice_id int, uid int, aid int, notice_date date, animal_name varchar(20), weight int, sex char, city varchar(20), birth_date date, breed varchar(20));
--------------------------------------
--
CREATE OR REPLACE FUNCTION searchAll(myID INTEGER) 
RETURNS SETOF tumtip AS $$
DECLARE
    ilan tumtip;
BEGIN
    RETURN QUERY 
        (SELECT n.notice_id, n.aid, n.notice_date, n.sex, n.city, n.birth_date, n.breed, u.user_name, n.weight, a.species, n.animal_name
        FROM notice_board n
        JOIN animal a ON n.aid = a.animal_id
        JOIN user_info u ON n.uid = u.user_id
        WHERE n.uid <> myID)
        EXCEPT
        (SELECT n.notice_id, n.aid, n.notice_date, n.sex, n.city, n.birth_date, n.breed, u.user_name, n.weight, a.species, n.animal_name
        FROM notice_board n
        JOIN animal a ON n.aid = a.animal_id
        JOIN user_info u ON n.uid = u.user_id
        JOIN application app ON n.notice_id = app.nid
        WHERE app.applicant_id = myID);
END;
$$ LANGUAGE 'plpgsql';
----------------------
--
CREATE OR REPLACE FUNCTION searchBySpecies(tur VARCHAR(20), myID INTEGER) 
RETURNS SETOF tumtip AS $$
DECLARE
    ilan tumtip;
BEGIN
    RETURN QUERY 
        SELECT n.notice_id, n.aid, n.notice_date, n.sex, n.city, n.birth_date, n.breed, u.user_name, n.weight, a.species, n.animal_name
        FROM notice_board n
        JOIN animal a ON n.aid = a.animal_id
        JOIN user_info u ON n.uid = u.user_id
        WHERE a.species = tur AND myID <> u.user_id
        AND NOT EXISTS (
            SELECT 1
            FROM application app
            WHERE app.nid = n.notice_id AND app.applicant_id = myID
        );
END;
$$ LANGUAGE 'plpgsql';
---------------------
--
CREATE OR REPLACE FUNCTION searchByTown(town VARCHAR(20), myID INTEGER) 
RETURNS SETOF tumtip AS $$
DECLARE
    ilan tumtip;
BEGIN
    RETURN QUERY 
        SELECT n.notice_id, n.aid, n.notice_date, n.sex, n.city, n.birth_date, n.breed, u.user_name, n.weight, a.species, n.animal_name
        FROM animal a
        JOIN notice_board n ON a.animal_id = n.aid
        JOIN user_info u ON u.user_id = n.uid
        WHERE u.city = town AND myID <> u.user_id
        EXCEPT
        SELECT n.notice_id, n.aid, n.notice_date, n.sex, n.city, n.birth_date, n.breed, u.user_name, n.weight, a.species, n.animal_name
        FROM notice_board n
        JOIN animal a ON n.aid = a.animal_id
        JOIN user_info u ON n.uid = u.user_id
        JOIN application app ON n.notice_id = app.nid
        WHERE app.applicant_id = myID
        GROUP BY n.notice_id, n.aid, n.notice_date, n.sex, n.city, n.birth_date, n.breed, u.user_name, n.weight, a.species, n.animal_name;
END;
$$ LANGUAGE 'plpgsql';
-----------------------------
--
CREATE OR REPLACE FUNCTION searchBySpeciesAndTown(tur VARCHAR(20), town VARCHAR(20), myID INTEGER) 
RETURNS SETOF tumtip AS $$
DECLARE
    ilan tumtip;
BEGIN
    RETURN QUERY	
        SELECT notice_id, aid, notice_date, sex, notice_board.city, notice_board.birth_date, breed, user_name,  weight, species, animal_name
        FROM notice_board
        JOIN animal ON aid = animal_id
        JOIN user_info ON uid = user_id
        WHERE species = tur AND myID <> user_id 
		
        INTERSECT
		
        SELECT notice_id, aid, notice_date,  sex, notice_board.city, notice_board.birth_date, breed, user_name,  weight, species, animal_name
        FROM notice_board
        JOIN animal ON aid = animal_id
        JOIN user_info ON uid = user_id
        WHERE town = notice_board.city AND myID <> user_id
        AND (notice_id, aid) NOT IN (
            SELECT n.notice_id, n.aid
            FROM notice_board n
            JOIN application app ON n.notice_id = app.nid
            WHERE app.applicant_id = myID
        );
END;
$$ LANGUAGE 'plpgsql';
-------------------------------
--
CREATE OR REPLACE FUNCTION listMyNotices(userid INT) 
RETURNS SETOF mynoticetip AS $$
DECLARE
    mynotice mynoticetip;
BEGIN
   	RETURN QUERY	
   		SELECT * 
		FROM notice_board
		WHERE uid=userid;

END;
$$ LANGUAGE 'plpgsql';
------------------------------
--
CREATE OR REPLACE FUNCTION applicantList(noticeid INT) 
RETURNS SETOF myapplicanttip AS $$
DECLARE
    myapplicant myapplicanttip;
BEGIN
   	RETURN QUERY	
   		SELECT DISTINCT use.city,use.f_name, use.l_name, use.user_name
		FROM notice_board noti,application app,user_info use
		WHERE app.nid=noticeid  and use.user_id=app.applicant_id ;

END;
$$ LANGUAGE 'plpgsql';
----------------------------------
--
CREATE OR REPLACE FUNCTION listAllProducts() 
RETURNS SETOF protip AS $$
DECLARE
    product_notice protip;
BEGIN
   	RETURN QUERY	
        SELECT *
        FROM products
		ORDER BY product_id;
END;
$$ LANGUAGE 'plpgsql';
--------------------------------
--
CREATE OR REPLACE FUNCTION searchProductByAnimal(aid int) 
RETURNS SETOF protip AS $$
DECLARE
    product_notice protip;
BEGIN
   	RETURN QUERY	
        SELECT *
        FROM products
		WHERE a_aid=aid ;
END;
$$ LANGUAGE 'plpgsql';
--------------------------------
--
CREATE OR REPLACE FUNCTION listMyApplications(userid INT) 
RETURNS SETOF mynoticetip AS $$
DECLARE
    mynotice mynoticetip;
BEGIN
   	RETURN QUERY	
   		SELECT notice_id, uid, aid, notice_date, animal_name, weight, sex, city, birth_date, breed 
		FROM notice_board
		join application on applicant_id = userid
		WHERE nid = notice_id;

END;
$$ LANGUAGE 'plpgsql';

SELECT * FROM notice_board;
/*CREATE OR REPLACE FUNCTION func2(town character varying)
    RETURNS bilgi[] AS$$
DECLARE
i integer;
bilgiler bilgi[];
curs cursor for SELECT COUNT(*) AS count, u.city, u.f_name, u.l_name
        FROM application app
        JOIN notice_board n ON app.nid = n.notice_id
        JOIN user_info u ON u.user_id = n.uid
        GROUP BY u.city, u.f_name, u.l_name
		HAVING town = u.city;
BEGIN
i:=0;
   FOR row_n in curs loop
    bilgiler[i]=row_n;
	i:=i+1;
	END LOOP;
	RETURN bilgiler;
END;
$$ LANGUAGE 'plpgsql'*/