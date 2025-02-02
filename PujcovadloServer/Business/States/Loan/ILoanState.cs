using PujcovadloServer.Business.Enums;

namespace PujcovadloServer.Business.States.Loan;

public interface ILoanState
{
    /// <summary>
    /// Updates the loan status based on the new status as a tenant.
    /// </summary>
    /// <param name="loan">Loan to be updated.</param>
    /// <param name="newStatus">New status to be set.</param>
    public void HandleTenant(Entities.Loan loan, LoanStatus newStatus);

    /// <summary>
    /// Updates the loan status based on the new status as an owner.
    /// </summary>
    /// <param name="loan">Loan to be updated.</param>
    /// <param name="newStatus">New status to be set.</param>
    public void HandleOwner(Entities.Loan loan, LoanStatus newStatus);

    public bool CanUpdatePickupProtocol(Entities.Loan loan);

    public bool CanCreatePickupProtocol(Entities.Loan loan);

    public bool CanUpdateReturnProtocol(Entities.Loan loan);

    public bool CanCreateReturnProtocol(Entities.Loan loan);

    public bool CanCreateReview(Entities.Loan loan);
}