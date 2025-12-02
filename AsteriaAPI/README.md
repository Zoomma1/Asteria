# Installation Guide

## 1. Env Variable

Set your postgres password as an environment variable named `POSTGRES_PASSWORD` on your machine or on your project IDE
and set whatever password you want.

## 2. Download maven
Download maven from this [link](https://maven.apache.org/download.cgi) and ensure that maven is correctly installed on
your machine by running the command `mvn -v` in a terminal

## 3. Install Docker Desktop
Download and install docker desktop from this [link](https://www.docker.com/products/docker-desktop/).
Make sure that docker is correctly installed by running the command `docker -v` in a terminal

## 4. Run Postgres container
Open a terminal in the project directory and run the following command to start a postgres container
```bash
docker-compose up -d
```

## 5. Install dependencies
Open a terminal in the projects directory and run the command `mvn clean install -DskipTests`
and then `mvn clean package -DskipTests`


## 6. Run the project
You can now run the project from your IDE. To check if your project is running properly,
you can go to this url : `http://localhost:8080/AsteriaAPI/swagger-ui/index.html#/`

You should see the swagger documentation of the project.

# Project Structure

In this project you will find different packages

## 1. Config

This packages handles spring configurations and the part that is responsible for the
token generation and validation as well as the security configuration.

## 2. Controller

This package handles the different endpoints of the application. You can find the
different controllers that are responsible for the different endpoints of the application.
By default, you can find a UserController and an AuthController. There is also an Exception handler that intercepts
java **controller** exception and decides what to send back to the client.

## 3. DAL (Data Access Layer)

It contains the different repositories that are responsible for the communication with the database as well as
dto objects or enums that are used in the application. In the postgres package you can find the database entities.

## 4. DTO (Data Transfer Object)

This package contains the different dto objects that are used to transfer data between the client and the server.
You can find request and response dto objects in this package.

## 5. Mapper

This project uses Mapstruct to map the different entities to dto objects. You can find the different mappers in this package.

## 6. Service

You can find all the different services in this package. Usually you can find one
service per controller, repository or database entity, but it is not mandatory.

## 7. Resources

- Flyway :

You can find the different sql scripts that are used to create the database tables in this package.
The scripts are executed in the order of their names. If you modify the scripts, you have to change the name of the script for them
to be run properly.

- Application profiles :

You can find the different application profiles with their parameters inside.
By default, the project uses the `local` profile. You can change the run profile when you run your project.