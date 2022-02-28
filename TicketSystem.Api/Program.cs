using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using TicketSystem.Api.Data;
using TicketSystem.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllers(setup =>
{
    //���������ͺͷ�������֧�����Ͳ�һ��ʱ�򷵻�406����������xml ��������֧��json��
    setup.ReturnHttpNotAcceptable = true;
})
    .ConfigureApiBehaviorOptions(setup =>
    {
        //�Զ�����󱨸�
        /*setup.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Type = "http://www.baidu.com",
                Title = "����",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "�뿴��ϸ��Ϣ",
                Instance = context.HttpContext.Request.Path
            };

            problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
            return new UnprocessableEntityObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };*/
    })
    .AddNewtonsoftJson(setup =>
    {
        setup.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver();
        setup.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    })
    //���xml���л���
    .AddXmlSerializerFormatters()
    .AddXmlDataContractSerializerFormatters();

//ע��AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//ע�����
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//ע�����ݿ�������
builder.Services.AddDbContext<TicketDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// ��������
else
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Unexpected Error!");
        });
    });
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Ǩ������
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetService<TicketDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database Migration Error!");
    }
}

app.Run();
