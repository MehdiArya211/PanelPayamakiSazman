using Domain.Enums.Project;

namespace DTO.Project.SmsTariffs
{
    public class SmsTariffListDTO
    {
        public string Id { get; set; }
        public string Operator { get; set; }
        public decimal PersianPricePerSegment { get; set; }
        public decimal EnglishPricePerSegment { get; set; }
    }

    public class SmsTariffCreateDTO
    {
        public SmsOperatorType Operator { get; set; }
        public decimal PersianPricePerSegment { get; set; }
        public decimal EnglishPricePerSegment { get; set; }
    }

    public class SmsTariffEditDTO : SmsTariffCreateDTO
    {
        public string Id { get; set; }
    }
}
