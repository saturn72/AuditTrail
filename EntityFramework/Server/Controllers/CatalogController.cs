using Microsoft.AspNetCore.Mvc;
using Server.Domain;

namespace Server.Controllers
{
    [ApiController]
    [Route("catalog")]
    public class CatalogController : ControllerBase
    {
        /// <summary>
        /// Creates multiple products
        /// </summary>
        /// <param name="count">number of products to add</param>
        /// <returns>the created products</returns>
        [HttpPost("product/{count}")]
        public async Task<IActionResult> CreateProducts(int count)
        {
            var list = new List<Product>();

            for (var i = 0; i < count; i++)
                list.Add(new Product { Name = $"product-{Guid.NewGuid()}" });

            using var context = HttpContext.RequestServices.GetRequiredService<CatalogContext>();
            await context.AddRangeAsync(list);
            await context.SaveChangesAsync();

            return Ok(list);
        }

        /// <summary>
        /// Creates multiple categories
        /// </summary>
        /// <param name="count">total number of categories to create</param>
        /// <returns>the created categories</returns>
        [HttpPost("category/{count}")]
        public async Task<IActionResult> CreateCategories(int count)
        {
            var list = new List<Category>();

            for (var i = 0; i < count; i++)
                list.Add(new Category { Name = $"category-{Guid.NewGuid()}" });

            using var context = HttpContext.RequestServices.GetRequiredService<CatalogContext>();
            await context.AddRangeAsync(list);
            await context.SaveChangesAsync();

            return Ok(list);
        }

        /// <summary>
        /// rename product
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="newName">new name</param>
        /// <returns>the updated product</returns>

        [HttpPut("product/{productId}/{newName}")]
        public async Task<IActionResult> AddCategoryToProduct(int productId, string newName)
        {
            using var context = HttpContext.RequestServices.GetRequiredService<CatalogContext>();
            var product = await context.FindAsync<Product>(productId);
            if (product == null)
                return BadRequest();

            product.Name = newName;
            product.Price += 10;

            await context.SaveChangesAsync();
            return Ok(product);
        }

        /// <summary>
        /// adds category to product
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="categoryId">category id</param>
        /// <returns>the updated product</returns>
        [HttpPut("product/{productId}/addCategory/{categoryId}")]
        public async Task<IActionResult> AddCategoryToProduct(int productId, int categoryId)
        {

            using var context = HttpContext.RequestServices.GetRequiredService<CatalogContext>();
            var product = await context.FindAsync<Product>(productId);
            if (product == null)
                return BadRequest();

            var category = await context.FindAsync<Category>(categoryId);
            if (category == null)
                return BadRequest();

            (product.Categories ??= new List<Category>()).Add(category);
            await context.SaveChangesAsync();
            return Ok(product);
        }

        /// <summary>
        /// deletes a product
        /// </summary>
        /// <param name="productId">prodct id</param>
        /// <returns>the deleted product</returns>
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            using var context = HttpContext.RequestServices.GetRequiredService<CatalogContext>();
            var product = await context.FindAsync<Product>(productId);
            if (product == null)
                return BadRequest();

            context.Remove(product);

            await context.SaveChangesAsync();
            return Ok(product);
        }
    }
}