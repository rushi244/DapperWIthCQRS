using Dapper;
using DapperWIthCQRS.Application.Queries;
using DapperWIthCQRS.Domain.Models;
using DapperWIthCQRS.Infrastructure.Context;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWIthCQRS.Application.Handlers
{
    public class GetProductByIdQueriesHandler : IRequestHandler<GetProductByIdQueries,Product>
    {
        private readonly DapperContext _dapper;

        public GetProductByIdQueriesHandler(DapperContext dapper)
        {
            _dapper = dapper;
        }
        public async Task<Product> Handle(GetProductByIdQueries command, CancellationToken cancellationToken)
        {
            var query = "SELECT Id,Name, Price FROM Products WHERE Id=@Id";

            using (var connection = _dapper.CreateConnection())
            {
                var product= await connection.QuerySingleOrDefaultAsync<Product>(query, new {command.id});
                return product;
            }
        }
    }
}
