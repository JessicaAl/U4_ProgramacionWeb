create database bd_roles;
use bd_roles;

create table Director(
IdDire int primary key auto_increment not null,
NumControl int not null,
Nombre varchar(30) not null,
DireContra varchar(200) not null comment 'dire7.1G' 
);

create table Maestro(
IdMaestro int primary key auto_increment not null,
NumControl int not null,
Nombre varchar(30) not null,
MaesContra varchar(200) not null,
Activo bit(1) default 1
);

create table Alumno(
IdAlumno int primary key auto_increment not null,
NoControl varchar(8) not null,
Nombre varchar(30) not null,
MaesId int not null,
constraint fk_IdMaestro foreign key(MaesId) references Maestro(IdMaestro)
);

insert into Director(NumControl, Nombre, DireContra) values (2908,'Jessica Alarcon', 
'C356E1300C855E4B5AF401B2B5AE0CB672493F5AF04D227E2325C77D0528FA06');