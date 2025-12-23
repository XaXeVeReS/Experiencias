using LIBRERIA_APP.Datos;
using LIBRERIA_APP.Models;
using PROYECTO_LIBRERIA.Models;
using System.Data;
using System.Data.SqlClient;


namespace PROYECTO_LIBRERIA.Datos
{
    public class ProductoDatos
    {
        Conexion cn = new Conexion();

        // LISTA
        public List<ProductoModel> ProductoListaDatos()
        {
            List<ProductoModel> lista = new List<ProductoModel>();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProductoLista", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var prod = new ProductoModel
                        {
                            sku = rd["SKU"].ToString(),
                            nombre = rd["ProductoNombre"].ToString(),
                            descripcion = rd["ProductoDescripcion"].ToString(),

                            precio = rd["PrecioUnid"] == DBNull.Value? (decimal?)null: Convert.ToDecimal(rd["PrecioUnid"]),
                            stockMin = rd["StockMin"] == DBNull.Value? (decimal?)null: Convert.ToDecimal(rd["StockMin"]),
                            idCategoria = rd["IDCategoria"] == DBNull.Value? (int?)null: Convert.ToInt32(rd["IDCategoria"]),
                            nombreCategoria = rd["NombreCategoria"].ToString(),
                            entidadIdProveedor = rd["EntidadIdProveedor"] == DBNull.Value? (int?)null: Convert.ToInt32(rd["EntidadIdProveedor"]),
                            nombreProveedor = rd["NombreProveedor"].ToString(),
                            estado = rd["Estado"].ToString()
                        };

                        lista.Add(prod);
                    }
                }
            }

            return lista;
        }

        // OBTENER
        public ProductoModel ProductoObtenerDatos(string sku)
        {
            ProductoModel prod = new ProductoModel();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProductoObtener", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SKU", sku);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        prod.sku = rd["SKU"].ToString();
                        prod.nombre = rd["ProductoNombre"].ToString();
                        prod.descripcion = rd["ProductoDescripcion"].ToString();

                        prod.precio = rd["PrecioUnid"] == DBNull.Value
                                            ? (decimal?)null
                                            : Convert.ToDecimal(rd["PrecioUnid"]);

                        prod.stockMin = rd["StockMin"] == DBNull.Value
                                            ? (decimal?)null
                                            : Convert.ToDecimal(rd["StockMin"]);

                        prod.idCategoria = rd["IDCategoria"] == DBNull.Value
                                            ? (int?)null
                                            : Convert.ToInt32(rd["IDCategoria"]);

                        prod.nombreCategoria = rd["NombreCategoria"].ToString();

                        prod.entidadIdProveedor = rd["EntidadIdProveedor"] == DBNull.Value
                                                    ? (int?)null
                                                    : Convert.ToInt32(rd["EntidadIdProveedor"]);

                        prod.nombreProveedor = rd["NombreProveedor"].ToString();
                        prod.estado = rd["Estado"].ToString();
                    }
                }
            }

            return prod;
        }

        // CREAR
        public ProductoModel ProductoCrearDatos(ProductoModel prod)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProductoCrear", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@SKU", prod.sku ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductoNombre", prod.nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductoDescripcion", prod.descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioUnid", (object?)prod.precio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StockMin", (object?)prod.stockMin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IDCategoria", (object?)prod.idCategoria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EntidadIdProveedor", (object?)prod.entidadIdProveedor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", prod.estado ?? (object)DBNull.Value);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        prod.codMensaje = rd["CodMensaje"].ToString();
                        prod.mensaje = rd["Mensaje"].ToString();
                    }
                }
            }

            return prod;
        }

        // ACTUALIZAR
        public ProductoModel ProductoActualizarDatos(ProductoModel prod)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProductoActualizar", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@SKU", prod.sku ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductoNombre", prod.nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductoDescripcion", prod.descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioUnid", (object?)prod.precio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StockMin", (object?)prod.stockMin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IDCategoria", (object?)prod.idCategoria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EntidadIdProveedor", (object?)prod.entidadIdProveedor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", prod.estado ?? (object)DBNull.Value);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        prod.codMensaje = rd["CodMensaje"].ToString();
                        prod.mensaje = rd["Mensaje"].ToString();
                    }
                }
            }

            return prod;
        }
    }
}
