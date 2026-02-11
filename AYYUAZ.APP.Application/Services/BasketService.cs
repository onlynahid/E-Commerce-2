using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Application.Service 
{
    //public class BasketService  : IBasketService 
    //{
    //    private const string BasketKey = "Basket";
    //    private readonly IHttpContextAccessor _httpContext;
    //    private List<BasketItemDto> Read()
    //    {
    //        var cookie = _httpContext.HttpContext.Request.Cookies[BasketKey];
    //        return string.IsNullOrEmpty(cookie)
    //            ? new List<BasketItemDto>()
    //            : JsonSerializer.Deserialize<List<BasketItemDto>>(cookie);
    //    }
    //    private void Write(List<BasketItemDto> basket)
    //    {
    //        _httpContext.HttpContext.Response.Cookies.Append(
    //            BasketKey,
    //            JsonSerializer.Serialize(basket),
    //            new CookieOptions
    //            {
    //                HttpOnly = true,
    //                Expires = DateTimeOffset.Now.AddDays(7)
    //            });
    //    }
    //    public BasketService (IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContext = httpContextAccessor;
    //    }
    //    public Task AddToBasketAsync(int productId)
    //    {
    //        var basket = Read();
    //        var item = basket.FirstOrDefault(b => b.ProductId == productId);
    //        if (item == null)
    //        {
    //            basket.Add(new BasketItemDto() { ProductId = productId, Quantity = 1 });
    //        }
    //        else
    //        {
    //            item.Quantity++;
    //        }
    //        Write(basket);
    //        return Task.CompletedTask;
          
    //    }
    //    public Task Clear()
    //    {
    //        _httpContext.HttpContext.Response.Cookies.Delete(BasketKey);
    //        return Task.CompletedTask;
    //    }
    //    public Task Decrase(int productid)
    //    {
    //        var basket = Read();
    //        var item = basket.FirstOrDefault(basket => basket.ProductId == productid);
    //        if(item == null)
    //        {
    //            return Task.CompletedTask;
    //        }
    //        item.Quantity--;
    //        if(item.Quantity <= 0)
    //        {
    //            basket.Remove(item);
    //        }
    //        Write(basket);
    //        return Task.CompletedTask;
    //    }
    //    public Task<List<BasketItemDto>> GetBasketItemsAsync()
    //    {
    //        return Task.FromResult(Read());

    //    }
    //    public Task Increase(int productid)
    //    {
    //        var basket = Read();
    //        var item = basket.FirstOrDefault(b => b.ProductId == productid);
    //        if (item != null)
    //        {
    //            item.Quantity++;
    //            Write(basket);
    //        }
    //        return Task.CompletedTask;
    //    }
    //    public Task RemoveFromBasketAsync(int productId)
    //    {
    //        var basket = Read();
    //        basket.RemoveAll(b => b.ProductId == productId);
    //        Write(basket);
    //        return Task.CompletedTask;

    //    }
    //}
}
