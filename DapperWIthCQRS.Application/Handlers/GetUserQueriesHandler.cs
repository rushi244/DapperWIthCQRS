using Dapper;
using DapperWIthCQRS.Application.Queries;
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
    public class GetUserQueriesHandler : IRequestHandler<GetUserQueries,Users>
    {
        private readonly DapperContext _dapper;

        public GetUserQueriesHandler(DapperContext dapper)
        {
            _dapper = dapper;
        }
        public async Task<Users> Handle(GetUserQueries request, CancellationToken cancellationToken)
        {
            var query = "SELECT UserName, Password FROM Users WHERE UserName=@UserName and Password=@Password";

            var parameters = new DynamicParameters();
            parameters.Add("@UserName",request.UserName,DbType.String);
            parameters.Add("@Password",request.Password,DbType.String);

            using (var connection=_dapper.CreateConnection())
            {
                var userDetails = await connection.QuerySingleOrDefaultAsync<Users>(query,parameters);
                return userDetails;
            }

        }
    }
}
