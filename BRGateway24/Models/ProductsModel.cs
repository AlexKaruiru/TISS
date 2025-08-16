namespace BRGateway24.Models
{
    public class ProductsModel
    {

    }

    public class ProductWorkflowModel
    {
        public string productid { get; set; }
    }

    public class FDRatesModel
    {
        public string ProductID { get; set; }
        public int Term { get; set; }
        public double Amount { get; set; }        

    }
}
