using System;
using NewInstagram.Model;
using NewInstagram.DataLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Instagram.Tests
{
    [TestClass]
    public class DataLayerSqlTests
    {
        private const string ConnectionString = "Data Source=LENOVO-PC\\SQLEXPRESS;Initial Catalog=insta2;Integrated Security=True";

        private User GetNewUser()
        {
            var user = new User
            {
                Name = Guid.NewGuid().ToString().Substring(20)
            };

            user = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString).AddUser(user);
            return user;
        }

        private byte[] GetRandomImage()
        {
            byte[] image = new byte[new Random().Next(30) + 1];
            new Random().NextBytes(image);
            return image;
        }

        [TestMethod]
        public void ShouldAddPhoto()
        {
            //arrange
            var photo = new Photo 
            {
                UserId = GetNewUser().Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            //act
            photo = dataLayer.AddPhoto(photo);
            //asserts
            var resultPhoto = dataLayer.GetPhoto(photo.Id);
            CollectionAssert.AreEqual(photo.Image, resultPhoto.Image);
        }

        [TestMethod]
        public void ShouldAddPhotoAndMarks()
        {
            //arrange
            var photo = new Photo
            {
                UserId = GetNewUser().Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[] { GetNewUser(), GetNewUser() },
                Hashtags = new String[0]
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            //act
            photo = dataLayer.AddPhoto(photo);
            //asserts
            var resultPhoto = dataLayer.GetPhoto(photo.Id);
            for (int i = 0; i < resultPhoto.Marks.Length; i++)
            {
                Assert.AreEqual(photo.Marks[i].Name, resultPhoto.Marks[i].Name);
            }
        }

        [TestMethod]
        public void ShouldAddPhotoAndTags()
        {
            //arrange
            var photo = new Photo
            {
                UserId = GetNewUser().Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[] { Guid.NewGuid().ToString().Substring(20), "Tag2" }
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            //act
            photo = dataLayer.AddPhoto(photo);
            //asserts
            var resultPhoto = dataLayer.GetPhoto(photo.Id);
            CollectionAssert.AreEqual(photo.Hashtags, resultPhoto.Hashtags);
        }

        [TestMethod]
        public void ShouldDeletePhoto()
        {
            //arrange
            var photo = new Photo
            {
                UserId = GetNewUser().Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            photo = dataLayer.AddPhoto(photo);
            //act
            dataLayer.DeletePhoto(photo.Id);
            //asserts
            Assert.AreEqual(dataLayer.GetPhoto(photo.Id), null);
        }

        [TestMethod]
        public void ShouldAddUser()
        {
            //arrange
            var user = new User
            {
                Name = Guid.NewGuid().ToString().Substring(20)
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            //act
            user = dataLayer.AddUser(user);
            //asserts
            var resultUser = dataLayer.GetUser(user.Id);
            Assert.AreEqual(user.Name, resultUser.Name);
        }

        [TestMethod]
        public void ShouldAddComment()
        {
            //arrange
            var user = GetNewUser();
            var photo = new Photo
            {
                UserId = user.Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            dataLayer.AddPhoto(photo);
            var comment = new Comment
            {
                UserId = GetNewUser().Id,
                PhotoId = photo.Id,
                Date = DateTime.Now,
                Text = photo.Id.ToString().Substring(10)
            };
            //act
            comment = dataLayer.AddComment(comment);
            //asserts
            var resultComment = dataLayer.GetComment(comment.Id);
            Assert.AreEqual(comment.Text, resultComment.Text);
        }

        [TestMethod]
        public void ShouldGetAllCommentsOnPhoto()
        {
            //arrange
            var user = GetNewUser();
            var photo = new Photo
            {
                UserId = user.Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            dataLayer.AddPhoto(photo);
            var comment1 = new Comment
            {
                UserId = GetNewUser().Id,
                PhotoId = photo.Id,
                Date = DateTime.Now,
                Text = photo.Id.ToString().Substring(10)
            };
            var comment2 = new Comment
            {
                UserId = GetNewUser().Id,
                PhotoId = photo.Id,
                Date = DateTime.Now,
                Text = photo.Id.ToString().Substring(10)
            };
            comment1 = dataLayer.AddComment(comment1);
            comment2 = dataLayer.AddComment(comment2);
            var comments = new Comment[] { comment1, comment2 };
            //act
            var commentsResults = dataLayer.GetAllCommentsOnPhoto(photo.Id);
            //asserts
            for (int i = 0; i < commentsResults.Length; i++)
            {
                Assert.AreEqual(comments[i].Text, commentsResults[i].Text);
            }
        }

        [TestMethod]
        public void ShouldDeleteComment()
        {
            //arrange
            var user = GetNewUser();
            var photo = new Photo
            {
                UserId = user.Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            dataLayer.AddPhoto(photo);
            var comment = new Comment
            {
                UserId = GetNewUser().Id,
                PhotoId = photo.Id,
                Date = DateTime.Now,
                Text = photo.Id.ToString().Substring(10)
            };
            comment = dataLayer.AddComment(comment);
            //act
            dataLayer.DeleteComment(comment.Id);
            //asserts
            var resultComment = dataLayer.GetComment(comment.Id);
            Assert.AreEqual(null, resultComment);
        }

        [TestMethod]
        public void ShouldDeleteUser()
        {
            //arrange
            var user = new User
            {
                Name = Guid.NewGuid().ToString().Substring(20)
            };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            user = dataLayer.AddUser(user);
            //act
            dataLayer.DeleteUser(user.Id);
            //asserts
            var resultUser = dataLayer.GetUser(user.Id);
            Assert.AreEqual("delete", resultUser.Name);
        }

        [TestMethod]
        public void ShouldDeleteUserAndHisPhotos()
        {
            //arrange
            var user = GetNewUser();
            var photo1 = new Photo
            {
                UserId = user.Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            var photo2 = new Photo
            {
                UserId = user.Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            Photo[] photos = new Photo[] { photo1, photo2 };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            dataLayer.AddPhoto(photo1);
            dataLayer.AddPhoto(photo2);
            //act
            dataLayer.DeleteUser(user.Id);
            //asserts
            var resultPhotos = dataLayer.GetUserPhotos(user.Id);
            Assert.AreEqual(null, resultPhotos);
        }

        [TestMethod]
        public void ShouldGetUserPhotos()
        {
            //arrange
            var user = GetNewUser();
            var photo1 = new Photo
            {
                UserId = user.Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            var photo2 = new Photo
            {
                UserId = user.Id,
                Date = DateTime.Now,
                Image = GetRandomImage(),
                Marks = new User[0],
                Hashtags = new String[0]
            };
            Photo[] photos = new Photo[] { photo1, photo2 };
            var dataLayer = new NewInstagram.DataLayer.Sql.DataLayer(ConnectionString);
            dataLayer.AddPhoto(photo1);
            dataLayer.AddPhoto(photo2);
            //act
            var resultPhotos = dataLayer.GetUserPhotos(user.Id);
            //asserts
            for (int i = 0; i < resultPhotos.Length; i++)
            {
                Assert.AreEqual(resultPhotos[i].Id, photos[i].Id);
            }
        }
    }
}
