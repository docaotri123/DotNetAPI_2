using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Library.API.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController: Controller
    {
        private ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBooks(Guid authorId)
        {
            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var booksfromRepo = _libraryRepository.GetBooksForAuthor(authorId);
            var books = Mapper.Map<IEnumerable<BookDto>>(booksfromRepo);
            return Ok(books);
        }

        [HttpGet("{bookId}",Name ="GetBook")]
        public IActionResult GetBook(Guid authorId, Guid bookId)
        {
            var book = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if(book == null)
            {
                return NotFound();
            }
            return Ok(Mapper.Map<BookDto>(book));
        }


        [HttpPost()]
        public IActionResult CreateBookForAuthor(Guid authorId,[FromBody] BookForCreateDto book)
        {
            if(book == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            book.AuthorId = authorId;
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var bookEntity = Mapper.Map<Book>(book);
            _libraryRepository.AddBookForAuthor(authorId, bookEntity);
            if (!_libraryRepository.Save())
            {
                return StatusCode(500, "A problem when add book");
            }
            var bookToReturn = Mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBook", new { authorId = book.AuthorId, bookId = bookToReturn.Id }, bookToReturn);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id,
            [FromBody] BookForUpdateDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }


            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(book, bookForAuthorFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Updating book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }


        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id,
           [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if(bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            var bookToPatch = Mapper.Map<BookForUpdateDto>(bookForAuthorFromRepo);

            patchDoc.ApplyTo(bookToPatch);

            Mapper.Map(bookToPatch, bookForAuthorFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Patching book {id} for author {authorId} failed on save.");
            }


            return NoContent();
        }
    }
}
