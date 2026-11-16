# SkyPort Manager — requisitos y MVP

Fecha de inspección: 2026-11-09.  
Workspace: `C:\Users\Tecsup\Downloads\lab11\lab11`.  
Este documento corresponde al Prompt 1 del plan de trabajo. No modifica código de la solución.

## 1. Fuente del enunciado

El plan indica el PDF `D:\Definición del Proyecto Final – SkyPort Manager (1).pdf`.

**Ese archivo no está en esta máquina.** Se buscó en `D:\`, `C:\Users\Tecsup\Downloads`, Escritorio, Documentos y la raíz de `E:\`. `D:\` solo contiene la carpeta `VM`. Las capturas PNG del workspace son diapositivas de configuración de MediatR, no el brief de SkyPort.

Por eso este documento **no atribuye requisitos funcionales, actores, reglas, entregables ni criterios de aceptación a SkyPort**. Esas secciones quedan vacías a propósito. Completarlas exige el PDF (o una copia colocada en el repo).

Leyenda usada abajo:

- **Explícito:** está en el código o en el plan de trabajo como hecho observado.
- **Inferencia:** lectura del código que no es una especificación escrita.
- **No confirmado:** no aparece en ninguna fuente leída. No se usa como alcance.

## 2. Requisitos del PDF

| Tema | Estado |
|---|---|
| Requisitos funcionales de SkyPort | No confirmado. El PDF no se pudo leer. |
| Actores | No confirmado. |
| Reglas de negocio | No confirmado. |
| Entregables y rúbrica | No confirmado. |
| Restricciones tecnológicas exigidas por el curso | No confirmado. |
| Criterios de aceptación | No confirmado. |
| Pantallas o endpoints obligatorios | No confirmado. |

**MVP priorizado de SkyPort:** no se propone. Vuelos, aviones, puertas, pasajeros, reservas y embarque aparecen en el plan solo como ejemplos de dominio. El anexo B del plan dice que no son requisitos y que, sin el PDF, no se debe implementar SkyPort inventando el dominio.

## 3. Solución existente (explícito)

`lab11.sln` tiene cinco proyectos, todos en `net10.0`:

| Proyecto | Rol observado |
|---|---|
| `SQR.Domain` | Entidades y puertos. Referencia EF Core 9.0.0. |
| `SQR.Applicattion` | Casos de uso MediatR 12.4.1. El nombre del proyecto conserva la doble `t`. Depende solo de Domain. |
| `SQR.Infrastructure` | Repositorios, Unit of Work, JWT y `AddInfrastructure`. Depende de Domain y Persistence. |
| `SQR.Persistence` | `TicketSystemContext`, Pomelo MySQL 9.0.0. Depende de Domain. |
| `SQR.API` | Controladores, Swagger, composición. Depende de Application e Infrastructure. |

El título de Swagger en `Program.cs` es **SQR API — Sistema de Quejas y Reclamos**. El dominio implementado es tickets, no aeropuerto.

Flujo observado:

`Controlador -> IMediator -> handler en Application -> IUnitOfWork / IGenericRepository -> GenericRepository + TicketSystemContext (MySQL)`.

`Program.cs` llama a `AddApplication()` y `AddInfrastructure(configuration)`. En desarrollo habilita Swagger. El pipeline usa HTTPS, autenticación JWT y autorización.

### 3.1 Entidades y relaciones actuales

Grafo persistido por `TicketSystemContext` (no es el dominio futuro):

- `User` 1—N `Ticket` (`tickets.user_id`).
- `Ticket` 1—N `Response` (`responses.ticket_id`).
- `User` 1—N `Response` como respondedor (`responses.responder_id`).
- `User` N—N `Role` mediante `UserRole` (clave compuesta `user_id` + `role_id`, más `assigned_at`).

Unicidad declarada en atributos: `users.username`, `users.email`, `roles.role_name`.

Estados de ticket que el caso de uso acepta, en `UpdateTicketStatusHandler`: `abierto`, `en progreso`, `cerrado`. Al pasar a `cerrado` se escribe `ClosedAt` en UTC; en otro estado se limpia. Esto es una regla del código SQR, no una regla de SkyPort.

Fechas `created_at` / `assigned_at` usan `CURRENT_TIMESTAMP` en el modelo EF. Collation del modelo: `utf8mb4_0900_ai_ci`. El servidor MySQL declarado en DI es versión 9.1.0.

Las entidades de Domain llevan anotaciones de EF (`Table`, `Column`, `Index`, `ForeignKey`, `InverseProperty`). Domain no es independiente del framework.

### 3.2 API actual

Rutas bajo `api/[controller]`. JWT Bearer está documentado en Swagger.

| Recurso | Métodos | Autorización observada |
|---|---|---|
| `api/Auth` | `POST login` | Sin `[Authorize]` en el controlador. |
| `api/Users` | GET lista, GET por id, POST, DELETE | GET y DELETE llevan `[Authorize]`. El POST de alta no lo lleva en el método. |
| `api/Roles` | GET, POST, `POST assign` | Controlador con `[Authorize]`. |
| `api/Tickets` | GET, GET por id, POST, `PATCH {id}/status`, DELETE | Controlador con `[Authorize]`. |
| `api/Responses` | `GET ticket/{ticketId}`, POST | Controlador con `[Authorize]`. |

No hay políticas por rol en esos atributos: el filtro exige un usuario autenticado, no un nombre de rol concreto.

### 3.3 Seguridad observada, sin valores

- `appsettings.json` contiene cadena de conexión y sección JWT. Este documento no copia esos valores.
- `AddInfrastructure` falla al arrancar si falta `DefaultConnection` o la sección JWT.
- `LoginHandler` compara `user.PasswordHash` con el texto recibido (`!=`). No hay verificación de hash.
- `CreateUserCommand` guarda `PasswordHash` tal como llega en el comando.

### 3.4 Otras observaciones

- `ClosedXML` 0.105.1 está referenciado en los cinco proyectos. No hay uso de `XLWorkbook` ni `ClosedXML` en código `.cs`.
- Los paquetes ASP.NET / EF citados están en 9.0.0 y el TFM es `net10.0`.
- No hay carpeta `.git` en el workspace. No se inicializó un repositorio: el plan pide no hacer `git init` sin confirmar el remoto del equipo.
- No se ejecutó build ni tests en esta pasada. Este documento no afirma que la solución compile.

## 4. Contraste con un futuro SkyPort

| Lo que ya sirve | Lo que no se puede decidir todavía |
|---|---|
| Separación API / Application / Domain / Infrastructure / Persistence. | Qué módulos reemplazan a Ticket y Response. |
| MediatR, puertos `IGenericRepository` y `IUnitOfWork`, JWT ya cableados. | Si usuarios y roles se reutilizan o se redefinen. |
| Patrón de `TicketSystemContext` (claves, FK, defaults, collation). | Tablas, índices y si hace falta migración. |
| Controladores delgados que traducen null a 404 en varios flujos de ticket. | Contratos HTTP, códigos y autorización del aeropuerto. |

Conservar el nombre `SQR.Applicattion` durante el MVP es una decisión del plan de trabajo, no un requisito del PDF.

## 5. Mapa de entidades SkyPort

No hay mapa aprobado. Dibujar vuelos, aviones o reservas aquí sería inventar alcance.

## 6. Decisiones pendientes (bloquean el Prompt 2)

1. Entregar el PDF o pegar su texto para llenar la sección 2 con citas literales.
2. Confirmar el repositorio Git compartido antes de cualquier commit. El commit documental `docs: define SkyPort MVP from project brief` **no se hizo** porque no hay `.git`.
3. Listar, a partir del PDF, actores, flujos obligatorios, estados, reglas y endpoints. Solo después se nombra el primer módulo.
4. Decidir si Domain puede seguir con anotaciones EF o si el curso exige quitarlas. No mezclar esa refactorización con el primer módulo.
5. Decidir qué hacer con el dominio SQR (convivir, reemplazar o retirar) cuando el PDF lo indique.
6. Sacar secretos de `appsettings.json` a configuración local. Eso es el Prompt 9, no esta entrega.
7. Confirmar SDK .NET 10 y compatibilidad de los paquetes 9.0.0 antes de subir versiones.

## 7. Alcance de esta entrega

- Se creó solo este archivo.
- No se editó código, configuración ni historial Git.
- No se copiaron secretos.
- El MVP de SkyPort sigue sin definir hasta leer el enunciado.
