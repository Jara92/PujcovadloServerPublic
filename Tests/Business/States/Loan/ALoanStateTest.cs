using PujcovadloServer.Business.Enums;
using PujcovadloServer.Business.Factories.State;
using PujcovadloServer.Business.States.Loan;

namespace Tests.Business.States.Loan;

public abstract class ALoanStateTest
{
    protected PujcovadloServer.Business.Entities.Loan _loan;
    protected ILoanState _state;
    protected LoanStatus _status;
    
    
    public void Setup(LoanStatus status)
    {
        _loan = new PujcovadloServer.Business.Entities.Loan()
        {
            Id = 1,
            Status = status,
            CreatedAt = DateTime.FromFileTimeUtc(132620355672698761),
            UpdatedAt = DateTime.FromFileTimeUtc(132620355672898761),
        };

        // Should not be like this but it is the easiest way to test it
        _state = new LoanStateFactory().GetState(status);
        _status = status;
    }
}