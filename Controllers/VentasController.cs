using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UspgPOS.Data;
using UspgPOS.Models;

namespace UspgPOS.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Ventas.Include(v => v.Cliente).Include(v => v.Sucursal);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Detalle_Ventas", new { VentaId = venta.Id });
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre");
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", HttpContext.Session.GetString("SucursalId"));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,Total,ClienteId,SucursalId")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("Id,Fecha,Total,ClienteId,SucursalId")] Venta venta)
        {
            if (id != venta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta != null)
            {
                _context.Ventas.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(long? id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }

        public IActionResult ImprimirFactura(long? id)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var venta = _context.Ventas
                .Include(v=>v.Cliente)
                .Include (v => v.Sucursal)
                .Include(v=>v.DetalleVentas)
                        .ThenInclude(d=>d.Producto)
                .FirstOrDefault(v=> v.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            var logo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/icons/apple-touch-icon-180x180.png");
            var monedaGuatemala = new System.Globalization.CultureInfo("es-GT");

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);

                    page.Header().Row(header =>

                    {
                        header.RelativeItem().Text($"Fecha: {venta.Fecha.ToString("yyyy/MM/dd")}");
                        header.RelativeItem().Text("Factura Comercial").FontSize(20).Bold().AlignCenter();

                        header.ConstantItem(80).Image(logo, ImageScaling.FitArea);
                    });

                    page.Content().Column(column =>
                    {

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Cliente: {venta.Cliente?.Nombre}");
                            //row.RelativeItem().Text($"NIT: {venta.Cliente?.Nit}");
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Dirección: ");
                            row.RelativeItem().Text($"Teléfono: ");
                        });
                        column.Item().PaddingVertical(1, Unit.Centimetre);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);
                                columns.RelativeColumn();
                                
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(EstiloCelda).Text("Cantidad").FontSize(12).Bold();
                                header.Cell().Element(EstiloCelda).Text("Producto").FontSize(12).Bold();
                               
                                header.Cell().Element(EstiloCelda).Text("Precio Unitario").FontSize(12).Bold();
                                header.Cell().Element(EstiloCelda).Text("Subtotal").FontSize(12).Bold();

                                static IContainer EstiloCelda(IContainer container)
                                {
                                    return container.Background("#d6eaf8").Border(1).BorderColor("#d6eaf8").Padding(5).AlignCenter();
                                }

                            });

                            foreach (DetalleVenta detalle in venta.DetalleVentas)
                            {
                                table.Cell().Border(1).BorderColor("#85c1e9").Padding(5).AlignCenter().Text(detalle.Cantidad.ToString());
                                table.Cell().Border(1).BorderColor("#85c1e9").Padding(5).Text(detalle.Producto?.Nombre ?? "N/A");
                                
                                table.Cell().Border(1).BorderColor("#85c1e9").Padding(5).AlignRight().Text(detalle.PrecioUnitario.ToString("C", monedaGuatemala));
                                table.Cell().Border(1).BorderColor("#85c1e9").Padding(5).AlignRight().Text((detalle.Cantidad * detalle.PrecioUnitario).ToString("C", monedaGuatemala));
                            }
                           // table.Cell().ColumnSpan(3).Background("#2e86c1").Border(1).BorderColor("#85c1e9").Padding(5).AlignRight()
                           //     .Text("TOTAL").FontSize(12).Bold();
                           // table.Cell().Background("#2e86c1").Border(1).BorderColor("#85c1e9").Padding(5).AlignRight()
                           //     .Text(venta.Total.ToString("C", monedaGuatemala));


                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Sucursal: {venta.Sucursal?.Nombre}");
                            row.RelativeItem().Text($"Sitio Web: @sitioincreible");
                            row.RelativeItem().Background("#aed6f1").AlignRight().Text($"Total: {venta.Total.ToString("C", monedaGuatemala)}").FontSize(14).Bold();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Correo: Hola@sitioincreible.com ");
                            row.RelativeItem().Text($"Teléfono: 1234-5678");
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"Firma Cliente").FontSize(12).Bold();
                        });
                        

                    });
                    page.Footer().Text($"Servicio A Domicilio").FontSize(28).AlignCenter();
                });
            });

            var stream = new MemoryStream();
            documento.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Factura_{id}.pdf");
        }
    }
}