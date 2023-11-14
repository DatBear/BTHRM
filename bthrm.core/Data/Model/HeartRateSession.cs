namespace bthrm.core.Data.Model;

public class HeartRateSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<HeartRateReading> Readings { get; set; } = new();
}