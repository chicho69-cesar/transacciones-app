using ClosedXML.Excel;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ManejoPresupuestos.Services {
    public class ServicioGenerarExcel : Controller, IServicioGenerarExcel {
        public FileResult GenerarExcel(string nombreArchivo, IEnumerable<Transaccion> transacciones) {
            DataTable dataTable = new DataTable("Transacciones");
            dataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("Fecha"),
                new DataColumn("Cuenta"),
                new DataColumn("Categoria"),
                new DataColumn("Nota"),
                new DataColumn("Monto"),
                new DataColumn("Ingreso / Gasto"),
            });

            foreach (var transaccion in transacciones) {
                dataTable.Rows.Add(
                    transaccion.FechaTransaccion,
                    transaccion.Cuenta,
                    transaccion.Categoria,
                    transaccion.Nota,
                    transaccion.Monto.ToString("C2"),
                    transaccion.TipoOperacionId
                );
            }

            using (XLWorkbook wb = new XLWorkbook()) {
                wb.Worksheets.Add(dataTable);

                using (MemoryStream stream = new MemoryStream()) {
                    wb.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nombreArchivo
                    );
                }
            }
        }
    }
}