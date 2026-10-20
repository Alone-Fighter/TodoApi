<h1 dir="rtl" align="right">راهنمای تست Todo API</h1>

<p dir="rtl" align="right">این راهنما توضیح می‌دهد تست‌های فعلی چه چیزی را بررسی می‌کنند، چطور اجرا شوند و چطور برای endpointهای بعدی تست بنویسی.</p>

<h2 dir="rtl" align="right">1. تفاوت Unit Test و Integration Test</h2>

<h3 dir="rtl" align="right">Unit Test</h3>

<p dir="rtl" align="right">در unit test یک بخش کوچک را جدا از زیرساخت واقعی بررسی می‌کنیم. تست‌های Controller این پروژه، <code dir="ltr">TasksController</code> را مستقیم صدا می‌زنند و وابستگی آن یعنی <code dir="ltr">ITodoTaskService</code> را با <bdi dir="ltr">Moq</bdi> جایگزین می‌کنند. بنابراین SQL Server، شبکه و HTTP server اجرا نمی‌شوند؛ تست سریع و قابل‌تکرار است.</p>

<h3 dir="rtl" align="right">Integration Test</h3>

<p dir="rtl" align="right">integration test چند بخش را با هم بررسی می‌کند؛ مثلا ارسال JSON واقعی از مسیر HTTP، اجرای model binding، فیلتر <code dir="ltr">[ApiController]</code>، DI و پاسخ نهایی. برای تست واقعی پاسخ <code dir="ltr">400</code> خودکارِ اعتبارسنجی، integration test مناسب‌تر از صدا زدن مستقیم متد Controller است. integration testهای HTTP هنوز در این پروژه اضافه نشده‌اند.</p>

<h2 dir="rtl" align="right">2. ابزارهای تست</h2>

<table dir="rtl" align="right">
	<thead>
		<tr><th>ابزار</th><th>کاربرد</th></tr>
	</thead>
	<tbody>
		<tr><td><code dir="ltr">xUnit</code></td><td>تعریف تست با <code dir="ltr">[Fact]</code> و assertionها</td></tr>
		<tr><td><code dir="ltr">Moq</code></td><td>ساخت mock برای <code dir="ltr">ITodoTaskService</code> و تعیین پاسخ آن در هر سناریو</td></tr>
		<tr><td><code dir="ltr">Microsoft.NET.Test.Sdk</code></td><td>ساخت و اجرای تست‌ها با دستور <code dir="ltr">dotnet test</code></td></tr>
	</tbody>
</table>

<p dir="rtl" align="right">پروژه‌ی تست در <code dir="ltr">TodoApi.Tests/TodoApi.Tests.csproj</code> است و به پروژه‌ی API reference دارد. تست‌ها به دیتابیس متصل نمی‌شوند.</p>

<h2 dir="rtl" align="right">3. اجرای تست‌ها</h2>

<p dir="rtl" align="right">دستورها را از پوشه‌ی اصلی solution اجرا کن:</p>

```powershell
dotnet restore TodoApi.sln
dotnet test TodoApi.Tests/TodoApi.Tests.csproj
```

<p dir="rtl" align="right">برای اجرای کل solution هم می‌توانی از این دستور استفاده کنی:</p>

```powershell
dotnet test TodoApi.sln
```

<p dir="rtl" align="right">برای اجرای فقط یک تست، نام متد را در filter بگذار:</p>

```powershell
dotnet test TodoApi.Tests/TodoApi.Tests.csproj --filter "FullyQualifiedName~Delete_WhenTaskExists_ReturnsNoContent"
```

<p dir="rtl" align="right">برای دیدن نام‌های کامل تست‌ها و جزئیات بیشتر:</p>

```powershell
dotnet test TodoApi.Tests/TodoApi.Tests.csproj --list-tests
dotnet test TodoApi.Tests/TodoApi.Tests.csproj --verbosity normal
```

<h2 dir="rtl" align="right">4. تست‌های فعلی</h2>

<table dir="rtl" align="right">
	<thead>
		<tr><th>سناریو</th><th>رفتار مورد انتظار</th></tr>
	</thead>
	<tbody>
		<tr><td>دریافت فهرست</td><td><code dir="ltr">200 OK</code> و همان فهرستی که service برمی‌گرداند</td></tr>
		<tr><td>دریافت با شناسه‌ی موجود</td><td><code dir="ltr">200 OK</code> و DTO همان وظیفه</td></tr>
		<tr><td>دریافت با شناسه‌ی ناموجود</td><td><code dir="ltr">404 Not Found</code> و <code dir="ltr">ProblemDetails</code></td></tr>
		<tr><td>ساخت وظیفه</td><td><code dir="ltr">201 Created</code> و route مقصد <code dir="ltr">GetById</code></td></tr>
		<tr><td>ویرایش وظیفه‌ی موجود</td><td><code dir="ltr">200 OK</code></td></tr>
		<tr><td>ویرایش وظیفه‌ی ناموجود</td><td><code dir="ltr">404 Not Found</code></td></tr>
		<tr><td>حذف وظیفه‌ی موجود</td><td><code dir="ltr">204 No Content</code></td></tr>
		<tr><td>حذف وظیفه‌ی ناموجود</td><td><code dir="ltr">404 Not Found</code></td></tr>
		<tr><td>اعتبارسنجی DTO ساخت</td><td>عنوان و تاریخ سررسیدِ ارسال‌نشده خطای validation تولید کنند</td></tr>
	</tbody>
</table>

<h2 dir="rtl" align="right">5. الگوی نوشتن Unit Test</h2>

<p dir="rtl" align="right">هر تست را با الگوی <bdi dir="ltr">Arrange – Act – Assert</bdi> بنویس:</p>

<ol dir="rtl" align="right">
	<li><bdi dir="ltr">Arrange:</bdi> داده و رفتار mock را آماده کن.</li>
	<li><bdi dir="ltr">Act:</bdi> متدی را که می‌خواهی بررسی کنی اجرا کن.</li>
	<li><bdi dir="ltr">Assert:</bdi> نتیجه و تعامل‌های مهم را بررسی کن.</li>
</ol>

<p dir="rtl" align="right">نمونه‌ی زیر شبیه تست DELETE موجود در <code dir="ltr">TodoApi.Tests/UnitTest1.cs</code> است:</p>

```csharp
[Fact]
public async Task Delete_WhenTaskExists_ReturnsNoContent()
{
    var id = Guid.NewGuid();
    todoTaskService
        .Setup(service => service.DeleteAsync(id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);
    var controller = CreateController();

    var result = await controller.Delete(id, CancellationToken.None);

    Assert.IsType<NoContentResult>(result);
    todoTaskService.Verify(
        service => service.DeleteAsync(id, It.IsAny<CancellationToken>()),
        Times.Once);
}
```

<p dir="rtl" align="right">در این مثال، <code dir="ltr">Setup</code> می‌گوید service حذف را موفق بداند؛ <code dir="ltr">Act</code>، action کنترلر را اجرا می‌کند؛ و <code dir="ltr">Assert</code> نتیجه‌ی HTTP را بررسی می‌کند. <code dir="ltr">Verify</code> هم اطمینان می‌دهد Controller واقعا service را یک‌بار صدا زده است.</p>

<h2 dir="rtl" align="right">6. چرا DELETE پاسخ 204 می‌دهد؟</h2>

<p dir="rtl" align="right"><code dir="ltr">204 No Content</code> برای حذف موفق استاندارد است: یعنی سرور درخواست را انجام داده و عمدا پاسخ بدنه ندارد. پس خالی بودن response نشانه‌ی خطا نیست. اگر بخواهیم پیام یا شیء JSON برگردانیم، باید به‌جای 204 از <code dir="ltr">200 OK</code> و یک response body استفاده کنیم؛ برای این تسک نیازی به پیام اضافه نیست. شناسه‌ی ناموجود همچنان باید <code dir="ltr">404 Not Found</code> باشد.</p>

<h2 dir="rtl" align="right">7. محدودیت تست‌های واحد فعلی</h2>

<p dir="rtl" align="right">صدا زدن مستقیم <code dir="ltr">TasksController</code> باعث اجرای middleware، routing، model binding یا فیلتر <code dir="ltr">[ApiController]</code> نمی‌شود. تست Data Annotations در این پروژه مستقیما <code dir="ltr">Validator.TryValidateObject</code> را اجرا می‌کند؛ بنابراین ثابت می‌کند ruleهای DTO کار می‌کنند، اما ثابت نمی‌کند HTTP pipeline پاسخ 400 تولید می‌کند. برای آن رفتار باید بعدا integration test با <code dir="ltr">WebApplicationFactory</code> نوشته شود.</p>

<p dir="rtl" align="right">تست‌های فعلی به SQL Server نیازی ندارند. برای integration test دیتابیس، بهتر است از یک دیتابیس تستی جدا استفاده شود تا داده‌های توسعه یا production تغییر نکنند.</p>