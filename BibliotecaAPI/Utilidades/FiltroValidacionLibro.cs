using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Utilidades
{
    public class FiltroValidacionLibro : IAsyncActionFilter
    {
        private readonly ApplicationDbContext dbContext;

        public FiltroValidacionLibro(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.ActionArguments.TryGetValue("libroCreateDTO", out var value) || value is not LibroCreateDTO libroCreateDTO)
            {
                context.ModelState.AddModelError(string.Empty, "El modelo enviado no es válido");
                context.Result = context.ModelState.ContruirProblemDetail();
                return;
            }
            if (libroCreateDTO.AutoresIds is null || libroCreateDTO.AutoresIds.Count == 0)
            {
                context.ModelState.AddModelError(nameof(libroCreateDTO.AutoresIds), "No se puede crear un libro sin autores");
                context.Result = context.ModelState.ContruirProblemDetail();
                return;
            }

            var autoresIdsExisten = await dbContext.Autores
                                        .Where(x => libroCreateDTO.AutoresIds.Contains(x.Id))
                                        .Select(x => x.Id).ToListAsync();

            if (autoresIdsExisten.Count != libroCreateDTO.AutoresIds.Count)
            {
                var autoresNoExisten = libroCreateDTO.AutoresIds.Except(autoresIdsExisten);
                var autoresNoExistenString = string.Join(",", autoresIdsExisten);
                var msjError = $"Los siguientes autores no existen: {autoresNoExistenString}";
                context.ModelState.AddModelError(nameof(LibroCreateDTO.AutoresIds), msjError);
                context.Result = context.ModelState.ContruirProblemDetail();
                return;
            }
            await next();
        }
    }
}
