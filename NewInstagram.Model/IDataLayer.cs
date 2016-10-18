using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewInstagram.Model
{
    public interface IDataLayer
    {
        User AddUser(User user);

        Photo AddPhoto(Photo photo);

        User GetUser(Guid id);

        Photo GetPhoto(Guid photoId);
    }
}
