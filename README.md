<h1 dir="rtl" align="right">مستندات Todo API</h1>

<p dir="rtl" align="right">این پروژه یک API مدیریت وظایف با <bdi dir="ltr">ASP.NET Core 8</bdi> و <bdi dir="ltr">SQL Server</bdi> است و بر اساس تسک To-do List ریرا پیش می‌رود.</p>

<h2 dir="rtl" align="right">وضعیت پیاده‌سازی</h2>

<ul dir="rtl" align="right">
	<li>زیرساخت <bdi dir="ltr">SQL Server</bdi>، مدل <code dir="ltr">TodoTask</code>، <code dir="ltr">TodoDbContext</code> و migration اولیه آماده است.</li>
	<li>برنامه هنگام اجرا migrationهای موجود را خودکار اعمال می‌کند.</li>
	<li><code dir="ltr">GET /api/tasks</code> و <code dir="ltr">GET /api/tasks/{id}</code> برای دریافت وظایف پیاده‌سازی شده‌اند.</li>
	<li><code dir="ltr">POST /api/tasks</code> وظیفه می‌سازد و اعتبارسنجی ورودی دارد.</li>
	<li>هر پنج endpoint موردنیاز تسک پیاده‌سازی شده‌اند.</li>
	<li>خطاهای مدیریت‌نشده و پاسخ‌های status خالی با <bdi dir="ltr">Problem Details</bdi> به‌صورت سراسری مدیریت می‌شوند.</li>
</ul>

<h2 dir="rtl" align="right">ساختار کد</h2>

```text
TodoApi/
├── Controllers/       # HTTP endpoints
├── Domain/Entities/   # TodoTask
├── DTOs/Tasks/        # مدل‌های ورودی/خروجی و اعتبارسنجی
├── Data/              # DbContext و EF Core migrations
└── Services/Tasks/    # منطق دسترسی و عملیات وظایف
```

<p dir="rtl" align="right">در این پروژه‌ی کوچک، هر لایه پوشه‌ی جدا دارد و در یک پروژه‌ی <bdi dir="ltr">.NET</bdi> نگه‌داری می‌شود. <code dir="ltr">TodoDbContext</code> نقش <bdi dir="ltr">Repository/Unit of Work</bdi> مربوط به <bdi dir="ltr">EF Core</bdi> را دارد؛ بنابراین Repository عمومی جداگانه اضافه نشده است.</p>

<h2 dir="rtl" align="right">پیش‌نیازها</h2>

<ul dir="rtl" align="right">
	<li><bdi dir="ltr">.NET 8 SDK</bdi></li>
	<li><bdi dir="ltr">SQL Server</bdi> در دسترس از طریق اتصال تعریف‌شده در تنظیمات</li>
	<li>دسترسی کاربر <bdi dir="ltr">SQL Server</bdi> برای ایجاد دیتابیس در اولین اجرا</li>
</ul>

<p dir="rtl" align="right">اتصال پیش‌فرض در فایل <code dir="ltr">TodoApi/appsettings.json</code> از <bdi dir="ltr">Windows Authentication</bdi> استفاده می‌کند:</p>

```text
Server=localhost;Database=TodoApiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

<p dir="rtl" align="right">اگر <bdi dir="ltr">SQL Server</bdi> شما روی نمونه‌ای دیگر اجرا می‌شود، مقدار <code dir="ltr">TodoDatabase</code> را تغییر دهید. برای نمونه‌ی <bdi dir="ltr">SQLEXPRESS</bdi> مقدار <code dir="ltr">Server=localhost\SQLEXPRESS</code> را به کار ببرید. در محیط‌های دیگر می‌توان رشته اتصال را با متغیر محیطی <code dir="ltr">ConnectionStrings__TodoDatabase</code> جایگزین کرد.</p>

<h2 dir="rtl" align="right">ساخت پروژه از صفر</h2>

<p dir="rtl" align="right">برای ساخت اسکلت مشابه این مخزن با <bdi dir="ltr">.NET 8</bdi> و Controllerها، از پوشه‌ای خالی این دستورها را اجرا کنید:</p>

```powershell
dotnet new sln -n TodoApi
dotnet new webapi -n TodoApi -f net8.0 --use-controllers
dotnet sln TodoApi.sln add TodoApi/TodoApi.csproj
dotnet add TodoApi/TodoApi.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.32
dotnet add TodoApi/TodoApi.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.32
```

<p dir="rtl" align="right">سپس مدل <code dir="ltr">TodoTask</code>، <code dir="ltr">TodoDbContext</code>، تنظیم رشته اتصال و اجرای <code dir="ltr">Database.MigrateAsync()</code> در startup را اضافه کنید. migration اولیه‌ی این پروژه با دستور زیر تولید شده و در مخزن نگه‌داری می‌شود:</p>

```powershell
dotnet ef migrations add InitialCreate --project TodoApi/TodoApi.csproj --startup-project TodoApi/TodoApi.csproj --output-dir Data/Migrations
```

<h2 dir="rtl" align="right">دریافت و اجرای پروژه</h2>

<p dir="rtl" align="right">دستورها را از پوشه‌ی اصلی مخزن اجرا کنید:</p>

```powershell
dotnet restore TodoApi.sln
dotnet build TodoApi.sln
dotnet run --project TodoApi/TodoApi.csproj
```

<p dir="rtl" align="right">در اولین اجرا، اگر دیتابیس <code dir="ltr">TodoApiDb</code> وجود نداشته باشد، <bdi dir="ltr">EF Core</bdi> آن را ایجاد می‌کند و migration اولیه را اعمال می‌کند. مستندات Swagger در حالت Development در این نشانی‌ها در دسترس است:</p>

<ul dir="rtl" align="right">
	<li><code dir="ltr">http://localhost:5248/swagger</code></li>
	<li><code dir="ltr">https://localhost:7187/swagger</code></li>
</ul>

<h2 dir="rtl" align="right">API پیاده‌سازی‌شده</h2>

<p dir="rtl" align="right">همه‌ی عملیات روی منبع جمع <code dir="ltr">/api/tasks</code> انجام می‌شوند. endpoint با ترکیب متد HTTP و مسیر انتخاب می‌شود: GET بدون شناسه فهرست را می‌خواند، POST روی همان مسیر وظیفه می‌سازد، و برای کار روی یک وظیفه شناسه به انتهای مسیر اضافه می‌شود. مسیر درست <code dir="ltr">/api/tasks</code> است، نه <code dir="ltr">/api/task</code>.</p>

<table dir="rtl" align="right">
	<thead>
		<tr><th>متد</th><th>مسیر</th><th>کاربرد</th><th>وضعیت</th></tr>
	</thead>
	<tbody>
		<tr><td><code dir="ltr">GET</code></td><td><code dir="ltr">/api/tasks</code></td><td>دریافت همه‌ی وظایف</td><td>پیاده‌سازی شده</td></tr>
		<tr><td><code dir="ltr">GET</code></td><td><code dir="ltr">/api/tasks/{id}</code></td><td>دریافت یک وظیفه</td><td>پیاده‌سازی شده</td></tr>
		<tr><td><code dir="ltr">POST</code></td><td><code dir="ltr">/api/tasks</code></td><td>ساخت وظیفه‌ی جدید</td><td>پیاده‌سازی شده</td></tr>
		<tr><td><code dir="ltr">PUT</code></td><td><code dir="ltr">/api/tasks/{id}</code></td><td>جایگزینی کامل فیلدهای وظیفه</td><td>پیاده‌سازی شده</td></tr>
		<tr><td><code dir="ltr">DELETE</code></td><td><code dir="ltr">/api/tasks/{id}</code></td><td>حذف وظیفه</td><td>پیاده‌سازی شده</td></tr>
	</tbody>
</table>

<h3 dir="rtl" align="right">دریافت وظایف</h3>

```http
GET /api/tasks
GET /api/tasks/{id}
```

<p dir="rtl" align="right">پاسخ هر دو route در صورت موفقیت <code dir="ltr">200 OK</code> است. اگر لیست خالی باشد بدنه‌ی پاسخ <code dir="ltr">[]</code> است؛ شناسه‌ی ناموجود در route دوم پاسخ <code dir="ltr">404 Not Found</code> می‌دهد.</p>

<h3 dir="rtl" align="right">ساخت وظیفه و نمایش 400</h3>

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

<p dir="rtl" align="right">درخواست معتبر پاسخ <code dir="ltr">201 Created</code> و header <code dir="ltr">Location</code> با مسیر <code dir="ltr">GET /api/tasks/{id}</code> می‌دهد. اگر <code dir="ltr">title</code> یا <code dir="ltr">description</code> خالی/حذف شده باشد، طول عنوان بیش از 200، طول توضیحات بیش از 2000، یا <code dir="ltr">dueDate</code> ارسال نشده باشد، <code dir="ltr">[ApiController]</code> قبل از اجرای action پاسخ <code dir="ltr">400 Bad Request</code> با <code dir="ltr">ValidationProblemDetails</code> تولید می‌کند. در Swagger نیز پاسخ‌های 201 و 400 برای POST مستند شده‌اند.</p>

<p dir="rtl" align="right">نمونه‌ی آماده‌ی درخواست نامعتبر در فایل <code dir="ltr">TodoApi/TodoApi.http</code> است؛ در آن <code dir="ltr">title</code> خالی و <code dir="ltr">dueDate</code> حذف شده تا خطاهای اعتبارسنجی را ببینی.</p>

<h3 dir="rtl" align="right">ویرایش کامل وظیفه</h3>

<p dir="rtl" align="right">در <code dir="ltr">PUT /api/tasks/{id}</code> همه‌ی فیلدهای قابل‌ویرایش باید ارسال شوند. پاسخ موفق <code dir="ltr">200 OK</code> است؛ شناسه‌ی ناموجود <code dir="ltr">404 Not Found</code> و payload نامعتبر <code dir="ltr">400 ValidationProblemDetails</code> می‌دهد.</p>

```http
PUT /api/tasks/{id}
Content-Type: application/json

{
	"title": "Prepare API demo",
	"description": "Updated task description.",
	"isCompleted": true,
	"dueDate": "2026-10-26T12:00:00"
}
```

<h3 dir="rtl" align="right">حذف وظیفه</h3>

<p dir="rtl" align="right">درخواست <code dir="ltr">DELETE /api/tasks/{id}</code> در صورت موفقیت <code dir="ltr">204 No Content</code> و برای شناسه‌ی ناموجود <code dir="ltr">404 Not Found</code> برمی‌گرداند.</p>

```http
DELETE /api/tasks/{id}
```

<h2 dir="rtl" align="right">Unit test</h2>

<p dir="rtl" align="right">تست‌های واحد در پروژه‌ی جداگانه‌ی <code dir="ltr">TodoApi.Tests</code> با <bdi dir="ltr">xUnit</bdi> و <bdi dir="ltr">Moq</bdi> قرار دارند. در تست Controller، <code dir="ltr">ITodoTaskService</code> را mock می‌کنیم؛ بنابراین تست به SQL Server وصل نمی‌شود و فقط تصمیم Controller، مثل برگرداندن <code dir="ltr">204</code> برای حذف موفق، بررسی می‌شود.</p>

<p dir="rtl" align="right">از پوشه‌ی اصلی مخزن تست‌ها را اجرا کنید:</p>

```powershell
dotnet test TodoApi.Tests/TodoApi.Tests.csproj
```

<p dir="rtl" align="right">الگوی هر تست <bdi dir="ltr">Arrange, Act, Assert</bdi> است: ابتدا mock و داده‌ی مورد انتظار را آماده کنید، action را اجرا کنید و سپس نوع نتیجه و مقدار آن را assert کنید. برای مثال تست DELETE بررسی می‌کند که وقتی Service مقدار <code dir="ltr">true</code> برمی‌گرداند، Controller یک <code dir="ltr">NoContentResult</code> می‌دهد.</p>

<p dir="rtl" align="right">Unit test مستقیم متد Controller، فیلتر خودکار <code dir="ltr">[ApiController]</code> و model binding را اجرا نمی‌کند؛ به همین دلیل خطای <code dir="ltr">400</code> حاصل از JSON نامعتبر باید با integration test روی HTTP client تست شود. تست اعتبارسنجی Data Annotations در این پروژه فقط خود قواعد DTO را بررسی می‌کند.</p>

<h3 dir="rtl" align="right">مدیریت خطا و کدهای وضعیت</h3>

<table dir="rtl" align="right">
	<thead>
		<tr><th>کد</th><th>کاربرد</th><th>بدنه‌ی خطا</th></tr>
	</thead>
	<tbody>
		<tr><td><code dir="ltr">400 Bad Request</code></td><td>درخواست دارای ورودی نامعتبر؛ برای endpointهای دارای DTO ورودی مثل POST و PUT</td><td><code dir="ltr">ValidationProblemDetails</code> با جزئیات فیلدهای نامعتبر</td></tr>
		<tr><td><code dir="ltr">404 Not Found</code></td><td>شناسه‌ی وظیفه پیدا نشود</td><td><code dir="ltr">ProblemDetails</code></td></tr>
		<tr><td><code dir="ltr">500 Internal Server Error</code></td><td>خطای پیش‌بینی‌نشده، مثل در دسترس نبودن دیتابیس</td><td><code dir="ltr">ProblemDetails</code> عمومی؛ جزئیات داخلی exception به client داده نمی‌شود</td></tr>
	</tbody>
</table>

<p dir="rtl" align="right"><code dir="ltr">AddProblemDetails</code>، <code dir="ltr">UseExceptionHandler</code> و <code dir="ltr">UseStatusCodePages</code> پاسخ‌های خطای سراسری را پوشش می‌دهند. <code dir="ltr">[ApiController]</code> پیش از اجرای action، ورودی نامعتبر را خودکار با <code dir="ltr">400 ValidationProblemDetails</code> پاسخ می‌دهد. خطای ناشناخته با <code dir="ltr">500 ProblemDetails</code> برمی‌گردد و جزئیات داخلی exception برای client افشا نمی‌شود. در Swagger پاسخ‌های متناسب با هر endpoint با <code dir="ltr">[ProducesResponseType]</code> ثبت شده‌اند.</p>

<h2 dir="rtl" align="right">Migrationهای بعدی</h2>

<p dir="rtl" align="right">برای تغییر schema، ابتدا مدل را تغییر دهید و یک migration جدید بسازید. ابزار <code dir="ltr">dotnet-ef</code> را فقط یک‌بار نصب کنید:</p>

```powershell
dotnet tool install --global dotnet-ef --version 8.0.32
```

<p dir="rtl" align="right">سپس از پوشه‌ی اصلی مخزن migration بسازید؛ نام نمونه را متناسب با تغییر خود انتخاب کنید:</p>

```powershell
dotnet ef migrations add AddTaskField --project TodoApi/TodoApi.csproj --startup-project TodoApi/TodoApi.csproj --output-dir Data/Migrations
```

<p dir="rtl" align="right">نیازی به اجرای <code dir="ltr">dotnet ef database update</code> نیست. با اجرای بعدی برنامه، <code dir="ltr">Database.MigrateAsync()</code> migration جدید را روی دیتابیس اعمال می‌کند. <bdi dir="ltr">EF Core</bdi> migration را از روی تغییر مدل به‌صورت خودکار تولید نمی‌کند؛ ساخت migration برای هر تغییر schema یک‌بار لازم است.</p>

<h2 dir="rtl" align="right">مدل دیتابیس</h2>

<p dir="rtl" align="right">جدول <code dir="ltr">Tasks</code> شامل این فیلدهاست:</p>

<table dir="rtl" align="right">
	<thead>
		<tr><th>فیلد</th><th>نوع SQL Server</th></tr>
	</thead>
	<tbody>
		<tr><td><code dir="ltr">Id</code></td><td><code dir="ltr">uniqueidentifier</code></td></tr>
		<tr><td><code dir="ltr">Title</code></td><td><code dir="ltr">nvarchar(200)</code></td></tr>
		<tr><td><code dir="ltr">Description</code></td><td><code dir="ltr">nvarchar(2000)</code></td></tr>
		<tr><td><code dir="ltr">IsCompleted</code></td><td><code dir="ltr">bit</code></td></tr>
		<tr><td><code dir="ltr">DueDate</code></td><td><code dir="ltr">datetime2</code></td></tr>
	</tbody>
</table>