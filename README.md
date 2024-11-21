
# .NET Invoicing Application

## Architecture Overview

The application is structured into four main layers:

1. **API**: Contains minimal API endpoints for interacting with the system.
2. **Application**: Contains application logic, including service interfaces and the `InvoiceService` for handling domain logic.
3. **Infrastructure**: Contains an SQL Server repository, Dapper for data access, and custom `SqlContext` to wrap Dapper operations.
4. **Core**: Contains domain models, error models, and result models.

## Features

- **Invoice Endpoints**:
    - `POST /invoice`: Allows posting new invoices (requires role-based authorization).
    - `GET /invoice/sent`: Fetches invoices (authentication required, but not role-based).
    - `GET /invoice/received`: Fetches invoices (authentication required, but not role-based).

- **Token Generation**:
    - `GET /demotoken?companyId={companyId}`: A demo endpoint to generate a token for authentication (not secure or production-ready, for demo purposes only). 
    - Use company ID `39f87698-e181-4c5f-b2ba-b45b551e9e50` as it has pre-set invoices for both sent and received.

## Running the Application

### Prerequisites

- Docker (to run the application and database)
- Docker Compose (to manage the containers)

### Setup

1. Clone this repository:
   ```bash
   git clone https://github.com/tolhc/InvoicingApp.git
   ```

2. Navigate to the project directory:
   ```bash
   cd InvoicingApp
   ```

3. The `docker-compose.yml` file contains the necessary services:
   - **mssql**: The SQL Server database instance.
   - **mssql-init**: An image of `mssql-tools` used to initialize the database from `init-db.sql`.
   - **invoicing-api**: The application API running on port 5000.

   To start the services, run:
   ```bash
   docker-compose up -d
   ```

   This will:
    - Start a SQL Server container.
    - Use the `db-scripts/init-db.sql` file to initialize the database.
    - Start the invoicing API.

### Example Usage

1. **Get a token**:
    - Navigate to `GET /demotoken?companyId=39f87698-e181-4c5f-b2ba-b45b551e9e50` to get a demo token.
    - Example curl request:
   ```bash
   curl "http://localhost:5000/demotoken?companyId=39f87698-e181-4c5f-b2ba-b45b551e9e50"
   ```

2. **Post an invoice**:
    - Send a `POST` request to `/invoice` with the Authorization header and invoice details.

   - Example `curl` for posting an invoice:
   ```bash
   curl -X POST "http://localhost:5000/invoice" -H "Authorization: Bearer <your-token>" -d "{
        "date_issued": "2024-11-21T00:00:00",
        "net_amount": 4.2,
        "vat_amount": 0.5,
        "total_amount": 4.7,
        "description": "Invoice Description from Company 2 to Company 1",
        "company_id": "39f87698-e181-4c5f-b2ba-b45b551e9e50",
        "counter_party_company_id": "79e45fc1-902e-4f7c-9a3a-6590ea047710"
    }"
   ```

3. **Get Sent invoices**:
    - Send a `GET` request to `/invoice/sent` with the Authorization header.

   - Example `curl`:
   ```bash
   curl -X GET "http://localhost:5000/invoice/sent" -H "Authorization: Bearer <your-token>"
   ```

4. **Get Received invoices**:
    - Send a `GET` request to `/invoice/received` with the Authorization header.

   - Example `curl`:
   ```bash
   curl -X GET "http://localhost:5000/invoice/received" -H "Authorization: Bearer <your-token>"
   ```

### Testing

The application includes unit tests for both the Application and Infrastructure layers, as well as functional tests for the API.

1. **Application Unit Tests**: Tests the application logic
2. **Infrastructure Unit Tests**: Tests the repository logic
3. **API Functional Tests**: Tests the API endpoints with a mocked `SqlContext` to simulate database operations.

To run the tests, execute the following command from the project directory:
```bash
dotnet test
```

