using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IExpenseService
    {
        List<ExpenseVM> GetAll();
    }
}
