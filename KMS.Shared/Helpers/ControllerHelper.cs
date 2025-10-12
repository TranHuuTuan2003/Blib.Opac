using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KMS.Shared.Exceptions;
using UC.Core.Models;
using KMS.Shared.Constants;

namespace KMS.Shared.Helpers
{
    public static class ControllerHelper
    {
        public static async Task<IActionResult> ExecuteWithHandlingAsync(
            ILogger logger,
            Func<Task<IActionResult>> action,
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            logger.LogInformation($"Start {methodName}");
            try
            {
                return await action();
            }
            catch (BusinessException ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{methodName} error: {ex.Message}");
                return ResponseMessage.Error(null, NotificationMessages.UnexpectedError);
            }
        }

        public static IActionResult ExecuteWithHandling(
            ILogger logger,
            Func<IActionResult> action,
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            logger.LogInformation($"Start {methodName}");
            try
            {
                return action();
            }
            catch (BusinessException ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{methodName} error: {ex.Message}");
                return ResponseMessage.Error(null, NotificationMessages.UnexpectedError);
            }
        }
    }
}
