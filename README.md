![Build Status](https://github.com/dondomingoo/GoTorz/actions/workflows/build-and-test.yml/badge.svg)

# GoTorz – Setup Guide

This guide explains how to run the GoTorz application locally for demonstration and review purposes.  
The full source code is hosted in this GitHub repository. Required environment secrets are provided in the exam PDF submitted on WiseFlow.

---

## 📄 System Description

GoTorz is a travel booking and management platform that allows users to **create**, **search**, and **book** travel packages. It features role-based access with Admin, Sales Representative and Regular User accounts. The system integrates with external APIs for real-time flight and hotel data, enabling users to plan their trips efficiently.

Key features include:
- **User authentication** and **role-based access** (Admin, Sales Rep, User)
- **Create**, **search**, and **book** travel packages
- **Payment processing** via Stripe (test mode)
- **Real-time support chat** for users and admins
- **Admin and Sales dashboards** for managing bookings and users

---

## 🛠 Requirements

- **Visual Studio 2022 or newer**
- **.NET 8 SDK**
- **EF Core tools** (see below)
- Internet access (for API calls)

Install EF Core CLI tools (only needed once per machine):

```bash
dotnet tool install --global dotnet-ef
```

---

## ⚙️ Environment Configuration

Before running the application, you must create a `.env` file in the root of the **GoTorz.API** project.

1. Navigate to the root of the `GoTorz.API` project (where `.env.example` is located).
2. Copy the provided `.env.example` file:

```bash
cp .env.example .env
```

2. Open the `.env` file and insert the real secret values.

   - These values are included in the exam PDF (WiseFlow submission).

The following values are required:

- `JwtSettings__SecretKey`: used for signing JWT tokens.
- `RapidApiSettings__ApiKey`: used for hotel/flight data.
- `RapidApiSettings__Host`: the API host (not a secret - already filled in).
- `Stripe__SecretKey`: used for test payment processing.

> **Tip:** The `.env` file may not appear in Visual Studio’s Solution Explorer because it is excluded by `.gitignore`.  
> You can still open it manually using **File → Open → File…**, or by browsing directly to the file in your project folder.

---

## 📦 Database Setup (Required)

This project uses Entity Framework Core for database management.

After cloning or downloading the project, navigate to the `GoTorz.API` project folder and 
run these commands **once** to create the database schema:

```bash
cd GoTorz.API
dotnet ef database update
```

Make sure you run it from the folder that contains `GoTorz.API.csproj`.

If you skip this step, the app will start but fail when accessing data.

---

## ▶️ How to Run the Project

1. Open **Visual Studio**
2. Select `File → Open → Project/Solution`
3. Open the file named `GoTorz.sln`
4. In **Solution Explorer**, right-click the solution (`GoTorz`) → **Configure Startup Projects...**
5. In the popup:
   - Choose **Multiple startup projects**
   - Set both `GoTorz.API` and `GoTorz.Client` to **Start**
   - Apply changes and click **OK**
6. Press **Ctrl+F5** (Start Without Debugging)

This will launch:
- The backend API
- The Blazor WebAssembly frontend (in your browser)

> Note: You must use **Ctrl+F5** (not F5) for Stripe payments to work.

---

## 💳 Stripe Test Payments

Stripe test payments **only work when running without debugging**.

To test payments:
- Use **Ctrl+F5** in Visual Studio (Start Without Debugging)
- The payment flow should now work correctly in test mode.
- Use the following **test card details** for making payments:
  - **Card Number**: `4242 4242 4242 4242` (This is a standard test card used by Stripe)
  - **Expiration Date**: Any future date (e.g., `12/25`)
  - **CVC**: Any 3 digits (e.g., `123`)

Do **not** use regular `F5` (debug mode), as this may block Stripe callbacks.

---

## 👤 Test Users

You can log in using the following test accounts:

### Admin
- **Email:** 1@1
- **Password:** 1

### Sales Representative
- **Email:** 2@2
- **Password:** 2

### Regular User
- **Email:** 3@3
- **Password:** 3

---

## 💬 Testing the Support Chat

To test the support chat feature:

1. **Log in as the regular user (3@3)** in your normal browser window  
2. In the user dashboard, **click "Support"** and wait for a support agent to join  
   - This puts the user into a waiting state, visible to admins
3. **Open a private/incognito window** and:
   - Type **`https://localhost:7272/`** into the address bar.
   - Log in as the admin (1@1)
   - Go to the **Admin Dashboard → Support Chat**
   - You will see a list of users waiting for support
   - **Click "Join"** to enter the user's chat room
5. You can now chat between the two roles using the two separate browser windows

---

## ✅ Features You Can Explore

- User registration & login
- JWT authentication and role-based access
- Travel package search and booking
- 1-on-1 and group chat
- Booking history
- Admin and Sales dashboards
- **Live API integration with airline and hotel data**

---

## 🌐 Live Deployment (Coming Soon)

This version is intended for development and demonstration.

The team is currently implementing a CI/CD pipeline using GitHub Actions and Azure. Once completed, the project will be automatically built, tested, and deployed to a public domain.

Users will then be able to explore the full application online without running it locally.

---

Developed by Team 3  
Datamatiker Semester Project – Spring 2025  
UCL University College, Denmark



