using LIBRERIA_APP.Datos;
using PROYECTO_LIBRERIA.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PROYECTO_LIBRERIA.Datos
{
    public class InventarioDatos
    {
        Conexion cn = new Conexion();

        private DataTable CrearDataTableEntrada(List<InventarioEntradaItemModel> detalle)
        {
            var dt = new DataTable();
            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Cantidad", typeof(decimal));
            dt.Columns.Add("CostoUnit", typeof(decimal));

            foreach (var item in detalle)
            {
                var row = dt.NewRow();
                row["SKU"] = item.sku ?? "";
                row["Cantidad"] = item.cantidad;
                row["CostoUnit"] = item.costoUnit.HasValue ? item.costoUnit.Value : (object)DBNull.Value;
                dt.Rows.Add(row);
            }

            return dt;
        }

        public InventarioResponseModel InventarioEntradaGrupoDatos(int idUsuario, string origen, List<InventarioEntradaItemModel> detalle)
        {
            var result = new InventarioResponseModel();

            try
            {
                using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("usp_Inventario_EntradaGrupo", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@Origen", origen ?? "AJUSTE");
                    cmd.Parameters.AddWithValue("@RefTabla", DBNull.Value);
                    cmd.Parameters.AddWithValue("@RefId", DBNull.Value);

                    var dt = CrearDataTableEntrada(detalle);
                    var pDetalle = cmd.Parameters.AddWithValue("@Detalle", dt);
                    pDetalle.SqlDbType = SqlDbType.Structured;
                    pDetalle.TypeName = "dbo.TVP_InventarioEntrada";

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            result.codMensaje = rd["CodMensaje"]?.ToString();
                            result.mensaje = rd["Mensaje"]?.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.codMensaje = "0";
                result.mensaje = "Error: " + ex.Message;
            }

            return result;
        }

        public decimal StockDisponibleDatos(string sku)
        {
            decimal stock = 0;

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_Inventario_StockDisponible", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SKU", sku);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        stock = rd["Stock"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["Stock"]);
                    }
                }
            }

            return stock;
        }

        public List<KardexMovimientoModel> KardexPorSkuDatos(int idUsuario, string sku)
        {
            var lista = new List<KardexMovimientoModel>();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_Inventario_KardexPorSKU", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@SKU", sku);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new KardexMovimientoModel
                        {
                            idMovimiento = Convert.ToInt32(rd["IdMovimiento"]),
                            fecha = Convert.ToDateTime(rd["Fecha"]),
                            sku = rd["SKU"].ToString(),
                            productoNombre = rd["ProductoNombre"].ToString(),
                            tipo = rd["Tipo"].ToString(),
                            cantidad = Convert.ToDecimal(rd["Cantidad"]),
                            costoUnit = rd["CostoUnit"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(rd["CostoUnit"]),
                            origen = rd["Origen"] == DBNull.Value ? null : rd["Origen"].ToString(),
                            refTabla = rd["RefTabla"] == DBNull.Value ? null : rd["RefTabla"].ToString(),
                            refId = rd["RefId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["RefId"]),
                            saldo = Convert.ToDecimal(rd["Saldo"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
