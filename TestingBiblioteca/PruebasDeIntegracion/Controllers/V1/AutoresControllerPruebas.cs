using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using System.Net;
using System.Text.Json;
using TestingBiblioteca.Utilidades;

namespace TestingBiblioteca.PruebasDeIntegracion.Controllers.V1
{
    [TestClass]
    public class AutoresControllerPruebas : BasePruebas
    {
        private static readonly string url = "/api/v1/autores";
        private string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Get_Devuelve404_CuandoAutorNoExiste()
        {
            //Preparacion
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();

            //Prueba
            var respuesta = await cliente.GetAsync($"{url}/1");

            //Verificacion
            var statusCode = respuesta.StatusCode;
            Assert.AreEqual(expected: HttpStatusCode.NotFound, actual: respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Get_DevuelveAutor_CuandoAutorExiste()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor() { Nombres = "Uriel", Apellidos = "Quiroz", Identificacion = "123" });
            context.Autores.Add(new Autor() { Nombres = "Alexander", Apellidos = "Pienda", Identificacion = "1234" });
            await context.SaveChangesAsync();

            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();

            //Prueba
            var respuesta = await cliente.GetAsync($"{url}/1");

            //Verificacion
            respuesta.EnsureSuccessStatusCode();

            var autor = JsonSerializer.Deserialize<AutorConLibrosDTO>(
                await respuesta.Content.ReadAsStringAsync(), jsonSerializerOptions)!;

            Assert.AreEqual(expected: 1, actual: autor.Id);
        }

        [TestMethod]
        public async Task Post_Devuelve401_CuandoUsuarioNoEstaAutenticado()
        {
            //Preparacion
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);

            var cliente = factory.CreateClient();
            var autorCreacionDTO = new AutorCreateDTO
            {
                Nombres = "Uriel",
                Apellidos = "Quiroz",
                Identificacion = "123"
            };

            //Prueba
            var respuesta = await cliente.PostAsJsonAsync(url, autorCreacionDTO);

            //Verificacion
            Assert.AreEqual(expected: HttpStatusCode.Unauthorized, actual: respuesta.StatusCode);
        }
    }
}
