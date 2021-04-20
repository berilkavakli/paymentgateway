
  
  

# Payment Gateway

  

This project includes a .net core 3.1 based cross-platform payment gateway service which imitates payment system. It exposes */api/payment* and */api/payment/{paymentId}* endpoints for issuing and viewing payment operations respectively. When a new request received by */api/payment* endpoint, the following operations are performed successively:

  

- First, provided API-KEY in the request is validated. The payment gateway endpoint calls are restricted by using API-KEY validation. Currently a static value is used for API-KEY validation however for production usage; the consumers of the payment gateway should register themselves, get unique API-KEYs and use them for payment gateway communication.

  

- The request parameters such as credit card number, cvv, expiry month, expiry year, amount and currency are validated by FluentValidation framework.

  

- The system analyses the credit card number, decides the appropriate bank service and navigate payment request to decided one. To simulate bank services; 2 sample micro-service (BankServiceA and BankServiceB) have been added to the project. Those sample services expose a single endpoint namely */api/payment* and accept payments. For the payment scenario; the payment requests whose credit card number starts with 1111... are sent to BankServiceA and the rest of them are sent to BankServiceB. The factory design pattern is being used (BankServiceFactory.cs) for deciding appropriate bank service and instantiating its client (an instance of IBankService). So new BankServices clients and rules can easily be added to system.

  

 - To imitate a real bank service behaviours, different payment constraints have been added sample bank services.
	 - BankServiceA only approves the payments whose amount lower than 1000
	 - BankServiceB only approves the payments whose amount lower than 2000

  

- When a payment request is approved by an appropriate bank service, it returns a unique paymentId, success status and message, then the payment gateway stores approved payment information along with paymentId to its PostgreSQL database. In case the payment is not approved, the payment gateway service store it db with failed status info.


- Previously processed payments could also be fetched by using */api/payment/{paymentId}* endpoint of the payment gateway. It simply returns payment infomation of provided paymentId with masked credit card number.

 
The whole components of the project were containerized. The PaymentGateway, BankServiceA, BankService and the PostgreSQL can be run easily via docker-compose, please refer to 2. Building and Running The System for more information.

  
  

## Project structure

  

The project has the following structure.

  

```
├── readme.md # This file
├── bulld.cmd # builds PaymentGateway, BankServiceA and BankServiceB images (for Windows)
├── docker-compose.yaml # yaml file for api and db container definitions
├── PaymentGateway.dockerfile # Dockerfile of PaymentGateway service
├── BankServiceA.dockerfile   # Dockerfile of sample BankServiceA 
├── BankServiceB.dockerfile   # Dockerfile of sample BankServiceB
├── PaymentGateway # Includes PaymentGateway project source codes
│ ├── ...
├── PaymentGateway.Contract # Includes common data contracts to ease sample bank service calls
│ ├── ...
├── PaymentGateway.Test # Includes PaymentGateway project test codes
│ ├── ...
├── BankServiceA # Includes BankServiceA project source codes
│ ├── ...
├── BankServiceB # Includes BankServiceB project source codes
│ ├── ...

```

  

## How to run

### 1. Installing Dependencies

For the sake of portability, the project was designed to be run on docker environment and to run PaymentGateway, BankServiceA, BankServiceB along with PostgreSQL, a docker-compose file has been prepared. So the `docker` and `docker-compose` utilities should be installed on your system. The service has been tested on *Docker version 20.10.5, build 55c4c88* and *docker-compose version 1.28.5, build c4eb3a1f*.

  

### 2. Building and Running The System

The docker images of the PaymentGateway, BankServiceA and BankServiceB can be built by running the following commands.

  

You may use build script:

```shell
> ./build.cmd
```



Or run following commands;

```shell
> docker build -t paymentgateway -f PaymentGateway.dockerfile .
> docker build -t bankservicea -f BankServiceA.dockerfile .
> docker build -t bankserviceb -f BankServiceB.dockerfile .
```

  

To run PaymentGateway, BankServiceA, BankServiceB and PostgreSQL containers:

```shell
> docker-compose up
```

  

The payment gateway and the other services could also be run on local environment, to this end .NET Core 3.1 SDK (v3.1.408) and PostgreSQL should be installed on your system. Then BankServiceAUrl, BankServiceBUrl, PaymentConnectionString parameters should be modified accordingly.

```shell
"AppSettings": {
"BankServiceAUrl": "http://bank-service-a/api/",
"BankServiceBUrl": "http://bank-service-b/api/"
},
...
"ConnectionStrings": {
"PaymentConnectionString": "Server=db;Port=5432;Database=PAYMENTGATEWAY;User Id=postgres;Password=...;"
},
```

  

### 3. Accessing the Swagger UI

The service has Swagger documentation and a built-in Swagger-UI, so the payment endpoints can also be tested via its Swagger UI.

  

PaymentGateway Service Swagger UI (If it runs on Docker (via docker-compose)):

http://localhost:7777/

  

PaymentGateway Service Swagger UI (If it runs on locally (via Kestrel)):

http://localhost:5001/

  

For endpoint calls; 'securekey' value could be used as ApiKey header.

  

### 4. Accessing Service Logs

The payment gateway service was configured to use console log sink for ease of use, so the service logs could be monitored via docker container logs. For logging nlog is implemented.

  

```shell
> docker logs -tf paymentgateway_payment-gateway_1
```

  

## Unit and Integration Tests


The project includes unit tests about various functionalities. To run unit tests locally, the .NET Core 3.1 SDK (v3.1.408) should be installed on your system, unless you may install the compatible SDK (Linux, Mac-OS, or Windows) by using the following link.

https://dotnet.microsoft.com/download/dotnet/3.1

  
Unit tests are written for methods in PaymentController and PaymentService. For simulating other layers, Moq framework is used in the project.

  

To run unit tests;

```shell
> dotnet test
```

  

## Extensibility

- The project takes the advantages of Inversion of Control (IoC) pattern for various parts such as gateway implementation, logging system and data access layer. Thus new logging implementations, data access layer could be added to system by changing service dependecy injection parts of Startup.cs

  

- Payment gateway takes the advantages of Factory design pattern for deciding and creating appropriate bank service clients. Thanks to this approach, new decision rules and Bank Services could easily be integrated with the system.

  

## Other Constraints & Assumptions

- The ApiKey value for payment gateway endpoints is static ('securekey') however for actual production usage; the consumers of the payment gateway should register themselves, get unique API-KEYs and use them for payment gateway communication.

  

- Credit card format validation is performed by using FluentValidation frameworks. However to ease system tests, the credit card format validation was disabled (Line: 16 in CardValidator.cs)

## Potential Improvements

The scope of the project was kept limited due to time constraints, so there are plenty rooms for various improvements such as;

  

- Sensitive values such as connection strings, usernames, passwords, ApiKeys are currently being kept in app.settings config file. However, for production usage, they should be store as *secrets* (for Docker Swarm and Kubernetes).

  

- Unit test count might be improved and also can be added integration tests.

  

- CI pipeline could be created on BitBucket (or any other alternative) and the unit test could automatically be run for each commit.

  

- For better error troubleshooting; the 3rd party log data collectors such as Logstash (ELK) or FluentD could be integrated with the system.
- Database and tables are created in the startup.cs by reading TableScripts.sql. Intead of this, a data migration tool like FluentMigration could be used.
