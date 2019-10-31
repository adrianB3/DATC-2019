using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using simplewebapp.Models;

namespace simplewebapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        public List<Student> listOfStudents = new List<Student>{
          new Student{
            id = 0,
            Name = "Adrian",
            Department = "IS",
            YearOfStudy = 4
          }
        };
        // GET api/students
        [HttpGet]
        public ActionResult<IEnumerable<Student>> Get()
        {
            return Ok(listOfStudents.ToList());
        }

        // GET api/students/{id}
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/students
        [HttpPost]
        public ActionResult<Student> Post([FromBody] Student student)
        {
          var studentToAdd = student;
          if(!ModelState.IsValid){
            return BadRequest("Model state invalid");
          }
          
          foreach(var stud in listOfStudents){
            if(stud.id == studentToAdd.id)
              return BadRequest("Student already exists");
          }

          listOfStudents.Add(studentToAdd);
          return Ok(studentToAdd);
            
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
