using Microsoft.EntityFrameworkCore;
using Routine.Api.Data;
using Routine.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllers(setup =>
{
    //���������ͺͷ�������֧�����Ͳ�һ��ʱ�򷵻�406����������xml ��������֧��json��
    setup.ReturnHttpNotAcceptable = true;
    //����Э�̣���֧��xml��ʽ����(��д��)
    //setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
    //setup.Input...
    
    //���xml��ʽ����
}).AddXmlDataContractSerializerFormatters();

//ע��AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//ע�����
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ע�����ݿ�������
builder.Services.AddDbContext<RoutineDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
// ��������
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
        appBuilder.Run(async context=>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Unexpected Error!");
        });
    });
}

app.UseAuthorization();

app.MapControllers();

//Ǩ������
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetService<RoutineDbContext>();

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
