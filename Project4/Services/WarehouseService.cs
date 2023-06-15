using System.Data.SqlClient;
using Zadanie5.DTOs;
using Zadanie5.Enum;
using System.Data;

namespace Zadanie5.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IConfiguration _configuration;

        public WarehouseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> AddProduct(ProductDTO product)
        {
            var connectionString = _configuration.GetConnectionString("Database");
            using SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception)
            {
                return nameof(EnumMessage.ERROR);
            }
            using SqlTransaction transaction = connection.BeginTransaction();
            
            try
            {
                using SqlCommand command1 = new SqlCommand("SELECT COUNT(*) FROM PRODUCT WHERE IdProduct = @IdProduct", connection, transaction);
                command1.Parameters.AddWithValue("@IdProduct", product.IdProduct);

                using SqlCommand command2 = new SqlCommand("SELECT COUNT(*) FROM WAREHOUSE WHERE IdWarehouse = @IdWarehouse", connection, transaction);
                command2.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);

                var result1 = command1.ExecuteScalarAsync();
                var result2 = command2.ExecuteScalarAsync();
                
                await Task.WhenAll(new[] { result1, result2 });
                if ((int)(result1.Result ?? 0) == 0 || (int)(result2.Result ?? 0) == 0)
                {
                    await transaction.RollbackAsync();
                    return nameof(EnumMessage.PRODUCT_OR_WHOLESALER_NOT_FOUND);
                }
                using SqlCommand command3 = new SqlCommand("SELECT IdOrder FROM [ORDER] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt", connection, transaction);
                command3.Parameters.AddWithValue("@IdProduct", product.IdProduct);
                command3.Parameters.AddWithValue("@Amount", product.Amount);
                command3.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);
                var result3 = await command3.ExecuteScalarAsync();

                if (result3 is null)
                {
                    await transaction.RollbackAsync();
                    return nameof(EnumMessage.ORDER_NOT_FOUND);
                }

                var idOrder = (int)(result3);
                using SqlCommand command4 = new SqlCommand("SELECT FulfilledAt FROM [ORDER] WHERE IdOrder = @IdOrder", connection, transaction);
                command4.Parameters.AddWithValue("@IdOrder", idOrder);
                using SqlCommand command5 = new SqlCommand("SELECT COUNT(*) FROM PRODUCT_WAREHOUSE WHERE IdOrder = @IdOrder", connection, transaction);
                command5.Parameters.AddWithValue("@IdOrder", idOrder);

                var result4 = command4.ExecuteScalarAsync();
                var result5 = command5.ExecuteScalarAsync();
                await Task.WhenAll(new[] { result4, result5 });
                if (result4.Result != DBNull.Value || (int)(result5.Result ?? 0) != 0)
                {
                    await transaction.RollbackAsync();
                    return nameof(EnumMessage.ORDER_ALREADY_PROCESSED);
                }
                
                using SqlCommand command6 = new SqlCommand("SELECT Price FROM PRODUCT WHERE IdProduct = @IdProduct", connection, transaction);
                command6.Parameters.AddWithValue("@IdProduct", product.IdProduct);
                
                using SqlCommand command7 = new SqlCommand("UPDATE [ORDER] SET FulfilledAt = @DateNow WHERE IdOrder = @IdOrder", connection, transaction);
                command7.Parameters.AddWithValue("@DateNow", product.CreatedAt);
                command7.Parameters.AddWithValue("@IdOrder", idOrder);

                var price = await command6.ExecuteScalarAsync();
                if (price == null)
                {
                    throw new Exception();
                }
                var priceDouble = Convert.ToDouble(price);

                using SqlCommand command8 = new SqlCommand("INSERT INTO PRODUCT_WAREHOUSE(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) OUTPUT INSERTED.IdProductWarehouse " +
                                                            "VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)", connection, transaction);

                command8.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
                command8.Parameters.AddWithValue("@IdProduct", product.IdProduct);
                command8.Parameters.AddWithValue("@IdOrder", idOrder);
                command8.Parameters.AddWithValue("@Amount", product.Amount);
                command8.Parameters.AddWithValue("@Price", priceDouble * product.Amount);
                command8.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);
                

                var rowUpdated = await command7.ExecuteNonQueryAsync();
                var rowInserted = await command8.ExecuteScalarAsync();

                Console.WriteLine(rowInserted);

                if ((int)(rowInserted ?? 0) == 0 || rowUpdated == 0)
                {
                    throw new Exception();
                }
                await transaction.CommitAsync();
                return Convert.ToString(rowInserted);

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return nameof(EnumMessage.ERROR);
            }
        }

        public async Task<string> AddProductWithProcedure(ProductDTO product)
        {
            var connectionString = _configuration.GetConnectionString("Database");
            using SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception)
            {
                return nameof(EnumMessage.ERROR);
            }
            try
            {
                using SqlCommand sqlCommand1 = new SqlCommand("AddProductToWarehouse", connection);
                sqlCommand1.CommandType = CommandType.StoredProcedure;
                sqlCommand1.Parameters.AddWithValue("@IdProduct", product.IdProduct);
                sqlCommand1.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
                sqlCommand1.Parameters.AddWithValue("@Amount", product.Amount);
                sqlCommand1.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);
                await sqlCommand1.ExecuteNonQueryAsync();
                sqlCommand1.CommandType = CommandType.Text;
                sqlCommand1.CommandText = "SELECT MAX(IdProductWarehouse) from PRODUCT_WAREHOUSE WHERE " +
                    "IdWarehouse = @IdWarehouse AND IdProduct = @IdProduct AND Amount = @Amount AND Price = @Amount * (SELECT Price FROM PRODUCT WHERE IdProduct = @IdProduct)";
                var rowInserted = await sqlCommand1.ExecuteScalarAsync();
                return Convert.ToString(rowInserted);
            }
            catch(SqlException sqlEx)
            {
                return sqlEx.Message;
            }

        }
    }
}
