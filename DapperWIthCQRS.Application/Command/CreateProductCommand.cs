using DapperWIthCQRS.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWIthCQRS.Application.Command
{ 
       public record CreateProductCommand(Product product) : IRequest<Product>;  
}
