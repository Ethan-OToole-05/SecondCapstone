using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public List<Transfer> ListByUser(int userId, bool pending)
        {
            List<Transfer> returnedTransfers = new List<Transfer>();

            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();


                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfers JOIN accounts ON account_from = accounts.account_id OR account_to = accounts.account_id WHERE user_id = @user_id;", conn);
                    
                    if (pending)
                    {
                        cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfers JOIN accounts ON account_from = accounts.account_id OR account_to = accounts.account_id WHERE user_id = @user_id AND transfer_status_id = 1 AND account_from = (SELECT account_id FROM accounts WHERE user_id = @user_id)", conn);
                    }

                    cmd.Parameters.AddWithValue("@user_id", userId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        Transfer tr = GetTransferFromReader(reader);
                        returnedTransfers.Add(tr);
                    }
                }
            } 
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }

            return returnedTransfers;
        }

        public Transfer GetTransferById(int Id)
        {
            Transfer returnedTransfer = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM transfers WHERE transfer_id = @id;", conn);
                    cmd.Parameters.AddWithValue("@id", Id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        returnedTransfer = GetTransferFromReader(reader);
                    }
                }
            } 
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }

            return returnedTransfer;
        }

        public Transfer Create(Transfer transfer)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_to, account_from, amount) VALUES (@transfer_type_id, @transfer_status_id, @account_to, @account_from, @amount)", conn);
                    cmd.Parameters.AddWithValue("@transfer_type_id", transfer.TransferTypeId);
                    cmd.Parameters.AddWithValue("@transfer_status_id", transfer.TransferStatusId);
                    cmd.Parameters.AddWithValue("@account_to", transfer.AccountTo);
                    cmd.Parameters.AddWithValue("@account_from", transfer.AccountFrom);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);

                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    transfer.Id = newId;
                    return transfer;
                }
            } catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }
        public bool UpdateStatus(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE transfers SET transfer_status_id = @transfer_status_id WHERE transfer_id = @transfer_id", conn );
                    cmd.Parameters.AddWithValue("@transfer_status_id", transfer.TransferStatusId);
                    cmd.Parameters.AddWithValue("@transfer_id", transfer.Id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return (rowsAffected > 0);
                }
            }catch(SqlException)
            {
                throw;
            }
        }

        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer newTransfer = new Transfer()
            {
                Id = Convert.ToInt32(reader["transfer_id"]),
                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

            return newTransfer;
        }
    }
}
