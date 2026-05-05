using OrangePi.Common.Models;

namespace OrangePi.Common.Services
{
    public interface IPiHoleService
    {
        Task<PiHoleSummaryModel> GetSummary();
    }
}
