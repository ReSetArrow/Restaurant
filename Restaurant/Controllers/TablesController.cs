using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Filters;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [ServiceFilter(typeof(LoginStatusFilter))]
    public class TablesController : Controller
    {
        private readonly RestaurantContext _context;

        public TablesController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: Tables
        public async Task<IActionResult> Index()
        {
            return View(await _context.Table.ToListAsync());
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TableID,Limit,Pricing,Remark")] Table table, IFormFile? Picture)
        {
            // 檢查 Picture 是否存在
            if (Picture == null)
            {
                ModelState.AddModelError("Picture", "請上傳圖片");
                return View(table);
            }

            // 檢查圖片類型
            if (Picture.ContentType != null && Picture.ContentType != "image/jpeg")
            {
                ViewData["Message"] = "請上傳jpg格式的檔案!!";
                return View(table);
            }

            // 儲存圖片邏輯 (假設儲存到 wwwroot/images 資料夾)
            string fileName = table.TableID + ".jpg";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Picture.CopyTo(stream);
            }
            table.ImageType = Picture.ContentType;
            table.Picture = fileName;
            if (ModelState.IsValid)
            {

                // 檢查主鍵是否重複
                if (await _context.Table.FindAsync(table.TableID) != null)
                {
                    ModelState.AddModelError("TableID", "廳位已存在");
                    return View(table);
                }
               

                _context.Add(table);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"新增成功：{table.TableID} - {table.Limit} - {table.Pricing}";
                return RedirectToAction(nameof(Index));
            }
           

            return View(table);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .FirstOrDefaultAsync(m => m.TableID == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var table = await _context.Table.FindAsync(id);
            if (table != null)
            {
                //////////////////////GPT////////////////////////////
                // 檢查是否有依賴資料
                bool hasOrderDependencies = await _context.OrderDetails.AnyAsync(od => od.TableID == table.TableID);
                if (hasOrderDependencies)
                {
                    // 有依賴資料，給予提示並阻止刪除操作
                    TempData["SuccessMessage"] = $"◢▆▅▄▃崩╰(〒皿〒)╯潰▃▄▅▇◣無法刪除：{table.TableID}，因為有依賴的訂單資料。";
                    return RedirectToAction(nameof(Index));
                }
                //////////////////////GPT////////////////////////////
                _context.Table.Remove(table);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"刪除成功：{table.TableID} - {table.Limit} - {table.Remark} - {table.Pricing}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TableExists(string id)
        {
            return _context.Table.Any(e => e.TableID == id);
        }
    }
}
