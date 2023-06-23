using Microsoft.AspNetCore.Mvc;
using Cassandra;
using System.Data;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Login : Controller
    {
        [HttpGet]
        [Route("Login")]
        public ActionResult User_Login(string username,string password)
        {
            var cluster = Cluster.Builder()
              .AddContactPoint("127.0.0.1")
              .Build();
            var session = cluster.Connect("user");
            var preparedStatement = session.Prepare("SELECT * FROM users WHERE username = ? AND password = ? ALLOW FILTERING");
            var boundStatement = preparedStatement.Bind(username, password);
            var row = session.Execute(boundStatement).FirstOrDefault();
            int count = (row != null) ? 1 : 0;
            return Ok(count==1);
        }
        [HttpGet]
        [Route("Register")]
        public ActionResult User_Register(string username, string name, string password,string email,string phonenumber)
        {
            var cluster = Cluster.Builder()
              .AddContactPoint("127.0.0.1")
              .Build();
            var session = cluster.Connect("user");
            var preparedStatement = session.Prepare("INSERT INTO users (username, password) VALUES (?, ?);");
            var boundStatement = preparedStatement.Bind(username, password);
            session.Execute(boundStatement);
            preparedStatement = session.Prepare("INSERT INTO customer (username, name, email,phonenumber) VALUES (?, ?, ?, ?);");
            boundStatement = preparedStatement.Bind(username, name, email, phonenumber);
            session.Execute(boundStatement);
            return Ok(true);
        }
        [HttpGet]
        [Route("Getinfo")]
        public ActionResult User_Getinfo(string username)
        {
            var cluster = Cluster.Builder()
                .AddContactPoint("127.0.0.1")
                .Build();
            var session = cluster.Connect("user");
            var preparedStatement = session.Prepare("SELECT * FROM customer WHERE username = ?");
            var boundStatement = preparedStatement.Bind(username);
            var resultSet = session.Execute(boundStatement);
            var row = resultSet.FirstOrDefault();
            return Ok(row);
        }
    }
}
