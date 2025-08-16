using BRGateway24.Helpers.ESS.Dto;
using BRGateway24.Models.ESSModels.Request;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace BRGateway24.Helpers.ESS.LoanHelpter
{
    public class LoanDataProcessor
    {
        private readonly LoanDTO _dataTableHelper;
        public LoanDataProcessor()
        {
            _dataTableHelper = new LoanDTO();
        }
        public (TModel Model, DataTable DataTable) ProcessRequest<TModel>(string xmlContent) where TModel : class
        {
            try
            {
                var model = XmlParser.ParseXml<TModel>(xmlContent);

                if (model == null)
                    throw new ApplicationException($"Failed to parse {typeof(TModel).Name} from XML");

                DataTable dt = _dataTableHelper.CreateDataTableForModel<TModel>();
                _dataTableHelper.InsertModelIntoDataTable(dt, model);

                return (model, dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.UtcNow:u}] Error processing {typeof(TModel).Name} request: {ex}");
                throw new ApplicationException($"Failed to process {typeof(TModel).Name} request", ex);
            }
        }
        public (object Model, DataTable DataTable) ProcessRequestDynamic(string xmlContent)
        {
            try
            {
                var document = XmlParser.ParseXmlDynamic(xmlContent);

                var dataProperty = document.GetType().GetProperty("Data");
                if (dataProperty == null)
                    throw new ApplicationException("Invalid document structure - missing Data property");

                dynamic data = dataProperty.GetValue(document);
                dynamic model = data.MessageDetails;

                if (model == null)
                    throw new ApplicationException("Failed to extract model from XML document");

                DataTable dt = _dataTableHelper.CreateDataTableForModel(model.GetType());
                _dataTableHelper.InsertModelIntoDataTable(dt, model);

                return (model, dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.UtcNow:u}] Error processing request: {ex}");
                throw new ApplicationException("Failed to process request", ex);
            }
        }
    }
}
