using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Services
{
    public interface ICurrencyConverter
    {
        public decimal ConvertMoney(decimal money, Currency from, Currency to);
    }

    public class CurrencyConverterCbr: ICurrencyConverter
    {
        public decimal ConvertMoney(decimal money, Currency from, Currency to)
        {
            if (from == to) return money;
            // var enc1251 = CodePagesEncodingProvider.Instance.GetEncoding(1251);
            // CodePagesEncodingProvider.Instance.GetEncoding(1251);
            var xdoc= XDocument.Load("https://www.cbr-xml-daily.ru/daily_utf8.xml");
            var valCurs = xdoc.Element("ValCurs").Elements("Valute");

            (int nominal, decimal value) a = (1, 1);
            (int nominal, decimal value) b = (1, 1);
            if (from.CharCode!="RUB")
            {
                var valFrom = valCurs.First(c => c.Element("CharCode").Value == from.CharCode);
                a = (
                    int.Parse(valFrom.Element(("Nominal")).Value), 
                    decimal.Parse(valFrom.Element("Value").Value));
            }
            if (to.CharCode != "RUB")
            {
                var valTo = valCurs.First(c => c.Element("CharCode").Value == to.CharCode);
                b = (
                    int.Parse(valTo.Element(("Nominal")).Value), 
                    decimal.Parse(valTo.Element("Value").Value));
            }
            
            //<convert @from to RUB and then convert to @to currency>
            return money * a.value / a.nominal / b.value * b.nominal;
            
        }
    }

    public class CurrencyConverterUseless : ICurrencyConverter
    {
        public decimal ConvertMoney(decimal money, Currency from, Currency to)
        {
            return money;
        }
    }
}