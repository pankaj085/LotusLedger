# Product Inventory REST API

## Project Overview

The **Product Inventory REST API** is a simple and efficient API built using **ASP.NET Core Web API**. It allows users to manage products in an inventory system with features such as adding, retrieving, updating, and soft-deleting products.

---

## Features

- **Add Product**: Create a new product with details like `Name`, `Description`, `Price`, `StockQuantity`, and `Category`.
- **Get All Products**: Retrieve all products with optional filtering by `Category` and sorting by `Price`.
- **Get Product by ID**: Fetch details of a specific product using its unique ID.
- **Update Product**: Modify product details such as `Price` or `StockQuantity`.
- **Delete Product**: Soft delete a product by marking it as inactive.

---

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your system.
- Basic knowledge of RESTful APIs and ASP.NET Core.

### Project Initialization

To begin, the project was created using the following commands:

```bash
dotnet new webapi -n product_inventory

cd product_inventory/

mkdir AppDataContext Contracts Controllers Interface MappingProfiles Middleware Models Services

touch [README.md](http://_vscodecontentref_/1)
```

## Packages to install

```bash
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.8
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.8
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.8
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.8
dotnet add package AutoMapper --version 13.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add package FluentValidation.AspNetCore --version 11.3.1
dotnet add package Swashbuckle.AspNetCore --version 6.6.2  #already installed, but if not do it.
```
