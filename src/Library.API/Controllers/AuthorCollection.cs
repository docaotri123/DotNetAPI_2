using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Library.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authorCollection")]
    public class AuthorCollection: Controller
    {

        private ILibraryRepository _libraryRepository;

        public AuthorCollection(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection([FromBody] IEnumerable<AuthorForCreateDto> authors)
        {
            if(authors == null)
            {
                return BadRequest();
            }

            var authorsEntity = Mapper.Map<IEnumerable<Author>>(authors);

            foreach(var aut in authorsEntity)
            {
                _libraryRepository.AddAuthor(aut);
            }

            if(!_libraryRepository.Save())
            {
                return StatusCode(500, "A problem when add authors");
            }

            var authorsToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorsEntity);
            var idsAsString = string.Join(",", authorsToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new {ids = idsAsString }, authorsToReturn);
            //return Ok();
        }

        [HttpGet("{(ids)}", Name ="GetAuthorCollection")]
        public IActionResult GetAuthorCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                return BadRequest();
            }
            var authorEntities = _libraryRepository.GetAuthors(ids);

            if(ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }
            var authorToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorToReturn);
        }
    }
}
