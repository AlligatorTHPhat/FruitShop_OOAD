using System;
using System.Collections.Generic;

namespace FruitShopG4P.Data;

public partial class VChiTietHoaDon
{
    public int MaCt { get; set; }

    public int MaHd { get; set; }

    public int MaHh { get; set; }

    public string TenHh { get; set; } = null!;

    public double DonGia { get; set; }

    public int SoLuong { get; set; }

    public double GiamGia { get; set; }

    public double? ThanhTien { get; set; }
}
