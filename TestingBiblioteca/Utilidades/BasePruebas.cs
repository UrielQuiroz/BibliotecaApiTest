using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Utilidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingBiblioteca.Utilidades
{
    public class BasePruebas
    {
        protected ApplicationDbContext ConstruirContext(string nombreBD)
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(nombreBD).Options;

            var dbContext = new ApplicationDbContext(opciones);
            return dbContext;
        }

        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(opciones =>
            {
                opciones.AddProfile(new AutoMapperProfiles());
            });

            return config.CreateMapper();
        }
    }
}
