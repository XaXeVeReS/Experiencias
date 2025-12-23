using LIBRERIA_APP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LIBRERIA_APP.Datos
{
    public class LibreriaDatos
    {
        private readonly Conexion cn = new Conexion();

        // ================== REGISTRAR VENTA / COT ==================
        public LibreriaModel RegistrarVenta(LibreriaModel venta)
        {
            var resultado = new LibreriaModel();

            try
            {
                using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
                {
                    conexion.Open();

                    // Asegurar que exista el cliente usando usp_ClienteBuscarOCrear
                    int entidadId = AsegurarEntidadCliente(conexion, venta);
                    venta.EntidadCliente = entidadId;

                    using (var cmd = new SqlCommand("dbo.usp_VentaRegistrar", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // CABECERA
                        cmd.Parameters.AddWithValue("@IdUsuario", venta.IdUsuario);
                        cmd.Parameters.AddWithValue("@EntidadIdCliente", venta.EntidadCliente);
                        cmd.Parameters.AddWithValue("@IDEmpresa", venta.IdEmpresa);
                        cmd.Parameters.AddWithValue("@Direccion", (object?)venta.Direccion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TipoMoneda", (object?)venta.TipoMoneda ?? "PEN");
                        cmd.Parameters.AddWithValue("@TipoDocumento", (object?)venta.TipoDocumento ?? "VEN");

                        if (venta.FechaValidez.HasValue)
                            cmd.Parameters.AddWithValue("@FechaValidez", venta.FechaValidez.Value);
                        else
                            cmd.Parameters.AddWithValue("@FechaValidez", DBNull.Value);

                        if (venta.IdCabeceraOrigen > 0)
                            cmd.Parameters.AddWithValue("@IdCabeceraOrigen", venta.IdCabeceraOrigen);
                        else
                            cmd.Parameters.AddWithValue("@IdCabeceraOrigen", DBNull.Value);

                        // DETALLE (TVP)
                        DataTable dtDetalle = CrearDataTableDetalle(venta.Detalle);
                        var paramDetalle = cmd.Parameters.AddWithValue("@Detalle", dtDetalle);
                        paramDetalle.SqlDbType = SqlDbType.Structured;
                        paramDetalle.TypeName = "dbo.VentaDetalleType";

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                resultado.CodMensaje = dr["CodMensaje"]?.ToString();
                                resultado.Mensaje = dr["Mensaje"]?.ToString();

                                if (dr["IDCabecera"] != DBNull.Value)
                                    resultado.IdCabecera = Convert.ToInt32(dr["IDCabecera"]);

                                if (dr["ImporteTotal"] != DBNull.Value)
                                    resultado.ImporteTotal = Convert.ToDecimal(dr["ImporteTotal"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.CodMensaje = "0";
                resultado.Mensaje = "Error en la aplicación: " + ex.Message;
            }

            return resultado;
        }

        public LibreriaModel RegistrarCotizacion(LibreriaModel cot)
        {
            // Es una COT
            cot.TipoDocumento = "COT";
            cot.TipoMoneda ??= "PEN";

            // Si no viene fecha, la calculamos con DiasValidez; si no, 30 días
            if (!cot.FechaValidez.HasValue)
            {
                int dias = cot.DiasValidez > 0 ? cot.DiasValidez : 30;
                cot.FechaValidez = DateTime.UtcNow.Date.AddDays(dias);
            }

            // Reutilizamos la lógica de RegistrarVenta (mismo SP)
            return RegistrarVenta(cot);
        }

        // ================== OBTENER COTIZACIÓN (CAB + DET) ==================
        public LibreriaModel? ObtenerCotizacion(int idCabecera)
        {
            LibreriaModel? result = null;

            try
            {
                using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
                {
                    conexion.Open();

                    using (var cmd = new SqlCommand("dbo.usp_Cotizacion_Obtener", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IDCabecera", idCabecera);

                        using (var dr = cmd.ExecuteReader())
                        {
                            // CABECERA
                            if (dr.Read())
                            {
                                result = new LibreriaModel
                                {
                                    IdCabecera = Convert.ToInt32(dr["IDCabecera"]),
                                    EntidadCliente = Convert.ToInt32(dr["EntidadIdCliente"]),
                                    IdEmpresa = Convert.ToInt32(dr["IDEmpresa"]),
                                    TipoMoneda = dr["TipoMoneda"]?.ToString(),
                                    ImporteTotal = Convert.ToDecimal(dr["ImporteTotal"]),

                                    IdTipoDocumentoCliente = Convert.ToInt32(dr["IDTipoDocumento"]),
                                    NroDocumentoCliente = dr["NroDocumento"]?.ToString(),
                                    NombreCliente = dr["NombreRazon"]?.ToString(),

                                    // El SP devuelve sólo una Dirección (de la entidad)
                                    DireccionCliente = dr["Direccion"] as string,
                                    Direccion = dr["Direccion"] as string,
                                    EmailCliente = dr["Email"] as string,
                                    TelefonoCliente = dr["Telefono"] as string,

                                    Detalle = new List<DetalleVentaModel>()
                                };
                            }

                            if (result == null)
                                return null;

                            // DETALLE
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    var det = new DetalleVentaModel
                                    {
                                        Item = Convert.ToInt32(dr["Item"]),
                                        SKU = dr["SKU"]?.ToString() ?? "",
                                        Descripcion = dr["ProductoNombre"]?.ToString() ?? "",
                                        Cantidad = Convert.ToDecimal(dr["Cantidad"]),
                                        PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                                        Descuento = Convert.ToDecimal(dr["Descuento"])
                                    };

                                    result.Detalle.Add(det);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return result;
        }

        // ================== BUSCAR CLIENTE POR DOCUMENTO ==================
        public ClienteModel? ClienteBuscarPorDocumento(string nroDocumento)
        {
            if (string.IsNullOrWhiteSpace(nroDocumento))
                return null;

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();

                using (var cmd = new SqlCommand("dbo.usp_ClienteObtenerPorDocumento", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NroDocumento", nroDocumento.Trim());

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new ClienteModel
                            {
                                EntidadId = Convert.ToInt32(dr["EntidadId"]),
                                IdTipoDocumento = Convert.ToInt32(dr["IDTipoDocumento"]),
                                NroDocumento = dr["NroDocumento"]?.ToString() ?? "",
                                NombreRazon = dr["NombreRazon"]?.ToString() ?? "",
                                Direccion = dr["Direccion"] as string,
                                Email = dr["Email"] as string,
                                Telefono = dr["Telefono"] as string
                            };
                        }
                    }
                }
            }

            return null;
        }

        // ================== HELPERS ==================

        private int AsegurarEntidadCliente(SqlConnection conexion, LibreriaModel doc)
        {
            // Si ya viene un Id de Entidad, lo usamos directo
            if (doc.EntidadCliente > 0)
                return doc.EntidadCliente;

            int idTipoDoc = doc.IdTipoDocumentoCliente > 0 ? doc.IdTipoDocumentoCliente : 1; // 1 = DNI por defecto

            using (var cmd = new SqlCommand("dbo.usp_ClienteBuscarOCrear", conexion))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IDTipoDocumento", idTipoDoc);
                cmd.Parameters.AddWithValue("@NroDocumento", (object?)doc.NroDocumentoCliente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NombreRazon", (object?)doc.NombreCliente ?? "CLIENTE SIN NOMBRE");
                cmd.Parameters.AddWithValue("@Direccion", (object?)doc.DireccionCliente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ubigeo", DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)doc.EmailCliente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object?)doc.TelefonoCliente ?? DBNull.Value);

                var pOut = cmd.Parameters.Add("@EntidadId", SqlDbType.Int);
                pOut.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                int entidadId = (pOut.Value != DBNull.Value) ? Convert.ToInt32(pOut.Value) : 0;
                return entidadId;
            }
        }

        private DataTable CrearDataTableDetalle(List<DetalleVentaModel> detalle)
        {
            var table = new DataTable();
            table.Columns.Add("Item", typeof(int));
            table.Columns.Add("SKU", typeof(string));
            table.Columns.Add("Cantidad", typeof(decimal));
            table.Columns.Add("PrecioUnitario", typeof(decimal));
            table.Columns.Add("Descuento", typeof(decimal));

            if (detalle == null)
                return table;

            foreach (var d in detalle)
            {
                var row = table.NewRow();
                row["Item"] = d.Item;
                row["SKU"] = d.SKU;
                row["Cantidad"] = d.Cantidad;
                row["PrecioUnitario"] = d.PrecioUnitario;
                row["Descuento"] = d.Descuento;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
