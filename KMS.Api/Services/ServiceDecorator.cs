using UC.Core.Interfaces;

namespace KMS.Api.Services
{
    class ServiceDecorator<TKeyId, TEntity>
    {
        private IRepositoryBase<TKeyId, TEntity> _repositoryBase;
        public ServiceDecorator(IServiceWrapper repository)
        {
            #region config repository
            if (typeof(TEntity) == typeof(Entities.uc_sample))
            {
                _repositoryBase = (IRepositoryBase<TKeyId, TEntity>)repository.uc_sample;
            }

            #endregion
            if (_repositoryBase == null)
            {
                throw new Exception("Class ServiceDecorator not configured yet");
            }
        }

        #region base service
        public async Task<List<TEntity>> GetEntitiesAsync(string? columnsQuery, string? whereQuery, string? orderQuery)
        {
            return await _repositoryBase.GetEntitiesAsync(columnsQuery, whereQuery, orderQuery, null);
        }
        public async Task<TEntity> GetEntityByIdAsync(TKeyId id)
        {
            return await _repositoryBase.GetEntityByIdAsync(id, null);
        }
        public async Task<TEntity> InsertEntityAsync(TEntity entity)
        {
            return await _repositoryBase.InsertEntityAsync(entity, null);
        }
        public async Task<TEntity> UpdateEntityAsync(TEntity entity)
        {
            return await _repositoryBase.UpdateEntityAsync(entity, null);
        }
        public async Task DeleteEntityAsync(TKeyId id)
        {
            await _repositoryBase.DeleteEntityAsync(id, null);
        }
        #endregion
    }
}
