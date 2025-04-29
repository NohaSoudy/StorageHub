# StorageHub Microservice

This project is a **storage microservice** designed to handle **file uploads**, **metadata management**, and **file storage** using:

- **MinIO** (S3-compatible Object Storage)
- **SQL Server / SQLite / In-Memory Database** for metadata storage
- **.NET 9 Web API**
- **Clean Code** and **Best Practices**

## Features

- **File Upload**: Save file content to **MinIO** and metadata to a database.
- **File Management**: Download and delete files from **MinIO**.
- **Flexible Database**: Supports **SQL Server**, **SQLite**, and **In-Memory** databases.
- **Enum Handling**: Uses enums for storage types (e.g., `SQLite`, `SQL Server`, `In-Memory`) and converts to strings.
- **Dynamic Database Configuration**: Easily switch between different database providers (SQL Server, SQLite, In-Memory).

## Project Structure

| Layer               | Description                                |
|---------------------|--------------------------------------------|
| **API**      | Handles HTTP requests.                    |
| **Application**         | Contains business logic (file upload, download, delete), DTOs, AuotMapper. |
| **Infrastructure**     | Handles database operations (saving metadata). |
|             | Includes `DbContext` and models for file metadata. |
|        | Helpers -- MinIO client setup and utility functions. |
| **Domain**          | Entity represents tables in database. |

---

## Setup Instructions

### 1. Clone the Repository

Clone the repository and navigate to the project folder.

```bash
git clone https://github.com/your-username/filehub-microservice.git
cd filehub-microservice
