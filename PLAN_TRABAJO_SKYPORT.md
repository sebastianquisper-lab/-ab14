# Plan de trabajo colaborativo: SkyPort Manager

## 1. Propósito

Este informe convierte el repositorio actual en un plan de evolución hacia un sistema de gestión aeroportuaria con arquitectura limpia y puertos/adaptadores (hexagonal). Está pensado para cuatro integrantes y diez commits pequeños, revisables y con responsabilidad clara.

**No se modificó código.** Este documento es únicamente una guía para ejecutar el trabajo después.

## 2. Evidencia revisada y límites

El workspace `C:\lab11` contiene la solución `lab11.sln` y cinco proyectos:

- `SQR.Domain`: entidades actuales (`User`, `Role`, `UserRole`, `Ticket`, `Response`) y puertos (`IGenericRepository`, `IUnitOfWork`, `IJwtTokenService`).
- `SQR.Applicattion`: casos de uso MediatR para usuarios, roles, tickets y respuestas.
- `SQR.Infrastructure`: repositorios, Unit of Work, JWT y registro de dependencias.
- `SQR.Persistence`: EF Core, Pomelo MySQL y `TicketSystemContext`.
- `SQR.API`: controladores REST, Swagger, autenticación y composición de servicios.

`TicketSystemContext` configura MySQL, claves, relaciones, borrado y valores por defecto de fechas. El flujo actual pasa de controladores a MediatR/casos de uso, puertos del dominio, adaptadores de infraestructura y persistencia EF Core. Esto ofrece una base razonable para conservar los límites por capas mientras se reemplaza el dominio de tickets por el aeropuerto.

**El PDF indicado existe en `D:\Definición del Proyecto Final – SkyPort Manager (1).pdf`, pero no se pudo extraer su contenido en este entorno porque no está disponible `pdfplumber` ni `pdftotext`.** Por tanto, no atribuyo requisitos concretos al PDF. Antes de programar, la persona coordinadora debe abrirlo y convertir sus requisitos en una lista aprobada. Los prompts 1 y 2 incluyen ese paso de forma explícita.

Tampoco se encontró `.git` en `C:\lab11`; por ello no se pudo confirmar historial, rama ni commits previos. Antes de dividir trabajo, inicialicen/ubiquen el repositorio Git correcto y confirmen el estado compartido. No hagan `git init` ni reescriban historia si la carpeta pertenece a otro repositorio.

## 3. Diagnóstico de partida

- El sistema actual se llama SQR en nombres, rutas y Swagger; el dominio funcional es quejas/reclamos, no vuelos/aeropuerto.
- La arquitectura ya separa API, Application, Domain, Infrastructure y Persistence. `Infrastructure` referencia `Persistence`; `API` compone Application e Infrastructure.
- Las entidades Domain actuales usan anotaciones de EF Core y representan tablas existentes. Si el requisito académico pide un Domain independiente de frameworks, habrá que decidir expresamente si se conserva el mapeo actual o se separan modelos de persistencia.
- El login compara `PasswordHash` con el texto recibido directamente; esto no es hashing seguro. Los prompts lo señalan para corregir, sin filtrar secretos.
- `appsettings.json` contiene una cadena de conexión y una clave JWT con apariencia de valores de desarrollo. No se deben copiar a prompts públicos, commits ni documentación; mover secretos a configuración local/variables de entorno y rotar si son credenciales reales.
- Los paquetes citados usan versión 9.0.0 mientras proyectos apuntan a `net10.0`; confirmar compatibilidad y SDK instalado antes de actualizar versiones.
- `TicketSystemContext` y las clases actuales son evidencia del patrón EF existente, no una especificación de las tablas de SkyPort.

## 4. Propuesta de dominio aeroportuario

Usen únicamente los módulos que el PDF confirme. Como mapa inicial para discutir, un sistema de aeropuerto puede separar:

1. **Vuelos:** código/número, origen, destino, horario, estado y avión asignado.
2. **Aviones:** matrícula, modelo, capacidad y estado operativo.
3. **Puertas y terminales:** ubicación y disponibilidad para asignaciones.
4. **Pasajeros:** datos mínimos necesarios y validación de identidad según alcance.
5. **Reservas/boletos:** pasajero, vuelo, asiento, código y estado; impedir duplicidad de asiento.
6. **Embarque:** asignación de puerta, estado y registro de abordaje.
7. **Usuarios/roles:** autenticación y autorización de personal/administración, solo según requisitos.

Eviten intentar implementar todo a la vez. El MVP se determina con la rúbrica/PDF: actores, flujos obligatorios, restricciones y pantallas/endpoints requeridos.

## 5. Límites arquitectónicos que deben respetarse

Flujo recomendado:

`API (adaptador HTTP) -> Application (casos de uso) -> Domain (reglas y puertos)`

`Infrastructure/Persistence (adaptadores EF, MySQL, servicios externos) -> implementa puertos definidos hacia adentro`

Reglas de equipo:

- Application depende de Domain; Domain no debe depender de API, EF, MySQL ni Infrastructure.
- Los controladores validan/transportan HTTP y delegan; no contienen reglas de negocio ni consultas EF.
- Casos de uso expresan operaciones del negocio; reglas como evitar asiento duplicado o asignar un avión no disponible se validan en el dominio/caso de uso, no solo en el controlador.
- Puertos de repositorio y servicios necesarios se declaran hacia adentro; adaptadores se implementan afuera.
- `TicketSystemContext` pertenece a Persistence. No acoplar casos de uso a `DbContext`.
- DTOs de API no tienen que ser entidades EF. No exponer contraseñas, hashes ni navegación EF en respuestas.
- Toda persona debe revisar el árbol y los namespaces reales antes de crear archivos; conservar el nombre existente `SQR.Applicattion` durante el MVP para evitar una renombrada masiva incidental.

## 6. División entre cuatro personas

| Persona | Responsabilidad principal | Archivos/áreas típicas | Evitar |
|---|---|---|---|
| A - Coordinación e integración | Leer PDF, definir MVP, convenciones, contratos de API, integración y releases | documento de requisitos, `SQR.API`, integración de ramas | cambiar reglas internas de otros módulos sin coordinar |
| B - Dominio y Application | Entidades/reglas/puertos y casos de uso MediatR | `SQR.Domain`, `SQR.Applicattion/UseCases` | EF Core, controladores, SQL |
| C - Persistencia | Contexto, configuración EF, migraciones/seed si la rúbrica lo pide | `SQR.Persistence` | reglas de negocio o endpoints |
| D - Infraestructura y seguridad | repositorios, Unit of Work/adaptadores, JWT/configuración | `SQR.Infrastructure`, configuración segura | modificar API o dominio sin acuerdo |

La persona A hace de integradora, pero también toma un módulo pequeño (por ejemplo contratos/endpoints de vuelos); no debe quedar con solo tareas administrativas. Cada PR/commit debe revisarlo otra persona. En equipo pequeño, alternen revisión para que cada cambio tenga autor y revisor distintos.

## 7. Cómo coordinar commits sin pisarse

1. Abran una rama base compartida y registren `git status` y `git log --oneline` antes de empezar. Protejan el trabajo ya existente.
2. Acuerden el alcance/MVP a partir del PDF y nombren explícitamente las entidades, estados, endpoints y reglas.
3. Cada commit se hace en una rama corta desde la integración más reciente; no permitan que dos agentes editen simultáneamente los mismos archivos.
4. Para los prompts 2-10, la persona coordinadora puede asignar commits a distintas personas, pero debe integrar en orden (o rebasar/cherry-pick de manera controlada). Si trabajan todos sobre la misma carpeta/branch, no ejecuten cuatro agentes de código en paralelo.
5. Un commit = un resultado coherente y pequeño. No se exige inventar cambios para inflar el conteo; si un commit no puede compilar por depender del siguiente, documenten el orden de integración.
6. Mensaje recomendado: `feat(flights): add flight domain model`; `feat(bookings): prevent duplicate seat`; `docs: record airport MVP decisions`.
7. Antes de cerrar cada commit, revisar diff, archivos incluidos, configuración sensible y resultado de compilación disponible. No incluir `bin/`, `obj/`, secretos, dumps ni archivos personales.
8. Si la carpeta no tiene Git, confirmar primero con el equipo cuál es el remoto/carpeta correcta. No cambiar historial ni inicializar Git a ciegas.

## 8. Secuencia de 10 prompts (un prompt produce como máximo un commit)

Copia cada bloque a una IA de programación en el repo correcto. Cada prompt pide primero inspeccionar y después limitar el cambio a su alcance. Si falta un dato de negocio del PDF, debe registrar la pregunta y no inventar la regla.

### Prompt 1 - Requisitos y plan de MVP (Persona A)

> Inspecciona el repositorio y lee completo `D:\Definición del Proyecto Final – SkyPort Manager (1).pdf`. No edites código. Resume requisitos funcionales, actores, reglas, entregables, restricciones tecnológicas y criterios de aceptación; distingue lo explícito de tus inferencias. Contrástalo con la solución existente (SQR.Domain, SQR.Applicattion, SQR.Infrastructure, SQR.Persistence, SQR.API y TicketSystemContext). Propón un MVP priorizado, un mapa de entidades/relaciones y decisiones pendientes. Guarda el resultado en `docs/skyport-requirements.md` y realiza solo un commit documental `docs: define SkyPort MVP from project brief`. No inventes alcance si el PDF no lo dice. No incluyas secretos de configuración.

### Prompt 2 - Modelo de dominio aeroportuario (Persona B)

> Lee `docs/skyport-requirements.md` y verifica las entidades/namespaces actuales. Implementa únicamente el modelo de dominio mínimo aprobado para el primer flujo aeroportuario del MVP (prioridad: vuelos, incluyendo estado y asignación si el documento lo requiere). Define invariantes de negocio en Domain y puertos solo cuando Application los necesite. No agregues EF, controladores, repositorios concretos ni entidades no justificadas. Conserva la organización/nombres actuales del proyecto. Añade una nota breve de decisiones si hay ambigüedades, revisa el diff y crea un commit: `feat(domain): add airport flight model`.

### Prompt 3 - Persistencia EF para el dominio (Persona C)

> Implementa en `SQR.Persistence` la persistencia EF Core para las entidades aprobadas en Prompt 2 y requisitos del PDF: DbSet, claves, longitudes, índices únicos, relaciones y restricciones necesarias. Sigue el patrón de `TicketSystemContext`, pero no alteres tablas SQR existentes sin necesidad documentada. Decide con el equipo si se requiere migración o solo mapeo según la base de datos disponible; nunca inventes una cadena real. No muevas reglas de negocio aquí. Un solo commit: `feat(persistence): map airport flight model`.

### Prompt 4 - Puertos y casos de uso de vuelos (Persona B)

> Implementa en `SQR.Applicattion` los casos de uso del MVP para consultar vuelos y registrar/actualizar un vuelo según el PDF. Usa MediatR y puertos de Domain; valida campos y reglas aprobadas en el límite Application/Domain. Define resultados previsibles para no encontrado y conflicto. No toques controladores, DbContext ni adaptadores concretos. No expongas entidades de persistencia como contratos HTTP. Un commit: `feat(application): add flight use cases`.

### Prompt 5 - Adaptadores de persistencia y DI (Persona D)

> Implementa adaptadores de Infrastructure para los puertos de vuelos definidos en Application/Domain, registra dependencias en `AddInfrastructure` y conserva Unit of Work/repository solo si encajan con el contrato requerido. Mantén la dependencia hacia adentro y no añadas DbContext a los handlers. Comprueba el diff de referencias entre proyectos. Un commit: `feat(infrastructure): implement flight persistence port`.

### Prompt 6 - API REST para vuelos (Persona A)

> Añade endpoints REST para los casos de uso de vuelos aprobados en `docs/skyport-requirements.md`. Controladores delgados, DTOs de request/response, códigos HTTP coherentes (201 al crear, 200 al consultar, 404 no encontrado, 400 validación y 409 conflicto si aplica), cancellation token y documentación Swagger. No inventes autorización: aplica roles solo si los requisitos lo definen. No cambies autenticación existente ni implementes lógica de negocio en el controlador. Un commit: `feat(api): expose airport flight endpoints`.

### Prompt 7 - Aviones y asignación (Persona B + C coordinados)

> Implementa el módulo de avión únicamente si el PDF lo exige para el MVP. Primero acuerda contrato/domain con Persona B; luego mapea persistencia. Añade la regla aprobada para impedir asignar aviones inexistentes o no disponibles y las restricciones persistentes pertinentes. No edites simultáneamente el mismo archivo: divide Domain/Application (B) y Persistence (C) en ramas o commits secuenciales. Deja un commit integrado por la persona coordinadora: `feat(aircraft): manage aircraft assignment`.

### Prompt 8 - Reservas y asiento (Persona B + D coordinados)

> Implementa el flujo de reserva/boletos que el PDF requiera: pasajero, vuelo, asiento y estado solo si forman parte del alcance. La regla de no duplicar asiento debe protegerse en el caso de uso y con una restricción/índice persistente cuando sea posible; manejar carreras/conflicto con respuesta documentada. Coordina contratos antes de codificar y divide archivos para evitar conflictos. No guardes datos de tarjeta ni información personal innecesaria. Un commit integrado: `feat(bookings): create reservations with seat uniqueness`.

### Prompt 9 - Seguridad y configuración (Persona D)

> Revisa autenticación/autorización existente y los requisitos de acceso del PDF. Nunca imprimas ni copies valores secretos de `appsettings.json`. Sustituye secretos de desarrollo por configuración externa documentada, valida ausencia de claves al iniciar y aplica políticas/roles solo donde el requisito los pida. Revisa el login: si `PasswordHash` se compara en texto plano, implementa hashing/verificación con una solución apropiada sin exponer contraseñas ni hashes en DTOs. Evita romper usuarios de prueba sin documentar la transición. Un commit: `fix(security): secure credentials and airport access`.

### Prompt 10 - Integración de MVP y documentación (Persona A, revisión del equipo)

> Integra los commits del MVP siguiendo los requisitos aprobados. Revisa referencias de proyectos y separación hexagonal, rutas Swagger, estados/validaciones consistentes, configuración segura y que el flujo principal funcione de extremo a extremo. No amplíes el alcance. Actualiza `README.md` con propósito, módulos, arquitectura, requisitos previos, configuración local sin secretos, cómo ejecutar y ejemplos de endpoints. Ejecuta solo las verificaciones que el equipo haya acordado y registra su resultado; no ocultes fallos. Revisa `git diff` y crea un único commit final: `docs: document SkyPort MVP and setup` (si hay correcciones de código imprescindibles, sepáralas en un commit fix explícito en vez de mezclarlas con docs).

## 9. Criterios para aceptar cada commit

- Cumple un requisito trazable del PDF o una decisión registrada en `docs/skyport-requirements.md`.
- El diff contiene solo archivos de su tarea; no reemplaza archivos completos para hacer cambios mínimos.
- Compila con el SDK objetivo acordado y las dependencias del repo; fallos previos o requisitos de base de datos quedan documentados.
- Las reglas de negocio no viven solo en API/EF.
- Los cambios de persistencia protegen unicidad/relaciones relevantes y no incluyen datos/secrets reales.
- La persona autora deja un resumen: qué cambió, qué requisito atiende, commit hash y limitaciones; otra persona revisa.

## 10. Recomendación práctica de arranque

1. El coordinador abre el PDF y completa el Prompt 1; el resto espera el documento de alcance.
2. Confirmen el repositorio Git correcto y publiquen el documento de requisitos.
3. Implementen el primer flujo completo de punta a punta: modelo -> persistencia -> caso de uso -> adaptador -> endpoint.
4. Solo después agreguen avión/reserva/embarque que la rúbrica realmente exija.
5. Mantengan los 10 prompts como máximo, no como obligación de crear código artificial. Si el PDF requiere módulos distintos, cambien prompts 7-9 antes de empezar.

## Anexo A - Guía de lectura archivo por archivo y flujo de dependencias

Este anexo precisa cómo se conectan los archivos existentes. “Puerto” es el contrato que necesita la capa interna; “adaptador” es la implementación técnica que conecta ese contrato con EF, HTTP o JWT.

### Composición de dependencias en tiempo de ejecución

`Program.cs` no llama manualmente a cada handler. Registra controladores y llama a dos extensiones: `AddApplication()` (registra handlers de MediatR del assembly de Application) y `AddInfrastructure(configuration)` (registra DbContext, repositorios, UnitOfWork y JWT). ASP.NET construye el contenedor al arrancar. Cuando un controlador recibe `IMediator`, DI entrega MediatR; cuando un handler recibe `IUnitOfWork`, DI entrega `UnitOfWork`; cuando UnitOfWork necesita el contexto, DI entrega `TicketSystemContext` configurado con connection string y provider MySQL.

| Archivo/contrato | Quién lo consume | Qué hace el consumidor | Quién lo implementa/provee |
|---|---|---|---|
| `Program.cs` | Host ASP.NET | Construye el pipeline HTTP y llama registros | Extensiones de Application e Infrastructure |
| `ApplicationServicesExtensions.cs` | `Program.cs` | Registra handlers localizando assembly por `ApplicationAssemblyMarker` | MediatR resuelve `IRequestHandler` al recibir Command/Query |
| `IUnitOfWork` | Handlers; también `AuthController` | Solicita repositorios y persiste unidad de trabajo | `Infrastructure/Repositories/Implement/UnitOfWork.cs` |
| `IGenericRepository<T>` | UnitOfWork y handlers a través de sus propiedades | CRUD/lecturas genéricas | `GenericRepository<T>` usa `DbSet<T>` |
| `TicketSystemContext` | Repositorios/EF y contenedor | Mantiene el tracking/modelo relacional y guarda cambios | EF Core; opciones son registradas en `AddInfrastructure` |
| `IJwtTokenService` | `AuthController` | Solicita token sin acoplarse a la biblioteca concreta | `JwtTokenService` |
| `JwtSettings` | `JwtTokenService` y configuración DI | Lee Key/Issuer/Audience/expiración tipados | `Configure<JwtSettings>` enlaza sección de configuración |

### Qué hace cada handler actual y a qué archivo conduce

| Caso de uso | Entrada HTTP actual | Handler | Puerto/repositorio | Efecto esperado |
|---|---|---|---|---|
| Listar tickets | `TicketsController.GetAll` | `GetTicketsHandler` | `IUnitOfWork.Tickets.GetAllAsync` | EF hace lectura sin tracking y API devuelve 200 |
| Obtener ticket | `TicketsController.GetById` | `GetTicketByIdHandler` | `Tickets.GetByIdAsync` | handler devuelve entidad o null; controller traduce null a 404 |
| Crear ticket | `TicketsController.Create` | `CreateTicketHandler` | `Tickets.AddAsync`, `SaveChangesAsync` | entidad nueva, estado inicial y timestamp; controller devuelve 201 |
| Cambiar estado | `TicketsController.UpdateStatus` | `UpdateTicketStatusHandler` | `Tickets.GetByIdAsync`, Update, SaveChanges | handler verifica existencia y estado permitido; API devuelve 204/404 |
| Eliminar ticket | `TicketsController.Delete` | `DeleteTicketHandler` | GetById, Delete, SaveChanges | elimina si existe; 204/404 |
| Crear/listar usuario | `UsersController` | comandos/queries User | `Users` | CRUD de usuarios; creación requiere revisar hashing y DTOs |
| Login | `AuthController.Login` | `LoginHandler` y después acceso directo a UoW | `Users`, `UserRoles`, `Roles`, `IJwtTokenService` | valida usuario, arma roles, emite JWT; controlador contiene coordinación que sería más limpia como caso de uso completo |
| Crear/listar/atribuir rol | `RolesController` | handlers Role | `Roles`, `Users`, `UserRoles` | persistencia de rol/relación; asignación consulta ambos lados |
| Crear/listar respuesta | `ResponsesController` | handlers Response | `Responses` | respuesta asociada a ticket y usuario/respondedor |

En el esquema actual, `GenericRepository<T>.GetAllAsync` y `FindAsync` aplican `AsNoTracking()`, mientras que `GetByIdAsync` usa `FindAsync` (que puede devolver entidad ya trackeada por el contexto). Por eso el patrón de lectura/tracking no es idéntico en todos los métodos. `Update` marca entidad y `Delete` la remueve; `SaveChangesAsync` solo sucede cuando el handler lo invoca.

### Relaciones de datos actuales

Las navegaciones y el mapeo muestran este grafo SQR: un `User` tiene muchos `Ticket`; un `Ticket` tiene muchas `Response`; un `User` puede responder muchos `Response`; `User` y `Role` se conectan muchos-a-muchos mediante `UserRole`, que además registra `AssignedAt`. En términos relacionales: `tickets.UserId -> users.UserId`; `responses.TicketId -> tickets.TicketId`; `responses.ResponderId -> users.UserId`; `user_roles.UserId -> users.UserId`; `user_roles.RoleId -> roles.RoleId`. `UserRole` usa clave compuesta, las columnas de username/email/role name se declaran únicas en atributos, y varias FK tienen índices definidos por atributos. Este grafo es lo que el `TicketSystemContext` actual sabe persistir; no implica relación alguna del futuro dominio aeroportuario.

### Nota sobre limpieza de arquitectura

Aunque los handlers no dependen directamente de EF, las clases Domain importan `Microsoft.EntityFrameworkCore` y contienen atributos EF como `[Index]`. Esto significa que la flecha conceptual Domain <- Persistence no es completamente independiente en el código: Domain tiene referencia de paquete a EF. Si el curso exige independencia estricta, hay dos caminos a discutir: (a) aceptar temporalmente el acoplamiento del scaffolding y no expandirlo, o (b) mover configuración relacional a `IEntityTypeConfiguration<T>` en Persistence y quitar anotaciones EF del Domain en una tarea transversal revisada. No ejecutar esa refactorización a mitad de una funcionalidad ni mezclarla con nuevos módulos sin acuerdo.

## Anexo B - Condición que prevalece sobre ejemplos de módulos

Los ejemplos previos de aviones/reservas/puertas/vuelos son posibilidades de dominio y **no requisitos confirmados**. La secuencia de diez prompts es una plantilla de trabajo: antes de usar cualquier prompt de implementación, reemplazar el nombre genérico por la primera capacidad que el PDF realmente exija. Si el PDF no se puede leer, solo se puede completar análisis del repo y arquitectura; no se debe iniciar implementación de SkyPort inventando el dominio. Este punto prevalece sobre cualquier título de ejemplo de módulo en los prompts.

## Anexo C - Precisión del estado Git y verificaciones

La carpeta inspeccionada `C:\lab11` no mostró `.git`; por ello no es posible afirmar “working tree limpio”, rama actual o cantidad de commits. El informe anterior se generó sin ejecutar Git mutante y sin editar código. La inspección fue de lectura. No se ejecutaron build ni tests; este informe no declara que compile ni que pase pruebas.
