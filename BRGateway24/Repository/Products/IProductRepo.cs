using BRGateway24.Models;

namespace BRGateway24.Repository.ProductRepo
{
    public interface IProductRepo
    {
        public Task<MainResponse> GetProductListAsync();

        //GetProductWorkflowAsync

        public Task<MainResponse> GetProductWorkflowAsync(ProductWorkflowModel productworkflowmodel);

        public Task<MainResponse> GetFDRatesAsync(FDRatesModel request);
    }
}