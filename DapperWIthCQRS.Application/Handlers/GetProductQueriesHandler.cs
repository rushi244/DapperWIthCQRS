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
    public class GetProductQueriesHandler : IRequestHandler<GetProductQueries, IEnumerable<Product>>
    {
        private readonly DapperContext _dapper;

        public GetProductQueriesHandler(DapperContext dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Product>> Handle(GetProductQueries request, CancellationToken cancellationToken)
        {
            var query = "SELECT Id,Name,Price FROM Products";

            using (var connection = _dapper.CreateConnection())
            {
                var productList = await connection.QueryAsync<Product>(query);
                return productList.ToList();
            }
        }
    }
}
