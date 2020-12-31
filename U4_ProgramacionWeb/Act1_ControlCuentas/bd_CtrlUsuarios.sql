create database ctrlusers;
use ctrlusers;
create table Usuario(
Id int primary key auto_increment not null,
nomUsuario varchar(30) not null,
Correo varchar(200) not null,
Contra varchar(200) not null,
Activo bit(1) default 0,
Codigo int not null
);
