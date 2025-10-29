using BibliotecaAPI.Controllers.V1;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Servicios.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingBiblioteca.Utilidades;

namespace TestingBiblioteca.PruebasUnitarias.Controllers.V1
{
    [TestClass]
    public class AutoresControllerPruebas : BasePruebas
    {
        [TestMethod]
        public async Task Get_Retorna404_CuandoAutorConIdNoExiste()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            IAlmacenadorArchivos almacenadorArchivos = null!;
            ILogger<AutoresController> logger = null!;
            IOutputCacheStore outputCacheStore = null!;
            IServicioAutores servicioAutores = null!;

            var controller = new AutoresController(context, mapper, almacenadorArchivos, logger, outputCacheStore, servicioAutores);

            //Prueba
            var respuesta = await controller.Get(1);

            //Verificacion
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(expected: 404, actual: resultado!.StatusCode);
        }


        [TestMethod]
        public async Task Get_RetornaAutor_CuandoAutorConIdExiste() 
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            IAlmacenadorArchivos almacenadorArchivos = null!;
            ILogger<AutoresController> logger = null!;
            IOutputCacheStore outputCacheStore = null!;
            IServicioAutores servicioAutores = null!;

            context.Autores.Add(new Autor { Nombres = "Uriel", Apellidos = "Quiroz", Identificacion = "ALKMC684" });
            context.Autores.Add(new Autor { Nombres = "Gercia", Apellidos = "Marquez", Identificacion = "CIOWJD484" });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);

            var controller = new AutoresController(context2, mapper, almacenadorArchivos, logger, outputCacheStore, servicioAutores);

            //Prueba
            var respuesta = await controller.Get(1);

            //Verificacion
            var resultado = respuesta.Value;
            Assert.AreEqual(expected: 1, actual: resultado!.Id);
        }
    }
}
