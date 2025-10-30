using BibliotecaAPI.Controllers.V1;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Servicios.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingBiblioteca.Utilidades;
using TestingBiblioteca.Utilidades.Dobles;

namespace TestingBiblioteca.PruebasUnitarias.Controllers.V1
{
    [TestClass]
    public class AutoresControllerPruebas : BasePruebas
    {
        IAlmacenadorArchivos almacenadorArchivos = null!;
        ILogger<AutoresController> logger = null!;
        IOutputCacheStore outputCacheStore = null!;
        IServicioAutores servicioAutores = null!;
        private string nombreBD = Guid.NewGuid().ToString();
        private AutoresController controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            almacenadorArchivos = Substitute.For<IAlmacenadorArchivos>();
            logger = Substitute.For<ILogger<AutoresController>>();
            outputCacheStore = Substitute.For<IOutputCacheStore>();
            servicioAutores = Substitute.For<IServicioAutores>();

            controller = new AutoresController(context, mapper, almacenadorArchivos, logger, outputCacheStore, servicioAutores);
        }

        [TestMethod]
        public async Task Get_Retorna404_CuandoAutorConIdNoExiste()
        {
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
            var context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor { Nombres = "Uriel", Apellidos = "Quiroz", Identificacion = "ALKMC684" });
            context.Autores.Add(new Autor { Nombres = "Gercia", Apellidos = "Marquez", Identificacion = "CIOWJD484" });

            await context.SaveChangesAsync(); 

            //Prueba
            var respuesta = await controller.Get(1);

            //Verificacion
            var resultado = respuesta.Value;
            Assert.AreEqual(expected: 1, actual: resultado!.Id);
        }


        [TestMethod]
        public async Task Get_RetornaAutorConLibros_CuandoAutorTieneLibros()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);

            var libro1 = new Libro { Titulo = "Libro 1" };
            var libro2 = new Libro { Titulo = "Libro 2" };

            var autor = new Autor()
            {
                Nombres = "Uriel",
                Apellidos = "Quiroz",
                Identificacion = "KSNDCJ5298",
                Libros = new List<AutorLibro>
                {
                    new AutorLibro { Libro  = libro1 },
                    new AutorLibro { Libro  = libro2 }
                }
            };

            context.Add(autor);
            await context.SaveChangesAsync();

            //Prueba
            var respuesta = await controller.Get(1);

            //Verificacion
            var resultado = respuesta.Value;
            Assert.AreEqual(expected: 1, actual: resultado!.Id);
            Assert.AreEqual(expected: 2, actual: resultado!.Libros.Count);
        }

        [TestMethod]
        public async Task Get_DebeLlamarGetDelServicioAutores()
        {
            //Preparacion
            var paginacionDTO = new PaginacionDTO(2, 3);

            //Prueba
            await controller.Get(paginacionDTO);

            //Verificacion
            await servicioAutores.Received(1).Get(paginacionDTO);
        }

        [TestMethod]
        public async Task Post_DebeCrearAutor_CuandoEnviamosAutor()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);
            var nuevoAutor = new AutorCreateDTO { Nombres = "Mesut", Apellidos = "Ozil", Identificacion = "LKSNDC9871984" };

            //Prueba
            var respuesta = await controller.Post(nuevoAutor);

            //Verificacion
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreBD);
            var cantidad = await contexto2.Autores.CountAsync();
            Assert.AreEqual(expected: 1, actual: cantidad);
        }

        [TestMethod]
        public async Task Put_Retorna404_CuandoAutorNoExiste()
        {
            //Prueba
            var respuesta = await controller.Put(1, autorCreateDTO: null!);

            //Verificacion
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado!.StatusCode);
        }

        private const string contenedor = "autores";
        private const string cache = "autores-obtener";

        [TestMethod]
        public async Task Put_ActualizaAutor_CuandoEnviamosAutorSinFoto()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor
            {
                Nombres = "Uriel",
                Apellidos = "Quiroz",
                Identificacion = "JSDNC65465"
            });

            await context.SaveChangesAsync();

            var autorCreacionDTO = new AutorCreacionDTOConFoto
            {
                Nombres = "Uriel Alexander",
                Apellidos = "Quiroz Pineda",
                Identificacion = "JSDNC65465987"
            };

            //Prueba
            var respuesta = await controller.Put(1, autorCreacionDTO);

            //Verificacion
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado!.StatusCode);

            var context3 = ConstruirContext(nombreBD);
            var autorActualizado = await context3.Autores.SingleAsync();

            Assert.AreEqual(expected: "Uriel Alexander", actual: autorActualizado.Nombres);
            Assert.AreEqual(expected: "Quiroz Pineda", actual: autorActualizado.Apellidos);
            Assert.AreEqual(expected: "JSDNC65465987", actual: autorActualizado.Identificacion);
            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);
            await almacenadorArchivos.DidNotReceiveWithAnyArgs().Editar(default, default!, default!);
        }

        [TestMethod]
        public async Task Put_ActualizaAutor_CuandoEnviamosAutorConFoto()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);

            var urlAnterior = "URL-1";
            var urlNueva = "URL-2";
            almacenadorArchivos.Editar(default, default!, default!).ReturnsForAnyArgs(urlNueva);

            context.Autores.Add(new Autor
            {
                Nombres = "Uriel",
                Apellidos = "Quiroz",
                Identificacion = "JSDNC65465",
                Foto = urlAnterior
            });

            await context.SaveChangesAsync();

            var formFile = Substitute.For<IFormFile>();

            var autorCreacionDTO = new AutorCreacionDTOConFoto
            {
                Nombres = "Uriel Alexander",
                Apellidos = "Quiroz Pineda",
                Identificacion = "JSDNC65465987",
                Foto = formFile
            };

            //Prueba
            var respuesta = await controller.Put(1, autorCreacionDTO);

            //Verificacion
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado!.StatusCode);

            var context3 = ConstruirContext(nombreBD);
            var autorActualizado = await context3.Autores.SingleAsync();

            Assert.AreEqual(expected: "Uriel Alexander", actual: autorActualizado.Nombres);
            Assert.AreEqual(expected: "Quiroz Pineda", actual: autorActualizado.Apellidos);
            Assert.AreEqual(expected: "JSDNC65465987", actual: autorActualizado.Identificacion);
            Assert.AreEqual(expected: urlNueva, actual: autorActualizado.Foto);
            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);
            await almacenadorArchivos.Received(1).Editar(urlAnterior, contenedor, formFile);
        }
    }
}
