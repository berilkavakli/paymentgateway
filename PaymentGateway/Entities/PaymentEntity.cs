using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Entities
{
    [TableName("Payment")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class PaymentEntity
    {
        [Column("Id")]
        public long Id { get; set; }
        [Column("Code")]
        public Guid Code { get; set; }
        [Column("Amount")]
        public double Amount { get; set; }
        [Column("Currency")]
        public string Currency { get; set; }
        [Column("CardId")]
        public long CardId { get; set; }
        [Column("Status")]
        public bool Status { get; set; }
        [Column("Message")]
        public string Message { get; set; }
    }
}
