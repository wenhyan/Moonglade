﻿using Dapper;
using MediatR;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Moonglade.Configuration
{
    public class GetAssetDataQuery : IRequest<string>
    {
        public GetAssetDataQuery(Guid assetId)
        {
            AssetId = assetId;
        }

        public Guid AssetId { get; set; }
    }

    public class GetAssetDataQueryHandler : IRequestHandler<GetAssetDataQuery, string>
    {
        private readonly IDbConnection _dbConnection;

        public GetAssetDataQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> Handle(GetAssetDataQuery request, CancellationToken cancellationToken)
        {
            var asset = await _dbConnection.QueryFirstOrDefaultAsync<BlogAsset>
                ("SELECT TOP 1 * FROM BlogAsset ba WHERE ba.Id = @assetId", new { request.AssetId });

            return asset?.Base64Data;
        }
    }
}