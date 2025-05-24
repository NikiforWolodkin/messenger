namespace MessengerApiDomain.Models;

public class OperationLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserDisplayName { get; set; }
    public DateTime Time { get; set; }
    public string OperationName { get; set; }
    public string OperationDescription { get; set; }
}
