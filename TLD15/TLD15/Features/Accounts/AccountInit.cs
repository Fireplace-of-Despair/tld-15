using ACherryPie.Feature;
using ACherryPie.Responses;
using ACherryPie.Security;
using Common.Composition;
using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Infrastructure;
using TLD15.Pages.Account;

namespace TLD15.Features.Accounts;

public sealed class AccountInit : IFeature
{
    public sealed class Request : IRequest<ResponseCreated<Guid>>
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
    }

    public sealed class Handler(
        IConfiguration configuration,
        IHashingService hashingService,
        IMongoDatabase database) : IRequestHandler<Request, ResponseCreated<Guid>>
    {
        public async Task<ResponseCreated<Guid>> Handle(Request request, CancellationToken cancellationToken)
        {
            var saId = configuration.GetSection(Globals.Security.Admin.IdString).Get<Guid>();
            var collection = database.GetCollection<EntityAccount>(EntityAccount.Collection);

            var document = await collection.Find(x => x.Id == saId)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                var hashResult = hashingService.Hash(request.Password);

                var account = new EntityAccount
                {
                    Id = saId,
                    Login = request.Login,
                    Password = hashResult.HexHash,
                    Salt = hashResult.HexSalt
                };

                await collection.InsertOneAsync(account, collection.GetDefaultInsert(), cancellationToken);
            }

            return new ResponseCreated<Guid> { Id = saId };
        }
    }
}
