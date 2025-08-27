<p align="center">
  <a href="https://github.com/pankaj085/LotusLedger">
    <img src="/assests/lotusledger.svg" alt="LotusLedger" width="150"/>
  </a>
</p>



# 🪷 LotusLedger - Product Inventory API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-12-4169E1?style=for-the-badge&logo=postgresql)
![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)
![Status](https://img.shields.io/badge/status-active-brightgreen.svg?style=for-the-badge)

</div>

A clean and robust **.NET 8 Web API** designed for modern product inventory management. Backed by **PostgreSQL**, LotusLedger provides a complete solution for CRUD operations, advanced filtering, pagination, and real-time stock monitoring, all wrapped in a well-structured and scalable architecture.

---

## ✨ Core Features

-   **Clean Architecture:** Follows best practices for separation of concerns, making the codebase maintainable and testable.
-   **Robust Validation:** Utilizes `FluentValidation` for clear and powerful input validation, returning proper HTTP status codes (`200`, `201`, `400`, `404`).
-   **Advanced Querying:**
    -   🔍 Full-text search on product **name** and **description**.
    -   🗂️ Filter products by **Category**.
    -   📑 Efficient **pagination** for all list-based endpoints.
    -   ↕️ **Sorting** capabilities.
-   **Smart Inventory Management:**
    -   ⚠️ **Low-stock alerts** to identify products needing replenishment (default threshold: quantity < 25).
    -   ♻️ **Soft Deletion** to safely deactivate products without permanent data loss.
-   **Comprehensive CRUD Operations:**
    -   `POST /products` - Add a new product.
    -   `GET /products` - Get all active products with filtering, searching, and pagination.
    -   `GET /products/{id}` - Get a single product by its ID.
    -   `PUT /products/{id}` - Update a product's details.
    -   `PATCH /products/{id}` - Partially modify a product (e.g., only update stock).
    -   `DELETE /products/{id}` - Soft delete a product.
-   **Utility Endpoints:**
    -   `GET /products/inactive` - Retrieve all soft-deleted products.
    -   `DELETE /products/inactive/{id}` - Permanently delete an inactive product.
    -   `POST /products/reactivate/{id}` - Restore a soft-deleted product.
    -   `GET /products/low-stock` - Get a list of all products below the stock threshold.

👉 For a complete list of endpoints, request/response models, and examples, please see the **[API Documentation](ENDPOINTS.md)**.

---

## 🚀 Tech Stack

| Component             | Technology                                                                                                  |
| --------------------- | ----------------------------------------------------------------------------------------------------------- |
| **Framework** | **.NET 8** (ASP.NET Core Web API)                                                                           |
| **Database** | **PostgreSQL** |
| **ORM** | **Entity Framework Core 8** with the `Npgsql` provider.                                                     |
| **API Documentation** | **Swagger (Swashbuckle)** for interactive API testing and documentation.                                    |
| **Object Mapping** | **AutoMapper** for clean and simple DTO-to-Entity mapping.                                                  |
| **Validation** | **FluentValidation** for expressive and decoupled validation logic.                                         |
| **Error Handling** | Custom **Exception Handling Middleware** to ensure consistent API error responses.                            |

---

## ⚙️ Getting Started

Follow these instructions to get a local copy of LotusLedger up and running on your machine.

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Git](https://git-scm.com/)
-   A running instance of [PostgreSQL](https://www.postgresql.org/download/)

### 1. Clone the Repository

```bash
git clone [https://github.com/your-username/LotusLedger.git](https://github.com/your-username/LotusLedger.git)
cd LotusLedger
````

### 2\. Install NuGet Packages

This project relies on several NuGet packages. Run the following commands to install them:

```bash
# EF Core & PostgreSQL Provider
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.8
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.8
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.8
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.8

# API & Utilities
dotnet add package Microsoft.AspNetCore.OpenApi --version 8.0.8
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add package FluentValidation.AspNetCore --version 11.3.0
```

### 3\. Configure the Database

Open `appsettings.Development.json` and update the `DefaultConnection` string with your PostgreSQL credentials.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=lotus_ledger_db;Username=your_postgres_user;Password=your_strong_password"
  }
}
```

### 4\. Apply Database Migrations

Use the `dotnet-ef` tool to create and apply the database schema.

```bash
# Create the initial migration files based on your DbContext and Models
dotnet ef migrations add InitialCreate

# Apply the migration to your database
dotnet ef database update
```

### 5\. Run the Application

You're all set\! Build and run the project.

```bash
dotnet build
dotnet run
```

The API will be available at `http://localhost:5000` (or a similar port).

  - **API Base URL:** `http://localhost:5000`
  - **Swagger UI:** `http://localhost:5000/swagger`

-----

## 📂 Project Structure

The project follows a clean and logical structure to promote separation of concerns.

```plaintext
LotusLedger/
│
├── AppDataContext/           # DbContext for EF Core
│   └── AppDbContext.cs
│
├── Contracts/                # DTOs and API request/response models
│   ├── ProductDto.cs
│   ├── CreateProductDto.cs
│   └── ...
│
├── Controllers/              # API controllers (the entry points)
│   └── ProductController.cs
│
├── Interfaces/               # Service and repository interfaces
│   └── IProductService.cs
│
├── Services/                 # Business logic implementations
│   └── ProductService.cs
│
├── MappingProfiles/          # AutoMapper configuration
│   └── ProductMappingProfile.cs
│
├── Middleware/               # Custom middlewares (e.g., error handling)
│   └── ExceptionMiddleware.cs
│
├── Models/                   # Database entity models
│   └── Product.cs
│
├── appsettings.json          # Configuration files
└── Program.cs                # Application startup and service registration
```

-----

## 🛠️ Future Enhancements

This project serves as a strong foundation. Future improvements could include:

  - [ ] **Authentication & Authorization:** Implement JWT for secure, role-based access.
  - [ ] **Unit & Integration Testing:** Add a dedicated test project for robust validation.
  - [ ] **Containerization:** Add `Dockerfile` and `docker-compose.yml` for easy deployment with Docker.
  - [ ] **CI/CD Pipeline:** Set up GitHub Actions to automate builds and deployments.
  - [ ] **Caching:** Introduce a caching layer (e.g., Redis) to improve performance for frequent read operations.

-----

## 🙌 Contributing

Contributions are welcome\! If you have suggestions for improvements, please open an issue or submit a pull request.

-----

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Author

Pankaj Kushwaha - [GitHub](https://github.com/pankaj085 "Follow me on GitHub") | [LinkedIn](https://www.linkedin.com/in/py--dev/ "Follow me on LinkedIn")