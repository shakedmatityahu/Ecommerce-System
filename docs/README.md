# Ecommerce-System

## Project Description

Ecommerce-System is an ecommerce application developed as part of the Workshop on Software Engineering Project course at Ben-Gurion University (BGU). The system is built using ASP.NET for the backend and React for the frontend, providing a robust and user-friendly platform for online shopping.

Beyond a regular ecommerce system, our application includes advanced features such as permissions and privileges for each store manager/owner. Additionally, we provide functionalities to set rules and discounts on purchases, making the platform flexible and adaptable to various business needs.

## Team Members

- Hadas Printz
- Yonatan Baruch
- Shaked Matityahu
- Kfir Nissim
- Olga Oznovich
- Noa Malul
- Nofar Cohen

## Prerequisites

Before you begin, ensure you have met the following requirements:

- **Node.js** (v14.0.0 or later)
- **npm** (v6.0.0 or later)
- **.NET Core SDK** (v3.1 or later)
- **SQL Server** (for the database)

## Installation

To install the Ecommerce-System, follow these steps:

### Backend (EcommerceAPI)

1. Clone the repository:

```bash {"id":"01J2BCT2A420D9PNKZJ642N8ST"}
git clone https://github.com/noamalu/Ecommerce-System.git
```

2. Navigate to the backend directory:

```bash {"id":"01J2BCT2A420D9PNKZJ7A3DGJJ"}
cd ecommerce-system/EcommerceAPI
```

3. Restore the dependencies:

```bash {"id":"01J2BCT2A420D9PNKZJATZ2CPN"}
dotnet restore
```

4. Update the `appsettings.json` file with your SQL Server connection string.
5. Run the backend server:

```bash {"id":"01J2BCT2A420D9PNKZJCYZC62N"}
dotnet run
```

### Frontend (EcommerceUI/ecommerceUI)

1. Navigate to the frontend directory:

```bash {"id":"01J2BCT2A420D9PNKZJF3KVN4H"}
cd ecommerce-system/EcommerceUI/ecommerceUI
```

2. Install the dependencies:

```bash {"id":"01J2BCT2A420D9PNKZJJ69SS0B"}
npm install
```

3. Start the development server:

```bash {"id":"01J2BCT2A420D9PNKZJMB2ZW88"}
npm run dev 
```

## Usage

Once the installation steps are completed, you can access the application in your web browser at `http://localhost:5173`. The backend API will be available at `http://localhost:4560`.
