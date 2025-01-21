using ACherryPie.Incidents;
using ACherryPie.Responses;
using ACherryPie.Security;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Infrastructure;

namespace TLD15.Pages.Account;

public sealed class AFeatureAccount
{
    public sealed class RequestLogin : IRequest<ResponseId<Guid>>
    {
        [BindProperty, MaxLength(140)]
        public string Login { get; set; } = string.Empty;

        [BindProperty, MaxLength(140), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;
    }

    public sealed class HandlerEmpty (
        IConfiguration configuration,
        IHashingService hashingService,
        IMongoClient client) : IRequestHandler<RequestLogin, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestLogin request, CancellationToken cancellationToken)
        {
            var saId = configuration.GetSection(Globals.Security.Admin.IdString).Get<Guid>();
            var database = client.GetDatabase(EntityAccount.Database);
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

            return new ResponseId<Guid> { Id = saId };
        }
    }

    public sealed class HandlerLogin (
        IHashingService hashingService,
        IMongoClient client)
        : IRequestHandler<RequestLogin, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestLogin request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityAccount.Database);
            var collection = database.GetCollection<EntityAccount>(EntityAccount.Collection);
            var document = await collection.Find(x => x.Login == request.Login)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new IncidentException(IncidentCode.LoginFailed);

            var incode = hashingService.Hash(request.Password, document.Salt);
            if (incode.HexHash != document.Password)
            {
                throw new IncidentException(IncidentCode.LoginFailed);
            }

            return new ResponseId<Guid>
            {
                Id = document.Id,
            };
        }
    }
}
