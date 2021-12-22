create table if not exists tb_address
(
	addressid int not null,
	street varchar(250) not null,
	number varchar(8) null,
	complement varchar(20) null,
	district varchar(30) not null,
	city varchar(30) not null,
	state char(2) not null,
	postalcode char(8) not null
);

create table if not exists tb_phone
(
	phoneid int not null,
	type int not null,
	code char(2) not null,
	number varchar(9) not null
);

create table if not exists tb_person
(
	personid int not null,
	name varchar(80) not null,
	birthdate date not null,
	document char(11) not null,
	email varchar(150) not null,
	gender int not null,
	addressid int null,
	mainphoneid int not null
);

create table if not exists tb_person_phone
(
	personphoneid int not null primary key auto_increment,
	personid int not null,
	phoneid int not null
);