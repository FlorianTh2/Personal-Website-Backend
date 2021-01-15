# Personal-Website-Backend

Repository to create a Backend for my [Frontend].

## Prerequisites
 - .net 5.0
 - running postgres-database (docker installation showed down below)

## Getting Started

 - Install mock-database
 ```
    $ docker run -p 5432:5432 \
        --name some-postgres \
        -e POSTGRES_PASSWORD=Florian1234 \
        -d \
        --rm \
        postgres
 ```

 - Clone the repository

 ```
    $ git clone git@github.com:FlorianTh2/Personal-Website-Backend.git`
 ```

 - Switch to directory
 
 ```
    $ cd ./Personal-Website-Backend
 ```

 - Install of all dependencies
 
 ```
    $ dotnet restore
 ```

 - Run application
 
 ```
   $ dotnet run --project .\Personal-Website-Backend
 ```

 - Open swagger
 
 ```
    https://localhost:5001/swagger/index.html
 ``` 
 
 

## Important commands
 - [optional] Add support for redis (needs minor flag-setting-changes)
 
 ```
    $ docker run -p 6379:6379 redis
 ```

 - Run tests
  
 ```
    dotnet test
 ```

## Build with
 - .net 5
 - entity framework 5
 - automapper
 - postgres
 - xunit
 - swagger (openApi)
 - fluentAssertions

## Acknowledgements
--


   [Frontend]: https://github.com/FlorianTh2/Personal-Website