using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Messenger.Utilities;

public class FilePathToFileNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string filePath = value as string;
        if (filePath != null)
        {
            return Path.GetFileName(filePath);
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
