
# Tienda UCN - REST API

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet) ![C#](https://img.shields.io/badge/C%23-12-blue) ![SQLite](https://img.shields.io/badge/SQLite-3-blue) ![Swagger](https://img.shields.io/badge/Swagger-API-green)

**TiendaUCN API** is the backend component for a modern e-commerce platform. Built with ASP.NET Core 9, this RESTful API provides all the necessary functionality to manage users, products, shopping carts, orders, and authentication, serving as the backbone for client applications (web or mobile).

A REST API for the Tienda UCN project, an e-commerce platform. This API handles business logic, data access, and security for the client application.

## ✨ Features

### Authentication & Authorization
- Complete registration and login system based on JWT tokens
- Email verification with time-limited verification codes
- Password recovery and reset functionality
- Role-based access control (Customer and Administrator roles)
- Secure password hashing with ASP.NET Core Identity

### User Profile Management
- View and update personal profile information
- Password change functionality
- Email change with verification
- RUT (Chilean national ID) validation
- Birth date validation (minimum age 18 years)

### Product Management (Admin)
- Full CRUD operations for products
- Image upload and management via Cloudinary
- Discount management
- Product activation/deactivation (soft delete)
- Advanced filtering and sorting capabilities
- Category and brand management

### Shopping Cart
- Anonymous and authenticated user cart support
- Cart persistence across sessions
- Automatic cart association when user logs in
- Real-time price calculations with discounts
- Abandoned cart detection and email reminders

### Order Management
- Order creation from shopping cart
- Order status tracking (Pending, Processing, Shipped, Delivered, Cancelled)
- Order status transition validation
- Paginated order listing with filters
- Order history for users
- Admin order management dashboard

### Email Notifications
- Welcome emails for new users
- Email verification codes
- Password reset codes
- Abandoned cart reminders
- Transactional email support via Resend

### Background Jobs
- Automated cleanup of unverified users (Hangfire)
- Scheduled abandoned cart reminder emails
- Configurable job scheduling

## 🏗️ Project Architecture

This project follows a **Clean Architecture** approach with clear separation of concerns:

```
TiendaUcnApi/
├── src/
│   ├── API/                          # Presentation Layer
│   │   ├── Controllers/              # API endpoints
│   │   ├── Middlewares/             # Custom middleware (error handling, buyer ID)
│   │   └── Extensions/              # Service configuration and data seeding
│   │
│   ├── Application/                  # Application Layer
│   │   ├── DTO/                     # Data Transfer Objects
│   │   │   ├── AuthDTO/            # Authentication DTOs
│   │   │   ├── ProductDTO/         # Product DTOs
│   │   │   ├── CartDTO/            # Shopping cart DTOs
│   │   │   ├── OrderDTO/           # Order DTOs
│   │   │   ├── UserDTO/            # User management DTOs
│   │   │   └── BaseResponse/       # Generic response DTOs
│   │   ├── Services/
│   │   │   ├── Interfaces/         # Service contracts
│   │   │   └── Implements/         # Service implementations
│   │   ├── Mappers/                # Object mapping logic
│   │   ├── Validators/             # Custom validation attributes
│   │   ├── Exceptions/             # Custom exception types
│   │   └── Jobs/                   # Background job definitions
│   │
│   ├── Domain/                      # Domain Layer
│   │   └── Models/                 # Entity models
│   │       ├── User.cs
│   │       ├── Product.cs
│   │       ├── Category.cs
│   │       ├── Brand.cs
│   │       ├── Cart.cs
│   │       ├── Order.cs
│   │       └── ...
│   │
│   └── Infrastructure/              # Infrastructure Layer
│       ├── Data/                   # Database context and configurations
│       │   ├── AppDbContext.cs
│       │   ├── DataSeeder.cs
│       │   └── Migrations/
│       └── Repositories/
│           ├── Interfaces/         # Repository contracts
│           └── Implements/         # Repository implementations
│
├── appsettings.json                # Configuration
├── Program.cs                       # Application entry point
└── README.md                        # Project documentation
```

### Layer Responsibilities

#### 1. **API Layer (Presentation)**
- **Controllers**: Handle HTTP requests and responses
- **Middlewares**: 
  - `ErrorHandlingMiddleware`: Global exception handling
  - `BuyerIdMiddleware`: Anonymous user identification
- **Extensions**: Dependency injection configuration and data seeding

#### 2. **Application Layer**
- **Services**: Business logic implementation
  - `IUserService`: User authentication and registration
  - `IProductService`: Product management
  - `ICartService`: Shopping cart operations
  - `IOrderService`: Order processing
  - `IEmailService`: Email notifications
  - `IFileService`: Image upload/deletion
- **DTOs**: Data transfer between layers
- **Validators**: Custom validation logic (RUT, birth date)
- **Mappers**: Object mapping utilities

#### 3. **Domain Layer**
- **Models**: Core business entities
- Enumerations: `OrderStatus`, `Gender`, `Status`, `CodeType`
- Business rules and domain logic

#### 4. **Infrastructure Layer**
- **DbContext**: Entity Framework Core database context
- **Repositories**: Data access abstraction
- **Migrations**: Database schema versioning

## 🛠️ Technologies & Libraries

### Core Framework
- **ASP.NET Core 9**: Modern web framework
- **C# 12**: Latest language features
- **Entity Framework Core 9**: ORM for database access

### Database
- **SQLite**: Lightweight relational database
- **Entity Framework Core**: Code-first migrations

### Authentication & Security
- **ASP.NET Core Identity**: User management
- **JWT Bearer Tokens**: Stateless authentication
- **BCrypt**: Password hashing

### External Services
- **Cloudinary**: Image storage and CDN
- **Resend**: Transactional email service
- **Hangfire**: Background job scheduling

### Developer Tools
- **Swagger/OpenAPI**: API documentation
- **Serilog**: Structured logging
- **Mapster**: Object mapping

---

## 🚀 Getting Started

Follow these instructions to clone and run the project on your local machine for development and testing.

### Prerequisites

-   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
-   [Git](https://git-scm.com/)
-   A code editor such as [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)
-   Cloudinary account (for image uploads)
-   Resend account (for email notifications)

### ⚙️ Installation & Setup

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/A-benites/TiendaUcnApi.git
    ```

2.  **Navigate to the project directory:**
    ```sh
    cd TiendaUcnApi
    ```

3.  **Configure environment variables:**
    Create an `appsettings.Development.json` file in the project root with the following content, replacing placeholders with your actual keys.

    ```json
    {
        "Logging": {
            "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
            }
        },
        "ConnectionStrings": {
            "DefaultConnection": "Data Source=tienda_ucn.db"
        },
        "JWTSecret": "YOUR_SECRET_KEY_FOR_JWT_MUST_BE_LONG_AND_COMPLEX",
        "ResendAPIKey": "YOUR_RESEND_API_KEY",
        "Cloudinary": {
            "CloudName": "YOUR_CLOUDINARY_CLOUD_NAME",
            "ApiKey": "YOUR_CLOUDINARY_API_KEY",
            "ApiSecret": "YOUR_CLOUDINARY_API_SECRET"
        },
        "AdminUser": {
            "Email": "admin@example.com",
            "Password": "Admin123!",
            "FirstName": "Admin",
            "LastName": "User",
            "Rut": "1-9",
            "Gender": "Masculino",
            "BirthDate": "1990-01-01",
            "PhoneNumber": "+56912345678"
        }
    }
    ```

    > **Note about Administrator**: The system will automatically create the administrator user defined in the `AdminUser` section during the first startup. You can modify these values as needed.

4.  **Restore .NET dependencies:**
    ```sh
    dotnet restore
    ```

5.  **Apply database migrations:**
    The seeder will create an admin user and initial data (categories, brands, sample products).
    ```sh
    dotnet ef database update
    ```

6.  **Run the application:**
    ```sh
    dotnet run
    ```
    The API will be running at `https://localhost:5177` or `http://localhost:5177`.

7.  **Access Swagger UI:**
    Open your browser and navigate to `https://localhost:5177/swagger` to explore the API documentation.

---

## 📚 API Documentation

### Base URL
```
https://localhost:5177/api
```

### Authentication

Most endpoints require authentication via JWT Bearer token. Include the token in the Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

### Roles
- **Customer**: Regular users who can browse products, manage cart, and place orders
- **Administrator**: Full access to all endpoints, including user and product management

### Key Endpoints

#### Authentication (`/api/Auth`)
- `POST /login` - User login
- `POST /register` - User registration
- `POST /verify` - Email verification
- `POST /resend-email-verification-code` - Resend verification code
- `POST /recover-password` - Request password reset
- `PATCH /reset-password` - Reset password with code

#### Profile (`/api/user`)
- `GET /profile` - Get user profile
- `PUT /profile` - Update profile
- `PATCH /change-password` - Change password
- `POST /verify-email-change` - Verify email change

#### Products (Public) (`/api/products`)
- `GET /` - Get all products (with filters and pagination)
- `GET /{id}` - Get product details

#### Products (Admin) (`/api/admin/products`)
- `GET /` - Get all products for admin
- `GET /{id}` - Get product details for admin
- `POST /` - Create product
- `PUT /{id}` - Update product
- `DELETE /{id}` - Toggle product availability
- `POST /{id}/images` - Upload product images
- `DELETE /{id}/images/{imageId}` - Delete product image
- `PATCH /{id}/discount` - Update product discount

#### Cart (`/api/cart`)
- `GET /` - Get user's cart
- `POST /items` - Add item to cart
- `DELETE /items/{productId}` - Remove item from cart
- `PUT /items/{productId}` - Update item quantity
- `DELETE /` - Clear cart

#### Orders (`/api/orders`)
- `POST /` - Create order from cart
- `GET /` - Get user's orders (paginated)
- `GET /{id}` - Get order details

#### Orders (Admin) (`/api/admin/orders`)
- `GET /` - Get all orders (with filters)
- `GET /{id}` - Get order details
- `PATCH /{id}/status` - Update order status

#### Categories (Admin) (`/api/admin/categories`)
- `GET /` - Get all categories
- `GET /{id}` - Get category by ID
- `POST /` - Create category
- `PUT /{id}` - Update category
- `DELETE /{id}` - Delete category

#### Brands (Admin) (`/api/admin/brands`)
- `GET /` - Get all brands
- `GET /{id}` - Get brand by ID
- `POST /` - Create brand
- `PUT /{id}` - Update brand
- `DELETE /{id}` - Delete brand

#### Users (Admin) (`/api/admin/users`)
- `GET /` - Get all users (paginated)
- `GET /{id}` - Get user details
- `PATCH /{id}/status` - Update user status (active/blocked)
- `PATCH /{id}/role` - Update user role

For detailed request/response examples, refer to the `TiendaUcnApi.http` file or explore the Swagger UI.

---

## 🔒 Security Features

- **Password Hashing**: Uses ASP.NET Core Identity with BCrypt
- **JWT Tokens**: Stateless authentication with configurable expiration
- **Email Verification**: Required for account activation
- **Role-Based Authorization**: Separate permissions for customers and administrators
- **Input Validation**: Comprehensive DTO validation
- **CORS Configuration**: Configurable cross-origin resource sharing
- **Error Handling**: Global exception middleware with sanitized error messages
- **Rate Limiting**: Configurable request throttling (can be added)

---

## 📊 Database Schema

### Core Entities

#### User
- Identity-based user with roles (Customer, Administrator)
- RUT validation (Chilean national ID)
- Email verification required
- Age validation (18+)

#### Product
- Title, description, price, discount, stock
- Category and brand relationships
- Multiple images support
- Soft delete (IsAvailable flag)
- New/Used status

#### Cart
- Anonymous and authenticated support
- BuyerId for session tracking
- Automatic price calculations

#### Order
- Order status workflow
- Immutable order items (snapshot at purchase time)
- User relationship

#### Category & Brand
- Simple lookup entities
- Product count tracking

### Relationships
- User ↔ Orders (One-to-Many)
- User ↔ Verification Codes (One-to-Many)
- Product ↔ Category (Many-to-One)
- Product ↔ Brand (Many-to-One)
- Product ↔ Images (One-to-Many)
- Cart ↔ Cart Items (One-to-Many)
- Order ↔ Order Items (One-to-Many)

---

## 🧪 Testing

### Manual Testing
Use the `TiendaUcnApi.http` file with REST Client extension in VS Code or Swagger UI for interactive testing.

### Test Data
The DataSeeder automatically creates:
- 1 Administrator user
- 2 Sample categories (Electronics, Clothing)
- 2 Sample brands (TechCorp, FashionCo)
- Sample products with images

---

## 📝 Development Notes

### Adding a New Entity

1. Create the model in `src/Domain/Models/`
2. Add DbSet to `AppDbContext.cs`
3. Create migration: `dotnet ef migrations add AddYourEntity`
4. Update database: `dotnet ef database update`
5. Create DTOs in `src/Application/DTO/`
6. Create repository interface and implementation
7. Create service interface and implementation
8. Register services in `Program.cs`
9. Create controller in `src/API/Controllers/`

### Code Style
- Use XML documentation comments for all public classes and methods
- Follow C# naming conventions
- Use async/await for all I/O operations
- Implement proper error handling
- Validate all user inputs

---

## 👥 Contributors

-   **Amir Benites** - Backend Developer
-   **Álvaro Zapana** - Backend Developer

---

## 📄 License

This project is developed as part of an academic project at Universidad Católica del Norte.

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📞 Support

For questions or issues, please open an issue on GitHub or contact the development team.
