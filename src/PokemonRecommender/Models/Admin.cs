namespace PokemonRecommender.Models;

public class Admin : Usuario
{
    private Admin() { }

    public Admin(Guid id, string nome) : base(id, nome) { }
}