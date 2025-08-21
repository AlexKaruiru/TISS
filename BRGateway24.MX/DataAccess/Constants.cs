using BRGateway24.Repository;
using FastMember;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Data.SqlClient;
using PhoneNumbers;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BRGateway24.Models;
using BRGateway24.Helpers;

namespace BRGateway24.DataAccess
{
    public static class Constants
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456"); 
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456");
        static HashSet<string> generatedTokens = new HashSet<string>();

        public static string GetConnectionString(string DBType, string DBServerName, string DatabaseName, string DBUserName, string DBUserPassword, string AppName = "")
        {
            string dbPort = DBServerName.Substring(DBServerName.Contains(',') ? DBServerName.IndexOf(',') : 0);
            DBUserPassword = DBClient.BRUserPassword(DBUserPassword);
            DBUserName = DBClient.BRUserName(DBUserName);
            AppName = string.IsNullOrEmpty(AppName) ? "BRGateway24API" : AppName;
            string strConnection;
            switch (DBType)
            {
                case "SQLSERVER":
                    strConnection = string.Format("Data source={0};Initial Catalog={1};User id={2};Password={3};Integrated Security=false;persist security info=True;App={4};MultipleActiveResultSets=false;",
                        DBServerName, DatabaseName, DBUserName, DBUserPassword, AppName);
                    break;
                case "MYSQL":
                    dbPort = string.IsNullOrEmpty(dbPort) ? "3306" : dbPort;
                    strConnection = string.Format("Server={0};Database={1};Uid={2};pwd={3};port={4};",
                        DBServerName, DatabaseName, DBUserName, DBUserPassword, dbPort);
                    break;
                case "ORACLE":
                    strConnection = string.Format("Data Source={0};user ID={1};password={2};",
                       DatabaseName, DBUserName, DBUserPassword);
                    break;
                default:
                    strConnection = string.Format("Data source={0};Initial Catalog={1};User id={2};Password={3};persist security info=True;App={4}",
                        DBServerName, DatabaseName, DBUserName, DBUserPassword, AppName);
                    break;
            }


            return strConnection;
        }


        //public static T ConvertToObject<T>(this SqlDataReader rd) where T : class, new()
        //{
        //    string fieldName = string.Empty;
        //    Type type = typeof(T);
        //    var accessor = TypeAccessor.Create(type);
        //    var members = accessor.GetMembers();
        //    var t = new T();

        //    for (int i = 0; i < rd.FieldCount; i++)
        //    {
        //        if (!rd.IsDBNull(i))
        //        {
        //            fieldName = rd.GetName(i);

        //            if (members.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
        //            {
        //                accessor[t, fieldName] = rd.GetValue(i);
        //            }
        //        }
        //    }

        //    return t;
        //}


        public static T ConvertToObject<T>(this SqlDataReader rd) where T : class, new()
        {
            var type = typeof(T);
            var t = new T();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in properties)
            {                
                if (rd.HasRows && rd.FieldCount > 0 && rd.GetColumnSchema().Any(c => string.Equals(c.ColumnName, property.Name, StringComparison.OrdinalIgnoreCase)))
                {                    
                    if (!property.CanWrite) continue;
                    
                    var value = rd[property.Name];
                    
                    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {                        
                        var nestedObject = Activator.CreateInstance(property.PropertyType);
                        var nestedReader = rd.GetChildReader(property.Name);
                        var nestedObjectConverted = nestedReader.ConvertToObject(property.PropertyType);
                        property.SetValue(t, nestedObjectConverted);
                    }                    
                    else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
                    {
                        var itemType = property.PropertyType.GetGenericArguments().First();
                        var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType)) as System.Collections.IList;
                        
                        var collectionReader = rd.GetCollectionReader(property.Name);
                        while (collectionReader.Read())
                        {
                            var item = collectionReader.ConvertToObject(itemType);
                            list.Add(item);
                        }
                        property.SetValue(t, list);
                    }
                    else
                    {                        
                        if (value != DBNull.Value)
                        {
                            property.SetValue(t, Convert.ChangeType(value, property.PropertyType));
                        }
                    }
                }
            }

            return t;
        }

        private static SqlDataReader GetChildReader(this SqlDataReader rd, string propertyName)
        {            
            throw new NotImplementedException();
        }

        private static SqlDataReader GetCollectionReader(this SqlDataReader rd, string propertyName)
        {            
            throw new NotImplementedException();
        }

        private static object ConvertToObject(this SqlDataReader rd, Type type)
        {            
            throw new NotImplementedException();
        }

        
        public const string BRAUTHENTICATEUSER = "p_AuthenticateApiUserGwy24";
        public const string BRUPDATELOGINSTATUS = "p_UpdateLoginStatusGwy24";
        public const string CLIENTONBOARDING = "p_AddEditClientsGwy24";
        public const string CORPRATEONBOARDING = "p_AddEditClientsGwy24";
        public const string JOINTONBOARDING = "p_AddEditCorporateClientsGwy24";
        public const string GROUPONBOARDING = "p_AddEditClientsGwy24";
        public const string CLIENTIMAGES = "p_AddEditClientImagesGwy24";
        public const string CLIENTDOCUEMENTS = "p_AddEditWFAdvDocumentsGwy24";
        public const string GETCLIENT = "p_GetClientGwy24";
        public const string GETCLIENTDETAILS = "p_GetClientDetailsServiceGwy24";
        public const string GETCLIENTACCOUNTS = "p_GetClientAccountsGwy24";
        public const string GETPENDINGBOARDACTION = "p_GetPendingBoardActionGwy24";
        public const string GETCLIENTLNELIGIBILITY = "p_GetClientEligibilityGwy24";
        public const string AUDITLOG = "p_AddEditAPIAuditLogGwy24";
        public const string CLIENTRELATIONONBOARDING = "p_AddEditClientRelationsGwy24";
        public const string CLIENTBYUNIQUENUMBER = "p_GetClientByUniqueNumberMobileGwy24";
        public const string IBMBREQUESTS = "p_GetIBandMBRequestsGwy24";
        public const string IBMBADD = "p_AddForIBandMBGwy24";
        public const string ADDACCOUNTSERVICE = "p_AddEditGatewayAccountCustomerGwy24";
        public const string MEMBERDMSDOCUMENTSVERIFICATION = "p_DMSDocumentsVerificationsGwy24";
        public const string LOANDMSDOCUMENTSVERIFICATION = "p_DMSLoanDocumentsVerificationsGwy24";
        public const string DMSBOARDAPPROVAL = "p_DMSBoardApprovalGwy24";
        public const string EDMSBOARDALOANSANCTION= "p_EDMSBoardLoanSanctionGwy24";
        public const string MEMBERCARDMODIFY = "p_AddEditMemberCardGwy24";
        public const string BRAUDITLOG = "p_AddAuditLogInformationGwy24";
        public const string GETCODES = "p_GetCodesGwy24";




        //Products
        public const string GETPRODUCTLIST = "p_GetProductListGwy24";
        public const string GETPRODUCTWORKFLOW = "p_GetProductWorkflowGwy24";
        public const string GETFDRATES = "p_GetFDRatesGwy24";

        public const string LOANCALCULATOR = "p_GetLoanInstalmentGwy24";
        public const string LOANCALCULATORMIN = "p_CalculateLoanScheduleGwy24";

        public const string ONBOARDINGCALLBACK = "CustomerLMSRegistrationCallback/registration";

        //ESS Loans------------------------------------------------------------------------------
        //Loan Operations
        public const string ESSLOAN_CHARGES_REQUEST = "p_ESSnewLoanLNRequestGwy24";
        public const string ESSLOAN_VERIFICATION = "p_ESSLoanVerificationLNRequestGwy24";
        public const string ESSLOAN_TOPUP_REQUEST = "p_ESSLoanTopUpLNRequestGwy24";
        public const string ESSLOAN_RESTRUCTURE_REQUEST = "p_ESSLoanRestructureLNRequestGwy24";
        public const string ESSLOAN_TAKEOVER_REQUEST = "p_ESSLoanTakeOverLNRequestGwy24";

        //Payment Operations
        public const string ESSPAYMENT_NOTIFICATION = "p_ESSPaymentNotifcationLNRequestGwy24";
        public const string ESSPAYOFF_BALANCE_REQUEST = "p_ESSPayOffBalanceLNRequestGwy24";
        public const string ESSPARTIAL_REPAYMENT_REQUEST = "p_ESSPartialRepaymentLNRequestGwy24";
        public const string ESSMONTHLY_DEDUCTION_RECORD = "p_ESSMonthlyDeductionLNRequestGwy24";

        //Status Notifications
        public const string ESSLOAN_STATUS_RESPONSE = "p_ESSLoanStatusLNRequestGwy24";
        public const string ESSDEFAULTER_DETAILS = "p_ESSDefaulterdetailsLNRequestGwy24";
        public const string ESSDEDUCTION_STOP_NOTIFICATION = "p_ESSDeductionStopNotificationLNRequestGwy24";
        public const string ESSEMPLOYEE_CANCELLATION = "p_ESSEmployeeCancellationLNRequestGwy24";
        public const string ESSDISBURSEMENT_FAILURE = "p_ESSDisbursmentFailureLNRequestGwy24";

        //System Operations
        public const string ESSACCOUNT_VALIDATION_REQUEST = "p_ESSAccountValidationLNRequestGwy24";
        public const string ESSFSP_BRANCHES_REQUEST = "p_ESSFSPBranchesLNRequestGwy24";
        public const string ESSGENERAL_RESPONSE = "p_ESSGeneralResponseLNRequestGwy24";
        //---------------------------------------------------------------------------------------



        public static string FormatMobile(string mobile,string CountryCode)
        {
            string mobileNo = mobile;
            
            PhoneNumber pn = PhoneNumberUtil.GetInstance().Parse(mobileNo, CountryCode);
            mobileNo = PhoneNumberUtil.GetInstance().Format(pn, PhoneNumberFormat.INTERNATIONAL);
            //Remove White space in between
            mobileNo = new string(mobileNo.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            return mobileNo;
        }

        

        public static string ConvertToBase64(string str)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(str);
            string base64String = Convert.ToBase64String(jsonBytes);
            return base64String;
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Method to encrypt the string
        public static string Encrypt(string input)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(input);
                    }

                    byte[] encryptedBytes = ms.ToArray();
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        // Method to decrypt the string
        public static string Decrypt(string input)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(Convert.FromBase64String(input)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string GenerateUniqueToken(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";  // Uppercase and digits only
            string token;

            do
            {
                token = new string(Enumerable.Range(0, length)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray());
            } while (generatedTokens.Contains(token));  // Check if the token is already generated

            generatedTokens.Add(token);  // Store the token to prevent future duplication

            return token;
        }

        public static string GeneratePassword(int Length, int NonAlphaNumericChars)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            string allowedNonAlphaNum = "!@#$%^&*()_-+=[{]};:<>|./?";
            Random rd = new Random();

            if (NonAlphaNumericChars > Length || Length <= 0 || NonAlphaNumericChars < 0)
                throw new ArgumentOutOfRangeException();

            char[] pass = new char[Length];
            int[] pos = new int[Length];
            int i = 0, j = 0, temp = 0;
            bool flag = false;

            //Random the position values of the pos array for the string Pass
            while (i < Length - 1)
            {
                j = 0;
                flag = false;
                temp = rd.Next(0, Length);
                for (j = 0; j < Length; j++)
                    if (temp == pos[j])
                    {
                        flag = true;
                        j = Length;
                    }

                if (!flag)
                {
                    pos[i] = temp;
                    i++;
                }
            }

            //Random the AlphaNumericChars
            for (i = 0; i < Length - NonAlphaNumericChars; i++)
                pass[i] = allowedChars[rd.Next(0, allowedChars.Length)];

            //Random the NonAlphaNumericChars
            for (i = Length - NonAlphaNumericChars; i < Length; i++)
                pass[i] = allowedNonAlphaNum[rd.Next(0, allowedNonAlphaNum.Length)];

            //Set the sorted array values by the pos array for the rigth posistion
            char[] sorted = new char[Length];
            for (i = 0; i < Length; i++)
                sorted[i] = pass[pos[i]];

            string Pass = new String(sorted);

            return Pass;
        }

        //Sends Email



    }

    static public class OurEncryption
    {
        static public string EncryptText(string strInputText)
        {
            byte[] data = Array.ConvertAll<char, byte>(strInputText.ToCharArray(), delegate (char ch) { return (byte)ch; });
            SHA256 shaM = new SHA256Managed();
            byte[] result = shaM.ComputeHash(data);
            return Convert.ToBase64String(result);
        }
        // Below Four Method Added by vijil.jones
        public static string Encrypt(string clearText, string Password)
        {
            // First we need to turn the input string into a byte array. 
            byte[] clearBytes =
              System.Text.Encoding.Unicode.GetBytes(clearText);

            // Then, we need to turn the password into Key and IV 
            // We are using salt to make it harder to guess our key
            // using a dictionary attack - 
            // trying to guess a password by enumerating all possible words. 
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // Now get the key/IV and do the encryption using the
            // function that accepts byte arrays. 
            // Using PasswordDeriveBytes object we are first getting
            // 32 bytes for the Key 
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV. 
            // IV should always be the block size, which is by default
            // 16 bytes (128 bit) for Rijndael. 
            // If you are using DES/TripleDES/RC2 the block size is
            // 8 bytes and so should be the IV size. 
            // You can also read KeySize/BlockSize properties off
            // the algorithm to find out the sizes. 
            byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            // Now we need to turn the resulting byte array into a string. 
            // A common mistake would be to use an Encoding class for that.
            //It does not work because not all byte values can be
            // represented by characters. 
            // We are going to be using Base64 encoding that is designed
            //exactly for what we are trying to do. 
            return Convert.ToBase64String(encryptedData);

        }
        public static string Decrypt(string cipherText, string Password)
        {
            // First we need to turn the input string into a byte array. 
            // We presume that Base64 encoding was used 
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // Then, we need to turn the password into Key and IV 
            // We are using salt to make it harder to guess our key
            // using a dictionary attack - 
            // trying to guess a password by enumerating all possible words. 
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // Now get the key/IV and do the decryption using
            // the function that accepts byte arrays. 
            // Using PasswordDeriveBytes object we are first
            // getting 32 bytes for the Key 
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV. 
            // IV should always be the block size, which is by
            // default 16 bytes (128 bit) for Rijndael. 
            // If you are using DES/TripleDES/RC2 the block size is
            // 8 bytes and so should be the IV size. 
            // You can also read KeySize/BlockSize properties off
            // the algorithm to find out the sizes. 
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            // Now we need to turn the resulting byte array into a string. 
            // A common mistake would be to use an Encoding class for that.
            // It does not work 
            // because not all byte values can be represented by characters. 
            // We are going to be using Base64 encoding that is 
            // designed exactly for what we are trying to do. 
            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }
        public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream to accept the encrypted bytes 
            MemoryStream ms = new MemoryStream();

            // Create a symmetric algorithm. 
            // We are going to use Rijndael because it is strong and
            // available on all platforms. 
            // You can use other algorithms, to do so substitute the
            // next line with something like 
            //      TripleDES alg = TripleDES.Create(); 
            Rijndael alg = Rijndael.Create();

            // Now set the key and the IV. 
            // We need the IV (Initialization Vector) because
            // the algorithm is operating in its default 
            // mode called CBC (Cipher Block Chaining).
            // The IV is XORed with the first block (8 byte) 
            // of the data before it is encrypted, and then each
            // encrypted block is XORed with the 
            // following block of plaintext.
            // This is done to make encryption more secure. 

            // There is also a mode called ECB which does not need an IV,
            // but it is much less secure. 
            alg.Key = Key;
            alg.IV = IV;

            // Create a CryptoStream through which we are going to be
            // pumping our data. 
            // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream and the output will be written
            // in the MemoryStream we have provided. 
            CryptoStream cs = new CryptoStream(ms,
               alg.CreateEncryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the encryption 
            cs.Write(clearData, 0, clearData.Length);

            // Close the crypto stream (or do FlushFinalBlock). 
            // This will tell it that we have done our encryption and
            // there is no more data coming in, 
            // and it is now a good time to apply the padding and
            // finalize the encryption process. 
            cs.Close();

            // Now get the encrypted data from the MemoryStream.
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way. 
            byte[] encryptedData = ms.ToArray();

            return encryptedData;
        }
        public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream that is going to accept the
            // decrypted bytes 
            MemoryStream ms = new MemoryStream();

            // Create a symmetric algorithm. 
            // We are going to use Rijndael because it is strong and
            // available on all platforms. 
            // You can use other algorithms, to do so substitute the next
            // line with something like 
            //     TripleDES alg = TripleDES.Create(); 
            Rijndael alg = Rijndael.Create();

            // Now set the key and the IV. 
            // We need the IV (Initialization Vector) because the algorithm
            // is operating in its default 
            // mode called CBC (Cipher Block Chaining). The IV is XORed with
            // the first block (8 byte) 
            // of the data after it is decrypted, and then each decrypted
            // block is XORed with the previous 
            // cipher block. This is done to make encryption more secure. 
            // There is also a mode called ECB which does not need an IV,
            // but it is much less secure. 
            alg.Key = Key;
            alg.IV = IV;

            // Create a CryptoStream through which we are going to be
            // pumping our data. 
            // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream 
            // and the output will be written in the MemoryStream
            // we have provided. 
            CryptoStream cs = new CryptoStream(ms,
                alg.CreateDecryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the decryption 
            cs.Write(cipherData, 0, cipherData.Length);

            // Close the crypto stream (or do FlushFinalBlock). 
            // This will tell it that we have done our decryption
            // and there is no more data coming in, 
            // and it is now a good time to remove the padding
            // and finalize the decryption process. 
            cs.Close();

            // Now get the decrypted data from the MemoryStream. 
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way. 
            byte[] decryptedData = ms.ToArray();

            return decryptedData;
        }
    }


    public class ApiService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> DecOutImageAsync(string val, string url)
        {            
            var content = new StringContent($"{{\"val\":\"{val}\"}}", null, "application/json");            
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            try
            {                
                var response = await client.SendAsync(request);                
                response.EnsureSuccessStatusCode();
                string res = await response.Content.ReadAsStringAsync();                
                return res;
            }
            catch (HttpRequestException e)
            {                
                return $"Request failed: {e.Message}";
            }
        }

        public async Task<string> EncInImageAsync(string val, string url)
        {
            var content = new StringContent($"{{\"val\":\"{val}\"}}", null, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                return $"Request failed: {e.Message}";
            }
        }

        public async Task<string> UploadToEdms(string val, string url)
        {
            var content = new StringContent(val, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); 

            try
            {
                var response = await client.SendAsync(request, cts.Token);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            {
                return "Request timed out.";
            }
            catch (HttpRequestException e)
            {
                return $"Request failed: {e.Message}";
            }
        }


    }
}
