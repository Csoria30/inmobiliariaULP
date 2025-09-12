using inmobiliariaULP.Models;

namespace inmobiliariaULP.Repositories.Implementations;

public static class FactoryRepository
{
    private static IConfiguration? _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static PersonaRepositoryImpl CreatePersonaRepository()
    {
        if (_configuration == null)
            throw new InvalidOperationException("FactoryRepository no ha sido inicializado");

        return new PersonaRepositoryImpl(_configuration);
    }

    public static InquilinoRepositoryImpl CreateInquilinoRepository()
    {
        if (_configuration == null)
            throw new InvalidOperationException("FactoryRepository no ha sido inicializado");

        return new InquilinoRepositoryImpl(_configuration);
    }

    public static PropietarioRepositoryImpl CreatePropietarioRepository()
    {
        if (_configuration == null)
            throw new InvalidOperationException("FactoryRepository no ha sido inicializado");

        return new PropietarioRepositoryImpl(_configuration);
    }

    public static InmuebleRepositoryImpl CreateInmuebleRepository()
    {
        if (_configuration == null)
            throw new InvalidOperationException("FactoryRepository no ha sido inicializado");

        return new InmuebleRepositoryImpl(_configuration);
    }
    
    
}