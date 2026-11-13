namespace PokemonRecommender.Models;

public class EfetividadeTipo
{
    public string Atacante { get; private set; } = default!;
    public string Defensor { get; private set; } = default!;
    public double Multiplicador { get; private set; }

    private EfetividadeTipo() { }

    public EfetividadeTipo(string atacante, string defensor, double multiplicador)
    {
        Atacante = atacante;
        Defensor = defensor;
        Multiplicador = multiplicador;
    }
}