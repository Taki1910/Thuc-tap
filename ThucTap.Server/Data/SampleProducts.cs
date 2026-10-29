using ThucTap.Server.Models;

namespace ThucTap.Server.Data;

public static class SampleProducts
{
    public static IReadOnlyList<Product> All { get; } =
    [
        new Product
        {
            Id = "coffee-dripper",
            Name = "Phễu lọc cà phê gốm",
            Category = "Đồ gia dụng",
            Price = 285000m,
            ImageUrl = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?auto=format&fit=crop&w=900&q=85",
            Description = "Phễu lọc gốm thủ công giúp bạn pha một tách cà phê cân bằng, đậm hương mỗi sáng.",
            Reviews =
            [
                new Review { Id = "review-1", Author = "Minh Anh", Rating = 5, Date = new DateOnly(2026, 9, 18), Comment = "Gốm đẹp, giữ nhiệt tốt và pha cà phê rất đều." },
                new Review { Id = "review-2", Author = "Quốc Bảo", Rating = 4, Date = new DateOnly(2026, 9, 12), Comment = "Hoàn thiện chắc chắn, đúng như mô tả." },
                new Review { Id = "review-3", Author = "Thu Hà", Rating = 5, Date = new DateOnly(2026, 8, 30), Comment = "Món đồ nhỏ xinh, dùng hàng ngày rất tiện." }
            ]
        },
        new Product
        {
            Id = "canvas-tote",
            Name = "Túi tote canvas Everyday",
            Category = "Phụ kiện",
            Price = 195000m,
            ImageUrl = "https://images.unsplash.com/photo-1544816155-12df9643f363?auto=format&fit=crop&w=900&q=85",
            Description = "Túi canvas dày dặn, thiết kế tối giản và đủ rộng cho những vật dụng cần thiết mỗi ngày.",
            Reviews =
            [
                new Review { Id = "review-4", Author = "Ngọc Linh", Rating = 5, Date = new DateOnly(2026, 9, 20), Comment = "Vải dày, đường may đẹp và đựng được nhiều đồ." },
                new Review { Id = "review-5", Author = "Thanh Tùng", Rating = 4, Date = new DateOnly(2026, 9, 4), Comment = "Màu sắc ngoài đời đẹp hơn ảnh." }
            ]
        },
        new Product
        {
            Id = "desk-lamp",
            Name = "Đèn bàn ánh sáng dịu",
            Category = "Đồ gia dụng",
            Price = 420000m,
            ImageUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=900&q=85",
            Description = "Đèn bàn nhỏ gọn với ánh sáng ấm dịu, phù hợp cho góc đọc sách hoặc bàn làm việc.",
            Reviews =
            [
                new Review { Id = "review-6", Author = "Hải Nam", Rating = 5, Date = new DateOnly(2026, 9, 15), Comment = "Ánh sáng dễ chịu, đặt cạnh giường rất hợp." },
                new Review { Id = "review-7", Author = "Mai Phương", Rating = 5, Date = new DateOnly(2026, 8, 28), Comment = "Thiết kế đẹp, lắp đặt đơn giản." },
                new Review { Id = "review-8", Author = "Gia Huy", Rating = 4, Date = new DateOnly(2026, 8, 8), Comment = "Đèn tốt trong tầm giá, giao hàng nhanh." }
            ]
        },
        new Product
        {
            Id = "linen-notebook",
            Name = "Sổ tay bìa vải Linen",
            Category = "Văn phòng phẩm",
            Price = 145000m,
            ImageUrl = "https://images.unsplash.com/photo-1531346878377-a5be20888e57?auto=format&fit=crop&w=900&q=85",
            Description = "Sổ tay giấy ngà mịn, bìa vải linen bền đẹp để ghi lại những ý tưởng và kế hoạch.",
            Reviews =
            [
                new Review { Id = "review-9", Author = "Khánh Vy", Rating = 5, Date = new DateOnly(2026, 9, 22), Comment = "Giấy viết mượt, bìa sổ có màu rất trang nhã." },
                new Review { Id = "review-10", Author = "Đức Anh", Rating = 4, Date = new DateOnly(2026, 9, 10), Comment = "Sổ đẹp, hợp để ghi chú công việc." }
            ]
        }
    ];
}
