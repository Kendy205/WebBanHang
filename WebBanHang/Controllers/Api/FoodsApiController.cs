using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.BLL.IServices;

namespace WebBanHang.Controllers.Api
{
    [Route("api/foods")]
    [ApiController]
    public class FoodsApiController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodsApiController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        // ======================================================
        // GET: /api/foods
        // Lấy danh sách món ăn với bộ lọc và phân trang (tương tự action Index)
        // ======================================================
        [HttpGet]
        public async Task<IActionResult> GetFoods(
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string sortBy = "name",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            try
            {
                var (foods, totalRecords) = await _foodService.GetFoodsByFilter(
                    categoryId, minPrice, maxPrice, sortBy, page, pageSize);

                var result = new
                {
                    Success = true,
                    Data = foods.Select(f => new
                    {
                        f.FoodId,
                        f.FoodName,
                        f.Price,
                        f.ImageUrl,
                        f.Rating,
                        CategoryName = f.Category?.CategoryName
                    }),
                    Pagination = new
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalRecords = totalRecords,
                        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi server: " + ex.Message });
            }
        }

        // ======================================================
        // GET: /api/foods/5
        // Lấy chi tiết một món ăn (tương tự action Details)
        // ======================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodDetails(int id)
        {
            try
            {
                var food = await _foodService.GetFoodById(id);

                if (food == null || !food.IsAvailable)
                {
                    return NotFound(new { Success = false, Message = "Món ăn không tồn tại hoặc đã ngừng bán." });
                }

                // Lấy các món ăn liên quan
                var relatedFoods = (await _foodService.GetFoodsByCategory(food.CategoryId))
                                       .Where(f => f.FoodId != id)
                                       .Take(4)
                                       .Select(f => new
                                       {
                                           f.FoodId,
                                           f.FoodName,
                                           f.Price,
                                           f.ImageUrl
                                       })
                                       .ToList();

                var result = new
                {
                    Success = true,
                    Data = new
                    {
                        food.FoodId,
                        food.FoodName,
                        food.Description,
                        food.Price,
                        food.ImageUrl,
                        food.Rating,
                        //food.TotalReviews,
                        CategoryName = food.Category?.CategoryName,
                        RelatedFoods = relatedFoods
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi server: " + ex.Message });
            }
        }

        // ======================================================
        // GET: /api/foods/search
        // Tìm kiếm món ăn (tương tự action Search)
        // ======================================================
        [HttpGet("search")]
        public async Task<IActionResult> SearchFoods(
            [FromQuery] string keyword,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            try
            {
                var foods = await _foodService.SearchFoods(keyword);

                if (minPrice.HasValue && maxPrice.HasValue)
                {
                    foods = foods.Where(f => f.Price >= minPrice && f.Price <= maxPrice).ToList();
                }

                var totalRecords = foods.Count();
                var pagedFoods = foods.Skip((page - 1) * pageSize).Take(pageSize);

                var result = new
                {
                    Success = true,
                    Data = pagedFoods.Select(f => new
                    {
                        f.FoodId,
                        f.FoodName,
                        f.Price,
                        f.ImageUrl,
                        f.Rating
                    }),
                    Pagination = new
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalRecords = totalRecords,
                        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi server: " + ex.Message });
            }
        }

        // ======================================================
        // GET: /api/foods/top-rated
        // Lấy các món ăn được đánh giá cao nhất
        // ======================================================
        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedFoods([FromQuery] int count = 6)
        {
            try
            {
                var foods = await _foodService.GetTopRatedFoods(count);
                var result = new
                {
                    Success = true,
                    Data = foods.Select(f => new
                    {
                        f.FoodId,
                        f.FoodName,
                        f.Price,
                        f.ImageUrl,
                        f.Rating
                    })
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi server: " + ex.Message });
            }
        }
    }
}
