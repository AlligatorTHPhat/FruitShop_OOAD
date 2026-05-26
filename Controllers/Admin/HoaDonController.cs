using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FruitShopG4P.Data;

namespace VanPhongPhamOnline.Controllers.Admin
{
    public class HoaDonController : Controller
    {
        private readonly FruitShopContext _context;

        public HoaDonController(FruitShopContext context)
        {
            _context = context;
        }

        // GET: HoaDon
        public async Task<IActionResult> Index()
        {
            var maNV = HttpContext.Session.GetString("MaNV"); 
            ViewBag.IsAdmin = string.Equals(maNV, "AD001", StringComparison.OrdinalIgnoreCase);
            var tongTienDict = _context.HoaDons
           .Select(h => new
           {
               h.MaHd,
               TongTien = h.ChiTietHds.Sum(ct => (decimal)ct.SoLuong * (decimal)ct.DonGia) + 10000
           })
           .ToDictionary(x => x.MaHd, x => x.TongTien);

            ViewBag.TongTienDict = tongTienDict;


            var multiShopContext = _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaTrangThaiNavigation);

            return View("~/Views/Admin/QuanLyHoaDon/Index.cshtml", multiShopContext);
        }


        // GET: HoaDon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/QuanLyHoaDon/Details.cshtml",hoaDon);
        }

        // GET: HoaDon/Create
        public IActionResult Create()
        {
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh");
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv");
            ViewData["MaTrangThai"] = new SelectList(_context.TrangThais, "MaTrangThai", "MaTrangThai");
            return View();
        }

        // POST: HoaDon/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHd,MaKh,NgayDat,NgayCan,NgayGiao,HoTenNguoiNhan,DiaChi,CachThanhToan,CachVanChuyen,PhiVanChuyen,MaTrangThai,MaNv,GhiChu")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", hoaDon.MaKh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hoaDon.MaNv);
            ViewData["MaTrangThai"] = new SelectList(_context.TrangThais, "MaTrangThai", "MaTrangThai", hoaDon.MaTrangThai);
            return View("~/Views/Admin/QuanLyHoaDon/Create.cshtml");
        }

        // GET: HoaDon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", hoaDon.MaKh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hoaDon.MaNv);
            ViewData["MaTrangThai"] = new SelectList(_context.TrangThais, "MaTrangThai", "MaTrangThai", hoaDon.MaTrangThai);

            return View("~/Views/Admin/QuanLyHoaDon/Edit.cshtml", hoaDon);
        }

        // POST: HoaDon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int MaTrangThai, string? GhiChu)
        {
            // Tìm hóa đơn thực tế trong DB
            var existingHoaDon = await _context.HoaDons.FindAsync(id);

            if (existingHoaDon == null)
            {
                return NotFound();
            }

            try
            {
                // CHỈ cập nhật những gì Shipper được phép sửa
                existingHoaDon.MaTrangThai = MaTrangThai;
                existingHoaDon.GhiChu = GhiChu;

                // Nếu chuyển sang trạng thái "Đã xác nhận" (2), tự cập nhật ngày giao nếu cần
                if (MaTrangThai == 2 && existingHoaDon.NgayGiao == null)
                {
                    existingHoaDon.NgayGiao = DateTime.Now;
                }

                _context.Update(existingHoaDon);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoaDonExists(existingHoaDon.MaHd)) return NotFound();
                else throw;
            }
        }


        // GET: HoaDon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/QuanLyHoaDon/Delete.cshtml");
        }

        // POST: HoaDon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.MaHd == id);
        }

        // GET: HoaDon/Search?query=KH001
        [HttpGet]
        public IActionResult Search(string query)
        {
            var hoaDonsQuery = _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                hoaDonsQuery = hoaDonsQuery
                    .Where(h => h.MaKhNavigation.MaKh.Contains(query));
            }

            var hoaDons = hoaDonsQuery.ToList();

            ViewBag.TongTienDict = hoaDons.ToDictionary(
                h => h.MaHd,
                h => h.ChiTietHds.Sum(ct => (decimal)ct.SoLuong * (decimal)ct.DonGia)
            );

            ViewBag.Query = query;

            return View("~/Views/Admin/QuanLyHoaDon/Index.cshtml", hoaDons);
        }

    }
}
