using System;
using System.Threading.Tasks;
using Kin.Stellar.Sdk;
using Kin.Stellar.Sdk.responses;
using KinExplorerCreateAccount.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KinExplorerCreateAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly KeyPair _keyPair;
        private readonly Server _server;
        private readonly string _app_id;
        public CreateController(ILoggerFactory loggerFactory, IConfiguration config, Server server)
        {
            _logger = loggerFactory.CreateLogger<CreateController>();
            _keyPair = KeyPair.FromSecretSeed(config["Secret_Seed"]);
            _app_id = config["App_Id"];
            _server = server;

        }

        // GET api/values/5
        [HttpGet("{addr}")]
        public  ActionResult<CreateAccountResponse> Get(string addr)
        {
            try
            {
                SubmitTransactionResponse txResponse = GetCreateAccountTransaction(addr).GetAwaiter().GetResult();
                return Ok(new CreateAccountResponse()
                {
                    Message = txResponse != null ? null : "Transaction failed to submit",
                    Success = txResponse != null,
                    TxResponse = txResponse
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new CreateAccountResponse()
                {
                    Message = e.Message,
                    Success = false
                });
            }
           
        }

        private async Task<SubmitTransactionResponse> GetCreateAccountTransaction(string destinationAddress)
        {
            KeyPair destinationKeyPair = KeyPair.FromAccountId(destinationAddress);
            AccountResponse sourceAccount = null;
            AccountResponse destinationAccount = null;

            try
            {
                sourceAccount = await GetAccount(_keyPair).ConfigureAwait(false);
                destinationAccount = await GetAccount(destinationKeyPair).ConfigureAwait(false);
            }
            catch (Exception ex){}


            if (sourceAccount == null)
            {
                throw new Exception("Source account doesn't exists");
            }

            if (destinationAccount != null)
            {
                throw new Exception("Account already exists");
            }


            CreateAccountOperation.Builder createAccountOperationBuilder = new CreateAccountOperation.Builder(destinationKeyPair, "0");
            createAccountOperationBuilder.SetSourceAccount(_keyPair);
            
            Transaction.Builder transactionBuilder = new Transaction.Builder(new Account(_keyPair, sourceAccount.SequenceNumber));
            transactionBuilder.AddOperation(createAccountOperationBuilder.Build());
            transactionBuilder.AddMemo(new MemoText($"1-{_app_id}"));

            Transaction transaction = transactionBuilder.Build();
            transaction.Sign(_keyPair);

            return await _server.SubmitTransaction(transaction).ConfigureAwait(false);
        }
        private async Task<AccountResponse> GetAccount(KeyPair account)
        {
            AccountResponse accountResponse = await _server.Accounts.Account(account).ConfigureAwait(false);
            return accountResponse;
        }
    }
}
