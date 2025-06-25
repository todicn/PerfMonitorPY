# Product Backend API

A minimal C# Web API for managing products with full CRUD operations.

## Features

- **Create** new products
- **Read** all products or get by ID/category
- **Update** existing products
- **Delete** products
- In-memory storage (data persists during application runtime)
- RESTful API design
- Comprehensive API documentation with Swagger/OpenAPI
- CORS enabled for frontend integration
- Input validation and error handling

## Technology Stack

- **.NET 6.0** - Web API framework
- **ASP.NET Core** - Web framework
- **Swagger/OpenAPI** - API documentation
- **In-memory storage** - Simple data persistence

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later

### Installation & Running

1. **Navigate to the project directory:**
   ```bash
   cd ProductBackend
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

4. **Access the API:**
   - API Base URL: `https://localhost:7000` (or as shown in console)
   - Swagger Documentation: `https://localhost:7000/swagger`
   - API Information: `https://localhost:7000/`

## API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get all products |
| `GET` | `/api/products/{id}` | Get product by ID |
| `GET` | `/api/products/category/{category}` | Get products by category |
| `POST` | `/api/products` | Create a new product |
| `PUT` | `/api/products/{id}` | Update an existing product |
| `DELETE` | `/api/products/{id}` | Delete a product |

### Utility

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check endpoint |
| `GET` | `/` | API information |

## Product Model

```json
{
  "id": 1,
  "name": "Product Name",
  "description": "Product Description",
  "price": 99.99,
  "category": "Category Name",
  "stock": 10,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

## API Usage Examples

### Get All Products
```bash
curl -X GET "https://localhost:7000/api/products" -H "accept: application/json"
```

### Get Product by ID
```bash
curl -X GET "https://localhost:7000/api/products/1" -H "accept: application/json"
```

### Create a New Product
```bash
curl -X POST "https://localhost:7000/api/products" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Product",
    "description": "A great new product",
    "price": 49.99,
    "category": "Electronics",
    "stock": 20
  }'
```

### Update a Product
```bash
curl -X PUT "https://localhost:7000/api/products/1" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Updated Product",
    "description": "Updated description",
    "price": 59.99,
    "category": "Electronics",
    "stock": 15
  }'
```

### Delete a Product
```bash
curl -X DELETE "https://localhost:7000/api/products/1" -H "accept: application/json"
```

## Sample Data

The application comes with pre-loaded sample data:

1. **Laptop** - High-performance laptop for work and gaming ($999.99)
2. **Coffee Mug** - Ceramic coffee mug with ergonomic handle ($15.99)
3. **Running Shoes** - Comfortable running shoes for all terrains ($79.99)

## Development

### Project Structure
```
ProductBackend/
├── Controllers/
│   └── ProductsController.cs    # API endpoints
├── Models/
│   └── Product.cs              # Product entity model
├── Services/
│   └── ProductService.cs       # Business logic layer
├── Program.cs                  # Application entry point
├── ProductBackend.csproj       # Project configuration
└── README.md                   # Documentation
```

### Building for Production
```bash
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

## Features for Future Enhancement

- **Database Integration** (Entity Framework with SQL Server/PostgreSQL)
- **Authentication & Authorization** (JWT tokens)
- **Caching** (Redis)
- **Logging** (Serilog)
- **Unit & Integration Tests**
- **Docker containerization**
- **Rate limiting**
- **API versioning**

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is open source and available under the [MIT License](LICENSE).