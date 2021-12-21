using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace euromilregister.Services
{
    public class EuroMilService : Euromil.EuromilBase
    {
        private readonly ILogger<EuroMilService> _logger;
        public EuroMilService(ILogger<EuroMilService> logger)
        {
            _logger = logger;
        }

        public override Task<RegisterReply> RegisterEuroMil(RegisterRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Recieved request to register Key {request.Key} with CheckID {request.Checkid}.");

            string replyMessage = "";

            if (!ValidateKey(request.Key) || !ValidateId(request.Checkid))
            {
                replyMessage = "Error. Key or CheckID is not Valid.";
                _logger.LogError("Error.Key or CheckID is not Valid.");
            }
            else
            {
                replyMessage = $"Success. Key ({request.Key}) is registered with CheckID ({request.Checkid})";
                _logger.LogInformation($"Success. Key ({request.Key}) is registered with CheckID ({request.Checkid})");
            }

            return Task.FromResult(new RegisterReply
            {
                Message = replyMessage
            });
        }

        private bool ValidateKey(string key)
        {
            string[] splitKey = key.Split(" ");
            int[] numKey = new int[5];
            int[] numStar = new int[2];

            if (splitKey.Count() != 7)
                return false;

            for (int i = 0; i < 7; i++)
            {
                if (i < 5)
                {
                    if (!Int32.TryParse(splitKey[i], out numKey[i]) || numKey[i] <= 0 || numKey[i] > 50)
                        return false;
                }
                else
                {
                    if (!Int32.TryParse(splitKey[i], out numStar[i - 5]) || numStar[i-5] <= 0 || numStar[i-5] > 12)
                        return false;
                }
            }

            if (numKey.Distinct().Count() != 5 || numStar.Distinct().Count() != 2)
            {
                return false;
            }

            return true;
        }

        private bool ValidateId(string checkid)
        {
            if (checkid.Length != 8 || !Double.TryParse(checkid, out _))
            {
                return false;
            }

            return true;
        }
    }
}
