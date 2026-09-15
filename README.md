# TestTaskApi

REST API для керування конференц-залами, додатковими послугами та бронюваннями

Проєкт реалізований на ASP.NET Core 8, Entity Framework Core та PostgreSQL

## Функціональність

Керування конференц-залами

Керування додатковими послугами

Прив'язка додаткових послуг до конкретного залу

Перевірка доступності залу за датою, часом та кількістю людей

Створення та перегляд бронювань

Перевірка перетинів бронювань

Розрахунок вартості бронювання залежно від часу

Перевірка доступності вибраних послуг для конкретного залу

JWT-аутентифікація

Swagger / OpenAPI

Автоматичне застосування EF Core міграцій при запуску

Початкове наповнення бази даних тестовими даними

Звітні endpoint-и

## Технології

.NET 8

ASP.NET Core Web API

Entity Framework Core 8

PostgreSQL 15

Npgsql

JWT Bearer Authentication

Swagger / OpenAPI

Docker

Docker Compose

## Структура проєкту

```text
TestTaskApi/
├── Controllers/
│   ├── AuthController.cs
│   ├── BookingController.cs
│   ├── ReportsController.cs
│   ├── RoomController.cs
│   └── UtilityController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── DTOs/
│   ├── Booking/
│   ├── Room/
│   └── Utility/
│
├── Entities/
│   ├── Booking.cs
│   ├── Room.cs
│   └── Utility.cs
│
├── Services/
│   ├── BookingService.cs
│   ├── PricingService.cs
│   └── ...
│
├── Migrations/
├── Program.cs
├── appsettings.json
└── Dockerfile
```

## Запуск

Для запуску необхідний Docker Desktop з підтримкою Docker Compose

Окреме встановлення PostgreSQL не потрібне, оскільки база даних запускається у Docker

### Клонування репозиторію

```bash
git clone https://github.com/pro100whp/TestTaskApi.git
cd TestTaskApi
```

### Налаштування змінних середовища

У корені проєкту необхідно створити файл `.env`

Приклад конфігурації

```env
DB_PASSWORD=your_database_password
DB_USER=postgres
DB_NAME=conference_db

ADMIN_USERNAME=admin
ADMIN_PASSWORD=your_admin_password
JWT_KEY=your_long_random_secret_key
```

Файл `.env` не повинен містити реальні секрети у Git репозиторії

### Запуск

У корені проєкту виконується

```bash
docker compose up --build
```

Docker Compose запускає API та PostgreSQL

PostgreSQL перед запуском API проходить healthcheck, після чого застосунок може підключитися до бази даних

### Перевірка контейнерів

```bash
docker compose ps
```

PostgreSQL повинен мати статус `healthy`

Перегляд логів API

```bash
docker compose logs --tail=100 testtaskapi
```

Зупинка контейнерів

```bash
docker compose down
```

## Порти

API доступний на

```text
http://localhost:8080
```

PostgreSQL доступний з локального комп'ютера на

```text
localhost:5433
```

При цьому всередині Docker мережі API підключається до PostgreSQL через

```text
Host=db
Port=5432
```

Порт `5433` використовується для доступу до PostgreSQL з хостової системи та дозволяє уникнути конфлікту з локальним PostgreSQL на стандартному порту `5432`

## Swagger

Swagger доступний за адресою

```text
http://localhost:8080/swagger
```

Swagger використовується для перегляду документації API та виконання запитів

У Development середовищі HTTPS redirect вимкнений, тому API працює через HTTP на порту `8080`

## Аутентифікація

API використовує JWT Bearer Authentication

Для отримання токена використовується

```http
POST /Auth/login
```

Облікові дані адміністратора задаються через змінні середовища

```env
ADMIN_USERNAME=admin
ADMIN_PASSWORD=your_admin_password
JWT_KEY=your_long_random_secret_key
```

Після отримання JWT токен можна передати через кнопку `Authorize` у Swagger

Формат

```text
Bearer <token>
```

Основні API endpoint-и захищені авторизацією

## Початкові дані

При першому запуску база даних автоматично заповнюється тестовими даними

### Конференц-зали

| Назва  | Місткість | Базова ціна за годину |
| ------ | --------: | --------------------: |
| Room A |        50 |                  2000 |
| Room B |       100 |                  3500 |
| Room C |        30 |                  1500 |

### Додаткові послуги

| Назва     | Базова ціна |
| --------- | ----------: |
| Projector |         500 |
| Wi-Fi     |         300 |
| Sound     |         700 |

## Бронювання

При створенні бронювання задаються зал, дата та час початку, дата та час завершення, кількість людей та вибрані додаткові послуги

Перед створенням бронювання виконується перевірка коректності часових параметрів

Час початку повинен бути меншим за час завершення

Кількість людей не повинна перевищувати місткість залу

Вибрані додаткові послуги повинні існувати

Вибрані додаткові послуги повинні бути доступні для конкретного залу

Зал не повинен мати іншого бронювання з перетином заданого часового інтервалу

Для перевірки перетину використовується умова

```text
existing.StartDate < requested.EndDate
AND
existing.EndDate > requested.StartDate
```

Такий підхід дозволяє створювати сусідні бронювання без конфлікту

Наприклад

```text
10:00 - 12:00
12:00 - 14:00
```

Такі бронювання не перетинаються

## Одночасні бронювання

Перевірка конфлікту виконується на рівні application logic перед створенням бронювання

Це захищає від звичайних повторних та пересічних бронювань

При цьому поточна реалізація не усуває race condition у випадку, коли два одночасні запити проходять перевірку доступності до того, як будь-який з них буде збережений у базі даних

Для production-рівня додатково необхідний database-level механізм або транзакційна блокування, яка гарантує неможливість створення двох конфліктних бронювань одночасно

## Перевірка доступності

API дозволяє отримати список залів, доступних для заданого часового інтервалу та кількості людей

При перевірці враховується місткість залу та наявність бронювань, які перетинаються із заданим інтервалом

## Розрахунок вартості

Базова ціна залу задається за одну годину

До вартості можуть додаватися вибрані додаткові послуги

Ціна залу залежить від часу бронювання

| Час         |           Тариф |
| ----------- | --------------: |
| 06:00–09:00 |            -10% |
| 09:00–12:00 | стандартна ціна |
| 12:00–14:00 |            +15% |
| 14:00–18:00 | стандартна ціна |
| 18:00–23:00 |            -20% |
| 23:00–06:00 | стандартна ціна |

Для розрахунку використовується окремий `PricingService`

Наприклад, для залу з базовою ціною 2000 за годину

```text
06:00–09:00  → 1800
09:00–12:00  → 2000
12:00–14:00  → 2300
18:00–23:00  → 1600
23:00–06:00  → 2000
```

## Робота з датою та часом

Дата та час бронювань обробляються в UTC

При передачі значень через API рекомендується використовувати UTC або значення з явним часовим offset

Це дозволяє уникнути проблем із часовими зонами та переходом між літнім і зимовим часом

## Міграції та база даних

При запуску застосунку автоматично виконується

```csharp
db.Database.Migrate();
```

Тому після запуску Docker Compose не потрібно вручну застосовувати EF Core migrations

Послідовність запуску виглядає так

```text
PostgreSQL запускається
        ↓
PostgreSQL проходить healthcheck
        ↓
API підключається до бази
        ↓
EF Core застосовує міграції
        ↓
Створюються початкові дані
        ↓
Запускається API
```

Docker Compose використовує healthcheck PostgreSQL

```yaml
healthcheck:
  test: ["CMD-SHELL", "pg_isready -U ${DB_USER} -d ${DB_NAME}"]
  interval: 5s
  timeout: 5s
  retries: 10
  start_period: 10s
```

API запускається після того, як база даних переходить у стан `healthy`

```yaml
depends_on:
  db:
    condition: service_healthy
```

## API Endpoint-и

### Auth

```http
POST /Auth/login
```

Отримання JWT токена

### Rooms

```http
GET    /api/Room
GET    /api/Room/{id}
POST   /api/Room
PUT    /api/Room/{id}
DELETE /api/Room/{id}
```

Також доступні операції для роботи з послугами залу та перевірки доступності

### Utilities

```http
GET    /api/Utility
GET    /api/Utility/{id}
POST   /api/Utility
PUT    /api/Utility/{id}
DELETE /api/Utility/{id}
```

### Bookings

```http
GET  /api/Booking
POST /api/Booking
```

### Reports

API також містить endpoint-и для отримання звітної інформації

Повний список endpoint-ів, параметрів та моделей доступний у Swagger

```text
http://localhost:8080/swagger
```

## HTTP статуси

API використовує стандартні HTTP статуси для обробки результатів запитів

```text
200 OK
```

Запит виконано успішно

```text
201 Created
```

Ресурс успішно створено

```text
400 Bad Request
```

Передані некоректні дані

```text
401 Unauthorized
```

Користувач не авторизований

```text
404 Not Found
```

Необхідний ресурс не знайдено

```text
409 Conflict
```

Виявлено бізнес-конфлікт, наприклад конфлікт бронювання

## Docker

Основна конфігурація Docker Compose містить два сервіси

```text
testtaskapi
db
```

API використовує порт `8080`

PostgreSQL всередині Docker використовує порт `5432`

Для хостової системи PostgreSQL проброшений на порт `5433`

Автоматичний healthcheck PostgreSQL потрібен для того, щоб API не намагався виконати міграції до готовності бази даних

## Production considerations

Поточна конфігурація орієнтована на тестове завдання та локальний запуск

Для production-середовища доцільно додатково реалізувати database-level захист від race condition при одночасному створенні конфліктних бронювань

Також для production необхідно використовувати постійне сховище ключів ASP.NET Data Protection замість ephemeral keys

Секрети та JWT ключі повинні зберігатися через безпечне сховище секретів

Для production бажано додати автоматизовані unit та integration tests

Для необроблених винятків доцільно використовувати централізований exception handling middleware

HTTPS повинен бути налаштований відповідно до production environment

## Перевірка роботи

Після запуску

```bash
docker compose up --build
```

API повинен бути доступний за адресою

```text
http://localhost:8080
```

Swagger

```text
http://localhost:8080/swagger
```

Перевірка контейнерів

```bash
docker compose ps
```

Перегляд логів

```bash
docker compose logs --tail=100 testtaskapi
```

Зупинка

```bash
docker compose down
```

## License

Проєкт створений у рамках тестового завдання
