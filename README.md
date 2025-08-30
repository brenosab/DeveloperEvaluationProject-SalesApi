# DeveloperEvaluationProject-SalesApi

As a developer on the DeveloperStore team, I implementated the API prototypes. Since we work with `DDD`, to reference entities from other domains we use the `External Identities` pattern with denormalization of entity descriptions. I developed an API (complete CRUD) that handles sales records.


---

## 🚀 Setup & Configuration

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- [PostgreSQL](https://www.postgresql.org/) (can run via Docker/WSL2)  
- [Docker & Docker Compose](https://docs.docker.com/get-docker/) (optional, for containerized execution)  

---

### 1️⃣ Database Setup

#### Option A: Run PostgreSQL via WSL2 (recommended)
1. Start your Postgres container in WSL2 with the correct exposed port:
   ```bash
   docker run --name sales-postgres -e POSTGRES_USER=user -e POSTGRES_PASSWORD=password -e POSTGRES_DB=developer_evaluation -p 58463:5432 -d postgres

2. Update your appsettings.Development.json (already configured by default):
   ```bash
   "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=58463;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"}

Option B: Run PostgreSQL with Docker Compose
If you want both API + DB inside Docker:
   ```bash
    docker-compose up --build
   ```

Option C: Run PostgreSQL locally
 - Install PostgreSQL on your machine.
 - Make sure the credentials and port match those defined in appsettings.Development.json.

2️⃣ Database Migrations

Apply EF Core migrations to create/update tables:
   ```bash
    dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM
   ```

3️⃣ Running the API
   ```bash
    Run via .NET CLI
    dotnet build
    dotnet run --project src/SalesApi
  ```
Run via IIS Express (local)
 - Open solution in Visual Studio
 - Select IIS Express and press F5

Run via Docker Compose
   ```bash
    docker-compose up --build
  ```

4️⃣ API Documentation & Testing

The API includes Swagger for exploration and testing.

Once running, access:

👉 http://localhost:5000/swagger

👉 or https://localhost:5001/swagger
 if using HTTPS

5️⃣ Running Tests

To run automated tests:
   ```bash
  dotnet test
  ```


⚙️ Environment Variables

Minimal required variables (already set in appsettings.Development.json):
   ```bash
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=58463;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"}
  ```


You may override them using environment variables if deploying to other environments.

✅ With these steps, you can:

 - Run the project locally (IIS Express / .NET CLI)
 - Use PostgreSQL in WSL2 or Docker Compose
 - Apply EF Core migrations
 - Explore/test the API through Swagger
 - Run tests with dotnet test
