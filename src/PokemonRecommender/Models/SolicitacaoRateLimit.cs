namespace PokemonRecommender.Models;

public enum StatusSolicitacao
{
    Pendente,
    Aprovada,
    Negada
}

public class SolicitacaoRateLimit
{
    public Guid Id { get; private set; }
    public Guid JogadorId { get; private set; }
    public int LimiteSolicitado { get; private set; }
    public StatusSolicitacao Status { get; private set; }
    public DateTime DataCriacao { get; private set; }

    private SolicitacaoRateLimit() { }

    public SolicitacaoRateLimit(Guid id, Guid jogadorId, int limiteSolicitado)
    {
        Id = id;
        JogadorId = jogadorId;
        LimiteSolicitado = limiteSolicitado;
        Status = StatusSolicitacao.Pendente;
        DataCriacao = DateTime.UtcNow;
    }

    public void DefinirStatus(StatusSolicitacao status) => Status = status;
}