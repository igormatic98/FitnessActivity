namespace Infrastracture.Dtos;

public class FitnessActivityResponseDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ActivityType { get; set; }
    public TimeSpan Time { get; set; }
    public TimeSpan Duration { get; set; }
}
