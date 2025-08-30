# Technical Documentation - Sales API

## Endpoints

The API exposes REST endpoints for the Sales domain, including:
- Full CRUD for sales:
  - `POST /sales` - Create a sale
  - `GET /sales` - List sales with pagination
  - `GET /sales/{id}` - Retrieve a sale by ID
  - `PUT /sales/{id}` - Update a sale
  - `DELETE /sales/{id}` - Delete a sale

> Detailed information on requests/responses, parameters, and examples can be found directly in the Swagger UI when running the application.

## Architecture & Design

- **DDD (Domain-Driven Design):**
  - Clear separation between domain, application, infrastructure, and API layers.
  - Entities, aggregates, and business rules are centralized in the domain layer.
  - Use of Value Objects and External Identities for cross-domain integration.

- **MediatR:**
  - Implements the CQRS (Command Query Responsibility Segregation) pattern.
  - Separate handlers for each command/query improve maintainability and testability.

- **Base Repository:**
  - A generic repository class centralizes core persistence operations (CRUD, pagination, etc.) using Entity Framework Core.
  - Promotes inheritance and logic reuse across repositories, reducing duplication and simplifying interface implementation in the infrastructure layer.

- **EventDispatcher:**
  - Lightweight implementation for publishing domain events.
  - Can be easily extended for future integration with Message Brokers such as Kafka or RabbitMQ by adapting the `PublishAsync` method to publish events externally.

## Notes
- Full API documentation is available via Swagger.
- The project follows best practices for organization, separation of concerns, and extensibility for future scenarios.
