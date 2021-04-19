using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Entities
{
    [TableName("CardInformation")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class CardInformationEntity
    {
        [Column("Id")]
        public long Id { get; set; }
        [Column("CardNumber")]
        public string CardNumber { get; set; }
        [Column("ExpiryMonth")]
        public int ExpiryMonth { get; set; }
        [Column("ExpiryYear")]
        public int ExpiryYear { get; set; }
        [Column("Cvv")]
        public int Cvv { get; set; }
    }
}
