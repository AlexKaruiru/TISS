using BRGateway24.Models.ESSModels.Request;
using System.Xml;
using System.Xml.Serialization;

namespace BRGateway24.Helpers.ESS.LoanHelpter
{
    public static class XmlParser
    {
        private static readonly Dictionary<string, Type> _documentTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                // Loan Operations
                { "LOAN_CHARGES_REQUEST", typeof(ESSNewLoanChargesDocument) },
                { "LOAN_VERIFICATION", typeof(ESSLoanVerificationDocument) },
                { "LOAN_TOPUP_REQUEST", typeof(ESSLoanTopUpDocument) },
                { "LOAN_RESTRUCTURE_REQUEST", typeof(ESSLoanRestructuringDocument) },
                { "LOAN_TAKEOVER_REQUEST", typeof(ESSLoanTakeoverDocument) },
    
                // Payment Operations
                { "PAYMENT_NOTIFICATION", typeof(ESSPaymentNotificationDocument) },
                { "PAYOFF_BALANCE_REQUEST", typeof(ESSPayOffBalanceDocument) },
                { "PARTIAL_REPAYMENT_REQUEST", typeof(ESSPartialRepaymentDocument) },
                { "MONTHLY_DEDUCTION_RECORD", typeof(ESSMonthlyDeductionDocument) },
    
                // Status Notifications
                { "LOAN_STATUS_RESPONSE", typeof(ESSLoanStatusDocument) },
                { "DEFAULTER_DETAILS", typeof(ESSDefaulterDocument) },
                { "DEDUCTION_STOP_NOTIFICATION", typeof(ESSDeductionStopDocument) },
                { "EMPLOYEE_CANCELLATION", typeof(ESSEmployeeCancellationDocument) },
                { "DISBURSEMENT_FAILURE", typeof(ESSDisbursementFailureDocument) },
    
                // System Operations
                { "ACCOUNT_VALIDATION_REQUEST", typeof(ESSAccountValidationDocument) },
                { "FSP_BRANCHES_REQUEST", typeof(ESSFSPBranchesDocument) },
                { "GENERAL_RESPONSE", typeof(ESSGeneralResponseDocument) },
            };

        public static TModel ParseXml<TModel>(string xmlContent) where TModel : class
        {
            try
            {
                Type documentType = GetDocumentTypeForModel(typeof(TModel));
                var serializer = new XmlSerializer(documentType);

                using (var reader = new StringReader(xmlContent))
                {
                    var document = serializer.Deserialize(reader);

                    if (document is TModel model)
                        return model;

                    if (document.GetType().GetProperty("Data")?.GetValue(document) is { } data
                        && data.GetType().GetProperty("MessageDetails")?.GetValue(data) is TModel details)
                        return details;

                    throw new ApplicationException($"Could not extract {typeof(TModel)} from document");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to parse XML to {typeof(TModel).Name}", ex);
            }
        }
        private static object DeserializeDocument(string xmlContent, Type modelType)
        {
            Type documentType = GetDocumentTypeForModel(modelType);
            var serializer = new XmlSerializer(documentType);
            using var reader = new StringReader(xmlContent);
            return serializer.Deserialize(reader);
        }

        private static TModel ExtractMessageDetails<TModel>(object document) where TModel : class
        {
            var data = document.GetType().GetProperty("Data")?.GetValue(document);
            var messageDetails = data?.GetType().GetProperty("MessageDetails")?.GetValue(data);

            if (messageDetails is TModel model)
            {
                return model;
            }

            throw new ApplicationException(
                $"Type mismatch. Expected {typeof(TModel)}, got {messageDetails?.GetType()}");
        }
        public static object ParseXmlDynamic(string xmlContent)
        {
            try
            {
                var messageType = GetMessageTypeFromXml(xmlContent);
                if (!_documentTypes.TryGetValue(messageType, out Type documentType))
                {
                    throw new ApplicationException($"Unsupported message type: {messageType}");
                }

                var serializer = new XmlSerializer(documentType);
                using (var reader = new StringReader(xmlContent))
                {
                    return serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to parse XML dynamically", ex);
            }
        }

        private static string GetMessageTypeFromXml(string xmlContent)
        {
            using (var reader = XmlReader.Create(new StringReader(xmlContent)))
            {
                if (reader.ReadToFollowing("MessageType"))
                {
                    return reader.ReadElementContentAsString();
                }
                throw new XmlException("MessageType element not found in XML");
            }
        }

        private static Type GetDocumentTypeForModel(Type modelType)
        {
            if (_documentTypes.Values.Contains(modelType))
            {
                return modelType;
            }
            string typeName = modelType.Name
                .Replace("Body", "")
                .Replace("Request", "")
                .Replace("Charges", ""); 

            string[] possibleNames =    {
                                            $"ESS{typeName}Document",                            // ESSNewLoanDocument
                                            $"ESS{typeName}Document, BRGateway24.Models",        // With namespace
                                            $"BRGateway24.Models.ESS{typeName}Document",         // Fully qualified
                                            $"BRGateway24.Models.Request.ESS{typeName}Document"  // With sub-namespace
                                        };

            foreach (var name in possibleNames)
            {
                Type type = Type.GetType(name);
                if (type != null) return type;
            }

            var documentType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == $"ESS{typeName}Document");

            if (documentType == null)
            {
                throw new ApplicationException(
                    $"Could not find document type for model: {modelType.Name}. " +
                    $"Tried: {string.Join(", ", possibleNames)}");
            }

            return documentType;
        }
    }
}
