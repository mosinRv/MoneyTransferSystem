using System;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem
{
    public interface ICurrencyConverter
    {
        public decimal ConvertMoney(decimal money, Currency from, Currency to);
    }

    public class CurrencyConverterCbr: ICurrencyConverter
    {
        //Cbr means ЦБР (Центральный Банк России)
        public decimal ConvertMoney(decimal money, Currency @from, Currency to)
        {
            if (from == to) return money;
            
            // TODO запрос на получение курсов валют
            // http://www.cbr.ru/scripts/XML_daily.asp
            return 0;
            //<convert @from to RUB and then convert to @to currency>
        }
    }

    public class CurrencyConverterUseless : ICurrencyConverter
    {
        public decimal ConvertMoney(decimal money, Currency @from, Currency to)
        {
            return money;
        }
    }
}