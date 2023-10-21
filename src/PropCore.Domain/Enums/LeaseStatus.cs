namespace PropCore.Domain.Enums;

public enum LeaseStatus
{
    Draft = 0,
    PendingApproval = 1,
    Active = 2,
    Expiring = 3,
    Expired = 4,
    Terminated = 5
}