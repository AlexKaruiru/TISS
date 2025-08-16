using BRGateway24.Models;

namespace BRGateway24.Repository.ESS.Interfaces
{
    public interface IESSLoansRepo
    {
        Task<MainResponse> ESSLoanApplicationAsync(string ESSRequest);

    }
}
