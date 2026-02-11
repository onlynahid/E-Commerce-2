# Custom Authorization System

Bu layih?d? ?n?n?vi ASP.NET Core Role sistemi ?v?zin?, sad? **IsAdmin** ?sasl? authorization sistemi istifad? olunur.

## Sistem nec? i?l?yir

### 1. ?stifad??i N?vl?ri
- **Admin ?stifad??i**: `IsAdmin = true` olan istifad??il?r
- **Sad? ?stifad??i**: `IsAdmin = false` olan istifad??il?r

### 2. Custom Authorization Attributes

#### `[RequireAdmin]`
- Yaln?z `IsAdmin = true` olan istifad??il?r ???n
- JWT token-da `Role = "Admin"` v? ya `IsAdmin = "true"` claim-ini yoxlay?r
- ?stifad? edildiyi yerl?r:
  - Product CRUD ?m?liyyatlar? (Create, Update, Delete)
  - Category CRUD ?m?liyyatlar? (Create, Update, Delete) 
  - About CRUD ?m?liyyatlar? (Create, Update, Delete)
  - Settings idar? edilm?si
  - Fayl upload/delete ?m?liyyatlar?
  - Order idar? edilm?si (admin panel ???n)

#### `[RequireAuth]`
- H?r hans? authenticate olmu? istifad??i ???n
- ?stifad? edildiyi yerl?r:
  - F?rdi order m?lumatlar?n?n g?r?lm?si
  - ?stifad??inin ?z m?lumatlar?n?n g?r?lm?si

### 3. Public Endpoint-l?r
Bu endpoint-l?r he? bir authentication t?l?b etmir:
- About m?lumatlar?n?n oxunmas?
- Product m?lumatlar?n?n oxunmas?  
- Category m?lumatlar?n?n oxunmas?
- Settings m?lumatlar?n?n oxunmas?
- Order yaratma (guest order)

### 4. JWT Token Claims
H?r JWT token-da a?a??daki m?lumatlar var:
```json
{
  "nameid": "123",           // User ID
  "name": "username",        // Username  
  "email": "user@email.com", // Email
  "role": "Admin",           // "Admin" v? ya "User"
  "IsAdmin": "true"          // "true" v? ya "false"
}
```

### 5. ?stifad? N?mun?l?ri

#### Admin endpoint-? giri?:
```http
POST /api/about
Authorization: Bearer <admin-user-jwt-token>
Content-Type: application/json

{
  "title": "Yeni m?lumat",
  "description": "Bu yaln?z admin t?r?find?n yarad?la bil?r"
}
```

#### Public endpoint:
```http
GET /api/about
# He? bir authorization t?l?b olunmur
```

#### Authenticated endpoint:
```http
GET /api/order/123
Authorization: Bearer <any-authenticated-user-token>
```

## Error Response-lar

### 401 Unauthorized (Authentication yoxdur):
```json
{
  "message": "Authentication required",
  "error": "unauthorized"
}
```

### 403 Forbidden (Admin h?ququn yoxdur):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

## F?rqi

| K?hn? Sistem | Yeni Sistem |
|---------------|-------------|
| `[Authorize(Roles = "Admin")]` | `[RequireAdmin]` |
| `[Authorize]` | `[RequireAuth]` |
| Role-based | IsAdmin-based |
| ASP.NET Core default | Custom implementation |

## Qeydiyyat zaman? Admin yaratma

Yeni istifad??i qeydiyyat zaman?nda admin olmaq ???n:

1. Normal qeydiyyat:
```http
POST /api/auth/register
{
  "username": "user123",
  "email": "user@email.com", 
  "password": "password123",
  "confirmPassword": "password123"
}
```

2. Admin yaratmaq ???n database-d? `IsAdmin` field-ini `true` etm?k laz?md?r (bu manual ?m?liyyatd?r v? ya admin panel vasit?sil?).

Bu sistem daha sad? v? ba?a d???l?ndir, h?l? ki adi role management-? ehtiyac yoxdur.