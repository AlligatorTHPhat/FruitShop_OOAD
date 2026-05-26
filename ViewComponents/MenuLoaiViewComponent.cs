using Microsoft.AspNetCore.Mvc;
using FruitShopG4P.Data;
using FruitShopG4P.ViewModels;

namespace VanPhongPhamOnline.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly FruitShopContext db;
        public MenuLoaiViewComponent(FruitShopContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(p => p.TenLoai);

            return View(data); /** Default.cshtml */
            /** return View("Default",data); */
        }
    }
}
