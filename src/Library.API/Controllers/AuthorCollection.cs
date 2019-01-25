using AutoMapper;
using Library.API.Entities;
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

            return Ok();
        }
    }
}
