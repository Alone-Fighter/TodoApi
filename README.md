# Todo API

API مدیریت وظایف با ASP.NET Core 8 و SQL Server. این پروژه بر اساس تسک To-do List ریرا پیش می‌رود.

## وضعیت پیاده‌سازی

- زیرساخت SQL Server، مدل `TodoTask`، `TodoDbContext` و migration اولیه آماده است.
- برنامه در زمان اجرا migrationهای موجود را خودکار اعمال می‌کند.
- `GET /api/tasks` و `GET /api/tasks/{id}` برای دریافت وظایف پیاده‌سازی شده‌اند.
- `POST /api/tasks` وظیفه می‌سازد و اعتبارسنجی ورودی دارد.
- `PUT /api/tasks/{id}` و `DELETE /api/tasks/{id}` مراحل بعدی هستند.
- خطاهای مدیریت‌نشده و پاسخ‌های status خالی با Problem Details به‌صورت سراسری مدیریت می‌شوند.

## ساختار کد

```text
TodoApi/
├── Controllers/       # HTTP endpoints
├── Domain/Entities/   # TodoTask
├── DTOs/Tasks/        # مدل‌های ورودی/خروجی و اعتبارسنجی
├── Data/              # DbContext و EF Core migrations
└── Services/Tasks/    # منطق دسترسی و عملیات وظایف
```

در این پروژه‌ی کوچک، هر لایه پوشه‌ی جدا دارد و در یک پروژه‌ی .NET نگه‌داری می‌شود. `TodoDbContext` نقش Repository/Unit of Work مربوط به EF Core را دارد؛ بنابراین Repository عمومی جداگانه اضافه نشده است.

## پیش‌نیازها

- .NET 8 SDK
- SQL Server در دسترس از طریق اتصال تعریف‌شده در تنظیمات
- دسترسی کاربر SQL Server برای ایجاد دیتابیس در اولین اجرا

اتصال پیش‌فرض در `TodoApi/appsettings.json` از Windows Authentication استفاده می‌کند:

```text
Server=localhost;Database=TodoApiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

اگر SQL Server شما روی نمونه‌ای دیگر اجرا می‌شود، مقدار `TodoDatabase` را تغییر دهید. برای نمونه‌ی `SQLEXPRESS` مقدار `Server=localhost\SQLEXPRESS` را به کار ببرید. در محیط‌های دیگر می‌توان رشته اتصال را با متغیر محیطی `ConnectionStrings__TodoDatabase` جایگزین کرد.

## ساخت پروژه از صفر

برای ساخت اسکلت مشابه این مخزن با .NET 8 و Controllerها، از پوشه‌ای خالی این دستورها را اجرا کنید:

```powershell
dotnet new sln -n TodoApi
dotnet new webapi -n TodoApi -f net8.0 --use-controllers
dotnet sln TodoApi.sln add TodoApi/TodoApi.csproj
dotnet add TodoApi/TodoApi.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.32
dotnet add TodoApi/TodoApi.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.32
```

سپس مدل `TodoTask`، `TodoDbContext`، تنظیم رشته اتصال و اجرای `Database.MigrateAsync()` در startup را اضافه کنید. migration اولیه‌ی این پروژه با دستور زیر تولید شده و در مخزن نگهداری می‌شود:

```powershell
dotnet ef migrations add InitialCreate --project TodoApi/TodoApi.csproj --startup-project TodoApi/TodoApi.csproj --output-dir Data/Migrations
```

## دریافت و اجرای پروژه

دستورها را از پوشه اصلی مخزن اجرا کنید:

```powershell
dotnet restore TodoApi.sln
dotnet build TodoApi.sln
dotnet run --project TodoApi/TodoApi.csproj
```

در اولین اجرا، اگر دیتابیس `TodoApiDb` وجود نداشته باشد، EF Core آن را ایجاد می‌کند و migration اولیه را اعمال می‌کند. مستندات Swagger در حالت Development در این نشانی‌ها در دسترس است:

- `http://localhost:5248/swagger`
- `https://localhost:7187/swagger`

## API پیاده‌سازی‌شده

### دریافت وظایف

```http
GET /api/tasks
GET /api/tasks/{id}
```

پاسخ هر دو route در صورت موفقیت `200 OK` است. اگر لیست خالی باشد بدنه‌ی پاسخ `[]` است؛ شناسه‌ی ناموجود در route دوم پاسخ `404 Not Found` می‌دهد.

### ساخت وظیفه و نمایش 400

```http
POST /api/tasks
Content-Type: application/json

{
	"title": "Prepare API demo",
	"description": "Verify task creation.",
	"isCompleted": false,
	"dueDate": "2026-10-25T12:00:00"
}
```

درخواست معتبر پاسخ `201 Created` و header `Location` با مسیر `GET /api/tasks/{id}` می‌دهد. اگر `title` یا `description` خالی/حذف شده باشد، طول عنوان بیش از 200، طول توضیحات بیش از 2000، یا `dueDate` ارسال نشده باشد، `[ApiController]` قبل از اجرای action پاسخ `400 Bad Request` با `ValidationProblemDetails` تولید می‌کند. در Swagger نیز پاسخ‌های `201` و `400` برای `POST` مستند شده‌اند.

نمونه‌ی آماده‌ی درخواست نامعتبر در `TodoApi/TodoApi.http` است؛ در آن `title` خالی و `dueDate` حذف شده تا خطاهای اعتبارسنجی را ببینی.

### مدیریت خطا و کدهای وضعیت

| کد | کاربرد | بدنه‌ی خطا |
| --- | --- | --- |
| `400 Bad Request` | درخواست دارای ورودی نامعتبر؛ برای endpointهای دارای DTO ورودی مثل `POST` و `PUT` | `ValidationProblemDetails` با جزئیات فیلدهای نامعتبر |
| `404 Not Found` | شناسه‌ی وظیفه پیدا نشود | `ProblemDetails` |
| `500 Internal Server Error` | خطای پیش‌بینی‌نشده، مثل در دسترس نبودن دیتابیس | `ProblemDetails` عمومی؛ جزئیات داخلی exception به client داده نمی‌شود |

`AddProblemDetails`، `UseExceptionHandler` و `UseStatusCodePages` پاسخ‌های خطای سراسری را پوشش می‌دهند. `[ApiController]` پیش از اجرای action، ورودی نامعتبر را خودکار با `400 ValidationProblemDetails` پاسخ می‌دهد. خطای ناشناخته با `500 ProblemDetails` برمی‌گردد و جزئیات داخلی exception برای client افشا نمی‌شود. در Swagger پاسخ‌های متناسب با هر endpoint با `[ProducesResponseType]` ثبت شده‌اند.

## Migrationهای بعدی

برای تغییر schema، ابتدا مدل را تغییر دهید و یک migration جدید بسازید. ابزار `dotnet-ef` را فقط یک‌بار نصب کنید:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.32
```

سپس از پوشه اصلی مخزن migration بسازید؛ نام نمونه را متناسب با تغییر خود انتخاب کنید:

```powershell
dotnet ef migrations add AddTaskField --project TodoApi/TodoApi.csproj --startup-project TodoApi/TodoApi.csproj --output-dir Data/Migrations
```

نیازی به اجرای `dotnet ef database update` نیست. با اجرای بعدی برنامه، `Database.MigrateAsync()` migration جدید را روی دیتابیس اعمال می‌کند. EF Core migration را از روی تغییر مدل به‌صورت خودکار تولید نمی‌کند؛ ساخت migration برای هر تغییر schema یک‌بار لازم است.

## مدل دیتابیس

جدول `Tasks` شامل این فیلدهاست:

| فیلد | نوع SQL Server |
| --- | --- |
| `Id` | `uniqueidentifier` |
| `Title` | `nvarchar(200)` |
| `Description` | `nvarchar(2000)` |
| `IsCompleted` | `bit` |
| `DueDate` | `datetime2` |