namespace inmobiliariaULP.Repositories;

public abstract class  BaseRepository
{
    protected readonly IConfiguration configuration;
	protected readonly string connectionString;

	protected BaseRepository(IConfiguration configuration)
	{
        this.configuration = configuration;
		connectionString = configuration["ConnectionStrings:MySql"];
	}
}