using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Galery.Data
{
    internal class AppDBContext
    {
        private static DataContext Instance = null!;
        public static DataContext getInstance()
        {
            if (Instance is DataContext) return Instance;

            Instance = new DataContext();

            return Instance;
        }

    }
}
