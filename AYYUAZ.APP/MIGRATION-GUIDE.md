# Migration Guide for JWT Authentication with Identity

Bu layih? JWT Authentication sistemini Identity il? konfiqurasiya etm?k ???n migration ?m?liyyatlar?n?n t?limatlar?.

## Migration ?m?liyyatlar?

### 1. Yeni migration yarad?n:
```bash
# AYYUAZ.APP.Infrastructure folder-ind?n icra edin:
dotnet ef migrations add "AddIdentitySupport" --startup-project ../AYYUAZ.APP --context AppDbContext

# V? ya project directory-d?n:
dotnet ef migrations add "AddIdentitySupport" --project AYYUAZ.APP.Infrastructure --startup-project AYYUAZ.APP --context AppDbContext
```

### 2. Database-i yenil?yin:
```bash
# AYYUAZ.APP.Infrastructure folder-ind?n icra edin:
dotnet ef database update --startup-project ../AYYUAZ.APP --context AppDbContext

# V? ya project directory-d?n:
dotnet ef database update --project AYYUAZ.APP.Infrastructure --startup-project AYYUAZ.APP --context AppDbContext
```

### 3. ?g?r k?hn? database varsa, onu silin v? yenid?n yarad?n:
```bash
dotnet ef database drop --startup-project ../AYYUAZ.APP --context AppDbContext
dotnet ef database update --startup-project ../AYYUAZ.APP --context AppDbContext
```

## Konfiqurasiya d?yi?iklikl?ri

### 1. User Entity
- Identity-d?n inherit edir: `IdentityUser`
- Yeni field-l?r: `IsAdmin`, `FullName`, `CreatedAt`
- `Username` property Identity-nin `UserName`-? map olunur

### 2. JWT Token Claims
H?r token-da a?a??daki claim-l?r var:
- `ClaimTypes.NameIdentifier` - User ID (string)
- `ClaimTypes.Name` - Username
- `ClaimTypes.Email` - Email
- `ClaimTypes.Role` - "Admin" v? ya "User" 
- `"IsAdmin"` - "true" v? ya "false"

### 3. Authentication Test
```http
# Admin istifad??i il? login:
POST https://localhost:7038/api/auth/login
Content-Type: application/json

{
    "email": "Admin045@gmail.com",
    "password": "admin123"
}

# Admin endpoint test:
POST https://localhost:7038/api/about
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

{
    "title": "Test Admin Access",
    "description": "Bu yaln?z admin istifad??il?r yarada bil?r"
}
```

## Troubleshooting

### Problem: Migration x?talar?
**H?ll**: K?hn? migration fayllar? silin v? yenid?n yarad?n:
```bash
# Migration folder-ini t?mizl?yin v? yeni migration yarad?n
rm -rf AYYUAZ.APP.Infrastructure/Migrations/*
dotnet ef migrations add "InitialWithIdentity" --startup-project AYYUAZ.APP --project AYYUAZ.APP.Infrastructure
```

### Problem: Database connection
**H?ll**: `appsettings.json`-da connection string yoxlay?n

### Problem: JWT token al?nm?r
**H?ll**: 
1. Admin user yarad?l?b-yarad?lmad???n? yoxlay?n
2. Password d?zg?n oldu?unu t?stiq edin
3. Log output-lar? yoxlay?n