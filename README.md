# FileHub Storage Microservice

This project is a **storage microservice** designed to handle **file uploads**, **metadata management**, and **file storage** using:

- **MinIO** (S3-compatible Object Storage)
- **SQL Server / SQLite / In-Memory Database** for metadata storage
- **.NET 9 Web API**
- **Clean Code** and **Best Practices**

## Features

- **File Upload**: Save file content to **MinIO** and metadata to a database.
- **File Management**: Download and delete files from **MinIO**.
- **Flexible Database**: Supports **SQL Server**, **SQLite**, and **In-Memory** databases.
- **Enum Handling**: Uses enums for file types (e.g., `Image`, `Video`, etc.) and converts to strings.
- **Dynamic Database Configuration**: Easily switch between different database providers (SQL Server, SQLite, In-Memory).

## Project Structure

| Layer               | Description                                |
|---------------------|--------------------------------------------|
| **Controllers**      | Handles HTTP requests.                    |
| **Services**         | Contains business logic (file upload, download, delete). |
| **Repositories**     | Handles database operations (saving metadata). |
| **Data**             | Includes `DbContext` and models for file metadata. |
| **Enums**            | Enum definitions for file types (`FileType`). |
| **DTOs**             | Data Transfer Objects for API communication. |
| **Helpers**          | MinIO client setup and utility functions. |

---

## Setup Instructions

### 1. Clone the Repository

Clone the repository and navigate to the project folder.

```bash
git clone https://github.com/your-username/filehub-microservice.git
cd filehub-microservice
