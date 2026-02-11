using AYYUAZ.APP.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AYYUAZ.APP.Controllers
{
    //[ApiController]
    //[Route("api/basket")]
    //public class BasketController : ControllerBase
    //{
    //    private readonly IBasketService  _basketService ;
    //    public BasketController(IBasketService  basketService )
    //    {
    //        _basketService  = basketService ;
    //    }
    //    [HttpGet]
    //    public async Task<IActionResult> GetBasket()
    //    {
    //        var basket = await _basketService .GetBasketItemsAsync();
    //        return Ok(basket);
    //    }
    //    [HttpPost("add/{productId}")]
    //    public async Task<IActionResult> AddToBasket(int productId)
    //    {
    //        await _basketService .AddToBasketAsync(productId);
    //        return Ok(new { message = "Product added to basket." });
    //    }
    //    [HttpPost("increase/{productId}")]
    //    public async Task<IActionResult> Increase(int productId)
    //    {
    //        await _basketService .Increase(productId);
    //        return Ok(new { message = "Product quantity increased." });
    //    }
    //    [HttpPost("decrease/{productId}")]
    //    public async Task<IActionResult> Decrease(int productId)
    //    {
    //        await _basketService .Decrase(productId);
    //        return Ok(new { message = "Product quantity decreased." });
    //    }
    //    [HttpDelete("remove/{productId}")]
    //    public async Task<IActionResult> RemoveFromBasket(int productId)
    //    {
    //        await _basketService .RemoveFromBasketAsync(productId);
    //        return Ok(new { message = "Product removed from basket." });
    //    }
    //    [HttpPost("clear")]
    //    public async Task<IActionResult> Clear()
    //    {
    //        await _basketService .Clear();
    //        return Ok(new { message = "Basket cleared." });
    //    }
    //}
}
