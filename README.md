# 🚀 PIMS-MS (Provincial Inventory & Logistics Management System API)

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-blue)](https://www.postgresql.org/)
[![CQRS](https://img.shields.io/badge/Architecture-Vertical_Slice-green)](#)

Enterprise RESTful API for the centralized management of provincial inventories and spare parts logistics. Designed as a pure backend (Headless) under a highly scalable Modular Monolith architecture and Vertical Slice Architecture (VSA).

---

## 🏗️ Architecture & Technologies

This repository exclusively contains the backend of the platform, structured to offer high performance and low coupling between its modules.

| Layer | Main Technologies | Purpose |
| :--- | :--- | :--- |
| **Backend (API)** | .NET 10 (C#), ASP.NET Core Minimal APIs | RESTful API structured with Vertical Slice Architecture (VSA). |
| **Database**| PostgreSQL, Entity Framework Core | Relational data persistence and tenant-scoping. |
| **Infrastructure**| Docker, MediatR, JWT | Containers, CQRS, event-driven communication, and security. |

---

## ✨ Key Features

*   **JWT Authentication & Authorization:** Secure user and context-based role management (`Administrator`, `ConsultantLogistic`, `OperatorManager`).
*   **Inventory & Logistics Management:** Automatic stock synchronization through domain events and replenishment tracking.
*   **Clean Architecture (VSA):** Modular backend organized by features (Use Cases) instead of technical layers, ensuring high maintainability and cohesion.
*   **Event-Driven Decoupling:** Communication between modules (e.g., Logistics and Inventory) using `Integration Events` to maintain strict structural isolation.

---

## 🗺️ Roadmap (Next Steps)

Currently, the core system operates with the base modules (`Identity`, `Inventory`, and `Logistics`). Active development is focused on integrating the following modules:

*   [ ] **FieldService Module (Work in Progress):** Field service operations management. Currently in the refactoring phase from the legacy codebase to adapt it to VSA.
*   [ ] **Couriers API Integration (Work in Progress):** Implement DHL and Shalom courier API integrations within the logistics module.
*   [x] **Notifications Module (Done):** Cross-cutting system for sending alerts (e.g., critical stock alerts, transfer approval emails). Currently in the refactoring phase.

---

## 📂 Repository Structure

The solution follows a strict Modular Monolith organization, contained entirely within the `/src` directory:

*   📁 `/src/PIMS-MS.Api` - Main entry point of the application, global dependency injection, and module registration.
*   📁 `/src/Modules` - Contains fully isolated business modules (`FieldService`, `Identity`, `Inventory`, `Logistics`, `Notifications`).
*   📁 `/src/Common` - *Shared Kernel* (Global exceptions, base interfaces, API contracts, and integration events).

---

## 🚀 Getting Started (Local Deployment)

To set up the API locally on your machine for testing or review, follow these steps:

### Prerequisites
*   .NET SDK (v10.0+)
*   Docker and Docker Compose (To spin up the PostgreSQL instance)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/Fab0705/PIMS-MS.git
   ```
2. Configure the environment variables (PostgreSQL connection string, JWT Secrets) in the `appsettings.Development.json` file inside the `PIMS-MS.Api` project.
3. (Optional) Start your database using Docker Compose if you have the file configured, or ensure you have a local PostgreSQL instance running.
4. Restore dependencies and run the development server from the root of the solution:
   ```bash
   dotnet restore
   dotnet run --project src/PIMS-MS.Api/PIMS-MS.Api.csproj
   ```
5. The API documentation and testing interface will be available at:
   - Swagger UI: `http://localhost:5275/swagger`

---

## 👨‍💻 Author
Fabian Cristobal Systems Information & Software Engineering

Developed as part of a modular system geared towards SaaS and scalable enterprise solutions in .NET and Azure[cite: 1].
