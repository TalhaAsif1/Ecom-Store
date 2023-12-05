using AutoMapper;
using Ecom.Dto;
using Ecom.Interfaces;
using Ecom.Models;
using Ecom.Repositoy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="User")]
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly IUserInterface _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserInterface userInterface, IMapper mapper)
        {
            _userRepository = userInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetAllUsers();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(users);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _userRepository.GetUserById(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        public IActionResult CreateUserWithOrders([FromBody] UserDto createUser)
        {
            if (createUser == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = _userRepository.GetAllUsers()
                .Where(u => u.LastName.ToUpper() == createUser.LastName.ToUpper()).FirstOrDefault();

            if (existingUser != null)
            {
                ModelState.AddModelError("", "User with this username already exists");
                return StatusCode(422, ModelState);
            }

            // Assuming you have a method in your repository to create a user with orders
            var userMap = _mapper.Map<User>(createUser);

            if(!_userRepository.AddUser(userMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
           
        }

        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateUser(int userId, [FromBody] UserDto updateUser)
        {
            if (updateUser == null || userId != updateUser.Id)
            {
                ModelState.AddModelError("", "Invalid user data or user ID mismatch");
                return BadRequest(ModelState);
            }

            var existingUser = _userRepository.GetUserById(userId);

            if (existingUser == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMap = _mapper.Map<User>(updateUser);

            if (!_userRepository.UpdateUser(userMap))
            {
                ModelState.AddModelError("", "Something went wrong updating user");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{userId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] // No content
        [ProducesResponseType(404)]
        public IActionResult DeleteUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var userToDelete = _userRepository.GetUserById(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.DeleteUser(userToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting user");
            }
            return NoContent();
        }



    }
}
