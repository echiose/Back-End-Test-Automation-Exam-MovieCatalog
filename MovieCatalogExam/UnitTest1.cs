using System;
using System.Net;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using MovieCatalogExam.Models;



namespace MovieCatalogExam
{
    [TestFixture]
    public class Tests
    {
        private RestClient client;
        private static string returnedmovieId;

        private const string BaseUrl = "http://144.91.123.158:5000";
        private const string StaticToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKd3RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI5OTZhZTQyOS03YjZjLTQ5ZDEtYjYwYi1jYzk2MTc4MmNiYTYiLCJpYXQiOiIwNC8xOC8yMDI2IDA2OjM2OjUwIiwiVXNlcklkIjoiZDZkNDc5OTUtYjg2Ny00ZDc3LTYyNmEtMDhkZTc2OTcxYWI5IiwiRW1haWwiOiJleGFtcWEyNkBzb2Z0dW5pLmNvbSIsIlVzZXJOYW1lIjoiRXhhbVFhIiwiZXhwIjoxNzc2NTE1ODEwLCJpc3MiOiJNb3ZpZUNhdGFsb2dfQXBwX1NvZnRVbmkiLCJhdWQiOiJNb3ZpZUNhdGFsb2dfV2ViQVBJX1NvZnRVbmkifQ.OnsLAwm--n-_kWO2RZuMG8UI0_MqzTwFA-_-wXqc-Xk";

        private const string LoginEmail = "examqa26@softuni.com";
        private const string LoginPassword = "examqa2026";

        [OneTimeSetUp]
        public void Setup()
        {
            string jwtToken;

            if (!string.IsNullOrWhiteSpace(StaticToken))
            {
                jwtToken = StaticToken;
            }
            else
            {
                jwtToken = GetJwtToken(LoginEmail, LoginPassword);
            }

            var options = new RestClientOptions(BaseUrl)
            {
                Authenticator = new JwtAuthenticator(jwtToken)
            };

            this.client = new RestClient(options);
        }

        private string GetJwtToken(string email, string password)
        {
            var tempClient = new RestClient(BaseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new { email, password });

            var response = tempClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = JsonSerializer.Deserialize<JsonElement>(response.Content);
                var token = content.GetProperty("token").GetString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("Token not found in the response.");
                }
                return token;
            }
            else
            {
                throw new InvalidOperationException($"Failed to authenticate. Status code: {response.StatusCode}, Response: {response.Content}");
            }
        }

        [Order(1)]
        [Test]
        public void CreateMovie_WithRequiredFIelds_ShouldReturnSuccess()
        {
            var movieData = new MovieDTO
            {
                Title = "Test Movie",
                Description = "This is a test movie description.",
                PosterUrl = "",
                TrailerLink = "",
                IsWatched = true

            };

            var request = new RestRequest("/api/Movie/Create", Method.Post);
            request.AddJsonBody(movieData);

            var response = this.client.Execute(request);

            var createResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);
            returnedmovieId = createResponse.Movie.Id;


            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code 200 OK.");
            Assert.That(createResponse, Is.Not.Null, "Response body was not deserialized.");
            Assert.That(createResponse.Movie, Is.Not.Null, "Expected a Movie object in the response.");
            Assert.That(createResponse.Movie.Id, Is.Not.Empty);
            Assert.That(createResponse.Movie.Id, Is.Not.Null);
            Assert.That(createResponse.Msg, Is.EqualTo("Movie created successfully!"));
        }

        [Order(2)]
        [Test]

        public void EditExistingMovie_ShouldReturnSuccess()
        {
            var editRequestData = new MovieDTO
            {
                Title = "Edited movie title",
                Description = "This is a edited movie description.",
                PosterUrl = "",
                TrailerLink = "",
                IsWatched = true
            };


            var request = new RestRequest("/api/Movie/Edit", Method.Put);

            request.AddQueryParameter("movieId", returnedmovieId);
            request.AddJsonBody(editRequestData);

            var response = this.client.Execute(request);

            var editResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);
            //Assert.That(returnedmovieId, Is.Not.Null.And.Not.Empty, "movieId was not saved from the create test.");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code 200 OK.");
            Assert.That(editResponse.Msg, Is.EqualTo("Movie edited successfully!"));
        }

        [Order(3)]
        [Test]
        public void GetAllMovies_ShouldReturnSuccess()
        {
            var request = new RestRequest("/api/Catalog/All", Method.Get);
            var response = this.client.Execute(request);

            var responseItems = JsonSerializer.Deserialize<List<ApiResponseDTO>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code 200 OK.");
            Assert.That(responseItems, Is.Not.Empty);
            Assert.That(responseItems, Is.Not.Null);
        }


        [Order(4)]
        [Test]

        public void DeleteCreatedMovie_ShouldReturnSuccess()
        {
            var request = new RestRequest("/api/Movie/Delete", Method.Delete);
            request.AddQueryParameter("movieId", returnedmovieId);
            var response = this.client.Execute(request);
            var responseContent = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code 200 OK.");
            //Assert.That(response.Content.Contains("Movie deleted successfully!"), Is.True);
            Assert.That(responseContent.Msg, Is.EqualTo("Movie deleted successfully!"));

        }

        [Order(5)]
        [Test]
        public void CreateMovie_WithoutRequiredFields_ShouldReturnBadRequest()
        {
            var movieData = new MovieDTO
            {
                Title = "",
                Description = "This is a test movie description.",
                PosterUrl = "",
                TrailerLink = "",
                IsWatched = true

            };
            var request = new RestRequest("/api/Movie/Create", Method.Post);
            request.AddJsonBody(movieData);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Expected status code 400 Bad Request.");
        }

        [Order(6)]
        [Test]

        public void EditNonExistingMovie_ShouldReturnNotFound()
        {
            string nonExistingMovieId = "999999999";
            var editRequestData = new MovieDTO
            {
                Title = "Edited non-existing movie title",
                Description = "This is a edited non-existing movie description.",
                PosterUrl = "",
                TrailerLink = "",
                IsWatched = true,

            };
            var request = new RestRequest("/api/Movie/Edit", Method.Put);
            request.AddQueryParameter("movieId", nonExistingMovieId);
            request.AddJsonBody(editRequestData);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Expected status code 400 Bad Request.");
        }

        [Order(7)]
        [Test]

        public void DeleteNonExistingMovie_ShouldReturnNotFound()
        {
            string nonExistingMovieId = "99999";

            var request = new RestRequest("/api/Movie/Delete", Method.Delete);
            request.AddQueryParameter("movieId", nonExistingMovieId);
            var response = this.client.Execute(request);
            var responseContent = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Expected status code 400 Bad Request.");
            Assert.That(responseContent.Msg, Is.EqualTo("Unable to delete the movie! Check the movieId parameter or user verification!"));

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this.client?.Dispose();
        }
    }
}