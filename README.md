# Developer Evaluation Project - Sales API

This repository contains the implementation of the DeveloperStore sales records API.

The solution was implemented on top of the provided .NET template, following a layered architecture with DDD concepts, MediatR, Entity Framework Core and PostgreSQL.

## Implemented Use Case

The API provides a complete CRUD for sales records and supports the following information:

- Sale number
- Sale date
- Customer external identity and denormalized customer name
- Branch external identity and denormalized branch name
- Products external identity and denormalized product name
- Quantity, unit price, discount and item total
- Sale total amount
- Cancelled / not cancelled status for sales and items

The Customer, Branch and Product references use the **External Identities** pattern: the API stores the external id and the denormalized description/name instead of creating foreign keys to other domain aggregates.

## Business Rules

Quantity-based discounts are enforced in the domain layer:

| Quantity | Discount |
| --- | --- |
| 1 to 3 identical items | 0% |
| 4 to 9 identical items | 10% |
| 10 to 20 identical items | 20% |
| Above 20 identical items | Not allowed |

The sale total is calculated from the non-cancelled items.

## Domain Events

The project includes MediatR notifications for the requested sales events:

- `SaleCreated`
- `SaleModified`
- `SaleCancelled`
- `ItemCancelled`

No message broker is required. The current implementation logs these events in the application log.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- MediatR
- FluentValidation
- AutoMapper from the base template
- xUnit / FluentAssertions / NSubstitute
- Docker Compose
- Swagger/OpenAPI

## Project Structure

```text
.template/backend
├── src
│   ├── Ambev.DeveloperEvaluation.Domain
│   │   ├── Entities
│   │   ├── Events
│   │   └── Repositories
│   ├── Ambev.DeveloperEvaluation.Application
│   │   └── Sales
│   ├── Ambev.DeveloperEvaluation.ORM
│   │   ├── Mapping
│   │   ├── Migrations
│   │   └── Repositories
│   ├── Ambev.DeveloperEvaluation.IoC
│   └── Ambev.DeveloperEvaluation.WebApi
│       └── Features/Sales
└── tests
    └── Ambev.DeveloperEvaluation.Unit
```

## How to Run with Docker

From the backend folder:

```bash
cd template/backend
docker compose up --build
```

The API will be available at:

```text
http://localhost:8080/swagger
```

The PostgreSQL database is exposed locally at port `5432`.

## How to Run Locally

Requirements:

- .NET 8 SDK
- PostgreSQL

Start PostgreSQL using Docker:

```bash
cd template/backend
docker compose up -d ambev.developerevaluation.database
```

Restore, build and run the API:

```bash
dotnet restore Ambev.DeveloperEvaluation.sln
dotnet build Ambev.DeveloperEvaluation.sln
dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Swagger will be available at the URL shown in the terminal, usually:

```text
https://localhost:7181/swagger
```

## How to Run Tests

From the backend folder:

```bash
cd template/backend
dotnet test Ambev.DeveloperEvaluation.sln
```

To generate the coverage report using the existing template scripts:

```bash
./coverage-report.sh
```

On Windows:

```bat
coverage-report.bat
```

## Sales API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/sales` | Creates a sale |
| GET | `/api/sales` | Lists sales |
| GET | `/api/sales/{id}` | Gets a sale by id |
| PUT | `/api/sales/{id}` | Updates a sale |
| DELETE | `/api/sales/{id}` | Deletes a sale |
| PATCH | `/api/sales/{id}/cancel` | Cancels a sale |
| PATCH | `/api/sales/{saleId}/items/{itemId}/cancel` | Cancels one sale item |

## Create Sale Example

```json
{
  "saleNumber": "SALE-0001",
  "saleDate": "2026-05-24T10:00:00Z",
  "customerId": "4b7f9c70-5db4-4a4f-babc-77e3a450d123",
  "customerName": "John Doe",
  "branchId": "d3d2e5b8-7c95-4f7a-91ff-c84eaa502111",
  "branchName": "Main Branch",
  "items": [
    {
      "productId": "80b31a21-2d42-4119-91c7-112233445566",
      "productName": "Keyboard",
      "quantity": 10,
      "unitPrice": 100
    }
  ]
}
```

Expected calculated values:

```text
Discount percentage: 20%
Discount amount: 200
Item total: 800
Sale total: 800
```

## Notes

- Discount calculation is not accepted from the request body. It is calculated by the domain model.
- Quantities above 20 throw a business exception.
- Cancelling an item recalculates the sale total.
- Cancelling a sale cancels all its items and sets the sale total to zero.
