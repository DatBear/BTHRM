namespace bthrm.core.Data.Model;

public class HeartRateReading
{
    public int Id { get; set; }
    public int HeartRateSessionId { get; set; }
    public DateTime Date { get; set; }
    public int HeartRate { get; set; }
}