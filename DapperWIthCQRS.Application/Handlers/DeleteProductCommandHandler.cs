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
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly DapperContext _dapper;

        public DeleteProductCommandHandler(DapperContext dapper)
        {
            _dapper = dapper;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var query = "DELETE FROM Products WHERE Id=@Id";

            var parameters = new DynamicParameters();
           // parameters.Add("@Id", request.id, DbType.Int32);

            using (var connection = _dapper.CreateConnection())
            {
                await connection.ExecuteAsync(query,new {request.id}); 
            }
        }
    }
}
