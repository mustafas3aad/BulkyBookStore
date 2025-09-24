# 📚 Bulky Book Store

**E-commerce ASP.NET Core MVC Web Application**

Designed and developed a dynamic e-commerce platform integrating leading-edge technologies to deliver a seamless user experience and efficient system management.

---

## 📝 Project Overview

Bulky Book Store is an educational and practical project that demonstrates how to build a scalable e-commerce platform using the Microsoft technology stack. The project follows a layered N-Tier architecture for maintainability, testability, and flexibility, and leverages robust authentication and role management to simulate a real-world online store.

---

## 🛠️ Technologies

### 🔙 Backend
- **ASP.NET Core MVC**: Main framework for building the web application.
- **Entity Framework Core**: ORM with automated seed database migrations.
- **ASP.NET Core Identity**: Authentication & Authorization system.

### 🎨 Frontend
- **HTML5, CSS, JavaScript**: Core web technologies for user interface.
- **Bootstrap v5**: Responsive and modern UI design.
- **Razor Views & View Components**: Dynamic rendering of UI components.

### 🗄️ Database
- **SQL Server**: Primary relational database.
- **Migrations**: Automated schema creation and updates.

### 🏗️ Architecture
- **N-Tier Architecture**: Ensures scalability and flexibility.
- **Dependency Injection & Unit of Work**: For better organization and maintainability.
- **Repository Pattern**: Clean and efficient database interaction.
- **Separation of Concerns**: Clear split between UI, Business Logic, and Data Access layers. 
---

## ✨ Features

- **Product Management**: Streamlined CRUD operations.
- **Shopping Cart**: Intuitive cart functionality.
- **Order Management**: Tracking, shipping, and cancellation options.
- **User & Role Management**: Secure registration and password recovery.
- **Stripe Integration**: Seamless payment gateway support.
- **Multi-Image Upload**: Enhanced product presentation.
- **External Login**: Providers with email confirmation.
- **DataTables API**: Efficient and dynamic product display.

---
## 📂 Solution Structure

```bash
BulkyBookStore.sln
├─ BulkyWeb/            # ASP.NET Core MVC web application (UI)
├─ Bulky.DataAccess/    # EF Core DbContext, Repositories, Migrations
├─ Bulky.Models/        # Domain models (Product, Category, Order, etc.)
└─ Bulky.Utility/       # Helpers, DTOs, constants, upload utilities

```


---

## 🖼️ Screenshots

### Homepage

Admin
![Homepage](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20003829.png)
![HomePage](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20003844.png)

Customer
![HomePage](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004503.png)
![HomePage](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004606.png)

### Product List


![Product List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20003927.png)
![Product List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20003945.png)
![Product List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004022.png)
![Product List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004048.png)

### Product Detail

![Product Detail](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004245.png)


### Category

![Category List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20003851.png)
![Category List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20003905.png)
![Category List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20003856.png)
![Category List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20003912.png)



### Company List

![Company List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004101.png)
![Company List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004109.png)
![Company List](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004118.png)


### Shopping Cart


![Shopping Cart](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004255.png)
![Shopping Cart](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004304.png)
![Shopping Cart](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004319.png)


### Order Management

Admin
![Order Managment](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004354.png)
![Order Managment](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004403.png)

Customer
![Order Managment](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004703.png)
![Order Management](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004711.png)


### User Management

![User](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004207.png)
![User](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004213.png)
![User](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004446.png)


### Register

Admin
![Register](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004136.png)
![Register](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004151.png)

Customer
![Register](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004728.png)


### Login

![Login](./BulkyWeb/wwwroot/images/Screenshot%202025-09-24%20004736.png)

---

