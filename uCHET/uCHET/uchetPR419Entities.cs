using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uCHET
{
    public partial class uchetPR419Entities
    {
        private static uchetPR419Entities _context;

        public static uchetPR419Entities GetContext()
        {
            if (_context != null)
            { 
            
            }
            else _context = new uchetPR419Entities();
            return _context;

        }
    }
}
