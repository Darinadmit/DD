using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewInstagram.Model;
using System.Data.SqlClient;

namespace NewInstagram.DataLayer.Sql
{
    public class DataLayer : IDataLayer
    {
        private readonly string _connectionString;

        public DataLayer(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(connectionString);

            _connectionString = connectionString;
        }

        public Comment AddComment(Comment comment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                comment.Id = Guid.NewGuid();
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert into Comments (id, id_photo, id_user, date, text) values (@id, @id_photo, @id_user, @date, @text)";
                    command.Parameters.AddWithValue("@id", comment.Id);
                    command.Parameters.AddWithValue("@id_photo", comment.PhotoId);
                    command.Parameters.AddWithValue("@id_user", comment.UserId);
                    command.Parameters.AddWithValue("@date", comment.Date);
                    command.Parameters.AddWithValue("@text", comment.Text);
                    command.ExecuteNonQuery();

                    return comment;
                }
            }
        }

        public Comment GetComment(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, id_photo, id_user, date, text from Comments where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        reader.Read();
                        return new Comment
                        {
                            Id = reader.GetGuid(0),
                            PhotoId = reader.GetGuid(1),
                            UserId = reader.GetGuid(2),
                            Date = reader.GetDateTime(3),
                            Text = reader.GetString(4)
                        };
                    }
                }
            }
        }

        public Comment[] GetAllCommentsOnPhoto(Guid PhotoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, id_photo, id_user, date, text from Comments where id_photo = @id";
                    command.Parameters.AddWithValue("@id", PhotoId);
                    using (var reader = command.ExecuteReader())
                    {
                        var comments = new List<Comment>();

                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        while (reader.Read())
                        {
                            comments.Add(
                                new Comment
                            {
                                Id = reader.GetGuid(0),
                                PhotoId = reader.GetGuid(1),
                                UserId = reader.GetGuid(2),
                                Date = reader.GetDateTime(3),
                                Text = reader.GetString(4)
                            }
                            );
                        }
                        return comments.ToArray();
                    }
                }
            }
        }

        public void DeleteComment(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete from Comments where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader()) { }
                }
            }
        }

        public Photo[] GetUserPhotos(Guid userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id from Photos where id_user = @id";
                    command.Parameters.AddWithValue("@id", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        var guids = new List<Guid>();
                        while (reader.Read())
                        {
                            guids.Add(reader.GetGuid(0));
                        }
                        var photos = new Photo[guids.Count];
                        for (int i = 0; i < photos.Length; i++)
                        {
                            photos[i] = GetPhoto(guids.ElementAt(i));
                        }
                        return photos;
                    }

                }
            }
        }

        public Photo AddPhoto(Photo photo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    photo.Id = Guid.NewGuid();
                    command.CommandText = "insert into Photos (id, id_user, date, image) values (@id, @id_user, @date, @image)";
                    command.Parameters.AddWithValue("@id", photo.Id);
                    command.Parameters.AddWithValue("@id_user", photo.UserId);
                    command.Parameters.AddWithValue("@date", photo.Date);
                    command.Parameters.AddWithValue("@image", photo.Image);
                    command.ExecuteNonQuery();

                    for (int i = 0; i < photo.Hashtags.Length; i++)
                    {
                        command.CommandText = "insert into Tags (id_photo, text) values (@id_photo" + i + ", @text" + i + ")";
                        command.Parameters.AddWithValue("@id_photo" + i, photo.Id);
                        command.Parameters.AddWithValue("@text" + i, photo.Hashtags[i]);
                        command.ExecuteNonQuery();
                    }

                    for (int i = 0; i < photo.Marks.Length; i++)
                    {
                        command.CommandText = "insert into Marks (id_photo, id_user) values (@id__photo" + i + ", @id_user" + i + ")";
                        command.Parameters.AddWithValue("@id__photo" + i, photo.Id);
                        command.Parameters.AddWithValue("@id_user" + i, photo.Marks[i].Id);
                        command.ExecuteNonQuery();
                    }

                    return photo;
                }
            }
        }

        public Photo GetPhoto(Guid photoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    Photo photo = new Photo();
                    command.CommandText = "select id, id_user, date, image from Photos where id = @id";
                    command.Parameters.AddWithValue("@id", photoId);
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        photo.Id = reader.GetGuid(0);
                        photo.UserId = reader.GetGuid(1);
                        photo.Date = reader.GetDateTime(2);
                        photo.Image = new byte[((byte[])reader["image"]).Length];
                        photo.Image = (byte[])reader["image"];
                    }

                    command.CommandText = "select text from Tags where id_photo = @id_photo";
                    command.Parameters.AddWithValue("@id_photo", photoId);
                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<string>();
                        while (reader.Read())
                        {
                            list.Add(reader.GetString(0));
                        }
                        photo.Hashtags = list.ToArray();
                    }

                    command.CommandText = "select id_user, Users.name from Marks join Users on (Marks.id_user = Users.id) where Marks.id_photo = @id__photo";
                    command.Parameters.AddWithValue("@id__photo", photoId);
                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<User>();
                        while (reader.Read())
                        {
                            list.Add(new User
                            {
                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1)
                            }
                            );
                        }
                        photo.Marks = list.ToArray();
                        return photo;
                    }
                }
            }
        }

        public void DeletePhoto(Guid photoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete from Tags where id_photo = @id_photo";
                    command.Parameters.AddWithValue("@id_photo", photoId);
                    using (var reader = command.ExecuteReader()) { }

                    command.CommandText = "delete from Marks where Marks.id_photo = @id__photo";
                    command.Parameters.AddWithValue("@id__photo", photoId);
                    using (var reader = command.ExecuteReader()) { }

                    command.CommandText = "delete from Photos where id = @id";
                    command.Parameters.AddWithValue("@id", photoId);
                    using (var reader = command.ExecuteReader()) { }

                }
            }
        }

        public User AddUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    user.Id = Guid.NewGuid();
                    command.CommandText = "insert into Users (id, name) values (@id, @name)";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.ExecuteNonQuery();
                    return user;
                }
            }
        }

        public User GetUser(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name from Users where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return new User
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        };
                    }
                }
            }
        }

        public void DeleteUser(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete from Photos where id_user = @id_user";
                    command.Parameters.AddWithValue("@id_user", id);
                    using (var reader = command.ExecuteReader()) { }

                    command.CommandText = "update Users set name=@name from Users where id = @id";
                    command.Parameters.AddWithValue("@name", "delete");
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader()) { }
                }
            }
        }

       
    }
}
