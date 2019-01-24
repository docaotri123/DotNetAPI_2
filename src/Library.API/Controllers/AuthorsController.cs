using Library.API.Services;
using Library.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Library.API.Helpers;
using AutoMapper;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController: Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetAuthors(bool includeBooks = false)
        {
            var authorsFromRepo = _libraryRepository.GetAuthors(includeBooks);

            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

            return Ok(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if(authorFromRepo == null)
            {
                return NotFound();
            }
            return Ok(Mapper.Map<AuthorDto>(authorFromRepo));
        }
    }
}
