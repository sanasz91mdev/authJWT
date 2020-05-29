using authSamples.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace authSamples.Controllers
{
    public class AuthController : ApiController
    {


        [HttpPost]
        public IHttpActionResult Login2(User user)
        {
            User u = new UserRepository().GetUser(user.Username);
            if (u == null)
                return Content(HttpStatusCode.NotFound,
                     "The user was not found.");
            bool credentials = u.Password.Equals(user.Password);
            if (!credentials) return Content(HttpStatusCode.Forbidden,
                "The username/password combination was wrong.");
            //return Request.CreateResponse(HttpStatusCode.OK,
            //     TokenManager.GenerateToken(user.Username));

            int expiresIn = 0;
            var jwt_token = TokenManager.GenerateToken(user.Username,out expiresIn);
            var responseModelAPI = new responseModel { token = jwt_token, expiresIn = expiresIn.ToString() };
            return Content(HttpStatusCode.OK, responseModelAPI);
        }

        [HttpGet]
        public HttpResponseMessage Get(string token, string username)
        {
            var tokenFromHeader = "";
            var apiKey = "";
            if (Request.Headers.Contains("Authorization"))
            {
                 tokenFromHeader = Request.Headers.GetValues("Authorization").FirstOrDefault()?.Split(' ')[1];
                apiKey = Request.Headers.GetValues("APIKey").FirstOrDefault();

            }
            bool exists = new UserRepository().GetUser(username) != null;
            if (!exists) return Request.CreateResponse(HttpStatusCode.NotFound,
                 "The user was not found.");
            string tokenUsername = TokenManager.ValidateToken(token);
            if (username.Equals(tokenUsername))
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

    }


    public class responseModel
    {
        public string token { get; set; }
        public string expiresIn { get; set; }
    }

    public class SingleModelResponse<TModel> : IDisposable
    {
        #region Public Properties

        [JsonIgnore]
        public bool IsError { get; set; }

        public TModel Model { get; set; }

        public void Dispose()
        {

        }

        #endregion Public Properties
    }

}
