# 🪷 LotusLedger API Endpoints

This document provides a complete and accurate list of API endpoints for the LotusLedger Product Inventory Management System.
<p align="center">
    <img src="/assests/Screenshot from 2025-08-27 11-36-22.png" alt="Endpoints" width="500"/>
</p>

---

### Base URL

All endpoint paths are relative to the base URL.

`http://localhost:5258/index.html`

Example: `http://localhost:5258/index.html`

---
---

## 🧪 API Testing with Postman

To make testing the LotusLedger API as easy as possible, a complete Postman collection is available. Click the button below to fork the collection to your own Postman workspace.

[![Run in Postman](https://run.pstmn.io/button.svg)](https://www.postman.com/mrlotus/workspace/lotusledger/collection/43877835-98b3d4d1-7a0b-406c-9766-de9b48d91a19?action=share&source=copy-link&creator=43877835)

After forking, remember to set the `baseUrl` variable in the collection's settings to your local environment (e.g., `http://localhost:5000`).
---

## 1️⃣ CRUD / Main Product Operations

These are the primary endpoints for managing products.

### Get All Products

Retrieves a paginated and filterable list of all **active** products.

-   **Endpoint:** `GET /api/Product`
-   **Description:** Use query parameters to search, filter by category, sort, and paginate results.
-   **Query Parameters:**
    -   `searchTerm` (string, optional): Searches against product name and description.
    -   `category` (string, optional): Filters products by category.
    -   `pageNumber` (int, optional): The page number for pagination (default: 1).
    -   `pageSize` (int, optional): The number of items per page (default: 10).
-   **Success Response (200 OK):**
    ```json
    [
      {
        "id": 1,
        "name": "Gaming Laptop",
        "description": "High-performance laptop for gaming.",
        "price": 1200.50,
        "stockQuantity": 25,
        "category": "Electronics"
      }
    ]
    ```

### Get Product by ID

Retrieves a single product by its unique identifier.

-   **Endpoint:** `GET /api/Product/{id}`
-   **Success Response (200 OK):**
    ```json
    {
      "id": 1,
      "name": "Gaming Laptop",
      "description": "High-performance laptop for gaming.",
      "price": 1200.50,
      "stockQuantity": 25,
      "category": "Electronics"
    }
    ```

### Create a New Product

Adds a new product to the inventory.

-   **Endpoint:** `POST /api/Product`
-   **Request Body:**
    ```json
    {
      "name": "Wireless Mouse",
      "description": "Ergonomic wireless mouse.",
      "price": 49.99,
      "stockQuantity": 150,
      "category": "Accessories"
    }
    ```
-   **Success Response (201 Created):** Returns the newly created product.

### Fully Update a Product

Replaces all data for a specific product. All fields are required.

-   **Endpoint:** `PUT /api/Product/{id}`
-   **Request Body:**
    ```json
    {
      "name": "Wireless Mouse v2",
      "description": "Upgraded ergonomic wireless mouse.",
      "price": 55.00,
      "stockQuantity": 200,
      "category": "Accessories"
    }
    ```
-   **Success Response (204 No Content):** Indicates the update was successful.

### Partially Update a Product

Updates one or more fields of a specific product. Only include the fields you want to change.

-   **Endpoint:** `PATCH /api/Product/{id}`
-   **Request Body:**
    ```json
    {
      "price": 52.50,
      "stockQuantity": 180
    }
    ```
-   **Success Response (204 No Content):** Indicates the update was successful.

### Soft Delete a Product

Marks a product as inactive instead of permanently deleting it.

-   **Endpoint:** `DELETE /api/Product/{id}`
-   **Success Response (204 No Content):** Indicates the product was successfully marked as inactive.

---

## 2️⃣ Special Product Queries

Endpoints for specialized views of the inventory.

### Get Low-Stock Products

Retrieves a list of all active products with a stock quantity below the configured threshold (default is 25).

-   **Endpoint:** `GET /api/Product/low-stock`
-   **Success Response (200 OK):**
    ```json
    [
      {
        "id": 3,
        "name": "USB-C Hub",
        "description": "7-in-1 USB-C Hub.",
        "price": 39.99,
        "stockQuantity": 15,
        "category": "Accessories"
      }
    ]
    ```

### Get Inactive Products

Retrieves a list of all products that have been soft-deleted.

-   **Endpoint:** `GET /api/Product/inactive`
-   **Success Response (200 OK):**
    ```json
    [
      {
        "id": 2,
        "name": "Old Keyboard",
        "description": "A discontinued model.",
        "price": 25.00,
        "stockQuantity": 0,
        "category": "Peripherals"
      }
    ]
    ```

---

## 3️⃣ Administrative Operations

Endpoints for managing the state of products.

### Reactivate a Product

Restores a soft-deleted (inactive) product back to an active state.

-   **Endpoint:** `PUT /api/Product/{id}/reactivate`
-   **Success Response (204 No Content):** Indicates the product was successfully reactivated.

### Permanently Delete a Product

Permanently removes a product from the database. **This action is irreversible.** Can only be performed on inactive products.

-   **Endpoint:** `DELETE /api/Product/delete/{id}`
-   **Success Response (204 No Content):** Indicates the product was permanently deleted.

---

## ⚠️ Error Responses

All endpoints may return the following error responses.

-   **400 Bad Request:** The request was malformed (e.g., failed validation).
    ```json
    {
      "errors": {
        "Name": [
          "'Name' must not be empty."
        ]
      }
    }
    ```
-   **404 Not Found:** The requested resource (e.g., a product with a specific ID) could not be found.
    ```json
    {
      "error": "Product with ID 999 not found."
    }
    ```
-   **500 Internal Server Error:** An unexpected error occurred on the server.