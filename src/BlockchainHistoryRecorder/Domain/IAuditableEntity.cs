namespace BlockchainHistoryRecorder.Domain;

public interface IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
}