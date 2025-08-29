using MDH.Application.Appliaction_Services;
using MDH.Infrastructure.Infrastructure_Repos;
using MDH.WebUI.Components;
using MDH.WebUI.Components.Pages.Download_Page;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<IDownloadService, DownRepos>();
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapGet("/download/{fileName}", async (string fileName, HttpResponse response) =>
{
    var tempFolder = Path.Combine(Path.GetTempPath(), "YTDownloads");
    var filePath = Path.Combine(tempFolder, fileName);

    if (!File.Exists(filePath))
    {
        response.StatusCode = 404;
        await response.WriteAsync("File not found");
        return;
    }

    var ext = Path.GetExtension(filePath).ToLower();
    var contentType = ext switch
    {
        ".mp4" => "video/mp4",
        ".webm" => "video/webm",
        ".mp3" => "audio/mpeg",
        ".m4a" => "audio/mp4",
        _ => "application/octet-stream"
    };

    response.ContentType = contentType;
    response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"";
    await response.SendFileAsync(filePath);
});

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
