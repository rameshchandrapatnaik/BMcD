using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using BMcDExtensibilityService.Core;
using BMcDExtensibilityService.Core.Services;
using Serilog;
namespace BMcDExtensibilityService.Custom.Workers
{
    public class AuthenticationHostedService : IHostedService
    {
        private Timer _timer;
        private readonly ILogger<AuthenticationHostedService> _logger;
        private AuthenticationService mobjAuthenticationService;
        public bool blnIsTokenReady = false;
        public bool blnIsErrorinFetchingToken = false;
        private ExtensibilityConfiguration _config;
        private ExtensibilityODataClient mobjODataClient;
        private ExtensibilityRestClient mobjRestClient;
        private struct State
        {
            public static int numberOfActiveJobs = 0;
            public const int maxNumberOfActiveJobs = 1;
        }
        public AuthenticationHostedService(ILogger<AuthenticationHostedService> logger,
            AuthenticationService authenticationService, ExtensibilityODataClient extensibilityOdataClient, ExtensibilityConfiguration config,
            ExtensibilityRestClient extensibilityRestClient)
        {
            _logger = logger;
            mobjAuthenticationService = authenticationService;
            _config = config;
            mobjODataClient = extensibilityOdataClient;
            mobjRestClient = extensibilityRestClient;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Int32 lintRenewIntervalmilliSeconds = (mobjAuthenticationService.mintTokenExpiryinSeconds - 1800) * 1000 ;
                Int32 lintRenewIntervalmilliSeconds = (mobjAuthenticationService.mintTokenExpiryinSeconds) * 1000;
                _timer = new Timer(AuthenticateWithSDx, null, lintRenewIntervalmilliSeconds, lintRenewIntervalmilliSeconds);
                Log.Information("Started Timer for Authentication: Token will be renewed in (seconds)" + (lintRenewIntervalmilliSeconds / 1000));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while starting the Token Renewal Timer.");
            }
            return Task.CompletedTask;
        }
        void AuthenticateWithSDx(Object state)
        {
            if (State.numberOfActiveJobs < State.maxNumberOfActiveJobs)
            {
                try
                {
                    Interlocked.Increment(ref State.numberOfActiveJobs);
                    Log.Information("Renewing Token....");
                    Console.WriteLine("Renewing Token");
                    mobjAuthenticationService.RenewToken(_config);
                    mobjODataClient.UpdateODataToken(mobjAuthenticationService.token);
                    mobjRestClient.UpdateTokenForRESTandHTTPClient(mobjAuthenticationService.token);
                    //Int32 lintRenewIntervalmilliSeconds = (mobjAuthenticationService.mintTokenExpiryinSeconds - 1800) * 1000;
                    Int32 lintRenewIntervalmilliSeconds = (mobjAuthenticationService.mintTokenExpiryinSeconds) * 1000;
                    _timer?.Change(lintRenewIntervalmilliSeconds, lintRenewIntervalmilliSeconds);
                   Console.WriteLine("Token Expires in Seconds...." + mobjAuthenticationService.mintTokenExpiryinSeconds, mobjAuthenticationService.token);
               
                   Log.Information("Token Expires in Seconds...." + mobjAuthenticationService.mintTokenExpiryinSeconds, mobjAuthenticationService.token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred in Token Renewal.");
                    Log.Error(ex, "Error occurred in Token Renewal.");
                }
                finally
                {
                    Interlocked.Decrement(ref State.numberOfActiveJobs);
                }
            }
            else
            {
                Console.WriteLine("Skipped Token Renewal");
               Log.Information("Skipped Token Renewal");
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}