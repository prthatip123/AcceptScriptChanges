﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;

namespace net.authorize.sample
{
    public class GetAnAcceptPaymentPage
    {
        public static ANetApiResponse Run(String ApiLoginID, String ApiTransactionKey,string hostedPaymentIFrameCommunicatorUrl,string customerProfileId = null)
        {
            Console.WriteLine("GetAnAcceptPaymentPage Sample");
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey,
            };

            settingType[] settings = new settingType[5];

            settings[0] = new settingType();
            settings[0].settingName = settingNameEnum.hostedPaymentButtonOptions.ToString();
            settings[0].settingValue = "{\"text\": \"Pay\"}";
            
            settings[1] = new settingType();
            settings[1].settingName = settingNameEnum.hostedPaymentOrderOptions.ToString();
            settings[1].settingValue = "{\"show\": false}";


	        settings[2] = new settingType();
	        settings[2].settingName = settingNameEnum.hostedPaymentIFrameCommunicatorUrl.ToString();
	        settings[2].settingValue = "{\"url\": \""+ hostedPaymentIFrameCommunicatorUrl +"\"}";

	        settings[3] = new settingType();
	        settings[3].settingName = settingNameEnum.hostedPaymentBillingAddressOptions.ToString();
	        settings[3].settingValue = "{\"show\": false}";
			
			settings[4] = new settingType();
			settings[4].settingName = settingNameEnum.hostedPaymentReturnOptions.ToString();
			settings[4].settingValue = "{\"showReceipt\": false,\"url\":\"" + hostedPaymentIFrameCommunicatorUrl + "\",\"urlText\":\"Continue\",\"cancelUrlText\":\"Cancel\"}";
		

			var custprofile = new customerProfilePaymentType
			{
				customerProfileId = customerProfileId
			};

			var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // authorize capture only
                amount = 99,
				profile = custprofile

			};

            var request = new getHostedPaymentPageRequest();
            request.transactionRequest = transactionRequest;
            request.hostedPaymentSettings = settings;

            // instantiate the contoller that will call the service
            var controller = new getHostedPaymentPageController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            //validate
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                Console.WriteLine("Message code : " + response.messages.message[0].code);
                Console.WriteLine("Message text : " + response.messages.message[0].text);
                Console.WriteLine("Token : " + response.token);
            }
            else if (response != null)
            {
                Console.WriteLine("Error: " + response.messages.message[0].code + "  " + response.messages.message[0].text);
                Console.WriteLine("Failed to get hosted payment page");
            }

            return response;
        }
    }
}
