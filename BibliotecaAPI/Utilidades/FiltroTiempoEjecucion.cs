using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace BibliotecaAPI.Utilidades
{
    public class FiltroTiempoEjecucion : IAsyncActionFilter
    {
        private readonly ILogger<FiltroTiempoEjecucion> logger;

        public FiltroTiempoEjecucion(ILogger<FiltroTiempoEjecucion> logger)
        {
            this.logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Antes de la ejecucion de la accion
            var stopWatch = Stopwatch.StartNew();
            logger.LogInformation($"INICIO Accion: {context.ActionDescriptor.DisplayName}");

            await next();

            //Despues de la ejecucion de la accion
            stopWatch.Stop();
            logger.LogInformation($"FIN Accion: {context.ActionDescriptor.DisplayName} - Tiempo: {stopWatch.ElapsedMilliseconds} ms");
        }
    }
}
