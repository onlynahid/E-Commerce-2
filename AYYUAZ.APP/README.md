# AYYUAZ.APP - E-Commerce API

A comprehensive e-commerce API built with ASP.NET Core 8, featuring JWT authentication, file upload capabilities, and a complete product management system.

## Features

? **User Authentication & Authorization**
- JWT Bearer token authentication
- Role-based access control (Admin/User)
- User registration and login

? **Product Management**
- Full CRUD operations for products
- Category-based organization
- Image upload support
- Search and filtering capabilities
- Pagination support

? **Order Management**
- Order creation and tracking
- Order history
- Admin order management

? **Content Management**
- Dynamic settings configuration
- About page management
- File upload and management

? **API Documentation**
- Swagger/OpenAPI integration
- Comprehensive endpoint documentation

## Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
```bash
git clone [repository-url]
cd AYYUAZ.APP
```

2. **Configure Database**
Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AYYUAZDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

3. **Configure JWT Settings**
Update JWT settings in `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "AYYUAZ.APP",
    "Audience": "AYYUAZ.APP.Users",
    "ExpiresInHours": 24
  }
}
```

4. **Run the Application**
```bash
dotnet run --project AYYUAZ.APP
```

5. **Access the API**
- API Base URL: `https://localhost:7000/api`
- Swagger Documentation: `https://localhost:7000/swagger`
- Health Check: `https://localhost:7000/api/health`

## Default Credentials

The application automatically seeds an admin user:
- **Email**: `Admin045@gmail.com`
- **Password**: `admin123`

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/me` - Get current user info

### Products
- `GET /api/product` - Get all products
- `GET /api/product/{id}` - Get product by ID
- `POST /api/product` - Create product (Admin)
- `PUT /api/product/{id}` - Update product (Admin)
- `DELETE /api/product/{id}` - Delete product (Admin)
- `GET /api/product/search?searchTerm={term}` - Search products
- `GET /api/product/category/{categoryId}` - Get products by category

### Categories
- `GET /api/category` - Get all categories
- `POST /api/category` - Create category (Admin)
- `PUT /api/category/{id}` - Update category (Admin)
- `DELETE /api/category/{id}` - Delete category (Admin)

### Orders
- `GET /api/order` - Get all orders (Admin)
- `GET /api/order/{id}` - Get order by ID
- `POST /api/order` - Create order
- `PUT /api/order/{id}` - Update order (Admin)

### File Management
- `POST /api/filestorage/upload` - Upload file (Admin)
- `DELETE /api/filestorage` - Delete file (Admin)
- `POST /api/filestorage/validate` - Validate file format

### Settings & Content
- `GET /api/settings/current` - Get current settings
- `PUT /api/settings/{id}` - Update settings (Admin)
- `GET /api/about` - Get about information
- `POST /api/about` - Create about content (Admin)

## Architecture

The application follows Clean Architecture principles with the following layers:

### ??? **Domain Layer** (`AYYUAZ.APP.Domain`)
- Entities (User, Product, Category, Order, etc.)
- Repository interfaces
- Domain logic and business rules

### ?? **Application Layer** (`AYYUAZ.APP.Application`)
- Service interfaces and implementations
- DTOs (Data Transfer Objects)
- Business logic and orchestration

### ??? **Infrastructure Layer** (`AYYUAZ.APP.Infrastructure`)
- Entity Framework DbContext
- Repository implementations
- External service implementations (File storage, JWT)
- Data seeding

### ?? **Presentation Layer** (`AYYUAZ.APP`)
- API Controllers
- Authentication and authorization
- Swagger configuration
- Startup configuration

## Testing

Use the included `api-tests.http` file for testing all endpoints. The file contains:

1. **Authentication tests** - Login, register, token validation
2. **Product management** - CRUD operations, search, filtering
3. **Order management** - Create orders, view order history
4. **File upload** - Image upload and validation
5. **Admin functions** - Settings management, user management

## Development Notes

### Database Seeding
The application automatically seeds:
- Default admin user
- Sample categories (Electronics, Clothing, Books, etc.)
- Sample products in each category
- Default settings and about information

### File Upload
- Supports image files: JPG, PNG, GIF, BMP, WEBP
- Maximum file size: 5MB
- Automatic directory creation
- Default placeholder images

### Security
- JWT Bearer token authentication
- Role-based authorization (Admin/User)
- Password hashing with SHA256
- Input validation and model binding

### Error Handling
- Consistent error response format
- Exception logging
- Graceful error responses

## Configuration

### Key Configuration Sections

**Database Connection**
```json
"ConnectionStrings": {
  "DefaultConnection": "your-connection-string"
}
```

**JWT Authentication**
```json
"Jwt": {
  "Key": "your-secret-key",
  "Issuer": "AYYUAZ.APP",
  "Audience": "AYYUAZ.APP.Users",
  "ExpiresInHours": 24
}
```

**Logging**
```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

## Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Verify SQL Server is running
   - Check connection string format
   - Ensure database permissions

2. **JWT Token Issues**
   - Verify JWT key length (minimum 32 characters)
   - Check token expiration
   - Ensure proper Bearer token format

3. **File Upload Issues**
   - Check wwwroot directory permissions
   - Verify file size limits
   - Ensure supported file formats

### Logs
Check application logs in the console output or configure file logging for detailed error information.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Support

For support and questions:
- Create an issue in the repository
- Check the Swagger documentation at `/swagger`
- Review the API test file for endpoint examples