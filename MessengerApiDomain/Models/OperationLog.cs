namespace MessengerApiDomain.Models;

public class OperationLog
{
    public virtual Guid Id { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual string UserName { get; set; }
    public virtual string UserDisplayName { get; set; }
    public virtual DateTime Time { get; set; }
    public virtual string OperationName { get; set; }
    public virtual string OperationDescription { get; set; }
}
