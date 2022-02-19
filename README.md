<!-- TABLE OF CONTENTS -->
## Table of Contents
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li><a href="#getting-started">Getting Started</a></li>
    <li>
      <a href="#catalog-microservice">Catalog Microservice</a>
      <ul>
        <li><a href="#catalog-events">Catalog Events</a></li>
        <li><a href="#catalog-events-diagram">Catalog Events Diagram</a></li>
      </ul>
    </li>
    <li><a href="#customer-microservice">Customer Microservice</a>
      <ul>
        <li><a href="#customer-events">Customer Events</a></li>
        <li><a href="#customer-events-diagram">Customer Events Diagram</a></li>        
      </ul>
    </li>
    <li><a href="#order-microservice">Order Microservice</a>
      <ul>
        <li><a href="#order-events">Order Events</a></li>
        <li><a href="#order-events-diagram">Order Events Diagram</a></li>        
      </ul>    
    </li>
  </ol>


&nbsp;
<!--------------------------------------------------------------------------------------------------------->
<!-- ABOUT THE PROJECT ------------------------------------------------------------------------------------>
<!--------------------------------------------------------------------------------------------------------->
## About The Project

This is a simple proof of concept (POC) to introduce and verify some challenges redarging Microservices Architecture.

![Macro Architecture Diagram](/images/macro-architecture.png)

The main concept within this project is to implement an enviroment with some microservices in a context of an inventory system, however for this first release there are only 3 microservices, **Catalog Microservice**, **Customer Microservice** and **Order Microservice**, each one with its own database instance and the inter-communication between them happens only through asyncronous events.



<!--------------------------------------------------------------------------------------------------------->
<!-- BUILT WITH ------------------------------------------------------------------------------------------->
<!--------------------------------------------------------------------------------------------------------->
### Built With

This section should list any major frameworks/libraries used to bootstrap the project.

* [.NET 5](https://dotnet.microsoft.com/en-us/download/dotnet/5.0/)
* [Mongo](https://www.mongodb.com/en-us/)
* [RabbitMQ](https://www.rabbitmq.com/#getstarted)
* [MassTransit](https://masstransit-project.com/)
* [Docker](https://www.docker.com/get-started/)
* [Automapper](https://automapper.org/)
* [Xabaril/HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/)


&nbsp;
<!--------------------------------------------------------------------------------------------------------->
<!-- GETTING STARTED -------------------------------------------------------------------------------------->
<!--------------------------------------------------------------------------------------------------------->
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


&nbsp;
<!--------------------------------------------------------------------------------------------------------->
<!--- CATALOG MICROSERVICE -------------------------------------------------------------------------------->
<!--------------------------------------------------------------------------------------------------------->
## Catalog Microservice

Service responsible for the catalog items context, including all the catalog items management, such as enrollment, update and deletion.

![Catalog Microservice](/images/swagger-catalog-service.png)

### Catalog Events

This microservice uses the **MassTransit** lib in order to implement the [pub/sub pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/publisher-subscriber) for the releated events, accordingly the official documentation available in the following links :

* [MassTransit Publisher](https://masstransit-project.com/usage/producers.html#publish)
* [MassTransit Consumer](https://masstransit-project.com/usage/consumers.html#consumers)

| Syntax                      | Type          | Description                                                                   |
| --------------------------- | ------------- | ----------------------------------------------------------------------------- |
| **CatalogItemCreated**      | Published     | When a new catalog item is **created**                                        |
| **CatalogItemUpdated**      | Published     | When a catalog item is **updated**                                            |
| **CatalogItemDeleted**      | Published     | When a catalog item is **deleted**                                            |
| **CatalogItemLowQuantity**  | Published     | When a catalog item quantity reachs out the quantity of 10 units              |
| **OrderCreated**            | Consumed      | When a new order is **created**, the catalog item quantity is **subtracted**  |
| **OrderUpdated**            | Consumed      | When an order is **update**, the catalog item quantity is **updated**         |
| **OrderRemoved**            | Consumed      | When an order is **delete**, the catalog item quantity is **incremented**     |

### Catalog Events Diagram

Accordingly the [RabbitMQ official documentation](https://www.rabbitmq.com/tutorials/amqp-concepts.html#exchanges), a message is never sent directly to a queue, it is sent to an exchange instead, which is a routing object, thus there is the **Primary Exchange**.

In order to implement the **pub/sub pattern** MassTransit creates the **Secondary Exchange**, which is bind to a destination queue, and there are individual queues for each published event and subscribed consumer.

![Catalog Events Diagram](/images/catalog-events-diagram.png)


&nbsp;
<!--------------------------------------------------------------------------------------------------------->
<!--- CUSTOMER MICROSERVICE ------------------------------------------------------------------------------->
<!--------------------------------------------------------------------------------------------------------->
## Customer Microservice

Service responsible for the customer context, including all the customers management, such as enrollment, update and deletion.

![Customer Microservice](/images/swagger-customer-service.png)

### Customer Events

This microservice uses the **MassTransit** lib in order to implement the [pub/sub pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/publisher-subscriber) for the releated events, accordingly the official documentation available in the following links :

* [MassTransit Publisher](https://masstransit-project.com/usage/producers.html#publish)
* [MassTransit Consumer](https://masstransit-project.com/usage/consumers.html#consumers)

| Syntax                      | Type          | Description                                                                   |
| --------------------------- | ------------- | ----------------------------------------------------------------------------- |
| **CustomerCreated**         | Published     | When a new customer is **created**                                            |
| **CustomerUpdated**         | Published     | When a customer is **updated**                                                |
| **CustomerDeleted**         | Published     | When a customer is **deleted**                                                |
| **OrderCreated**            | Consumed      | When a new order is **created**, the customer order history is **created**    |
| **OrderUpdated**            | Consumed      | When an order is **update**, the customer order history is **updated**        |
| **OrderDeleted**            | Consumed      | When an order is **delete**, the customer order history is **deleted**        |

### Customer Events Diagram

Accordingly the [RabbitMQ official documentation](https://www.rabbitmq.com/tutorials/amqp-concepts.html#exchanges), a message is never sent directly to a queue, it is sent to an exchange instead, which is a routing object, thus there is the **Primary Exchange**.

In order to implement the **pub/sub pattern** MassTransit creates the **Secondary Exchange**, which is bind to a destination queue, and there are individual queues for each published event and subscribed consumer.

![Customer Events Diagram](/images/customer-events-diagram.png)


&nbsp;
<!--------------------------------------------------------------------------------------------------------->
<!--- ORDER MICROSERVICE ---------------------------------------------------------------------------------->
<!--------------------------------------------------------------------------------------------------------->
## Order Microservice

Service responsible for the order context, including all the orders management, such as enrollment, update and deletion.

![Order Microservice](/images/swagger-order-service.png)

### Order Events

This microservice uses the **MassTransit** lib in order to implement the [pub/sub pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/publisher-subscriber) for the releated events, accordingly the official documentation available in the following links :

* [MassTransit Publisher](https://masstransit-project.com/usage/producers.html#publish)
* [MassTransit Consumer](https://masstransit-project.com/usage/consumers.html#consumers)

| Syntax                      | Type          | Description                                                                       |
| --------------------------- | ------------- | --------------------------------------------------------------------------------- |
| **OrderCreated**            | Published     | When a new order is **created**                                                   |
| **OrderUpdated**            | Published     | When an order is **updated**                                                      |
| **OrderDeleted**            | Published     | When an order is **deleted**                                                      |
| **CatalogItemCreated**      | Consumed      | When a new catalog item is **created**, the local catalog registry is **created** |
| **CatalogItemUpdated**      | Consumed      | When a catalog item is **updated**, the local catalog registry is **updated**     |
| **CatalogItemDeleted**      | Consumed      | When a catalog item is **deleted**, the local catalog registry is **deleted**     |
| **CustomerCreated**         | Consumed      | When a new customer is **created**, the local customer registry is **created**    |
| **CustomerUpdated**         | Consumed      | When a customer is **updated**, the local customer registry is **updated**        |
| **CustomerDeleted**         | Consumed      | When a customer is **deleted**, the local customer registry is **deleted**        |

(*) Within this microservice we are using a database for catalog and customer, but the difference from the original ones, is that within the order context these entities are simplified, only storaging the minimal data consumed by order microservice.

### Order Events Diagram

Accordingly the [RabbitMQ official documentation](https://www.rabbitmq.com/tutorials/amqp-concepts.html#exchanges), a message is never sent directly to a queue, it is sent to an exchange instead, which is a routing object, thus there is the **Primary Exchange**.

In order to implement the **pub/sub pattern** MassTransit creates the **Secondary Exchange**, which is bind to a destination queue, and there are individual queues for each published event and subscribed consumer.

![Order Events Diagram](/images/order-events-diagram.png)



<!--------------------------------------------------------------------------------------------------------->
<!--- EVENTS AND QUEUES ARCHITECTURE ---------------------------------------------------------------------->
<!--------------------------------------------------------------------------------------------------------->
