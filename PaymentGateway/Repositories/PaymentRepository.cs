using Microsoft.Extensions.Configuration;
using Npgsql;
using NPoco;
using PaymentGateway.Entities;

using PaymentGateway.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        readonly string _dbConnection;
        public PaymentRepository(IConfiguration configuration)
        {
            _dbConnection = configuration.GetConnectionString("PaymentConnectionString");
        }

        /// <summary>
        /// Get credit card information from postgres db.
        /// </summary>
        public CardInformationEntity GetCardInformation(long id)
        {
            var cardInfo = GetDbConenctor(_dbConnection).Query<CardInformationEntity>().Where(p => p.Id == id).FirstOrDefault();
            return cardInfo;
        }

        /// <summary>
        /// Get payment information from postgres db.
        /// </summary>
        public PaymentEntity GetPayment(Guid code)
        {
            var payment = GetDbConenctor(_dbConnection).Query<PaymentEntity>().Where(p => p.Code == code).FirstOrDefault();
            return payment;
        }

        /// <summary>
        /// Saves credit card information into postgres db.
        /// If same card number exists in db, does not save a new one.
        /// </summary>
        public void SaveCardInformation(CardInformationEntity cardInfo)
        {
            var existingCardInfo = GetDbConenctor(_dbConnection).Query<CardInformationEntity>().Where(p => p.CardNumber == cardInfo.CardNumber).FirstOrDefault();
            if (existingCardInfo != null)
                cardInfo.Id = existingCardInfo.Id;
            else
                GetDbConenctor(_dbConnection).Save(cardInfo);
        }

        /// <summary>
        /// Saves payment information into postgres db.
        /// </summary>
        public void SavePayment(PaymentEntity payment)
        {
            GetDbConenctor(_dbConnection).Save(payment);
        }

        /// <summary>
        /// Creates postgres client
        /// </summary>
        private Database GetDbConenctor(string connectionString)
        {
            var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            return new Database(conn);
        }
    }
}
