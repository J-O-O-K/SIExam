# SIExam - Library Management Microservices

A library management system demonstrating microservices architecture with CQRS, Clean Architecture, RabbitMQ messaging, and Docker deployment.

### Architectural Patterns
- **Microservices**: Independent, loosely-coupled services with separate databases
- **Clean Architecture**: Domain-driven design with separation of concerns
- **CQRS**: Command Query Responsibility Segregation using MediatR
- **Event-Driven Architecture**: Asynchronous communication via RabbitMQ
- **API Gateway Pattern**: Single entry point with Ocelot routing and JWT validation

### Services
- **AuthService** (Port 5001): User registration and JWT authentication
- **CatalogService** (Port 5002): Book inventory management
- **LendingService** (Port 5003): Book borrowing and returning with event publishing
- **ApiGateway** (Port 8080): Request routing, JWT validation, and aggregated Swagger UI

### Service Communication
- **Synchronous**: REST APIs via API Gateway
- **Asynchronous**: RabbitMQ message broker
  - `book.borrowed` event: LendingService -> CatalogService
  - `book.returned` event: LendingService -> CatalogService

### Infrastructure
- **Databases**: SQL Server 2022 with 3 isolated databases (AuthDb, CatalogDb, LendingDb)
- **Message Broker**: RabbitMQ with durable queues and persistent messages
- **Containerization**: Docker Compose orchestration with auto-restart