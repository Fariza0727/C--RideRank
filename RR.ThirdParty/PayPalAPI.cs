using Microsoft.Extensions.Options;
using PayPal.Payments.Common;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.DataObjects;
using PayPal.Payments.Transactions;
using RR.Core;
using RR.Dto;
using System;
using System.Linq;

namespace RR.ThirdParty
{
    public class PayPalAPI
    {
        private readonly PayPalSettings _payPalSettings;

        public PayPalAPI(IOptions<PayPalSettings> payPalSettings)
        {
            _payPalSettings = payPalSettings.Value;
        }

        public PayPalResponseDto PayNow(PayPalDto payPalDto)
        {
            try
            {
                UserInfo User = new UserInfo(_payPalSettings.User, _payPalSettings.Vendor, _payPalSettings.Partner, _payPalSettings.Password);

                // Create a new Invoice data object with the Amount, Billing Address etc. details.
                Invoice Inv = new Invoice();

                // Set Amount.
                Currency Amt = new Currency(payPalDto.Amount);
                Inv.Amt = Amt;
                Inv.PoNum = payPalDto.PONumber;
                Inv.InvNum = payPalDto.InvNumber;

                // Set the Billing Address details.
                BillTo Bill = new BillTo();
                Bill.Street = payPalDto.Address.Address1;
                Bill.Zip = payPalDto.Address.ZipCode;
                Inv.BillTo = Bill;

                // Create a new Payment Device - Credit Card data object.
                // The input parameters are Credit Card Number and Expiration Date of the Credit Card.
                // CreditCard CC = new CreditCard("5105105105105100", "0109");
                CreditCard CC = new CreditCard(payPalDto.CreditCardDetails.CardNumber, payPalDto.CreditCardDetails.ExpiryDate);
                CC.Cvv2 = payPalDto.CreditCardDetails.CVV;

                // Create a new Tender - Card Tender data object.
                CardTender Card = new CardTender(CC);
                ///////////////////////////////////////////////////////////////////

                // Create a new Sale Transaction.
                SaleTransaction Trans = new SaleTransaction(
                    User, new PayflowConnectionData(_payPalSettings.Host), Inv, Card, PayflowUtility.RequestId);

                // Set the transaction verbosity to MEDIUM.
                Trans.Verbosity = "MEDIUM";

                // Submit the Transaction
                Response Resp = Trans.SubmitTransaction();

                // Display the transaction response parameters.
                if (Resp != null)
                {
                    // Get the Transaction Response parameters.
                    TransactionResponse TrxnResponse = Resp.TransactionResponse;
                    if (TrxnResponse != null)
                    {
                        PayPalResponseDto payPalResponseDto = new PayPalResponseDto
                        {
                            IsSuccess = true,
                            AddlMsgs = TrxnResponse.AddlMsgs,
                            AuthCode = TrxnResponse.AuthCode,
                            AVSAddr = TrxnResponse.AVSAddr,
                            AVSZip = TrxnResponse.AVSZip,
                            CVV2Match = TrxnResponse.CVV2Match,
                            HostCode = TrxnResponse.HostCode,
                            IAVS = TrxnResponse.IAVS,
                            PnRef = TrxnResponse.Pnref,
                            ProcAVS = TrxnResponse.ProcAVS,
                            ProcCVV2 = TrxnResponse.ProcCVV2,
                            RespMsg = TrxnResponse.RespMsg,
                            RespText = TrxnResponse.RespText,
                            Result = TrxnResponse.Result
                        };

                        return payPalResponseDto;
                    }

                    // Get the Fraud Response parameters.
                    FraudResponse FraudResp = Resp.FraudResponse;
                    // Display Fraud Response parameter
                    if (FraudResp != null)
                    {
                        PayPalResponseDto payPalResponseDto = new PayPalResponseDto
                        {
                            IsSuccess = false,
                            RespText = FraudResp.PreFpsMsg + FraudResp.PostFpsMsg
                        };

                        return payPalResponseDto;
                    }

                    Context TransCtx = Resp.TransactionContext;
                    if (TransCtx != null && TransCtx.getErrorCount() > 0)
                    {
                        PayPalResponseDto payPalResponseDto = new PayPalResponseDto
                        {
                            IsSuccess = false,
                            RespText = string.Join(",", TransCtx.GetErrors().ToArray().Select(p => Convert.ToString(p)))
                        };

                        return payPalResponseDto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }
    }
}
