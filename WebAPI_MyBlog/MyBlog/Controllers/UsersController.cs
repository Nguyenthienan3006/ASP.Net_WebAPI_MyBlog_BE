﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Dto;
using MyBlog.Interfaces;
using MyBlog.Models;

namespace MyBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public IActionResult GetUsers()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userList = _mapper.Map<List<UserDto>>(_userRepository.GetUsers());

            return Ok(userList);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int userId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_userRepository.UserExist(userId))
            {
                return NotFound();
            }

            var userMapped = _mapper.Map<UserDto>(_userRepository.GetUser(userId));

            return Ok(userMapped);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUser([FromBody] UserToCreateDto userToCreate)
        {
            if(userToCreate == null)
            {
                return BadRequest(ModelState);
            }
            
            var userExist = _userRepository.GetUsers().
                Where(u => u.UserName.Trim().ToLower() == userToCreate.UserName.TrimEnd().ToLower()).FirstOrDefault();

            if (userExist != null)
            {
                ModelState.AddModelError("", "User already exists!");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMapped = _mapper.Map<User>(userToCreate);

            if (!_userRepository.CreateUser(userMapped))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Create Successfully");
        }

        [HttpPut("{userId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateUser(int userId, [FromBody] UserToCreateDto userToUpdate)
        {
            if(userToUpdate == null)
            {
                return BadRequest(ModelState);
            }

            if(userId != userToUpdate.Id)
            {
                return BadRequest(ModelState);
            }

            if(!_userRepository.UserExist(userId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMapped = _mapper.Map<User>(userToUpdate);

            if (!_userRepository.UpdateUser(userMapped))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Update Successfully");
        }

        [HttpDelete("userId")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult DeleteUser(int userId)
        {
            if (!_userRepository.UserExist(userId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMapped = _mapper.Map<User>(_userRepository.GetUser(userId));

            if (!_userRepository.DeleteUser(userMapped))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Delete Successfully");
        }
    }
}