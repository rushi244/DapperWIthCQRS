using Dapper;
using DapperWIthCQRS.Application.Command;
using DapperWIthCQRS.Domain.Models;
using DapperWIthCQRS.Infrastructure.Context;
using DapperWIthCQRS.Infrastructure.Helper.RabbitMQHelper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWIthCQRS.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly DapperContext _dapper;
        private readonly RabbitMQHelper _rabbitMQHelper;

        public CreateProductCommandHandler(DapperContext dapper,RabbitMQHelper rabbitMQHelper)
        {
            _dapper = dapper;
            _rabbitMQHelper = rabbitMQHelper;
        }
        public async Task<Product> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var query = "INSERT INTO Products(Name,Price) VALUES(@Name,@Price)"
                +"SELECT CAST(SCOPE_IDENTITY() AS int)";
            //using (var connection = _dapper.CreateConnection())
            //{
            //    return await connection.ExecuteScalarAsync<Unit>(query, command);
            //}


            var parameters = new DynamicParameters();
            parameters.Add("@Name", command.product.Name, DbType.String);
            parameters.Add("@Price", command.product.Price, DbType.Decimal);
            using (var connection = _dapper.CreateConnection())
            {
                // await connection.ExecuteAsync(query, parameters);
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdProduct = new Product
                {
                    Id = id,
                    Name = command.product.Name,
                    Price = command.product.Price
                };
              //  _rabbitMQHelper.PublishMessage($"Product Has been Created");
                return createdProduct;
            }
        }

        
    }
}
