using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BooksApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace nc.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        IMongoDatabase db = DbManager.BaseManager.GetDataBase("Bookdb");
        IMongoCollection<Book> books;
        public BooksController()
        {
            books = db.GetCollection<Book>("Books");
        }

        //customer api
        [Route("customers/{customerId}/orders/{orderId}")]
        public string GetOrderByCustomer(int customerId, int orderId)
        {
            var bd = new BsonDocument();
            bd.AddRange(new BsonDocument("a", customerId));
            bd.AddRange(new BsonDocument("b", orderId));
            return bd.ToJson();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("report/fileupload/{rpname}")]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return Ok();
        }

        [Route("report/file/{rpname}")]
        public IActionResult GetFile(string rpname)
        {
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            using (Stream s = new FileStream(filePath + rpname, FileMode.Open, FileAccess.Read))
            {
                return File(s, "application/octet-stream");
            }
        }

        //[DeflateCompression]//using zlib compression
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //禁止使用decimal，转用double型
            var names = await books.Find(x => x.Price >50 && x.Price <300).ToListAsync();
            return Ok(names);

            ////日期时间操作提交时必须是utc时间
            ////日期时间查询时必须先转为本地时间
            //var start = DateTime.Parse("2015-07-11T15:07:41").ToLocalTime();
            //var end = DateTime.Parse("2015-07-12T03:01:37").ToLocalTime();
            //var names = await books.Find(x => x.ReleaseDate.ToLocalTime() >= start && x.ReleaseDate.ToLocalTime() <= end).ToListAsync();
            //return Ok(names);

            //var bks = await books.Find(new BsonDocument()).ToListAsync();
            //return Ok(bks);
        }

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBook(string id)
        {
            //身份验证
            //var pwd = Request.Headers.SingleOrDefault(h => h.Key == "pwd");
            //if (pwd.Value.FirstOrDefault() != "123")
            //{
            //    return StatusCode(HttpStatusCode.NotFound);
            //}
            if (!ObjectId.TryParse(id, out ObjectId oid))
            {
                return NotFound();
            }
            var book = await books.Find(new BsonDocument("_id", oid)).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book.ToJson());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //时区+8
            book.ReleaseDate = book.ReleaseDate.ToLocalTime();
            await books.InsertOneAsync(book);

            return Ok(book.ToJson());
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Book bookIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ObjectId.Parse(id) != bookIn.Id)
            {
                return BadRequest();
            }

            //var result = await books.UpdateOneAsync(x => x.Id == ObjectId.Parse(id), Builders<Book>.Update.Set(x => x, book)); 
            var dbbook = await books.Find(x => x.Id == ObjectId.Parse(id)).SingleAsync();
            dbbook.BookName = bookIn.BookName;
            dbbook.Price = bookIn.Price;
            dbbook.ReleaseDate = bookIn.ReleaseDate.ToLocalTime();
            dbbook.Author = bookIn.Author;
            var result = await books.ReplaceOneAsync(x => x.Id == ObjectId.Parse(id), dbbook);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await BookExists(id))
            {
                return NotFound();
            }

            var result = await books.DeleteOneAsync(x => x.Id == ObjectId.Parse(id));

            return Ok(result);
        }

        private async Task<bool> BookExists(string id)
        {
            var c = await books.CountDocumentsAsync<Book>(e => e.Id == ObjectId.Parse(id));
            if (c > 0)
            {
                return true;
            }
            return false;
        }


    }
}
