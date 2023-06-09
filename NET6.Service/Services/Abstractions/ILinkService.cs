﻿using NET6.Entity.DTOs.Links;
using NET6.Entity.Entities;

namespace NET6.Service.Services.Abstractions
{
    public interface ILinkService
    {
        Task<List<LinkDto>> GetAllLinksNonDeleted();
        Task<List<LinkDto>> GetAllLinksDeleted();
        Task CreateLinkAsync(LinkAddDto linkAddDto);
        Task<Link> GetLinkByGuid(Guid id);
        Task<string> UpdateLinkAsync(LinkUpdateDto linkUpdateDto);
        Task<string> SafeDeleteLinkAsync(Guid linkId);
        Task<string> UndoDeleteLinkAsync(Guid linkId);
    }
}
