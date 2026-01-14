using VisorDoc.Services;
using VisorDoc.App;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://localhost:3000")
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

// Register custom services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<SharePointService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}*/

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapGet("/api/documentos/{id}", (string id) =>
{
    var today = DateTime.Today.ToString();
    return Results.Ok(new { message = $"ID de documento recibido: {id}  {today}" });
})
.WithName("GetDocumento");

app.MapGet("/api/auth/test-token", async (TokenService tokenService) =>
{
    try
    {
        var token = await tokenService.GetApplicationTokenAsync();
        // Por seguridad, no devolvemos el token. Solo confirmamos el éxito.
        return Results.Ok(new { status = "Exitoso", message = "El token de Azure AD fue adquirido correctamente.", token = token });
    }
    catch (Exception ex)
    {
        // Devolvemos el mensaje de error específico de la librería de autenticación.
        return Results.Problem(
            detail: $"Error al adquirir el token de Azure AD: {ex.Message}",
            statusCode: 500,
            title: "Error de Autenticación");
    }
})
.WithName("TestAzureAdToken");

app.MapGet("/api/auth/decode-token", async (TokenService tokenService) =>
{
    try
    {
        var token = await tokenService.GetApplicationTokenAsync();
        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            return Results.BadRequest("Token JWT inválido.");
        }

        var payload = parts[1];
        var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/') + new string('=', (4 - payload.Length % 4) % 4)));
        
        var decodedPayload = JsonDocument.Parse(payloadJson);

        return Results.Ok(decodedPayload.RootElement);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: $"Error al decodificar el token: {ex.Message}",
            statusCode: 500,
            title: "Error de Decodificación");
    }
})
.WithName("DecodeAzureAdToken");


app.MapPost("/api/sharepoint/datos", async ([FromBody] document doc, SharePointService spService) =>
{
    try
    {
        var siteInfo = await spService.GetSharePointSiteInfoAsync(doc.documentID);
        return Results.Ok(siteInfo);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("GetSharePointData");


app.Run();
