#!/bin/bash

# Product Backend API Test Script
# This script demonstrates the basic functionality of the Product API

echo "=== Product Backend API Test Script ==="
echo ""

# Base URL for the API
BASE_URL="https://localhost:7000"

echo "Testing Product Backend API endpoints..."
echo "Note: Make sure the API is running with 'dotnet run' before executing these tests"
echo ""

echo "1. Health Check:"
echo "curl -k $BASE_URL/health"
echo ""

echo "2. API Information:"
echo "curl -k $BASE_URL/"
echo ""

echo "3. Get All Products:"
echo "curl -k $BASE_URL/api/products"
echo ""

echo "4. Get Product by ID (ID=1):"
echo "curl -k $BASE_URL/api/products/1"
echo ""

echo "5. Get Products by Category (Electronics):"
echo "curl -k $BASE_URL/api/products/category/Electronics"
echo ""

echo "6. Create a New Product:"
echo "curl -k -X POST $BASE_URL/api/products \\"
echo "  -H \"Content-Type: application/json\" \\"
echo "  -d '{"
echo "    \"name\": \"Wireless Headphones\","
echo "    \"description\": \"High-quality bluetooth headphones with noise cancellation\","
echo "    \"price\": 129.99,"
echo "    \"category\": \"Electronics\","
echo "    \"stock\": 30"
echo "  }'"
echo ""

echo "7. Update a Product (ID=1):"
echo "curl -k -X PUT $BASE_URL/api/products/1 \\"
echo "  -H \"Content-Type: application/json\" \\"
echo "  -d '{"
echo "    \"name\": \"Gaming Laptop\","
echo "    \"description\": \"High-performance gaming laptop with RTX graphics\","
echo "    \"price\": 1299.99,"
echo "    \"category\": \"Electronics\","
echo "    \"stock\": 5"
echo "  }'"
echo ""

echo "8. Delete a Product (ID=2):"
echo "curl -k -X DELETE $BASE_URL/api/products/2"
echo ""

echo "=== End of Test Script ==="
echo ""
echo "To run the API server, use: dotnet run"
echo "To access Swagger documentation, go to: $BASE_URL/swagger"
echo ""
echo "For actual testing, execute these curl commands after starting the server."