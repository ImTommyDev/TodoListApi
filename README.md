# TodoList API

## Descripción
API REST para la gestión de tareas personales, desarrollada con .NET 8 y PostgreSQL.

## 🚀 Tecnologías
- .NET 8
- Entity Framework Core
- PostgreSQL
- JWT (JSON Web Token) para autenticación

## 🛠 Instalación y configuración

### 1️⃣ Clonar el repositorio
```sh
git clone https://github.com/tu_usuario/todolist-api.git
cd todolist-api
```

### 2️⃣ Configurar la base de datos
Crea una base de datos en PostgreSQL y agrega la cadena de conexión en `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=TodoDB;Username=postgres;Password=tu_password"
}
```

### 3️⃣ Configurar JWT
Modifica `appsettings.json` con tu clave secreta:
```json
"JwtSettings": {
  "SecretKey": "clave_super_secreta",
  "Issuer": "todolistapi",
  "Audience": "todolistapi_users"
}
```

### 4️⃣ Ejecutar migraciones
```sh
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5️⃣ Ejecutar la API
```sh
dotnet run
```
La API estará disponible en:
🔗 https://localhost:5001/swagger/index.html

## 🔐 Autenticación
La API usa JWT para proteger los endpoints. Para acceder a recursos protegidos, primero registra un usuario y luego usa el token en las peticiones.

### 🛠️ Flujo de autenticación
1. **Registro**: `POST /api/auth/register`
2. **Login**: `POST /api/auth/login` → Devuelve un token JWT.
3. **Usar el token**: Agregar el token en el header de cada petición:
```sh
Authorization: Bearer <TOKEN>
```

## 📌 Endpoints

### 🔑 Autenticación (`/api/auth`)
| Método | Ruta      | Descripción                   |
|--------|----------|-------------------------------|
| POST   | /register | Registrar un nuevo usuario    |
| POST   | /login    | Iniciar sesión y obtener token |

### 📋 Gestión de Tareas (`/api/tareas`)
| Método | Ruta      | Descripción                                |
|--------|----------|--------------------------------------------|
| GET    | /        | Obtener todas las tareas del usuario      |
| GET    | /{id}    | Obtener una tarea específica              |
| POST   | /        | Crear una nueva tarea                     |
| PUT    | /{id}    | Actualizar una tarea existente            |
| DELETE | /{id}    | Eliminar una tarea                        |

📌 Importante:
- Solo el usuario dueño de la tarea puede acceder o modificarla.
- Para todas las operaciones, el usuario debe estar autenticado con JWT.

## 🛠 Pruebas en Postman
Puedes probar la API con Postman o cURL:

### 🔹 Registrar usuario
```sh
curl -X POST "http://localhost:5000/api/auth/register" \
     -H "Content-Type: application/json" \
     -d '{"nombre": "Juan", "email": "juan@mail.com", "password": "123456"}'
```

### 🔹 Iniciar sesión
```sh
curl -X POST "http://localhost:5000/api/auth/login" \
     -H "Content-Type: application/json" \
     -d '{"email": "juan@mail.com", "password": "123456"}'
```
📌 Respuesta esperada (Ejemplo):
```json
{
  "token": "eyJhbGciOiJIUzI1..."
}
```

### 🔹 Crear una tarea (con token)
```sh
curl -X POST "http://localhost:5000/api/tareas" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer <TOKEN>" \
     -d '{"titulo": "Aprender .NET", "descripcion": "Estudiar sobre API REST en .NET", "completada": false}'
```

### 🔹 Obtener tareas
```sh
curl -X GET "http://localhost:5000/api/tareas" \
     -H "Authorization: Bearer <TOKEN>"
```

## 🚀 Mejoras futuras
📌 Implementar paginación para la lista de tareas.
📌 Agregar recuperación de contraseña.
📌 Soporte para usuarios administradores.

## 📄 Licencia
Este proyecto está bajo la licencia MIT. ¡Puedes usarlo y modificarlo libremente! 🎉

