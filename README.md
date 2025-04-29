# StorageHub Microservice

This project is a **storage microservice** designed to handle **file uploads**, **metadata management**, and **file storage** using:

- **MinIO** (S3-compatible Object Storage)
- **SQL Server / SQLite / In-Memory Database** for metadata storage
- **.NET 9 Core Web API**
- **Clean Code** and **Best Practices**
- Using **Swagger UI** for testing APT and JWT 

## Features

- **File Upload**: Save file content to **MinIO** and metadata to a database.
- **File Management**: Download and delete files from **MinIO**.
- **Flexible Database**: Supports **SQL Server**, **SQLite**, and **In-Memory** databases.
- **Enum Handling**: Uses enums for storage types (e.g., `SQLite`, `SQL Server`, `In-Memory`) and converts to strings.
- **Dynamic Database Configuration**: Easily switch between different database providers (SQL Server, SQLite, In-Memory).
- Docker-ready (for MinIO)
- Easily switch DB providers via config -- InMemory is the Default

## Project Structure

| Layer               | Description                                |
|---------------------|--------------------------------------------|
| **API**      | Handles HTTP requests.                    |
| **Application**         | Contains business logic (file upload, download, delete), DTOs, AutoMapper. |
| **Infrastructure**     | Handles database operations (saving metadata). |
|             | Includes `DbContext` and models for file metadata. |
|        | Helpers -- MinIO client setup and utility functions. |
| **Domain**          | Entity represents tables in the database. |

---



## Setup Instructions

### 1. Clone the Repository

Clone the repository and navigate to the project folder.

```bash
git clone https://github.com/your-username/filehub-microservice.git
cd filehub-microservice

```
### 2. Set Up MinIO Using Docker (Free)
The application supports setting up Minio at runtime if it does not exist

- **Console**: https://play.min.io
- **Login**: minioadmin / minioadmin
- **Create a bucket named**: filehub

For the manual, this step
```bash
docker run -p 9000:9000 -p 9001:9001 --name minio \
-e MINIO_ROOT_USER=minioadmin \
-e MINIO_ROOT_PASSWORD=minioadmin \
-v D:\minio-data:/data \
minio/minio server /data --console-address ":9001"

```

### 3. Configure the Application
**Login Credential in Swagger**
```json
  "LoginSettings": {
    "UserName": "admin",
    "Password": "admin"
  },
```
**Database Connection (InMemory is used)**
Change the provider value to switch to another Database
```json
 "DatabaseSettings": {
   "Provider": "InMemory", //"SQLite" "SQLServer", "InMemory"
   "InMemory": "InMemoryDb"
 },
 "ConnectionStrings": {
   "SQLite": "Data Source=StorageHubDb.db",
   "SQLServer": "Server=.;Database=StorageHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
 },

```
**MinioSettings**
```json
 "MinioSettings": {
   "Endpoint": "https://play.min.io", //"localhost:9000",
   "AccessKey": "minioadmin",
   "SecretKey": "minioadmin",
   "BucketName": "storage-hub-bucket",
   "UseSSL": false
 },
```

### 4. Install Required NuGet Packages
Open Package Manager Console and run (if one of them is missing):
```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package Microsoft.EntityFrameworkCore.InMemory
Install-Package AWSSDK.S3
Install-Package Minio
Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection
```

### 5. Apply Database Migrations (Skip if using InMemory)
The initial migration is added to the solution, and the migration is running at runtime.

For manual step :
```powershell
Add-Migration InitialCreate
Update-Database
```
### 6. Run the Application
If using IISExpress ==>  "https://localhost:44301/swagger/index.html"

If using https ==> https://localhost:7134/swagger/index.html

For manual step :
```powershell
Add-Migration InitialCreate
Update-Database
```

