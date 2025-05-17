using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Core
{
    /// <summary>
    /// Responsible for obtaining the config out of the App.config
    /// </summary>
    public class ExtensibilityConfiguration
    {
        public string AMQPBrokerHostname { set; get; }
        public string AMQPBrokerUsername{ set; get; }
        public string AMQPBrokerPassword{ set; get; }
        public int AMQPBrokerPort { set; get; }
        public string AMQPBrokerProtocol{ set; get; }
        public string AMQPBrokerQueue{ set; get; }
        public string ServerBaseUri{ set; get; }
        public string ServerResourceID{ set; get; }
        public bool AuthUseClientCredentialsFlow{ set; get; }
        public string AuthServerAuthority{ set; get; }
        public string AuthClientId{ set; get; }
        public string AuthClientSecret{ set; get; }
        public string AuthUsername{ set; get; }
        public string AuthPassword{ set; get; }
        public string AzureAuthServerAuthority{ set; get; }
        public string AzureAuthClientId{ set; get; }
        public string AzureAuthClientSecret{ set; get; }
        public string AzureResourceID{ set; get; }
        public string ControlFilePath { set; get; }

        public string ReportFolderPath { set; get; }

        public string EmailWorkflowName { set; get; }
       
        public string[] AutoStampClassification { set; get; }
        public string AutostampPath { set; get; }
        public string OldNameinXFDF { set; get; }
        public string OldDateInXFDF { set; get; }

        public ExtensibilityConfiguration()
        {
           Log.Information("Loaded system configuration ...");
        }
        public ExtensibilityConfiguration(bool loadFromAppConfig = false)
        {
            Log.Information("Loading system configuration ...");

            if (loadFromAppConfig)
            {
                LoadConfiguration();
            }

            Log.Information("Loaded system configuration");
        }


        private void LoadConfiguration()
        {
            try
            {
                AMQPBrokerHostname = ConfigurationManager.AppSettings["AMQPBrokerHostname"];
                AMQPBrokerUsername = ConfigurationManager.AppSettings["AMQPBrokerUsername"];
                AMQPBrokerPassword = ConfigurationManager.AppSettings["AMQPBrokerPassword"];
                AMQPBrokerPort = Int32.Parse(ConfigurationManager.AppSettings["AMQPBrokerPort"]);
                AMQPBrokerProtocol = ConfigurationManager.AppSettings["AMQPBrokerProtocol"];
                AMQPBrokerQueue = ConfigurationManager.AppSettings["AMQPBrokerQueue"];
                ServerBaseUri = ConfigurationManager.AppSettings["ServerBaseUri"];
                ServerResourceID = ConfigurationManager.AppSettings["ServerResourceID"];
                AuthServerAuthority = ConfigurationManager.AppSettings["AuthServerAuthority"];
                AuthUseClientCredentialsFlow = bool.Parse(ConfigurationManager.AppSettings["AuthUseClientCredentialsFlow"]);
                AuthClientId = ConfigurationManager.AppSettings["AuthClientId"];
                AuthClientSecret = ConfigurationManager.AppSettings["AuthClientSecret"];
                AuthUsername = ConfigurationManager.AppSettings["AuthUsername"];
                AuthPassword = ConfigurationManager.AppSettings["AuthPassword"];
                AzureAuthServerAuthority = ConfigurationManager.AppSettings["AzureAuthServerAuthority"];
                AzureAuthClientId = ConfigurationManager.AppSettings["AzureAuthClientId"];
                AzureAuthClientSecret = ConfigurationManager.AppSettings["AzureAuthClientSecret"];
                AzureResourceID = ConfigurationManager.AppSettings["AzureResourceID"];
                ControlFilePath= ConfigurationManager.AppSettings["ControlFilePath"];
                ReportFolderPath = ConfigurationManager.AppSettings["ReportFolderPath"];
                EmailWorkflowName = ConfigurationManager.AppSettings["EmailWorkflowName"];
                var AutoStampClassificationValues = ConfigurationManager.AppSettings["AutoStampClassification"];
                if (!string.IsNullOrEmpty(AutoStampClassificationValues))
                {
                    AutoStampClassification = AutoStampClassificationValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    AutoStampClassification = Array.Empty<string>();
                }
                AutostampPath = ConfigurationManager.AppSettings["AutostampPath"];
                OldNameinXFDF = ConfigurationManager.AppSettings["OldNameinXFDF"];
                OldDateInXFDF = ConfigurationManager.AppSettings["OldDateInXFDF"];
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while loading the configuration");
            }
        }
    }
}
