# Micro Services POC

<!-- ABOUT THE PROJECT -->
## About The Project

This is a simple proof of concept (POC) to introduce and verify some challenges redarging Microservices Architecture.

![Macro Architecture Diagram](/images/macro-architecture.png)

The main concept within this project is to implement an enviroment with some microservices in a context of an inventory system, however for this first release there are only 3 microservices, **Catalog Microservice**, **Customer Microservice** and **Order Microservice**, each one with its own database instance and the inter-communication between them happens only through asyncronous events.

<!-- BUILT WITH -->
### Built With

This section should list any major frameworks/libraries used to bootstrap the project.

* [.NET 5](https://dotnet.microsoft.com/en-us/download/dotnet/5.0/)
* [Mongo](https://www.mongodb.com/en-us/)
* [RabbitMQ](https://www.rabbitmq.com/#getstarted)
* [MassTransit](https://masstransit-project.com/)
* [Docker](https://www.docker.com/get-started/)
* [Automapper](https://automapper.org/)
* [Xabaril/HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/)



<!-- GETTING STARTED -->
## Getting Started

We are using in this project a docker compose file in order to simplify the steps related with build and deploy all the microservices, as well as, the setup and exection all the infrastructure resources such as the databases and rabbitmq.

All the docker images are available in DockerHub, including the 3 microservices images are available into my [public personal repository](https://hub.docker.com/u/ggodoi1986).

``` sh
  docker pull ggodoi1986/mspoc.catalog.service  
  docker pull ggodoi1986/mspoc.customer.service
  docker pull ggodoi1986/mspoc.order.service
```

To begin, after cloning the repository, in the base path there is a **docker-compose.yml** file, just execute the following docker command.

```sh
  docker compose up -d --remove-orphans
```
After executing we are gonna see that all the containers have been started, created and they are now available to be accessed.

![docker compose up](/images/docker-compose-up.png)

![docker ps](/images/docker-ps.png)

## Catalog Microservice

Service responsible for the catalog items context, including all the catalog items management, such as enrollment, update and deletion.

![Catalog Microservice](/images/swagger-catalog-service.png)

### Events Related

| Syntax                      | Type          | Description                                                                   |
| --------------------------- | ------------- | ----------------------------------------------------------------------------- |
| **CatalogItemCreated**      | Published     | When a new catalog item is **created**                                        |
| **CatalogItemUpdated**      | Published     | When a new catalog item is **updated**                                        |
| **CatalogItemDeleted**      | Published     | When a new catalog item is **deleted**                                        |
| **CatalogItemLowQuantity**  | Published     | When a catalog item quantity reachs out the quantity of 10 units              |
| **OrderCreated**            | Consumed      | When a new order is **created**, the catalog item quantity is **subtracted**  |
| **OrderUpdated**            | Consumed      | When a new order is **update**, the catalog item quantity is **updated**      |
| **OrderDeleted**            | Consumed      | When a new order is **delete**, the catalog item quantity is **incremented**  |


### Catalog Microservice

Service responsible for the customer context, including all the customers management, such as enrollment, update and deletion.

![Customer Microservice](/images/swagger-customer-service.png)

### Order Microservice

Service responsible for the order context, including all the orders management, such as enrollment, update and deletion.

![Order Microservice](/images/swagger-order-service.png)
