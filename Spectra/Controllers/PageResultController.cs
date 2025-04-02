using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;

namespace Spectra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageResultController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PageResultController(AppDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("Product")]
        public IActionResult ProductResult(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Products.Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var productsQuery = context.Products
                    .Join(context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        TitleSeo = x.gt.ai.TitleSeo,
                        MetaKeyWords = x.gt.ai.MetaKeyWords,
                        MetaDescription = x.gt.ai.MetaDescription,
                        Images = x.gt.ai.Images,
                        CategoryId = x.gt.ai.CategoryId,
                        Option = x.gt.ai.Option,
                        GiftId = x.gt.ai.GiftId,
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0,120),
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .AsNoTracking()
                    .OrderBy(x => x.Code)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
                    .ToList();

                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = productsQuery
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("ProductAdmin")]
        public IActionResult ProductResultAdmin(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Products.Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var productsQuery = context.Products.Join(context.Category, ai => ai.CategoryId,
                    al => al.Id, (ai, al) => new { ai, al }).Join(context.Gift, gt => gt.ai.GiftId,
                    pr => pr.Id, (gt, pr) => new { gt, pr }).Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Code = x.gt.ai.Code,
                        Name = x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Images = x.gt.ai.Images,
                        CategoryId = x.gt.ai.CategoryId,
                        ScheduleStatus = x.gt.ai.ScheduleStatus,
                        Status = x.gt.ai.Status,
                        AccountEdit = x.gt.ai.AccountEdit,
                        CreatedDate = x.gt.ai.CreatedDate,
                        ModifiedDate = x.gt.ai.ModifiedDate,
                        CategoryName = x.gt.al.Name,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()

                    })
                    .AsNoTracking()
                    .OrderBy(x => x.Code)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize);
                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = productsQuery.ToList()
                    };
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }

        }

        [HttpGet]
        [Route("ProductNew")]
        public IActionResult ProductNew(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            try
            {
                var countDetails = _context.Products.Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var productsQuery = context.Products
                    .Join(context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Option = x.gt.ai.Option,
                        Images = x.gt.ai.Images,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        CreatedDate = x.gt.ai.CreatedDate,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .AsNoTracking()
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
                    .ToList();

                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = productsQuery
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
        [HttpGet]
        [Route("ProductSale")]
        public IActionResult ProductSale(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            try
            {
                var countDetails = _context.Products.Count(x => x.SalePrice > 0);
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var productsQuery = context.Products
                    .Where(p => p.SalePrice > 0)
                    .Join(context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Option = x.gt.ai.Option,
                        Images = x.gt.ai.Images,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
                    .ToList();

                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = productsQuery
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("ProductPriceASC")]
        public IActionResult ProductPriceASC(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Products.Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var productsQuery = context.Products
                    .Join(context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Images = x.gt.ai.Images,
                        Option = x.gt.ai.Option,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .AsNoTracking()
                    .OrderBy(p => p.SalePrice > 0 ? p.SalePrice : p.Price)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
                    .ToList();

                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = productsQuery
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("ProductPriceASCBrand")]
        public IActionResult ProductPriceASCBrand(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Products.Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var productsQuery = context.Products
                    .Join(context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Images = x.gt.ai.Images,
                        Option = x.gt.ai.Option,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .AsNoTracking()
                    .OrderBy(p => p.Price)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
                    .ToList();

                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = productsQuery
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("ProductPriceDSC")]
        public IActionResult ProductPriceDSC(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Products.Count();
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var productsQuery = context.Products
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Images = x.gt.ai.Images,
                        Option = x.gt.ai.Option,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .AsNoTracking()
                    .OrderByDescending(p => p.SalePrice > 0 ? p.SalePrice : p.Price)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
                    .ToList();

                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = productsQuery
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("ProductPriceDSCBrand")]
        public IActionResult ProductPriceDSCBrand(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Products.Count();
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var productsQuery = context.Products.Join(context.Category, ai => ai.CategoryId,
                    al => al.Id, (ai, al) => new { ai, al }).Join(context.Gift, gt => gt.ai.GiftId,
                    pr => pr.Id, (gt, pr) => new { gt, pr }).Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Option = x.gt.ai.Option,
                        Images = x.gt.ai.Images,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                        .AsNoTracking()
                        .OrderByDescending(p => p.Price)
                        .Skip((currentPage - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = productsQuery
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("WelcomeCategory/{id}")]
        public IActionResult WelcomeCategory([FromRoute] int? id, int? page, int pagesize = 3)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.WelcomeDetails
                        .Where(x => x.WelcomeId == id && x.Status == true)
                        .Count();

                    var result = new PageResult<WelcomeDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 3,
                        Items = context.WelcomeDetails
                            .Where(x => x.WelcomeId == id && x.Status == true)
                            .Select(x => new WelcomeDetailDisplay
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Image = x.Image,
                                WelcomeId = x.WelcomeId,
                                TitleSeo = x.TitleSeo,
                                MetaKeyWords = x.MetaKeyWords,
                                CreatedDate = x.CreatedDate,
                                MetaDescription = x.MetaDescription,
                                Description = x.Description.Substring(0, Math.Min(x.Description.Length, 200)), // Đảm bảo không vượt quá độ dài chuỗi
                                Status = x.Status,
                                LinkName = rgx.Replace(x.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .OrderByDescending(x => x.CreatedDate)
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("NewsDetails")]
        public IActionResult NewsDetails(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.NewsDetails.Count();

                    var result = new PageResult<NewsDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 6,
                        Items = context.NewsDetails
                            .Join(context.CategoryNews, ai => ai.CategoryNewId, al => al.Id, (ai, al) => new { ai, al })
                            .Join(context.Category, ca => ca.ai.CategoryId, ne => ne.Id, (ca, ne) => new NewsDetailDisplay
                            {
                                Id = ca.ai.Id,
                                Name = ca.ai.Name,
                                Status = ca.ai.Status,
                                Description = ca.ai.Description.Substring(0, Math.Min(ca.ai.Description.Length, 200)), // Đảm bảo không vượt quá độ dài chuỗi
                                Image = ca.ai.Image,
                                CategoryNewId = ca.ai.CategoryNewId,
                                CategoryId = ca.ai.CategoryId,
                                TitleSeo = ca.ai.TitleSeo,
                                MetaKeyWords = ca.ai.MetaKeyWords,
                                MetaDescription = ca.ai.MetaDescription,
                                CreatedDate = ca.ai.CreatedDate,
                                CateNewName = ca.al.Name,
                                CateName = ne.Name,
                                LinkName = rgx.Replace(ca.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .Where(x => x.Status == true)
                            .OrderByDescending(p => p.CreatedDate)
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving news details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("NewsDetailsAdmin")]
        public IActionResult NewsDetailsAdmin(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.NewsDetails.Count();

                    var result = new PageResult<NewsDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 5,
                        Items = context.NewsDetails
                            .Join(context.CategoryNews, ai => ai.CategoryNewId, al => al.Id, (ai, al) => new { ai, al })
                            .Join(context.Category, ca => ca.ai.CategoryId, ne => ne.Id, (ca, ne) => new NewsDetailDisplay
                            {
                                Id = ca.ai.Id,
                                Code = ca.ai.Code,
                                Name = ca.ai.Name,
                                Status = ca.ai.Status,
                                Image = ca.ai.Image,
                                CategoryNewId = ca.ai.CategoryNewId,
                                CategoryId = ca.ai.CategoryId,
                                CreatedDate = ca.ai.CreatedDate,
                                ModifiedDate = ca.ai.ModifiedDate,
                                CateNewName = ca.al.Name,
                                CateName = ne.Name,
                                LinkName = rgx.Replace(ca.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .OrderByDescending(p => p.CreatedDate)
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving news details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("NewsDetailsASC")]
        public IActionResult NewsDetailsASC(int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.NewsDetails.Count();

                    var result = new PageResult<NewsDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 6,
                        Items = context.NewsDetails
                            .Join(context.CategoryNews, ai => ai.CategoryNewId, al => al.Id, (ai, al) => new { ai, al })
                            .Join(context.Category, ca => ca.ai.CategoryId, ne => ne.Id, (ca, ne) => new NewsDetailDisplay
                            {
                                Id = ca.ai.Id,
                                Name = ca.ai.Name,
                                Status = ca.ai.Status,
                                Description = ca.ai.Description.Substring(0, 200),
                                Image = ca.ai.Image,
                                CategoryNewId = ca.ai.CategoryNewId,
                                CategoryId = ca.ai.CategoryId,
                                TitleSeo = ca.ai.TitleSeo,
                                MetaKeyWords = ca.ai.MetaKeyWords,
                                MetaDescription = ca.ai.MetaDescription,
                                CreatedDate = ca.ai.CreatedDate,
                                CateNewName = ca.al.Name,
                                CateName = ne.Name,
                                LinkName = rgx.Replace(ca.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .Where(x => x.Status == true)
                            .OrderBy(p => p.CreatedDate)
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving news details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("getcategorynew/{id}")]
        public IActionResult NewsDetailsCategory([FromRoute] int? id, int? page, int pagesize = 4)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.NewsDetails
                        .Where(x => x.CategoryNewId == id && x.Status == true)
                        .Count();

                    var result = new PageResult<NewsDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 4,
                        Items = context.NewsDetails
                            .Join(context.CategoryNews, ai => ai.CategoryNewId, al => al.Id, (ai, al) => new { ai, al })
                            .Where(x => x.ai.CategoryNewId == id && x.ai.Status == true)
                            .Select(x => new NewsDetailDisplay
                            {
                                Id = x.ai.Id,
                                Name = x.ai.Name,
                                CategoryNewId = x.ai.CategoryNewId,
                                Description = x.ai.Description.Substring(0, 300),
                                Image = x.ai.Image,
                                TitleSeo = x.ai.TitleSeo,
                                MetaKeyWords = x.ai.MetaKeyWords,
                                MetaDescription = x.ai.MetaDescription,
                                CreatedDate = x.ai.CreatedDate,
                                Status = x.ai.Status,
                                LinkName = rgx.Replace(x.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .OrderByDescending(x => x.CreatedDate)
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving news details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("getnewcateDT/{id}")]
        public IActionResult NewsDetailsCategoryDT([FromRoute] int? id, int? page, int pagesize = 3)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.NewsDetails
                        .Where(x => x.CategoryNewId == id && x.Status == true)
                        .Count();

                    var result = new PageResult<NewsDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 3,
                        Items = context.NewsDetails
                            .Join(context.CategoryNews, ai => ai.CategoryNewId, al => al.Id, (ai, al) => new { ai, al })
                            .Where(x => x.ai.CategoryNewId == id && x.ai.Status == true)
                            .Select(x => new NewsDetailDisplay
                            {
                                Id = x.ai.Id,
                                Name = x.ai.Name,
                                Code = x.ai.Code,
                                CategoryNewId = x.ai.CategoryNewId,
                                Description = x.ai.Description,
                                Image = x.ai.Image,
                                TitleSeo = x.ai.TitleSeo,
                                MetaKeyWords = x.ai.MetaKeyWords,
                                MetaDescription = x.ai.MetaDescription,
                                CreatedDate = x.ai.CreatedDate,
                                Status = x.ai.Status,
                                LinkName = rgx.Replace(x.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .OrderByDescending(x => x.CreatedDate)
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving news details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("getServiceId/{id}")]
        public IActionResult getServiceId([FromRoute] int? id, int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.ServiceDetails
                        .Where(x => x.ServiceId == id && x.Status == true)
                        .Count();

                    var result = new PageResult<ServiceDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 6,
                        Items = context.ServiceDetails
                            .Join(context.Services, ai => ai.ServiceId, al => al.Id, (ai, al) => new { ai, al })
                            .Where(x => x.ai.ServiceId == id && x.ai.Status == true)
                            .Select(x => new ServiceDetailDisplay
                            {
                                Id = x.ai.Id,
                                Name = x.ai.Name,
                                Image = x.ai.Image,
                                ServiceId = x.ai.ServiceId,
                                ServiceName = x.al.Name,
                                Description = x.ai.Description.Substring(0, 200),
                                TitleSeo = x.ai.TitleSeo,
                                CreatedDate = x.ai.CreatedDate,
                                MetaKeyWords = x.ai.MetaKeyWords,
                                MetaDescription = x.ai.MetaDescription,
                                Status = x.ai.Status,
                                LinkName = rgx.Replace(x.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .OrderByDescending(x => x.CreatedDate)
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving service details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("getcategoryID/{id}")]
        public IActionResult getcategoryID([FromRoute] int? id, int? page, int pagesize = 6)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.Products
                        .Where(x => x.CategoryId == id)
                        .Count();

                    var result = new PageResult<ProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 6,
                        Items = context.Products
                            .Join(context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                            .Join(context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                            .Where(x => x.gt.ai.CategoryId == id)
                            .Select(x => new ProductDisplay
                            {
                                Id = x.gt.ai.Id,
                                Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                                Price = x.gt.ai.Price,
                                SalePrice = x.gt.ai.SalePrice,
                                TitleSeo = x.gt.ai.TitleSeo,
                                Option = x.gt.ai.Option,
                                MetaKeyWords = x.gt.ai.MetaKeyWords,
                                MetaDescription = x.gt.ai.MetaDescription,
                                Images = x.gt.ai.Images,
                                CategoryId = x.gt.ai.CategoryId,
                                GiftId = x.gt.ai.GiftId,
                                TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                                Status = x.gt.ai.Status,
                                CategoryName = x.gt.al.Name,
                                GiftName = x.pr.Name,
                                GiftPrice = x.pr.Price,
                                Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                                LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .OrderByDescending(x => x.CreatedDate)
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }


        [HttpGet]
        [Route("WelcomeDetail")]
        public IActionResult WelcomeDetailResult(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.WelcomeDetails.Count();

                    var result = new PageResult<WelcomeDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 5,
                        Items = context.WelcomeDetails
                            .Join(context.Welcomes, ai => ai.WelcomeId, al => al.Id, (ai, al) => new { ai, al })
                            .Select(x => new WelcomeDetailDisplay
                            {
                                Id = x.ai.Id,
                                Name = x.ai.Name,
                                Code = x.ai.Code,
                                Image = x.ai.Image,
                                WelcomeId = x.ai.WelcomeId,
                                CateWelName = x.al.Name,
                                CreatedDate = x.ai.CreatedDate,
                                Status = x.ai.Status,
                                LinkName = rgx.Replace(x.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving welcome details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("ServiceDetail")]
        public IActionResult ServiceDetail(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var currentPage = page ?? 1;

                using (var context = _context)
                {
                    var countDetails = context.ServiceDetails.Count();

                    var result = new PageResult<ServiceDetailDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = 5,
                        Items = context.ServiceDetails
                            .Join(context.Services, ai => ai.ServiceId, al => al.Id, (ai, al) => new { ai, al })
                            .Select(x => new ServiceDetailDisplay
                            {
                                Id = x.ai.Id,
                                Code = x.ai.Code,
                                Name = x.ai.Name,
                                Status = x.ai.Status,
                                Description = x.ai.Description,
                                Image = x.ai.Image,
                                ServiceId = x.ai.ServiceId,
                                TitleSeo = x.ai.TitleSeo,
                                MetaKeyWords = x.ai.MetaKeyWords,
                                MetaDescription = x.ai.MetaDescription,
                                CreatedDate = x.ai.CreatedDate,
                                ModifiedDate = x.ai.ModifiedDate,
                                ServiceName = x.al.Name,
                                LinkName = rgx.Replace(x.ai.Name, "-").ToLower()
                            })
                            .AsNoTracking()
                            .Skip((currentPage - 1) * pagesize)
                            .Take(pagesize)
                            .ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving service details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}