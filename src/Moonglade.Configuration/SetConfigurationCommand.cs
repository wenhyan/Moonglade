﻿using MediatR;
using Moonglade.Data;
using Moonglade.Data.Entities;
using Moonglade.Data.Infrastructure;

namespace Moonglade.Configuration;

public record SetConfigurationCommand(string Name, string Json) : IRequest<OperationCode>;

public class SetConfigurationCommandHandler : IRequestHandler<SetConfigurationCommand, OperationCode>
{
    private readonly IRepository<BlogConfigurationEntity> _repository;

    public SetConfigurationCommandHandler(IRepository<BlogConfigurationEntity> repository)
    {
        _repository = repository;
    }

    public async Task<OperationCode> Handle(SetConfigurationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(p => p.CfgKey == request.Name);
        if (entity == null) return OperationCode.ObjectNotFound;

        entity.CfgValue = request.Json;
        entity.LastModifiedTimeUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        return OperationCode.Done;
    }
}