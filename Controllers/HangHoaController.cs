using FruitShopG4P.Data;
using FruitShopG4P.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace VanPhongPhamOnline.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly FruitShopContext db;

        public HangHoaController(FruitShopContext context)
        {
            db = context;
        }

        private bool HangHoaExists(int id)
        {
            return db.HangHoas.Any(e => e.MaHh == id);
        }
        public IActionResult Index(int? loai, int page = 1)
        {
            LoadMenuLoai();
            const int pageSize = 12;
            var hangHoas = db.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            var totalItems = hangHoas.Count();
            var result = hangHoas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai,
                NgaySx = p.NgaySx,
                HanSd = p.HanSd,
            }).ToList();

            /* Đưa thông tin phân trang vào ViewBag */
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.Loai = loai;

            //var result = hangHoas.Select(p => new HangHoaVM
            //{
            //    MaHH = p.MaHh,
            //    TenHH = p.TenHh,
            //    DonGia = p.DonGia ?? 0,
            //    Hinh = p.Hinh ?? "",
            //    MoTaNgan = p.MoTaDonVi ?? "",
            //    TenLoai = p.MaLoaiNavigation.TenLoai
            //});

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadMenuLoai();
            var model = new HangHoa
            {
                NgaySx = DateTime.Now, // Gợi ý ngày hôm nay
                HanSd = DateTime.Now.AddYears(1), // Gợi ý 6 tháng sau
            };
            ViewBag.MaNcc = new SelectList(db.NhaCungCaps, "MaNcc", "TenNcc");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var hangHoa = await db.HangHoas.FindAsync(id);
            if (hangHoa == null) return NotFound();

            LoadMenuLoai();
            return View(hangHoa);
        }

        // POST: QuanLyHangHoa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bỏ [Bind] cũ, để ASP.NET tự nhận diện toàn bộ thuộc tính của HangHoa
        public async Task<IActionResult> Create(HangHoa hangHoa, IFormFile HinhAnh)
        {
            // Loại bỏ kiểm tra các bảng liên kết vì chúng ta gán ID trực tiếp
            ModelState.Remove("MaLoaiNavigation");
            ModelState.Remove("MaNccNavigation");
            ModelState.Remove("Hinh"); // Xóa kiểm tra trường Hinh vì ta xử lý file sau

            if (ModelState.IsValid)
            {
                // 1. Xử lý File Ảnh
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnh.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnh.CopyToAsync(stream);
                    }
                    hangHoa.Hinh = fileName;
                }

                // KHÔNG gán cứng hangHoa.MaNcc = "NCC01" ở đây nữa
                db.Add(hangHoa);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hangHoa);
        }

        // POST: QuanLyHangHoa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HangHoa hangHoa, IFormFile HinhAnh)
        {
            if (id != hangHoa.MaHh) return NotFound();

            ModelState.Remove("MaLoaiNavigation");
            ModelState.Remove("MaNccNavigation");
            ModelState.Remove("Hinh");

            if (ModelState.IsValid)
            {
                try
                {
                    if (HinhAnh != null && HinhAnh.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnh.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await HinhAnh.CopyToAsync(stream);
                        }
                        hangHoa.Hinh = fileName;
                    }
                    else
                    {
                        // Nếu không chọn ảnh mới, giữ lại tên ảnh cũ tránh bị null
                        db.Entry(hangHoa).Property(x => x.Hinh).IsModified = false;
                    }

                    db.Update(hangHoa);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HangHoaExists(hangHoa.MaHh)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hangHoa);
        }
        public IActionResult Search(string? query)
        {
            LoadMenuLoai();

            var hangHoas = db.HangHoas.AsQueryable();
            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai,
                NgaySx = p.NgaySx,
                HanSd = p.HanSd
            });

            return View(result);
        }
        
        public IActionResult Detail(int id)
        {
            LoadMenuLoai();

            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"Không thể tìm thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            var result = new HangHoaVM
            {
                MaHH = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,

                SoLuongTon = data.SoLuongTonKho, // Check again
                DiemDanhGia = 5, // Check again

                NgaySx = data.NgaySx,
                HanSd = data.HanSd,
                MaNcc = data.MaNccNavigation.MaNcc
            };
            return View(result);
        }
        private void LoadMenuLoai()
        {
            var data = db.Loais
                .Select(loai => new MenuLoaiVM
                {
                    MaLoai = loai.MaLoai,
                    TenLoai = loai.TenLoai,
                    SoLuong = loai.HangHoas.Count()
                }).ToList();

            ViewBag.MenuLoai = data;
        }

    }
}
