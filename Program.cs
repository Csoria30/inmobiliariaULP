using inmobiliariaULP.Repositories.Implementations;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Services.Implementations;
using inmobiliariaULP.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Inicializar el factory una sola vez
//FactoryRepository.Initialize(builder.Configuration);

//Inyeccion de dependencias
builder.Services.AddScoped<IInmuebleRepository, InmuebleRepositoryImpl>();
builder.Services.AddScoped<IInquilinoRepository, InquilinoRepositoryImpl>();
builder.Services.AddScoped<IPersonaRepository, PersonaRepositoryImpl>();
builder.Services.AddScoped<IPropietarioRepository, PropietarioRepositoryImpl>();
builder.Services.AddScoped<ITipoRepository, TipoRepositoryImpl>();

builder.Services.AddScoped<IInmuebleService, InmuebleServiceImpl>();
builder.Services.AddScoped<IInquilinoService, InquilinoServiceImpl>();
builder.Services.AddScoped<IPersonaService, PersonaServiceImpl>();
builder.Services.AddScoped<IPropietarioService, PropietarioServiceImpl>();
builder.Services.AddScoped<ITipoService, TipoServiceImpl>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();




app.Run();
