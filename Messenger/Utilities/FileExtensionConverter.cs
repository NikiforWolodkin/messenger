using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Messenger.Utilities
{
    public class FileExtensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) 
            {
                return true;
            }

            var filePath = value as string;

            if (filePath is not null)
            {
                var extension = Path.GetExtension(filePath).ToLower();

                return extension == ".jpg" || extension == ".png" || extension == ".jpeg";
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
