using Microsoft.EntityFrameworkCore;
using TransactionUpload.Application.Interface;
using TransactionUpload.Application.Service;
using TransactionUpload.Infrastructure;
using TransactionUpload.Infrastructure.Repository;
using TrasanctionUpload.Domain.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var connectionSring = builder.Configuration.GetConnectionString("TransactionConnectionString");
builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseSqlServer(connectionSring)
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddTransient<IUploadFileService, UploadFileService>();
builder.Services.AddTransient<IUploadFileRepository,UploadFileRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
