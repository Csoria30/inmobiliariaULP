using inmobiliariaULP.Repositories.Implementations;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Services.Implementations;
using inmobiliariaULP.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies; // Para autenticación con cookies


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Inicializar el factory una sola vez
//FactoryRepository.Initialize(builder.Configuration);

//Inyeccion de dependencias - Repositorios y Servicios
builder.Services.AddScoped<IInmuebleRepository, InmuebleRepositoryImpl>();
builder.Services.AddScoped<IInquilinoRepository, InquilinoRepositoryImpl>();
builder.Services.AddScoped<IPersonaRepository, PersonaRepositoryImpl>();
builder.Services.AddScoped<IPropietarioRepository, PropietarioRepositoryImpl>();
builder.Services.AddScoped<ITipoRepository, TipoRepositoryImpl>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepositoryImpl>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryImpl>();

builder.Services.AddScoped<IInmuebleService, InmuebleServiceImpl>();
builder.Services.AddScoped<IInquilinoService, InquilinoServiceImpl>();
builder.Services.AddScoped<IPersonaService, PersonaServiceImpl>();
builder.Services.AddScoped<IPropietarioService, PropietarioServiceImpl>();
builder.Services.AddScoped<ITipoService, TipoServiceImpl>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoServiceImpl>();
builder.Services.AddScoped<IUsuarioService, UsuarioServiceImpl>();

// Add authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Home/Restringido";
    });


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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Ruta personalizada para login amigable
app.MapControllerRoute( name: "login", pattern: "iniciar-sesion/{**accion}",  defaults: new { controller = "Auth", action = "Login" } );
app.MapControllerRoute( name: "login", pattern: "login/{**accion}",  defaults: new { controller = "Auth", action = "Login" } );
app.MapControllerRoute( name: "login", pattern: "entrar/{**accion}",  defaults: new { controller = "Auth", action = "Login" } );
app.MapControllerRoute( name: "login", pattern: "inicio/{**accion}",  defaults: new { controller = "Auth", action = "Login" } );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();




app.Run();
