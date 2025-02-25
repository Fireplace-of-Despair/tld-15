using ACherryPie.Incidents;
using ACherryPie.Responses;
using ACherryPie.Security;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Infrastructure;

namespace TLD15.Pages.Account;

public sealed class AFeatureAccount
{
    public static class CacheKey
    {
        public static string LoginAttempt => "account-failed-{0}";
        public static int LoginAttemptMax => 2;
    }



    public sealed class RequestEmptyLogin : IRequest<ResponseId<Guid>>
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
        IMongoClient client) : IRequestHandler<RequestEmptyLogin, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestEmptyLogin request, CancellationToken cancellationToken)
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

    public sealed class HandlerLogin (
        IHashingService hashingService,
        IMongoClient client,
        IMemoryCache memoryCache)
        : IRequestHandler<RequestLogin, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestLogin request, CancellationToken cancellationToken)
        {
            await Task.Delay(5_000, cancellationToken);

            var cacheKey = string.Format(CacheKey.LoginAttempt, request.Login);
            if (!memoryCache.TryGetValue(cacheKey, out int loginFailedAttempts))
            {
                memoryCache.Set(cacheKey, 1, TimeSpan.FromMinutes(30));
            }

            if (loginFailedAttempts >= CacheKey.LoginAttemptMax)
            {
                throw new IncidentException(IncidentCode.LoginFailed);
            }

            var database = client.GetDatabase(EntityAccount.Database);
            var collection = database.GetCollection<EntityAccount>(EntityAccount.Collection);
            var document = await collection.Find(x => x.Login == request.Login)
                .FirstOrDefaultAsync(cancellationToken);
            if (document == null)
            {
                memoryCache.Set(string.Format(CacheKey.LoginAttempt, request.Login), loginFailedAttempts + 1);
                throw new IncidentException(IncidentCode.LoginFailed);
            }

            var incode = hashingService.Hash(request.Password, document.Salt);
            if (incode.HexHash != document.Password)
            {
                memoryCache.Set(string.Format(CacheKey.LoginAttempt, request.Login), loginFailedAttempts + 1);
                throw new IncidentException(IncidentCode.LoginFailed);
            }

            return new ResponseId<Guid>
            {
                Id = document.Id,
            };
        }
    }


    public sealed class RequestEdit : IRequest<ResponseId<Guid>>
    {
        public required Guid? Id { get; set; }
        public required string Login { get; set; }
        public string Password { get; set; } = string.Empty;

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required long Version { get; set; }
    }

    public sealed class RequestView : IRequest<List<RequestEdit>>
    {
        public required Guid? Id { get; set; }
    }

    public sealed class HandlerView(IMongoClient client) : IRequestHandler<RequestView, List<RequestEdit>>
    {
        public async Task<List<RequestEdit>> Handle(RequestView request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityAccount.Database);
            var collection = database.GetCollection<EntityAccount>(EntityAccount.Collection);

            var filter = request.Id.HasValue ?
                Builders<EntityAccount>.Filter.Eq(x => x.Id, request.Id.Value)
                : FilterDefinition<EntityAccount>.Empty;

            var documents = await collection.FindAsync(filter, cancellationToken: cancellationToken);
            var document = await documents.ToListAsync(cancellationToken);

            return document.ConvertAll(x => new RequestEdit
            {
                Id = x.Id,
                Password = string.Empty,
                Login = x.Login,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Version = x.Version,
            });
        }
    }

    public sealed class HandlerSave(
        IMongoClient client,
        IHashingService hashingService) : IRequestHandler<RequestEdit, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestEdit request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityAccount.Database);
            var collection = database.GetCollection<EntityAccount>(EntityAccount.Collection);

            
            var item = new EntityAccount { Id = Guid.NewGuid(), Login = request.Login };
            if (request.Id.HasValue)
            {
                item = await collection.Find(x => x.Id == request.Id.Value)
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new IncidentException(IncidentCode.NotFound);
            }
            else
            {
                if (string.IsNullOrEmpty(item.Password))
                {
                    throw new IncidentException(IncidentCode.Invalid);
                }
                await collection.InsertOneAsync(item, collection.GetDefaultInsert(), cancellationToken);
            }

            item.Login = request.Login;
            if (!string.IsNullOrEmpty(request.Password))
            {
                var hashResult = hashingService.Hash(request.Password);
                item.Password = hashResult.HexHash;
                item.Salt = hashResult.HexSalt;
            }
            
            item.Bump(request.Version);

            await collection.ReplaceOneAsync(x => x.Id == item.Id, item, cancellationToken: cancellationToken);

            return new ResponseId<Guid> { Id = item.Id };
        }
    }



    public sealed class RequestDelete : IRequest<ResponseId<Guid>>
    {
        public required Guid Id { get; set; }
    }

    public sealed class HandlerDelete(IMongoClient client) : IRequestHandler<RequestDelete, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestDelete request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityAccount.Database);
            var collection = database.GetCollection<EntityAccount>(EntityAccount.Collection);

            var result = await collection.DeleteOneAsync(Builders<EntityAccount>.Filter.Eq(x => x.Id, request.Id), new DeleteOptions(), cancellationToken);
            if (result.DeletedCount <= 0)
            {
                throw new IncidentException(IncidentCode.NotFound);
            }

            return new ResponseId<Guid> { Id = request.Id };
        }
    }
}
