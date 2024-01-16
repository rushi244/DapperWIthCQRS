using Dapper;
using DapperWIthCQRS.Application.Command;
using DapperWIthCQRS.Domain.Models;
using DapperWIthCQRS.Infrastructure.Context;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWIthCQRS.Application.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly DapperContext _dapper;

        public UpdateProductCommandHandler(DapperContext dapper)
        {
            _dapper = dapper;
        }
        

        public async Task Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var query = "UPDATE Products SET Name=@Name, Price=@Price WHERE Id=@Id";

            var paramters = new DynamicParameters();
            paramters.Add("@Id", command.id, DbType.Int32);
            paramters.Add("@Name", command.product.Name, DbType.String);
            paramters.Add("@Price", command.product.Price, DbType.Decimal);
            using (var connection = _dapper.CreateConnection())
            {
                await connection.ExecuteAsync(query, paramters);
            }
        }
    }
}
