# Role-Based Task Management System

A full-stack Role-Based Task Management System built with **Angular 17**, **.NET 9 Web API**, and **SQL Server**. This system allows organizations to efficiently manage tasks across different access levels (Admin, Manager, and User) with secure authentication and real-time task tracking.

## 🚀 Tech Stack
* **Frontend:** Angular 17 (Standalone Components), Bootstrap 5
* **Backend:** .NET 9 Web API (C#)
* **Database:** Microsoft SQL Server (ADO.NET for data access)
* **Security:** JWT (JSON Web Tokens) Authentication, BCrypt Password Hashing

## ✨ Key Features

### 1. Role-Based Access Control (RBAC)
* **Admin:** Has full system access. Can manage all employees, assign managers, create tasks for anyone, and view all tasks across the organization.
* **Manager:** Has a scoped view. Can view their specific team members ("My Team"), and view/manage tasks that they created, tasks assigned to their team members, or tasks assigned directly to them.
* **User:** Has restricted access. Can only view tasks explicitly assigned to them, update their task statuses, and communicate via comments.

### 2. Task Management & Collaboration
* Full CRUD functionality for tasks.
* Real-time status updates (To Do, In Progress, Done).
* Integrated **Comments System** on every task allowing users to collaborate and leave notes.

### 3. Employee Management
* Secure registration and role assignment.
* Dynamic hierarchy mapping (assigning Users to specific Managers).

---

## 🛠️ Project Setup Instructions

Follow these steps to run the project locally on your machine.

### Step 1: Database Setup
1. Open Microsoft SQL Server Management Studio (SSMS).
2. Open the file `BACK_END/Full_Database_Setup_With_New_Features.sql`.
3. Execute the script. 
   *(This will automatically create the `Amit_Test` database, build all tables, create the stored procedures, and insert the default Admin user).*

### Step 2: Backend Setup (.NET API)
1. Open a terminal and navigate to the API directory:
   ```bash
   cd BACK_END/RoleBasedTaskManagement.API
   ```
2. *(Optional)* If your SQL Server instance is not `.\SQLEXPRESS`, update the `DefaultConnection` string in `appsettings.json`.
3. Run the API:
   ```bash
   dotnet run --launch-profile "https"
   ```
   *The API will start running on `https://localhost:7065`.*

### Step 3: Frontend Setup (Angular)
1. Open a new terminal and navigate to the Angular client directory:
   ```bash
   cd FRONT_END/TaskManagementClient
   ```
2. Install the necessary Node modules:
   ```bash
   npm install
   ```
3. Start the Angular development server:
   ```bash
   ng serve
   ```
4. Open your browser and navigate to `http://localhost:4200`.

---

## 🔐 Default Admin Credentials
A default Admin account is automatically created by the SQL script with a secure BCrypt hashed password. You can use these credentials to log in immediately:

* **Email:** `admin@gmail.com`
* **Password:** `admin@123`

Once logged in as Admin, you can navigate to the "Manage Employees" tab to create new Managers and Users to test the role-based features.
