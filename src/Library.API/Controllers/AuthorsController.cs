using Library.API.Services;
using Library.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Library.API.Helpers;
using AutoMapper;
using Library.API.Models;
using Library.API.Entities;

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

        [HttpGet(Name ="GetAuthors")]
        public IActionResult GetAuthors(bool includeBooks = false)
        {
            var authorsFromRepo = _libraryRepository.GetAuthors(includeBooks);

            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

            return Ok(authors);

        }

        [HttpGet("{id}",Name ="GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {
            if (! _libraryRepository.AuthorExists(id))
            {
                return NotFound();
            }
            var authorFromRepo = _libraryRepository.GetAuthor(id);

            return Ok(Mapper.Map<AuthorDto>(authorFromRepo));
        }

        [HttpPost()]
        public IActionResult CreateAuthor([FromBody] AuthorForCreateDto author)
        {
            if(author == null)
            {
                return BadRequest();
            }
            var authorEntity = Mapper.Map<Author>(author);
            _libraryRepository.AddAuthor(authorEntity);
            if(!_libraryRepository.Save())
            {
                return StatusCode(500, "A problem when add author");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);

        }
    }
}
