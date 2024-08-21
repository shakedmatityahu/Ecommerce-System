
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Payment
{
    public class PaymentDetails
    {
        private String _cardNumber;
        private String _exprYear;
        private String _exprMonth;
        private String _cvv;
        private String _holderID; 
        private String _holderName; 

        private String _currency;


        public PaymentDetails (String currency, String cardNumber, String exprYear, String exprMonth, String cvv, String cardId, String name)
        {
            this._cardNumber = cardNumber;
            this._exprMonth =  exprMonth;
            this._exprYear = exprYear;
            this._cvv = cvv;
            this._holderID = cardId;
            this._holderName = name;
            this._currency = currency;
        }

        public String CardNumber {get => _cardNumber; set => _cardNumber = value; }
        public String ExprMonth {get => _exprMonth; set => _exprMonth = value; }
        public String ExprYear {get => _exprYear; set => _exprYear = value; }
        public String Cvv {get => _cvv; set => _cvv = value; }
        public String HolderID {get => _holderID; set => _holderID = value; }
        public String HolderName {get => _holderName; set => _holderName = value; }

        public String Currency {get => _currency; set => _currency = value; }


    }

}
