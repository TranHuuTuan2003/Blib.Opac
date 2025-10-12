using KMS.Api.Infrastructure.DbContext.master;

namespace KMS.Api.Helpers
{
    public static class TransactionHelper
    {
        public static async Task ExecuteAsync(UnitOfWork unitOfWork, Func<Task> action, bool useTransaction = true)
        {
            if (useTransaction)
            {
                unitOfWork.Repository.BeginTransaction();

                try
                {
                    await action();
                    unitOfWork.Repository.Commit();
                }
                catch
                {
                    unitOfWork.Repository.Rollback();
                    throw;
                }
            }
            else
            {
                await action();
            }
        }

        public static async Task<T> ExecuteAsync<T>(UnitOfWork unitOfWork, Func<Task<T>> action, bool useTransaction = true)
        {
            if (useTransaction)
            {
                unitOfWork.Repository.BeginTransaction();

                try
                {
                    var result = await action();
                    unitOfWork.Repository.Commit();
                    return result;
                }
                catch
                {
                    unitOfWork.Repository.Rollback();
                    throw;
                }
            }
            else
            {
                return await action();
            }
        }
    }
}