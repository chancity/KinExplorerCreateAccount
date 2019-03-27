using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kin.Stellar.Sdk.responses;

namespace KinExplorerCreateAccount.Model
{
    public class CreateAccountResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public SubmitTransactionResponse TxResponse { get; set; }
    }
}
