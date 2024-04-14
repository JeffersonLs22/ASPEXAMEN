using Examen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Examen.Controllers
{
    public class ClientesController : Controller
    {

        string cadenaConexion =
          "Server=PC-1001\\SQLEXPRESS;" +
          "Database=EXAMEN;" +
          "User Id=root;" +
          "Password=9343;";



        public IEnumerable<Clientes> ListadoTotal(string indicador)
        {
            List<Clientes> lista = new List<Clientes>();
            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_cliente_crud", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@indicador", indicador);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Clientes cliente = new Clientes
                        {
                            Codigo = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            ApellidoPaterno = reader.GetString(2),
                            ApellidoMaterno = reader.GetString(3),
                            TipoDocumento = reader.GetInt32(4),
                            FechaRegistro = reader.GetDateTime(5)
                        };
                        lista.Add(cliente);
                    }
                }
                catch (Exception ex)
                {
                    // Manejar excepción
                    Console.WriteLine(ex.Message);
                }
            }

            return lista;
        }



        public IEnumerable<Clientes> ListadoTotal(string indicador, int fecha)
        {
            List<Clientes> lista = new List<Clientes>();
            SqlConnection con = new SqlConnection(cadenaConexion);
            SqlCommand cmd;

            try
            {
                con.Open();
                cmd = new SqlCommand("BuscarClientesPorFechaRegistro", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Indicador", indicador);
                cmd.Parameters.AddWithValue("@Codigo", fecha);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Clientes cliente = new Clientes
                    {
                        Codigo = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        ApellidoPaterno = reader.GetString(2),
                        ApellidoMaterno = reader.GetString(3),
                        TipoDocumento = reader.GetInt32(4),
                        FechaRegistro = reader.GetDateTime(5)
                    };
                    lista.Add(cliente);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return lista;
        }


        public IEnumerable<Documentos> ObtenerListadoDocumentos()
        {
            List<Documentos> lista = new List<Documentos>();
            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_obtener_documentos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Documentos documento = new Documentos
                    {
                        Codigo = reader.GetInt32(0),
                        Descripcion = reader.GetString(1)
                    };
                    lista.Add(documento);
                }
            }
            return lista;
        }

        public IEnumerable<Clientes> ObtenerListadoClientes()
        {
            List<Clientes> lista = new List<Clientes>();
            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM tb_clientes", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Clientes cliente = new Clientes
                    {
                        Codigo = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        ApellidoPaterno = reader.GetString(2),
                        ApellidoMaterno = reader.GetString(3),
                        TipoDocumento = reader.GetInt32(4),
                        FechaRegistro = reader.GetDateTime(5)
                    };
                    lista.Add(cliente);
                }
            }
            return lista;
        }

        public int InsertarCliente(Clientes cliente)
        {
            int resultado = 0;
            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_insertar_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@apellidoPaterno", cliente.ApellidoPaterno);
                cmd.Parameters.AddWithValue("@apellidoMaterno", cliente.ApellidoMaterno);
                cmd.Parameters.AddWithValue("@tipoDocumento", cliente.TipoDocumento);
                cmd.Parameters.AddWithValue("@fechaRegistro", cliente.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                con.Open();
                resultado = cmd.ExecuteNonQuery();
            }
            return resultado;
        }

        public IActionResult Create(Clientes cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int resultado = InsertarCliente(cliente);

                    if (resultado > 0)
                    {
                        ViewBag.Mensaje = "Cliente agregado correctamente.";
                        // Lógica para mostrar mensajes de validación
                        Debug.WriteLine("Nombre : " + cliente.Nombre);
                        Debug.WriteLine("Apellidos : " + cliente.ApellidoPaterno + " " + cliente.ApellidoMaterno);

                        // Actualizar la lista de documentos
                        ViewBag.Documentos = ObtenerListadoDocumentos();

                        return View(cliente); // Retornar la vista con el modelo del cliente
                    }
                    else
                    {
                        ViewBag.Mensaje = "Hubo un error al ingresar el cliente.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error al agregar el cliente: " + ex.Message;
            }

            // Recargar la lista de documentos para el DropDownList
            ViewBag.Documentos = ObtenerListadoDocumentos();
            return View(cliente);
        }


        public IActionResult Index()
        {
                return View(ObtenerListadoClientes());
        }

        public IActionResult BusquedaPorFecha(int fecha = 2020)
        {
            return View(ListadoTotal("busquedaPorFecha", fecha));
        }

        public IActionResult listadoPaginacion(int fecha, int paginaActual = 0)
        {
            int contadorTotal = ListadoTotal("ListarTodos").Count();
            int registrosPorHoja = 3;

            int numPaginas = contadorTotal % registrosPorHoja == 0
                            ? contadorTotal / registrosPorHoja
                            : (contadorTotal / registrosPorHoja) + 1;

            ViewBag.paginaActual = paginaActual;
            ViewBag.numPaginas = numPaginas;
            ViewBag.fecha = 2020;

            return View(
                ListadoTotal("ListarTodos").Skip(paginaActual * registrosPorHoja).Take(registrosPorHoja)
                );
        }

        public IActionResult Home()
        {
            return View();
        }

    }
}

